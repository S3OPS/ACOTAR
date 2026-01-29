using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ACOTAR
{
    /// <summary>
    /// Achievement UI for displaying unlocked and locked achievements
    /// Shows progress, points, and completion percentage
    /// </summary>
    public class AchievementUI : MonoBehaviour
    {
        public static AchievementUI Instance { get; private set; }

        [Header("UI References")]
        public GameObject achievementPanel;
        public GameObject achievementListContainer;
        public GameObject achievementEntryPrefab;
        public Text completionText;
        public Text pointsText;
        public Slider progressSlider;

        [Header("Category Filter")]
        public Dropdown categoryDropdown;
        public Toggle showLockedToggle;

        [Header("Achievement Details")]
        public GameObject detailsPanel;
        public Text detailNameText;
        public Text detailDescriptionText;
        public Text detailCategoryText;
        public Text detailPointsText;
        public Text detailStatusText;
        public Image detailIcon;

        [Header("Popup Notification")]
        public GameObject achievementPopup;
        public Text popupTitleText;
        public Text popupDescriptionText;
        public Image popupIcon;
        public float popupDuration = 5.0f;

        private List<GameObject> achievementEntries = new List<GameObject>();
        private AchievementCategory currentFilter = AchievementCategory.Story;
        private bool showLocked = true;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SetupListeners();
            HidePanel();
            
            if (achievementPopup != null)
            {
                achievementPopup.SetActive(false);
            }

            // Subscribe to achievement unlock events
            if (AchievementSystem.Instance != null)
            {
                AchievementSystem.Instance.OnAchievementUnlocked += OnAchievementUnlocked;
            }
        }

        /// <summary>
        /// Setup UI event listeners
        /// </summary>
        private void SetupListeners()
        {
            if (categoryDropdown != null)
            {
                categoryDropdown.onValueChanged.AddListener(OnCategoryChanged);
            }

            if (showLockedToggle != null)
            {
                showLockedToggle.onValueChanged.AddListener(OnShowLockedChanged);
            }
        }

        /// <summary>
        /// Show the achievement panel
        /// </summary>
        public void ShowPanel()
        {
            if (achievementPanel != null)
            {
                achievementPanel.SetActive(true);
                RefreshAchievementList();
                UpdateStats();
            }
        }

        /// <summary>
        /// Hide the achievement panel
        /// </summary>
        public void HidePanel()
        {
            if (achievementPanel != null)
            {
                achievementPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Toggle panel visibility
        /// </summary>
        public void TogglePanel()
        {
            if (achievementPanel != null)
            {
                if (achievementPanel.activeSelf)
                {
                    HidePanel();
                }
                else
                {
                    ShowPanel();
                }
            }
        }

        /// <summary>
        /// Refresh the achievement list display
        /// </summary>
        private void RefreshAchievementList()
        {
            if (AchievementSystem.Instance == null) return;

            // Clear existing entries
            ClearAchievementList();

            // Get filtered achievements
            List<Achievement> achievements;
            if (currentFilter == (AchievementCategory)(-1)) // All categories
            {
                achievements = AchievementSystem.Instance.GetAllAchievements();
            }
            else
            {
                achievements = AchievementSystem.Instance.GetAchievementsByCategory(currentFilter);
            }

            // Filter by locked/unlocked
            if (!showLocked)
            {
                achievements.RemoveAll(a => !a.IsUnlocked());
            }

            // Create entry for each achievement
            foreach (var achievement in achievements)
            {
                CreateAchievementEntry(achievement);
            }
        }

        /// <summary>
        /// Create an achievement entry in the list
        /// </summary>
        private void CreateAchievementEntry(Achievement achievement)
        {
            if (achievementEntryPrefab == null || achievementListContainer == null) return;

            GameObject entry = Instantiate(achievementEntryPrefab, achievementListContainer.transform);
            
            // Find and update text components
            Text nameText = entry.transform.Find("NameText")?.GetComponent<Text>();
            Text pointsText = entry.transform.Find("PointsText")?.GetComponent<Text>();
            Image iconImage = entry.transform.Find("IconImage")?.GetComponent<Image>();
            GameObject lockedOverlay = entry.transform.Find("LockedOverlay")?.gameObject;

            bool isUnlocked = AchievementSystem.Instance.IsAchievementUnlocked(achievement.id);

            if (nameText != null)
            {
                if (achievement.isSecret && !isUnlocked)
                {
                    nameText.text = "???";
                }
                else
                {
                    nameText.text = achievement.name;
                }
            }

            if (pointsText != null)
            {
                pointsText.text = $"{achievement.points} pts";
            }

            if (iconImage != null)
            {
                // Color-code by category
                Color categoryColor = GetCategoryColor(achievement.category);
                iconImage.color = isUnlocked ? categoryColor : Color.gray;
            }

            if (lockedOverlay != null)
            {
                lockedOverlay.SetActive(!isUnlocked);
            }

            // Add click listener to show details
            Button button = entry.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => ShowAchievementDetails(achievement));
            }

            achievementEntries.Add(entry);
        }

        /// <summary>
        /// Clear all achievement entries
        /// </summary>
        private void ClearAchievementList()
        {
            foreach (var entry in achievementEntries)
            {
                if (entry != null)
                {
                    Destroy(entry);
                }
            }
            achievementEntries.Clear();
        }

        /// <summary>
        /// Show achievement details panel
        /// </summary>
        private void ShowAchievementDetails(Achievement achievement)
        {
            if (detailsPanel == null) return;

            detailsPanel.SetActive(true);

            bool isUnlocked = AchievementSystem.Instance.IsAchievementUnlocked(achievement.id);

            if (detailNameText != null)
            {
                if (achievement.isSecret && !isUnlocked)
                {
                    detailNameText.text = "Secret Achievement";
                }
                else
                {
                    detailNameText.text = achievement.name;
                }
            }

            if (detailDescriptionText != null)
            {
                if (achievement.isSecret && !isUnlocked)
                {
                    detailDescriptionText.text = "This achievement is secret. Unlock it to see the details!";
                }
                else
                {
                    detailDescriptionText.text = achievement.description;
                }
            }

            if (detailCategoryText != null)
            {
                detailCategoryText.text = achievement.category.ToString();
            }

            if (detailPointsText != null)
            {
                detailPointsText.text = $"{achievement.points} Points";
            }

            if (detailStatusText != null)
            {
                if (isUnlocked)
                {
                    detailStatusText.text = $"Unlocked: {achievement.unlockedDate}";
                    detailStatusText.color = Color.green;
                }
                else
                {
                    detailStatusText.text = "Locked";
                    detailStatusText.color = Color.gray;
                }
            }

            if (detailIcon != null)
            {
                detailIcon.color = isUnlocked ? GetCategoryColor(achievement.category) : Color.gray;
            }
        }

        /// <summary>
        /// Update achievement statistics display
        /// </summary>
        private void UpdateStats()
        {
            if (AchievementSystem.Instance == null) return;

            int unlocked = AchievementSystem.Instance.GetUnlockedCount();
            int total = AchievementSystem.Instance.GetTotalCount();
            float percentage = AchievementSystem.Instance.GetCompletionPercentage();

            if (completionText != null)
            {
                completionText.text = $"{unlocked}/{total} Achievements ({percentage:F1}%)";
            }

            if (pointsText != null)
            {
                int earnedPoints = AchievementSystem.Instance.GetTotalPointsEarned();
                int maxPoints = AchievementSystem.Instance.GetMaxPoints();
                pointsText.text = $"{earnedPoints}/{maxPoints} Points";
            }

            if (progressSlider != null)
            {
                progressSlider.value = percentage / 100f;
            }
        }

        /// <summary>
        /// Handle category filter change
        /// </summary>
        private void OnCategoryChanged(int index)
        {
            if (index == 0)
            {
                currentFilter = (AchievementCategory)(-1); // All
            }
            else
            {
                currentFilter = (AchievementCategory)(index - 1);
            }
            
            RefreshAchievementList();
        }

        /// <summary>
        /// Handle show locked toggle change
        /// </summary>
        private void OnShowLockedChanged(bool value)
        {
            showLocked = value;
            RefreshAchievementList();
        }

        /// <summary>
        /// Get color for achievement category
        /// </summary>
        private Color GetCategoryColor(AchievementCategory category)
        {
            switch (category)
            {
                case AchievementCategory.Story:
                    return new Color(1f, 0.84f, 0f); // Gold
                case AchievementCategory.Combat:
                    return new Color(1f, 0f, 0f); // Red
                case AchievementCategory.Exploration:
                    return new Color(0f, 0.5f, 1f); // Blue
                case AchievementCategory.Companion:
                    return new Color(1f, 0.5f, 1f); // Pink
                case AchievementCategory.Collection:
                    return new Color(0.5f, 1f, 0.5f); // Green
                case AchievementCategory.Challenge:
                    return new Color(0.6f, 0f, 1f); // Purple
                case AchievementCategory.Secret:
                    return new Color(0.8f, 0.8f, 0.8f); // Silver
                default:
                    return Color.white;
            }
        }

        /// <summary>
        /// Handle achievement unlock event
        /// </summary>
        private void OnAchievementUnlocked(Achievement achievement)
        {
            ShowAchievementPopup(achievement);
            
            // Refresh list if panel is open
            if (achievementPanel != null && achievementPanel.activeSelf)
            {
                RefreshAchievementList();
                UpdateStats();
            }
        }

        /// <summary>
        /// Show achievement unlock popup notification
        /// </summary>
        public void ShowAchievementPopup(Achievement achievement)
        {
            if (achievementPopup == null) return;

            achievementPopup.SetActive(true);

            if (popupTitleText != null)
            {
                popupTitleText.text = achievement.name;
            }

            if (popupDescriptionText != null)
            {
                popupDescriptionText.text = $"{achievement.description}\n+{achievement.points} points";
            }

            if (popupIcon != null)
            {
                popupIcon.color = GetCategoryColor(achievement.category);
            }

            // Auto-hide after duration
            StartCoroutine(HidePopupAfterDelay());
        }

        /// <summary>
        /// Hide popup after delay
        /// </summary>
        private System.Collections.IEnumerator HidePopupAfterDelay()
        {
            yield return new WaitForSeconds(popupDuration);
            
            if (achievementPopup != null)
            {
                achievementPopup.SetActive(false);
            }
        }

        /// <summary>
        /// Manually close popup
        /// </summary>
        public void ClosePopup()
        {
            if (achievementPopup != null)
            {
                achievementPopup.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (AchievementSystem.Instance != null)
            {
                AchievementSystem.Instance.OnAchievementUnlocked -= OnAchievementUnlocked;
            }
        }
    }
}
