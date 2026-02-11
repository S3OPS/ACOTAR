using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ACOTAR
{
    /// <summary>
    /// Manages UI display for reputation levels with all seven courts
    /// Shows progress bars, rank names, and benefits of reputation
    /// </summary>
    public class ReputationUI : MonoBehaviour
    {
        [Header("UI Panels")]
        public GameObject reputationPanel;
        public Transform reputationListContainer;
        public GameObject reputationEntryPrefab;

        [Header("Detail Panel")]
        public GameObject reputationDetailPanel;
        public Text courtNameText;
        public Text reputationLevelText;
        public Slider reputationProgressBar;
        public Text reputationPointsText;
        public Text benefitsText;
        public Text penaltiesText;

        private ReputationSystem reputationSystem;
        private Dictionary<string, GameObject> reputationEntries = new Dictionary<string, GameObject>();
        
        // Court colors for visual distinction
        private Dictionary<string, Color> courtColors = new Dictionary<string, Color>()
        {
            { "Spring", new Color(0.4f, 0.8f, 0.3f) },      // Light green
            { "Summer", new Color(1f, 0.8f, 0.2f) },        // Golden yellow
            { "Autumn", new Color(0.9f, 0.4f, 0.1f) },      // Orange-red
            { "Winter", new Color(0.6f, 0.8f, 1f) },        // Light blue
            { "Night", new Color(0.2f, 0.1f, 0.4f) },       // Deep purple
            { "Dawn", new Color(1f, 0.7f, 0.9f) },          // Pink
            { "Day", new Color(1f, 0.95f, 0.5f) }           // Bright yellow
        };

        void Start()
        {
            if (reputationPanel != null)
            {
                reputationPanel.SetActive(false);
            }
            
            if (reputationDetailPanel != null)
            {
                reputationDetailPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Show the reputation panel
        /// </summary>
        public void ShowReputationPanel()
        {
            if (reputationPanel != null)
            {
                reputationPanel.SetActive(true);
                RefreshReputationDisplay();
            }
        }

        /// <summary>
        /// Hide the reputation panel
        /// </summary>
        public void HideReputationPanel()
        {
            if (reputationPanel != null)
            {
                reputationPanel.SetActive(false);
            }
            
            if (reputationDetailPanel != null)
            {
                reputationDetailPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Set the reputation system to display
        /// </summary>
        public void SetReputationSystem(ReputationSystem system)
        {
            reputationSystem = system;
            RefreshReputationDisplay();
        }

        /// <summary>
        /// Refresh the reputation display for all courts
        /// </summary>
        public void RefreshReputationDisplay()
        {
            if (reputationSystem == null || reputationListContainer == null)
                return;

            // Clear existing entries
            ClearReputationEntries();

            // Get all courts
            string[] courts = { "Spring", "Summer", "Autumn", "Winter", "Night", "Dawn", "Day" };

            foreach (string court in courts)
            {
                CreateReputationEntry(court);
            }
        }

        /// <summary>
        /// Create a reputation entry for a court
        /// </summary>
        private void CreateReputationEntry(string court)
        {
            if (reputationEntryPrefab == null)
            {
                Debug.LogWarning("Reputation entry prefab not set!");
                return;
            }

            GameObject entry = Instantiate(reputationEntryPrefab, reputationListContainer);
            
            // Set court name
            Text nameText = entry.transform.Find("CourtNameText")?.GetComponent<Text>();
            if (nameText != null)
            {
                nameText.text = $"{court} Court";
                if (courtColors.ContainsKey(court))
                {
                    nameText.color = courtColors[court];
                }
            }

            // Set reputation level text
            int reputation = reputationSystem.GetReputation(court);
            ReputationLevel level = reputationSystem.GetReputationLevel(court);
            Text levelText = entry.transform.Find("ReputationLevelText")?.GetComponent<Text>();
            if (levelText != null)
            {
                levelText.text = GetReputationLevelName(level);
                levelText.color = GetReputationLevelColor(level);
            }

            // Set progress bar
            Slider progressBar = entry.transform.Find("ProgressBar")?.GetComponent<Slider>();
            if (progressBar != null)
            {
                progressBar.value = GetReputationProgress(reputation, level);
                
                // Color the fill based on court
                Image fillImage = progressBar.fillRect?.GetComponent<Image>();
                if (fillImage != null && courtColors.ContainsKey(court))
                {
                    fillImage.color = courtColors[court];
                }
            }

            // Set reputation points text
            Text pointsText = entry.transform.Find("PointsText")?.GetComponent<Text>();
            if (pointsText != null)
            {
                pointsText.text = $"{reputation} / {GetNextLevelThreshold(level)}";
            }

            // Add button listener to show details
            Button detailButton = entry.GetComponent<Button>();
            if (detailButton != null)
            {
                string courtName = court; // Capture for closure
                detailButton.onClick.AddListener(() => ShowCourtDetails(courtName));
            }

            // Track the entry
            reputationEntries[court] = entry;
        }

        /// <summary>
        /// Get progress percentage for current reputation level
        /// </summary>
        private float GetReputationProgress(int reputation, ReputationLevel level)
        {
            int currentThreshold = GetLevelThreshold(level);
            int nextThreshold = GetNextLevelThreshold(level);
            
            if (nextThreshold == currentThreshold)
                return 1f; // Max level
            
            int progress = reputation - currentThreshold;
            int range = nextThreshold - currentThreshold;
            
            return Mathf.Clamp01((float)progress / range);
        }

        /// <summary>
        /// Get reputation threshold for a level
        /// </summary>
        private int GetLevelThreshold(ReputationLevel level)
        {
            switch (level)
            {
                case ReputationLevel.Hostile: return -100;
                case ReputationLevel.Unfriendly: return -50;
                case ReputationLevel.Neutral: return 0;
                case ReputationLevel.Friendly: return 25;
                case ReputationLevel.Honored: return 50;
                case ReputationLevel.Revered: return 75;
                case ReputationLevel.Exalted: return 100;
                default: return 0;
            }
        }

        /// <summary>
        /// Get reputation threshold for next level
        /// </summary>
        private int GetNextLevelThreshold(ReputationLevel level)
        {
            switch (level)
            {
                case ReputationLevel.Hostile: return -50;
                case ReputationLevel.Unfriendly: return 0;
                case ReputationLevel.Neutral: return 25;
                case ReputationLevel.Friendly: return 50;
                case ReputationLevel.Honored: return 75;
                case ReputationLevel.Revered: return 100;
                case ReputationLevel.Exalted: return 100;
                default: return 100;
            }
        }

        /// <summary>
        /// Show detailed reputation information for a specific court
        /// </summary>
        public void ShowCourtDetails(string court)
        {
            if (reputationSystem == null || reputationDetailPanel == null)
                return;

            reputationDetailPanel.SetActive(true);

            int reputation = reputationSystem.GetReputation(court);
            ReputationLevel level = reputationSystem.GetReputationLevel(court);

            // Set court name
            if (courtNameText != null)
            {
                courtNameText.text = $"{court} Court";
                if (courtColors.ContainsKey(court))
                {
                    courtNameText.color = courtColors[court];
                }
            }

            // Set reputation level
            if (reputationLevelText != null)
            {
                reputationLevelText.text = GetReputationLevelName(level);
                reputationLevelText.color = GetReputationLevelColor(level);
            }

            // Set progress bar
            if (reputationProgressBar != null)
            {
                reputationProgressBar.value = GetReputationProgress(reputation, level);
            }

            // Set points text
            if (reputationPointsText != null)
            {
                reputationPointsText.text = $"Reputation: {reputation} / {GetNextLevelThreshold(level)}";
            }

            // Set benefits text
            if (benefitsText != null)
            {
                benefitsText.text = GetReputationBenefits(court, level);
            }

            // Set penalties text
            if (penaltiesText != null)
            {
                penaltiesText.text = GetReputationPenalties(court, level);
            }
        }

        /// <summary>
        /// Get user-friendly name for reputation level
        /// </summary>
        private string GetReputationLevelName(ReputationLevel level)
        {
            return level.ToString();
        }

        /// <summary>
        /// Get color for reputation level
        /// </summary>
        private Color GetReputationLevelColor(ReputationLevel level)
        {
            switch (level)
            {
                case ReputationLevel.Hostile:
                    return new Color(0.8f, 0.1f, 0.1f); // Dark red
                case ReputationLevel.Unfriendly:
                    return new Color(1f, 0.4f, 0.2f); // Orange-red
                case ReputationLevel.Neutral:
                    return Color.yellow;
                case ReputationLevel.Friendly:
                    return new Color(0.7f, 1f, 0.5f); // Light green
                case ReputationLevel.Honored:
                    return new Color(0.4f, 0.9f, 0.4f); // Green
                case ReputationLevel.Revered:
                    return new Color(0.3f, 0.7f, 1f); // Light blue
                case ReputationLevel.Exalted:
                    return new Color(0.9f, 0.7f, 1f); // Purple
                default:
                    return Color.white;
            }
        }

        /// <summary>
        /// Get benefits description for reputation level
        /// </summary>
        private string GetReputationBenefits(string court, ReputationLevel level)
        {
            System.Text.StringBuilder benefits = new System.Text.StringBuilder();
            benefits.AppendLine("<b>Benefits:</b>\n");

            switch (level)
            {
                case ReputationLevel.Hostile:
                    benefits.AppendLine("• None");
                    break;
                case ReputationLevel.Unfriendly:
                    benefits.AppendLine("• Can enter court territory (begrudgingly)");
                    break;
                case ReputationLevel.Neutral:
                    benefits.AppendLine("• Normal prices at merchants");
                    benefits.AppendLine("• Basic quests available");
                    break;
                case ReputationLevel.Friendly:
                    benefits.AppendLine("• 10% discount at merchants");
                    benefits.AppendLine("• More quest options");
                    benefits.AppendLine("• Court members are helpful");
                    break;
                case ReputationLevel.Honored:
                    benefits.AppendLine("• 20% discount at merchants");
                    benefits.AppendLine("• Special quests available");
                    benefits.AppendLine("• Access to restricted areas");
                    break;
                case ReputationLevel.Revered:
                    benefits.AppendLine("• 35% discount at merchants");
                    benefits.AppendLine("• Rare items available");
                    benefits.AppendLine("• High Lord/Lady audiences");
                    benefits.AppendLine("• Court companions more effective");
                    break;
                case ReputationLevel.Exalted:
                    benefits.AppendLine("• 50% discount at merchants");
                    benefits.AppendLine("• Legendary items available");
                    benefits.AppendLine("• Court champion title");
                    benefits.AppendLine("• Access to all court secrets");
                    benefits.AppendLine("• Special court abilities");
                    break;
            }

            return benefits.ToString();
        }

        /// <summary>
        /// Get penalties description for reputation level
        /// </summary>
        private string GetReputationPenalties(string court, ReputationLevel level)
        {
            System.Text.StringBuilder penalties = new System.Text.StringBuilder();
            penalties.AppendLine("<b>Penalties:</b>\n");

            switch (level)
            {
                case ReputationLevel.Hostile:
                    penalties.AppendLine("• Attacked on sight");
                    penalties.AppendLine("• Cannot trade");
                    penalties.AppendLine("• 50% price penalty if allowed");
                    penalties.AppendLine("• All court members are hostile");
                    break;
                case ReputationLevel.Unfriendly:
                    penalties.AppendLine("• 25% price penalty");
                    penalties.AppendLine("• Limited quest access");
                    penalties.AppendLine("• Court members are cold");
                    break;
                case ReputationLevel.Neutral:
                    penalties.AppendLine("• None");
                    break;
                default:
                    penalties.AppendLine("• None");
                    break;
            }

            return penalties.ToString();
        }

        /// <summary>
        /// Clear all reputation entries
        /// </summary>
        private void ClearReputationEntries()
        {
            foreach (var entry in reputationEntries.Values)
            {
                if (entry != null)
                {
                    Destroy(entry);
                }
            }
            reputationEntries.Clear();
        }

        /// <summary>
        /// Close the detail panel
        /// </summary>
        public void CloseDetailPanel()
        {
            if (reputationDetailPanel != null)
            {
                reputationDetailPanel.SetActive(false);
            }
        }
    }
}
