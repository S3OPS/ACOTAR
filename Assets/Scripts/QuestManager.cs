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
        public bool isDLCContent;  // NEW: Marks quest as DLC content

        public Quest(string id, string title, string desc, QuestType type)
        {
            this.questId = id;
            this.title = title;
            this.description = desc;
            this.type = type;
            this.isCompleted = false;
            this.isActive = false;
            this.objectives = new List<string>();
            this.isDLCContent = false;
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
    /// 
    /// BASE GAME (Book 1): A Court of Thorns and Roses
    /// - Complete story from Human Lands to Under the Mountain
    /// - Feyre's transformation into High Fae
    /// - Main antagonist: Amarantha
    /// 
    /// DLC 1 (Book 2): A Court of Mist and Fury - Requires DLC purchase
    /// DLC 2 (Book 3): A Court of Wings and Ruin - Requires DLC purchase
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

            // =====================================================
            // BASE GAME: Book 1 - A Court of Thorns and Roses
            // =====================================================
            
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

            // Side Quests - Base Game
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

            // Companion Quests - Base Game
            // Note: These are preview/teaser quests that give hints about future DLC content
            // Full companion content is in DLC packages
            Quest comp1 = new Quest(
                "companion_001",
                "Glimpse of Starlight",
                "Under the Mountain, Rhysand shares a cryptic vision with you - a city of stars you've never seen.",
                QuestType.CompanionQuest
            );
            comp1.objectives.Add("Experience Rhysand's mysterious vision");
            comp1.objectives.Add("See glimpses of an impossible city");
            comp1.objectives.Add("Wonder about the Night Court's secrets");
            comp1.experienceReward = 300;
            AddQuest(comp1);

            Quest comp2 = new Quest(
                "companion_002",
                "The Illyrian's Challenge",
                "A powerful Illyrian warrior issues you a challenge during your time Under the Mountain.",
                QuestType.CompanionQuest
            );
            comp2.objectives.Add("Face the Illyrian's test of will");
            comp2.objectives.Add("Prove your determination");
            comp2.objectives.Add("Earn a measure of respect");
            comp2.experienceReward = 250;
            AddQuest(comp2);

            // Initialize Book 1 extended quests (base game content)
            Book1Quests.InitializeBook1Quests(quests);

            // =====================================================
            // DLC CONTENT - Only loaded if DLC is purchased
            // =====================================================
            LoadDLCContentIfOwned();
        }

        /// <summary>
        /// Load DLC content if the player owns the DLC
        /// </summary>
        private void LoadDLCContentIfOwned()
        {
            if (DLCManager.Instance != null)
            {
                // Load Book 2 DLC if owned
                if (DLCManager.Instance.IsDLCOwned(DLCPackage.ACOMAF_MistAndFury))
                {
                    DLCManager.Instance.InstallDLC(DLCPackage.ACOMAF_MistAndFury, quests);
                }

                // Load Book 3 DLC if owned
                if (DLCManager.Instance.IsDLCOwned(DLCPackage.ACOWAR_WingsAndRuin))
                {
                    DLCManager.Instance.InstallDLC(DLCPackage.ACOWAR_WingsAndRuin, quests);
                }
            }
            else
            {
                Debug.Log("DLCManager not found - DLC content not loaded. Base game (Book 1) only.");
            }
        }

        /// <summary>
        /// Manually load DLC content after purchase
        /// </summary>
        public void LoadDLCContent(DLCPackage package)
        {
            if (DLCManager.Instance != null && DLCManager.Instance.IsDLCOwned(package))
            {
                DLCManager.Instance.InstallDLC(package, quests);
            }
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
                
                // Check if quest is DLC content
                if (DLCManager.Instance != null)
                {
                    DLCPackage? dlcPackage = DLCManager.Instance.GetQuestDLCPackage(questId);
                    if (dlcPackage.HasValue && !DLCManager.Instance.IsDLCInstalled(dlcPackage.Value))
                    {
                        Debug.LogWarning($"Quest '{quest.title}' requires DLC: {dlcPackage.Value}");
                        return;
                    }
                }
                
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

                    // Start next quest if available (and not DLC locked)
                    if (!string.IsNullOrEmpty(quest.nextQuestId))
                    {
                        // Check if next quest is DLC content
                        if (DLCManager.Instance != null)
                        {
                            DLCPackage? dlcPackage = DLCManager.Instance.GetQuestDLCPackage(quest.nextQuestId);
                            if (dlcPackage.HasValue && !DLCManager.Instance.IsDLCInstalled(dlcPackage.Value))
                            {
                                Debug.Log($"Congratulations! You've completed the base game content.");
                                Debug.Log($"To continue the story, purchase DLC: {dlcPackage.Value}");
                                return;
                            }
                        }
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

        /// <summary>
        /// Get only base game quests (no DLC)
        /// </summary>
        public List<Quest> GetBaseGameQuests()
        {
            List<Quest> baseQuests = new List<Quest>();
            foreach (var quest in quests.Values)
            {
                if (DLCManager.Instance == null || 
                    DLCManager.Instance.GetQuestDLCPackage(quest.questId) == null)
                {
                    baseQuests.Add(quest);
                }
            }
            return baseQuests;
        }

        /// <summary>
        /// Get quests from a specific DLC
        /// </summary>
        public List<Quest> GetDLCQuests(DLCPackage package)
        {
            List<Quest> dlcQuests = new List<Quest>();
            if (DLCManager.Instance != null && DLCManager.Instance.IsDLCInstalled(package))
            {
                foreach (var quest in quests.Values)
                {
                    if (DLCManager.Instance.GetQuestDLCPackage(quest.questId) == package)
                    {
                        dlcQuests.Add(quest);
                    }
                }
            }
            return dlcQuests;
        }

        public Quest GetQuest(string questId)
        {
            if (quests.ContainsKey(questId))
            {
                return quests[questId];
            }
            return null;
        }

        /// <summary>
        /// Display quest statistics
        /// </summary>
        public void DisplayQuestStats()
        {
            int baseGameQuests = GetBaseGameQuests().Count;
            Debug.Log("\n=== Quest Statistics ===");
            Debug.Log($"Base Game Quests (Book 1): {baseGameQuests}");
            
            if (DLCManager.Instance != null)
            {
                if (DLCManager.Instance.IsDLCInstalled(DLCPackage.ACOMAF_MistAndFury))
                {
                    Debug.Log($"DLC 1 Quests (Book 2): {GetDLCQuests(DLCPackage.ACOMAF_MistAndFury).Count}");
                }
                else
                {
                    Debug.Log("DLC 1 (Book 2): Not Installed");
                }

                if (DLCManager.Instance.IsDLCInstalled(DLCPackage.ACOWAR_WingsAndRuin))
                {
                    Debug.Log($"DLC 2 Quests (Book 3): {GetDLCQuests(DLCPackage.ACOWAR_WingsAndRuin).Count}");
                }
                else
                {
                    Debug.Log("DLC 2 (Book 3): Not Installed");
                }
            }
            
            Debug.Log($"\nActive Quests: {activeQuests.Count}");
            Debug.Log($"Completed Quests: {completedQuests.Count}");
            Debug.Log("========================\n");
        }
    }
}
