using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Quest system for ACOTAR storylines
    /// </summary>
    [System.Serializable]
    public class Quest
    {
        public string questId;
        public string title;
        public string description;
        public QuestType type;
        public Location location;
        public bool isCompleted;
        public bool isActive;
        public List<string> objectives;
        public int experienceReward;
        public string nextQuestId;

        public Quest(string id, string title, string desc, QuestType type)
        {
            this.questId = id;
            this.title = title;
            this.description = desc;
            this.type = type;
            this.isCompleted = false;
            this.isActive = false;
            this.objectives = new List<string>();
        }
    }

    public enum QuestType
    {
        MainStory,
        SideQuest,
        CourtQuest,
        CompanionQuest,
        ExplorationQuest
    }

    /// <summary>
    /// Manages the quest system based on ACOTAR storylines
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        private Dictionary<string, Quest> quests;
        private List<Quest> activeQuests;
        private List<Quest> completedQuests;

        void Awake()
        {
            InitializeQuests();
            activeQuests = new List<Quest>();
            completedQuests = new List<Quest>();
        }

        private void InitializeQuests()
        {
            quests = new Dictionary<string, Quest>();

            // Main Story Quests based on ACOTAR Book 1
            Quest q1 = new Quest(
                "main_001",
                "Beyond the Wall",
                "You've killed a wolf in the forest. But this was no ordinary wolf - it was a Fae. Now you must face the consequences.",
                QuestType.MainStory
            );
            q1.objectives.Add("Survive in the mortal lands");
            q1.objectives.Add("Hunt to feed your family");
            q1.experienceReward = 100;
            q1.nextQuestId = "main_002";
            AddQuest(q1);

            Quest q2 = new Quest(
                "main_002",
                "The Spring Court's Beast",
                "A terrifying beast has come to take you to Prythian as payment for the Fae life you took.",
                QuestType.MainStory
            );
            q2.objectives.Add("Accept your fate and cross the Wall");
            q2.objectives.Add("Enter the Spring Court");
            q2.experienceReward = 150;
            q2.nextQuestId = "main_003";
            AddQuest(q2);

            Quest q3 = new Quest(
                "main_003",
                "Life at the Manor",
                "Learn about the Spring Court and its mysterious High Lord, Tamlin.",
                QuestType.MainStory
            );
            q3.objectives.Add("Explore the Spring Court Manor");
            q3.objectives.Add("Meet the court members: Lucien, Alis, and others");
            q3.objectives.Add("Learn about the curse plaguing Prythian");
            q3.experienceReward = 200;
            q3.nextQuestId = "main_004";
            AddQuest(q3);

            Quest q4 = new Quest(
                "main_004",
                "Calanmai",
                "The Fire Night approaches - a sacred ritual of the Spring Court.",
                QuestType.MainStory
            );
            q4.objectives.Add("Witness the ancient ritual");
            q4.objectives.Add("Discover the truth about Tamlin's powers");
            q4.experienceReward = 250;
            q4.nextQuestId = "main_005";
            AddQuest(q4);

            Quest q5 = new Quest(
                "main_005",
                "Under the Mountain",
                "Amarantha has taken Tamlin. To save him and all of Prythian, you must face impossible trials.",
                QuestType.MainStory
            );
            q5.objectives.Add("Travel Under the Mountain");
            q5.objectives.Add("Complete the Three Trials");
            q5.objectives.Add("Solve Amarantha's riddle");
            q5.objectives.Add("Break the curse");
            q5.experienceReward = 1000;
            AddQuest(q5);

            // Side Quests
            Quest side1 = new Quest(
                "side_001",
                "The Suriel's Wisdom",
                "Capture a Suriel to gain valuable information about Prythian.",
                QuestType.SideQuest
            );
            side1.objectives.Add("Find a Suriel in the forest");
            side1.objectives.Add("Trap it with ash wood");
            side1.objectives.Add("Ask your questions wisely");
            side1.experienceReward = 150;
            AddQuest(side1);

            Quest side2 = new Quest(
                "side_002",
                "Summer Court Alliance",
                "Help forge an alliance with Tarquin, High Lord of the Summer Court.",
                QuestType.CourtQuest
            );
            side2.objectives.Add("Travel to Adriata");
            side2.objectives.Add("Meet with High Lord Tarquin");
            side2.objectives.Add("Negotiate terms");
            side2.experienceReward = 300;
            AddQuest(side2);

            Quest side3 = new Quest(
                "side_003",
                "The Book of Breathings",
                "Recover the powerful Book of Breathings to aid in the coming war.",
                QuestType.MainStory
            );
            side3.objectives.Add("Locate the Book");
            side3.objectives.Add("Retrieve it from the Summer Court");
            side3.objectives.Add("Return safely to the Night Court");
            side3.experienceReward = 400;
            AddQuest(side3);

            // Companion Quests
            Quest comp1 = new Quest(
                "companion_001",
                "Rhysand's Secret",
                "Discover the truth about the High Lord of the Night Court and the City of Starlight.",
                QuestType.CompanionQuest
            );
            comp1.objectives.Add("Visit Velaris for the first time");
            comp1.objectives.Add("Meet the Inner Circle");
            comp1.objectives.Add("Learn about Rhysand's true nature");
            comp1.experienceReward = 500;
            AddQuest(comp1);

            Quest comp2 = new Quest(
                "companion_002",
                "Cassian's Training",
                "Train with the legendary Illyrian warrior to master combat.",
                QuestType.CompanionQuest
            );
            comp2.objectives.Add("Train at the House of Wind");
            comp2.objectives.Add("Master basic Illyrian combat techniques");
            comp2.objectives.Add("Prove yourself worthy");
            comp2.experienceReward = 350;
            AddQuest(comp2);

            // Initialize Book 1 extended quests
            Book1Quests.InitializeBook1Quests(quests);

            // Initialize Book 2 quests (A Court of Mist and Fury)
            Book2Quests.InitializeBook2Quests(quests);

            // Initialize Book 3 quests (A Court of Wings and Ruin)
            Book3Quests.InitializeBook3Quests(quests);
        }

        private void AddQuest(Quest quest)
        {
            quests[quest.questId] = quest;
        }

        public void StartQuest(string questId)
        {
            if (quests.ContainsKey(questId))
            {
                Quest quest = quests[questId];
                if (!quest.isActive && !quest.isCompleted)
                {
                    quest.isActive = true;
                    activeQuests.Add(quest);
                    Debug.Log($"Quest Started: {quest.title}");
                    GameEvents.TriggerQuestStarted(quest);
                }
            }
            else
            {
                Debug.LogWarning($"Quest not found: {questId}");
            }
        }

        public void CompleteQuest(string questId)
        {
            if (quests.ContainsKey(questId))
            {
                Quest quest = quests[questId];
                if (quest.isActive)
                {
                    quest.isCompleted = true;
                    quest.isActive = false;
                    activeQuests.Remove(quest);
                    completedQuests.Add(quest);
                    
                    // Grant experience to player if GameManager exists
                    if (GameManager.Instance != null && GameManager.Instance.playerCharacter != null)
                    {
                        GameManager.Instance.playerCharacter.GainExperience(quest.experienceReward);
                        Debug.Log($"Quest Completed: {quest.title} - Granted {quest.experienceReward} XP to {GameManager.Instance.playerCharacter.name}");
                    }
                    else
                    {
                        Debug.Log($"Quest Completed: {quest.title} - Reward: {quest.experienceReward} XP");
                    }

                    GameEvents.TriggerQuestCompleted(quest);

                    // Start next quest if available
                    if (!string.IsNullOrEmpty(quest.nextQuestId))
                    {
                        StartQuest(quest.nextQuestId);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Quest not found: {questId}");
            }
        }

        public List<Quest> GetActiveQuests()
        {
            return new List<Quest>(activeQuests);
        }

        public Quest GetQuest(string questId)
        {
            if (quests.ContainsKey(questId))
            {
                return quests[questId];
            }
            return null;
        }
    }
}
