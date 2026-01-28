using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace ACOTAR
{
    /// <summary>
    /// Manages quest log UI display
    /// Shows active and completed quests with objectives
    /// </summary>
    public class QuestLogUI : MonoBehaviour
    {
        [Header("Quest List")]
        public GameObject questListContainer;
        public GameObject questListItemPrefab;

        [Header("Quest Details")]
        public GameObject questDetailsPanel;
        public Text questTitleText;
        public Text questTypeText;
        public Text questDescriptionText;
        public GameObject objectivesContainer;
        public GameObject objectivePrefab;
        public Text rewardsText;
        public Text statusText;

        [Header("Filters")]
        public Toggle showActiveToggle;
        public Toggle showCompletedToggle;
        public Dropdown typeFilterDropdown;

        [Header("Categories")]
        public Button mainQuestsButton;
        public Button sideQuestsButton;
        public Button courtQuestsButton;
        public Button companionQuestsButton;
        public Button allQuestsButton;

        private QuestManager questManager;
        private List<GameObject> questListItems = new List<GameObject>();
        private Quest selectedQuest;
        private QuestType currentFilter = QuestType.MainStory;
        private bool showActive = true;
        private bool showCompleted = false;

        void Start()
        {
            InitializeQuestLog();
            SetupEventListeners();
        }

        /// <summary>
        /// Initialize quest log UI
        /// </summary>
        private void InitializeQuestLog()
        {
            // Get quest manager from GameManager
            if (GameManager.Instance != null)
            {
                questManager = GameManager.Instance.questManager;
            }

            // Hide details initially
            if (questDetailsPanel != null)
            {
                questDetailsPanel.SetActive(false);
            }

            Debug.Log("QuestLogUI initialized");
        }

        /// <summary>
        /// Setup button and toggle event listeners
        /// </summary>
        private void SetupEventListeners()
        {
            if (mainQuestsButton != null)
            {
                mainQuestsButton.onClick.AddListener(() => FilterByType(QuestType.MainStory));
            }

            if (sideQuestsButton != null)
            {
                sideQuestsButton.onClick.AddListener(() => FilterByType(QuestType.SideQuest));
            }

            if (courtQuestsButton != null)
            {
                courtQuestsButton.onClick.AddListener(() => FilterByType(QuestType.CourtQuest));
            }

            if (companionQuestsButton != null)
            {
                companionQuestsButton.onClick.AddListener(() => FilterByType(QuestType.CompanionQuest));
            }

            if (allQuestsButton != null)
            {
                allQuestsButton.onClick.AddListener(ShowAllQuests);
            }

            if (showActiveToggle != null)
            {
                showActiveToggle.onValueChanged.AddListener(OnActiveToggleChanged);
            }

            if (showCompletedToggle != null)
            {
                showCompletedToggle.onValueChanged.AddListener(OnCompletedToggleChanged);
            }
        }

        /// <summary>
        /// Filter quests by type
        /// </summary>
        private void FilterByType(QuestType questType)
        {
            currentFilter = questType;
            RefreshQuestList();
        }

        /// <summary>
        /// Show all quests regardless of type
        /// </summary>
        private void ShowAllQuests()
        {
            currentFilter = QuestType.MainStory; // Use as "all" indicator
            RefreshQuestList();
        }

        /// <summary>
        /// Handle active quests toggle
        /// </summary>
        private void OnActiveToggleChanged(bool value)
        {
            showActive = value;
            RefreshQuestList();
        }

        /// <summary>
        /// Handle completed quests toggle
        /// </summary>
        private void OnCompletedToggleChanged(bool value)
        {
            showCompleted = value;
            RefreshQuestList();
        }

        /// <summary>
        /// Refresh quest list display
        /// </summary>
        public void RefreshQuestList()
        {
            if (questManager == null || questListContainer == null)
                return;

            // Clear existing list items
            ClearQuestList();

            // Get all quests
            List<Quest> allQuests = questManager.GetAllQuests();
            
            // Filter quests
            List<Quest> filteredQuests = allQuests.Where(q => 
            {
                // Status filter
                bool statusMatch = false;
                if (showActive && q.questStatus == QuestStatus.InProgress)
                    statusMatch = true;
                if (showCompleted && q.questStatus == QuestStatus.Completed)
                    statusMatch = true;

                // Type filter (show all if no specific filter)
                bool typeMatch = true; // Show all by default
                
                return statusMatch && typeMatch;
            }).ToList();

            // Create list items
            foreach (Quest quest in filteredQuests)
            {
                CreateQuestListItem(quest);
            }

            Debug.Log($"Quest list refreshed: {filteredQuests.Count} quests displayed");
        }

        /// <summary>
        /// Clear quest list
        /// </summary>
        private void ClearQuestList()
        {
            foreach (GameObject item in questListItems)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }
            questListItems.Clear();
        }

        /// <summary>
        /// Create a quest list item
        /// </summary>
        private void CreateQuestListItem(Quest quest)
        {
            if (questListItemPrefab == null || questListContainer == null)
                return;

            GameObject listItem = Instantiate(questListItemPrefab, questListContainer.transform);
            
            // Setup item text
            Text itemText = listItem.GetComponentInChildren<Text>();
            if (itemText != null)
            {
                string statusIcon = quest.questStatus == QuestStatus.Completed ? "✓" : "○";
                itemText.text = $"{statusIcon} {quest.questName}";
            }

            // Add click listener
            Button itemButton = listItem.GetComponent<Button>();
            if (itemButton != null)
            {
                itemButton.onClick.AddListener(() => ShowQuestDetails(quest));
            }

            // Color code by type
            Image itemImage = listItem.GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.color = GetQuestTypeColor(quest.questType);
            }

            questListItems.Add(listItem);
        }

        /// <summary>
        /// Get color based on quest type
        /// </summary>
        private Color GetQuestTypeColor(QuestType questType)
        {
            switch (questType)
            {
                case QuestType.MainStory:
                    return new Color(1f, 0.8f, 0f); // Gold
                case QuestType.SideQuest:
                    return Color.cyan;
                case QuestType.CourtQuest:
                    return new Color(0.8f, 0f, 1f); // Purple
                case QuestType.CompanionQuest:
                    return new Color(1f, 0.5f, 0.5f); // Pink
                default:
                    return Color.white;
            }
        }

        /// <summary>
        /// Show detailed information about a quest
        /// </summary>
        public void ShowQuestDetails(Quest quest)
        {
            if (quest == null)
                return;

            selectedQuest = quest;

            if (questDetailsPanel != null)
            {
                questDetailsPanel.SetActive(true);
            }

            // Quest title
            if (questTitleText != null)
            {
                questTitleText.text = quest.questName;
            }

            // Quest type
            if (questTypeText != null)
            {
                questTypeText.text = $"Type: {quest.questType}";
            }

            // Quest description
            if (questDescriptionText != null)
            {
                questDescriptionText.text = quest.description;
            }

            // Quest status
            if (statusText != null)
            {
                statusText.text = $"Status: {quest.questStatus}";
            }

            // Objectives
            DisplayObjectives(quest);

            // Rewards
            if (rewardsText != null)
            {
                string rewards = "Rewards:\n";
                rewards += $"- {quest.experienceReward} XP\n";
                
                if (quest.goldReward > 0)
                {
                    rewards += $"- {quest.goldReward} Gold\n";
                }

                if (quest.itemRewards != null && quest.itemRewards.Count > 0)
                {
                    rewards += "- Items:\n";
                    foreach (string itemId in quest.itemRewards)
                    {
                        rewards += $"  • {itemId}\n";
                    }
                }

                rewardsText.text = rewards;
            }

            Debug.Log($"Showing details for quest: {quest.questName}");
        }

        /// <summary>
        /// Display quest objectives with checkboxes
        /// </summary>
        private void DisplayObjectives(Quest quest)
        {
            if (objectivesContainer == null)
                return;

            // Clear existing objectives
            foreach (Transform child in objectivesContainer.transform)
            {
                Destroy(child.gameObject);
            }

            // Create objective items
            for (int i = 0; i < quest.objectives.Count; i++)
            {
                if (objectivePrefab != null)
                {
                    GameObject objectiveItem = Instantiate(objectivePrefab, objectivesContainer.transform);
                    
                    Text objectiveText = objectiveItem.GetComponentInChildren<Text>();
                    if (objectiveText != null)
                    {
                        bool isComplete = i < quest.objectivesCompleted.Count && quest.objectivesCompleted[i];
                        string checkmark = isComplete ? "☑" : "☐";
                        objectiveText.text = $"{checkmark} {quest.objectives[i]}";
                    }
                }
            }
        }

        /// <summary>
        /// Track active quest
        /// </summary>
        public void TrackQuest(Quest quest)
        {
            if (quest == null)
                return;

            // Set as tracked quest
            Debug.Log($"Now tracking: {quest.questName}");
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowNotification($"Tracking: {quest.questName}");
            }

            // Update HUD to show tracked quest
            UpdateTrackedQuestDisplay(quest);
        }

        /// <summary>
        /// Update HUD with tracked quest info
        /// </summary>
        private void UpdateTrackedQuestDisplay(Quest quest)
        {
            // This would update a small quest tracker on the HUD
            Debug.Log($"Active Quest: {quest.questName}");
            
            // Display current objective
            int currentObjective = quest.objectivesCompleted.Count(complete => complete);
            if (currentObjective < quest.objectives.Count)
            {
                Debug.Log($"Objective: {quest.objectives[currentObjective]}");
            }
        }

        /// <summary>
        /// Close quest details
        /// </summary>
        public void CloseQuestDetails()
        {
            if (questDetailsPanel != null)
            {
                questDetailsPanel.SetActive(false);
            }
            selectedQuest = null;
        }

        /// <summary>
        /// Get quest statistics
        /// </summary>
        public void ShowQuestStatistics()
        {
            if (questManager == null)
                return;

            List<Quest> allQuests = questManager.GetAllQuests();
            
            int activeQuests = allQuests.Count(q => q.questStatus == QuestStatus.InProgress);
            int completedQuests = allQuests.Count(q => q.questStatus == QuestStatus.Completed);
            int totalQuests = allQuests.Count;

            Debug.Log($"Quest Statistics:");
            Debug.Log($"Active: {activeQuests}");
            Debug.Log($"Completed: {completedQuests}");
            Debug.Log($"Total: {totalQuests}");
        }
    }
}
