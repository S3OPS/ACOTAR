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
        
        // v2.3.1: Optional challenge objectives for replayability
        public List<string> optionalObjectives;
        public int bonusExperienceReward;  // Bonus XP for completing optional objectives
        public int bonusGoldReward;  // Bonus gold for completing optional objectives
        
        // v2.6.7: Track optional objective completion
        public List<bool> optionalObjectivesCompleted;
        
        // v2.3.2: Combat preparation hints for boss fights
        public string preparationHint;  // Strategic hint for challenging encounters

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
            this.optionalObjectives = new List<string>();
            this.bonusExperienceReward = 0;
            this.bonusGoldReward = 0;
            this.preparationHint = "";  // v2.3.2
            this.optionalObjectivesCompleted = new List<bool>();  // v2.6.7
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

        /// <summary>
        /// Start a quest by ID
        /// v2.6.1: Enhanced with error handling and logging
        /// </summary>
        public void StartQuest(string questId)
        {
            try
            {
                if (string.IsNullOrEmpty(questId))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Quest", "StartQuest called with null or empty questId");
                    return;
                }
                
                if (quests.ContainsKey(questId))
            {
                Quest quest = quests[questId];
                
                // Check if quest is DLC content
                if (DLCManager.Instance != null)
                {
                    DLCPackage? dlcPackage = DLCManager.Instance.GetQuestDLCPackage(questId);
                    if (dlcPackage.HasValue && !DLCManager.Instance.IsDLCInstalled(dlcPackage.Value))
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Quest", 
                            $"Quest '{quest.title}' requires DLC: {dlcPackage.Value}");
                        return;
                    }
                }
                
                if (!quest.isActive && !quest.isCompleted)
                {
                    quest.isActive = true;
                    activeQuests.Add(quest);
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Quest", 
                        $"Quest Started: {quest.title}");
                    GameEvents.TriggerQuestStarted(quest);
                    
                    // v2.6.8: Notify player of new quest
                    NotificationSystem.ShowQuest("New Quest", quest.title);
                }
            }
            else
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Quest", 
                    $"Quest not found: {questId}");
            }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Quest", 
                    $"Exception in StartQuest({questId}): {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Complete a quest by ID
        /// v2.6.1: Enhanced with error handling and logging
        /// </summary>
        public void CompleteQuest(string questId)
        {
            try
            {
                if (string.IsNullOrEmpty(questId))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Quest", "CompleteQuest called with null or empty questId");
                    return;
                }
                
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
                        int totalXP = quest.experienceReward;
                        
                        // v2.6.7: Check for optional objectives bonus
                        if (quest.optionalObjectives != null && quest.optionalObjectives.Count > 0)
                        {
                            int completedOptionals = 0;
                            if (quest.optionalObjectivesCompleted != null)
                            {
                                for (int i = 0; i < quest.optionalObjectivesCompleted.Count; i++)
                                {
                                    if (quest.optionalObjectivesCompleted[i]) completedOptionals++;
                                }
                            }
                            
                            // Award bonus if all optional objectives completed
                            if (completedOptionals == quest.optionalObjectives.Count && completedOptionals > 0)
                            {
                                totalXP += quest.bonusExperienceReward;
                                
                                // Award bonus gold if applicable
                                if (quest.bonusGoldReward > 0 && GameManager.Instance.playerCharacter.currencySystem != null)
                                {
                                    GameManager.Instance.playerCharacter.currencySystem.AddCurrency("Gold", quest.bonusGoldReward);
                                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Quest", 
                                        $"⭐ ALL OPTIONAL OBJECTIVES COMPLETE! ⭐ Bonus: +{quest.bonusExperienceReward} XP, +{quest.bonusGoldReward} Gold");
                                }
                                else
                                {
                                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Quest", 
                                        $"⭐ ALL OPTIONAL OBJECTIVES COMPLETE! ⭐ Bonus: +{quest.bonusExperienceReward} XP");
                                }
                            }
                            else if (completedOptionals > 0)
                            {
                                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Quest", 
                                    $"Optional objectives: {completedOptionals}/{quest.optionalObjectives.Count} completed");
                            }
                        }
                        
                        GameManager.Instance.playerCharacter.GainExperience(totalXP);
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Quest", 
                            $"Quest Completed: {quest.title} - Granted {totalXP} XP to {GameManager.Instance.playerCharacter.name}");
                    }
                    else
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Quest", 
                            $"Quest Completed: {quest.title} - Reward: {quest.experienceReward} XP");
                    }

                    GameEvents.TriggerQuestCompleted(quest);

                    // v2.6.8: Notify player of quest completion
                    NotificationSystem.ShowSuccess($"Quest Complete: {quest.title}! +{quest.experienceReward} XP", 4f);

                    // Start next quest if available (and not DLC locked)
                    if (!string.IsNullOrEmpty(quest.nextQuestId))
                    {
                        // Check if next quest is DLC content
                        if (DLCManager.Instance != null)
                        {
                            DLCPackage? dlcPackage = DLCManager.Instance.GetQuestDLCPackage(quest.nextQuestId);
                            if (dlcPackage.HasValue && !DLCManager.Instance.IsDLCInstalled(dlcPackage.Value))
                            {
                                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Quest", 
                                    $"Congratulations! You've completed the base game content. To continue, purchase DLC: {dlcPackage.Value}");
                                return;
                            }
                        }
                        StartQuest(quest.nextQuestId);
                    }
                }
            }
            else
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Quest", 
                    $"Quest not found: {questId}");
            }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Quest", 
                    $"Exception in CompleteQuest({questId}): {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Get all active quests
        /// </summary>
        /// <returns>List of currently active quests</returns>
        /// <remarks>
        /// Returns a new list copy to prevent external modification of the active quest collection.
        /// Active quests are those that have been started but not yet completed.
        /// </remarks>
        public List<Quest> GetActiveQuests()
        {
            return new List<Quest>(activeQuests);
        }

        /// <summary>
        /// Get only base game quests (no DLC)
        /// </summary>
        /// <returns>List of quests available in the base game</returns>
        /// <remarks>
        /// Filters quests to return only those that don't require DLC packages.
        /// Useful for displaying available content to players without DLC.
        /// Uses DLCManager to determine quest ownership.
        /// </remarks>
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
        /// Complete an optional objective for a quest
        /// v2.6.7: NEW - Track optional objective completion for bonus rewards
        /// </summary>
        /// <param name="questId">ID of the quest</param>
        /// <param name="objectiveIndex">Index of the optional objective (0-based)</param>
        public void CompleteOptionalObjective(string questId, int objectiveIndex)
        {
            try
            {
                if (string.IsNullOrEmpty(questId))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Quest", 
                        "CompleteOptionalObjective called with null or empty questId");
                    return;
                }
                
                if (!quests.ContainsKey(questId))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Quest", 
                        $"Quest not found: {questId}");
                    return;
                }
                
                Quest quest = quests[questId];
                
                // Ensure optional objectives list exists
                if (quest.optionalObjectives == null || quest.optionalObjectives.Count == 0)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Quest", 
                        $"Quest {questId} has no optional objectives");
                    return;
                }
                
                // Ensure completion tracking list exists and is sized correctly
                if (quest.optionalObjectivesCompleted == null)
                {
                    quest.optionalObjectivesCompleted = new List<bool>();
                }
                
                // Initialize completion tracking if needed
                while (quest.optionalObjectivesCompleted.Count < quest.optionalObjectives.Count)
                {
                    quest.optionalObjectivesCompleted.Add(false);
                }
                
                // Validate index
                if (objectiveIndex < 0 || objectiveIndex >= quest.optionalObjectives.Count)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Quest", 
                        $"Invalid optional objective index {objectiveIndex} for quest {questId}");
                    return;
                }
                
                // Mark as completed
                if (!quest.optionalObjectivesCompleted[objectiveIndex])
                {
                    quest.optionalObjectivesCompleted[objectiveIndex] = true;
                    
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Quest", 
                        $"✓ Optional Objective Complete: {quest.optionalObjectives[objectiveIndex]}");
                    
                    // Check if all optional objectives are complete
                    bool allComplete = true;
                    foreach (bool completed in quest.optionalObjectivesCompleted)
                    {
                        if (!completed)
                        {
                            allComplete = false;
                            break;
                        }
                    }
                    
                    if (allComplete)
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Quest", 
                            $"⭐ ALL Optional Objectives Complete! Complete quest for bonus rewards! ⭐");
                        
                        if (UIManager.Instance != null)
                        {
                            UIManager.Instance.ShowNotification(
                                $"All Optional Objectives Complete!\nComplete quest for +{quest.bonusExperienceReward} XP bonus!", 
                                3f);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Quest", 
                    $"Exception in CompleteOptionalObjective({questId}, {objectiveIndex}): {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Get quests from a specific DLC
        /// </summary>
        /// <param name="package">The DLC package to filter by</param>
        /// <returns>List of quests belonging to the specified DLC</returns>
        /// <remarks>
        /// Returns quests that are part of the specified DLC package.
        /// Empty list if DLC is not installed or no quests found.
        /// Requires DLCManager to be initialized.
        /// </remarks>
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

        /// <summary>
        /// Get quests from a specific DLC
        /// </summary>
        /// <param name="package">The DLC package to filter by</param>
        /// <returns>List of quests belonging to the specified DLC</returns>
        /// <remarks>
        /// Returns quests that are part of the specified DLC package.
        /// Empty list if DLC is not installed or no quests found.
        /// Requires DLCManager to be initialized.
        /// </remarks>
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

        /// <summary>
        /// Mark a quest objective as complete and send a progress notification
        /// v2.6.8: NEW - Mid-quest progress tracking with notifications
        /// </summary>
        /// <param name="questId">ID of the quest</param>
        /// <param name="objectiveIndex">Zero-based index of the objective to mark complete</param>
        public void UpdateQuestObjectiveProgress(string questId, int objectiveIndex)
        {
            try
            {
                if (string.IsNullOrEmpty(questId))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Quest",
                        "UpdateQuestObjectiveProgress called with null or empty questId");
                    return;
                }

                if (!quests.ContainsKey(questId))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Quest",
                        $"Quest not found: {questId}");
                    return;
                }

                Quest quest = quests[questId];

                if (!quest.isActive)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Quest",
                        $"Quest {questId} is not active, cannot update objective progress");
                    return;
                }

                if (quest.objectives == null || objectiveIndex < 0 || objectiveIndex >= quest.objectives.Count)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Quest",
                        $"Invalid objective index {objectiveIndex} for quest {questId}");
                    return;
                }

                string objectiveText = quest.objectives[objectiveIndex];
                int currentObjectiveNumber = objectiveIndex + 1;
                int totalCount = quest.objectives.Count;

                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Quest",
                    $"✓ Objective Complete [{currentObjectiveNumber}/{totalCount}]: {objectiveText}");

                // v2.6.8: Send progress notification via NotificationSystem
                NotificationSystem.ShowQuest(
                    $"Quest Progress: {quest.title}",
                    $"✓ {objectiveText} ({currentObjectiveNumber}/{totalCount})");
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Quest",
                    $"Exception in UpdateQuestObjectiveProgress({questId}, {objectiveIndex}): {ex.Message}\nStack: {ex.StackTrace}");
            }
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
