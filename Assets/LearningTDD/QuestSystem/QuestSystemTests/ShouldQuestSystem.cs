using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine;

namespace Assets.LearningTDD.QuestSystem.QuestSystemTests
{
    public class ShouldQuestSystem
    {
        /*
         * Nuestro jugador debe poder hablar con un NPC y solicitarle una Quest. 
            Se debe validar si nuestro jugador cumple con el requisito requerido de la Quest (nivel), si el 
            jugador cumple con el requisito, la Quest se activa, de lo contrario, le notifica que no cumple con 
            los requerimientos necesarios
            Una vez que pide la Quest y es activada, no se debe poder pedir por segunda vez.
         */

        /*
            Se deben modelar los Jobs de tal manera que sean adaptables a modificaciones de ordenamiento 
            para cualquier tipo de Quest.
            Una vez teniendo modelado los Jobs, nuestro jugador debe poder comenzar la Quest, realizar 
            todos los Jobs según el orden asignado y finalizar.
            No debe poder saltearse Jobs, ni avanzar sin haber cumplido con los anteriores.
            Cada Job tiene que tener su descripción, explicando lo que se debe realizar.
        
         */

        private Quest _quest;
        private Player _player;

        [SetUp]
        public void SetUp()
        {
            _quest = new Quest();
            _player = new Player();

            _quest.questName = "Kill all zombies!";
            _quest.questLevel = 20;
            _quest.questReward = "Sword";


            var npc = new NPC {message = "HOla!", npcName = "Tokyo", waitADelivery = false};

            var jobs = new QuestJobs();
            jobs.npcs.Add(npc);
            _quest.jobs = jobs;
        }

        [Test]

            public void
                Should_Player_Request_Quest_To_Npc_And_Should_Check_IfP_layer_Have_The_Requeriments_And_Once_Requested_Should_Not_Be_Able_To_Request_Again()
            {
                // When
                _quest.GetQuest(_player);

                // Assert
                Assert.AreEqual(_quest.questLevel, _player.playerLevel);
                Assert.AreNotEqual(_quest.questName, "K");
            }

            [Test]
            public void
                Should_Player_Make_All_Jobs_From_Quest_In_Order_And_Cannot_Skip_Jobs_Each_Job_Must_Have_Description()
            {
                // When
                foreach (var q in _player.GetQuestList())
                {
                    Debug.Log(q.jobs.npcs[0].npcName);
                }

                // Assert
            }
        }

        public class Player : IQuest
        {
            private List<Quest> _quests = new List<Quest>();
            public int playerLevel = 20;

            public void AddQuest(Quest quest)
            {
                _quests.Add(quest);
                Debug.Log($"New quest added to player {quest.questName}");
            }

            public List<Quest> GetQuestList()
            {
                return _quests;
            }
        }

        public interface IQuest
        {
            void AddQuest(Quest quest);
            List<Quest> GetQuestList();
        }

        public class Quest
        {
            public string questName;
            public int questLevel;
            public string questReward;
            public bool isActive;
            public QuestJobs jobs;

            public void GetQuest(IQuest iQuest)
            {
                if (!CanTakeQuest(iQuest)) return;

                isActive = true;
                iQuest.AddQuest(this);
            }

            private bool CanTakeQuest(IQuest iQuest)
            {
                foreach (var quest in iQuest.GetQuestList())
                {
                    if (quest.questName == questName)
                    {
                        Debug.Log("You have this quest in progress!");
                        return false;
                    }

                    if (quest.questLevel >= questLevel) continue;
                    Debug.Log("You don't have enough level for this Quest!");
                    return false;
                }

                return true;
            }
        }

        public class QuestJobs : IQuestJobs
        {
            public List<NPC> npcs = new List<NPC>();
            public List<Mobs> mobs = new List<Mobs>();

            public bool StartQuest(NPC npc)
            {
                if (!npc.hasTalked) return false;
                Debug.Log($"You talked with NPC!{npc.npcName}");
                return true;
            }

            public bool TalkToNpc(List<NPC> npcs)
            {
                foreach (var npc in npcs)
                {
                    if (npc.hasTalked && !npc.waitADelivery)
                        return true;

                    switch (npc.hasDelivered)
                    {
                        case true when npc.waitADelivery && npc.hasDelivered:
                            return true;
                        case true when npc.waitADelivery && !npc.hasDelivered:
                            return false;
                    }

                    if (!npc.hasTalked)
                        return false;
                }

                return false;
            }

            public bool KillMobs(List<Mobs> mobs)
            {
                if (mobs.All(mob => mob.isKilled)) return true;
                Debug.Log("You didn't kill all mobs!");
                return false;
            }

            public bool FinishQuest(NPC npc)
            {
                if (npc.hasTalked)
                {
                    Debug.Log("Quest finished!");
                    return true;
                }

                return false;
            }
        }

        public interface IQuestJobs
        {
            bool StartQuest(NPC npc);
            bool TalkToNpc(List<NPC> npcs);
            bool KillMobs(List<Mobs> mobs);
            bool FinishQuest(NPC npc);
        }

        public class Mobs
        {
            public bool isKilled;
        }

        public class NPC
        {
            public string npcName;
            public bool hasTalked;
            public bool hasDelivered;
            public bool waitADelivery;
            public string itemToDeliver;
            public string message;

            public void Talk()
            {
                hasTalked = true;
                message = "Hola!";
                Debug.Log(message);
            }

            public void Deliver(string item)
            {
                if (item == itemToDeliver)
                    hasDelivered = true;
            }
        }
}