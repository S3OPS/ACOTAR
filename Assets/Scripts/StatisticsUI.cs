using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// UI panel for displaying comprehensive player statistics and analytics.
/// Shows playtime, combat stats, exploration progress, economic data, and more.
/// </summary>
public class StatisticsUI : MonoBehaviour
{
    [Header("Main Panel")]
    [SerializeField] private GameObject statisticsPanel;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button exportButton;
    [SerializeField] private Button resetButton;
    
    [Header("Category Tabs")]
    [SerializeField] private Button overviewTabButton;
    [SerializeField] private Button combatTabButton;
    [SerializeField] private Button explorationTabButton;
    [SerializeField] private Button economyTabButton;
    [SerializeField] private Button characterTabButton;
    
    [Header("Tab Panels")]
    [SerializeField] private GameObject overviewPanel;
    [SerializeField] private GameObject combatPanel;
    [SerializeField] private GameObject explorationPanel;
    [SerializeField] private GameObject economyPanel;
    [SerializeField] private GameObject characterPanel;
    
    [Header("Overview Tab Elements")]
    [SerializeField] private TextMeshProUGUI totalPlaytimeText;
    [SerializeField] private TextMeshProUGUI sessionCountText;
    [SerializeField] private TextMeshProUGUI avgSessionText;
    [SerializeField] private TextMeshProUGUI lastPlayedText;
    [SerializeField] private TextMeshProUGUI combatWinRateText;
    [SerializeField] private TextMeshProUGUI questCompletionText;
    [SerializeField] private TextMeshProUGUI worldCompletionText;
    [SerializeField] private TextMeshProUGUI achievementsText;
    [SerializeField] private TextMeshProUGUI favoriteClassText;
    
    [Header("Combat Tab Elements")]
    [SerializeField] private TextMeshProUGUI combatWinsText;
    [SerializeField] private TextMeshProUGUI combatLossesText;
    [SerializeField] private TextMeshProUGUI combatFleesText;
    [SerializeField] private TextMeshProUGUI winRateDetailedText;
    [SerializeField] private TextMeshProUGUI damageDealtText;
    [SerializeField] private TextMeshProUGUI damageTakenText;
    [SerializeField] private TextMeshProUGUI healingText;
    [SerializeField] private TextMeshProUGUI criticalHitsText;
    [SerializeField] private TextMeshProUGUI flawlessVictoriesText;
    [SerializeField] private TextMeshProUGUI abilityUsesText;
    [SerializeField] private TextMeshProUGUI favoriteAbilityText;
    [SerializeField] private TextMeshProUGUI mostDefeatedEnemyText;
    
    [Header("Exploration Tab Elements")]
    [SerializeField] private TextMeshProUGUI locationsVisitedText;
    [SerializeField] private TextMeshProUGUI worldProgressText;
    [SerializeField] private TextMeshProUGUI travelCountText;
    [SerializeField] private TextMeshProUGUI questsStartedText;
    [SerializeField] private TextMeshProUGUI questsCompletedText;
    [SerializeField] private TextMeshProUGUI questCompletionRateText;
    [SerializeField] private Slider worldCompletionSlider;
    
    [Header("Economy Tab Elements")]
    [SerializeField] private TextMeshProUGUI goldEarnedText;
    [SerializeField] private TextMeshProUGUI goldSpentText;
    [SerializeField] private TextMeshProUGUI netGoldText;
    [SerializeField] private TextMeshProUGUI itemsCraftedText;
    [SerializeField] private TextMeshProUGUI itemsPurchasedText;
    [SerializeField] private TextMeshProUGUI itemsSoldText;
    [SerializeField] private TextMeshProUGUI craftingEfficiencyText;
    [SerializeField] private TextMeshProUGUI merchantInteractionsText;
    [SerializeField] private Slider craftingEfficiencySlider;
    
    [Header("Character Tab Elements")]
    [SerializeField] private TextMeshProUGUI currentClassText;
    [SerializeField] private TextMeshProUGUI favoriteClassDetailedText;
    [SerializeField] private TextMeshProUGUI levelsGainedText;
    [SerializeField] private TextMeshProUGUI totalXPText;
    [SerializeField] private TextMeshProUGUI deathsText;
    [SerializeField] private TextMeshProUGUI companionsRecruitedText;
    [SerializeField] private TextMeshProUGUI favoriteCompanionText;
    [SerializeField] private TextMeshProUGUI dialogueChoicesText;
    [SerializeField] private TextMeshProUGUI achievementsUnlockedText;
    [SerializeField] private TextMeshProUGUI achievementPointsText;
    
    private StatisticsManager statsManager;
    private string currentTab = "overview";

    void Start()
    {
        statsManager = StatisticsManager.Instance;
        
        // Setup button listeners
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
        
        if (exportButton != null)
            exportButton.onClick.AddListener(ExportStatistics);
        
        if (resetButton != null)
            resetButton.onClick.AddListener(ShowResetConfirmation);
        
        // Setup tab buttons
        if (overviewTabButton != null)
            overviewTabButton.onClick.AddListener(() => SwitchTab("overview"));
        
        if (combatTabButton != null)
            combatTabButton.onClick.AddListener(() => SwitchTab("combat"));
        
        if (explorationTabButton != null)
            explorationTabButton.onClick.AddListener(() => SwitchTab("exploration"));
        
        if (economyTabButton != null)
            economyTabButton.onClick.AddListener(() => SwitchTab("economy"));
        
        if (characterTabButton != null)
            characterTabButton.onClick.AddListener(() => SwitchTab("character"));
        
        // Initially hide panel
        if (statisticsPanel != null)
            statisticsPanel.SetActive(false);
        
        // Show overview tab by default
        SwitchTab("overview");
    }

    /// <summary>
    /// Opens the statistics panel and refreshes all data.
    /// </summary>
    public void OpenPanel()
    {
        if (statisticsPanel != null)
        {
            statisticsPanel.SetActive(true);
            RefreshAllStatistics();
        }
    }

    /// <summary>
    /// Closes the statistics panel.
    /// </summary>
    public void ClosePanel()
    {
        if (statisticsPanel != null)
            statisticsPanel.SetActive(false);
    }

    /// <summary>
    /// Switches between different statistic category tabs.
    /// </summary>
    private void SwitchTab(string tabName)
    {
        currentTab = tabName;
        
        // Hide all panels
        if (overviewPanel != null) overviewPanel.SetActive(false);
        if (combatPanel != null) combatPanel.SetActive(false);
        if (explorationPanel != null) explorationPanel.SetActive(false);
        if (economyPanel != null) economyPanel.SetActive(false);
        if (characterPanel != null) characterPanel.SetActive(false);
        
        // Show selected panel
        switch (tabName)
        {
            case "overview":
                if (overviewPanel != null) overviewPanel.SetActive(true);
                RefreshOverviewTab();
                break;
            case "combat":
                if (combatPanel != null) combatPanel.SetActive(true);
                RefreshCombatTab();
                break;
            case "exploration":
                if (explorationPanel != null) explorationPanel.SetActive(true);
                RefreshExplorationTab();
                break;
            case "economy":
                if (economyPanel != null) economyPanel.SetActive(true);
                RefreshEconomyTab();
                break;
            case "character":
                if (characterPanel != null) characterPanel.SetActive(true);
                RefreshCharacterTab();
                break;
        }
    }

    /// <summary>
    /// Refreshes all statistics displays.
    /// </summary>
    private void RefreshAllStatistics()
    {
        RefreshOverviewTab();
        RefreshCombatTab();
        RefreshExplorationTab();
        RefreshEconomyTab();
        RefreshCharacterTab();
    }

    /// <summary>
    /// Refreshes the overview tab with summary statistics.
    /// </summary>
    private void RefreshOverviewTab()
    {
        if (statsManager == null) return;
        
        if (totalPlaytimeText != null)
            totalPlaytimeText.text = statsManager.GetFormattedPlaytime();
        
        if (sessionCountText != null)
            sessionCountText.text = statsManager.GetSessionCount().ToString();
        
        if (avgSessionText != null)
        {
            float avgSeconds = statsManager.GetAverageSessionLength();
            int hours = Mathf.FloorToInt(avgSeconds / 3600f);
            int minutes = Mathf.FloorToInt((avgSeconds % 3600f) / 60f);
            avgSessionText.text = $"{hours:00}:{minutes:00}";
        }
        
        if (lastPlayedText != null)
            lastPlayedText.text = DateTime.Now.ToString("MMM dd, yyyy");
        
        if (combatWinRateText != null)
            combatWinRateText.text = $"{statsManager.GetWinRate():F1}%";
        
        if (questCompletionText != null)
            questCompletionText.text = $"{statsManager.GetQuestCompletionRate():F1}%";
        
        if (worldCompletionText != null)
            worldCompletionText.text = $"{statsManager.GetWorldCompletionPercentage():F1}%";
        
        if (achievementsText != null)
        {
            int unlocked = statsManager.GetAchievementsUnlocked();
            int points = statsManager.GetTotalAchievementPoints();
            achievementsText.text = $"{unlocked} ({points} pts)";
        }
        
        if (favoriteClassText != null)
            favoriteClassText.text = statsManager.GetFavoriteClass();
    }

    /// <summary>
    /// Refreshes the combat tab with detailed combat statistics.
    /// </summary>
    private void RefreshCombatTab()
    {
        if (statsManager == null) return;
        
        if (combatWinsText != null)
            combatWinsText.text = statsManager.GetTotalCombatsWon().ToString();
        
        if (combatLossesText != null)
            combatLossesText.text = statsManager.GetTotalCombatsLost().ToString();
        
        if (combatFleesText != null)
            combatFleesText.text = statsManager.GetTotalCombatsFled().ToString();
        
        if (winRateDetailedText != null)
            winRateDetailedText.text = $"{statsManager.GetWinRate():F2}%";
        
        if (damageDealtText != null)
            damageDealtText.text = statsManager.GetTotalDamageDealt().ToString("N0");
        
        if (damageTakenText != null)
            damageTakenText.text = statsManager.GetTotalDamageTaken().ToString("N0");
        
        if (healingText != null)
            healingText.text = statsManager.GetTotalHealing().ToString("N0");
        
        if (criticalHitsText != null)
            criticalHitsText.text = statsManager.GetTotalCriticalHits().ToString();
        
        if (flawlessVictoriesText != null)
            flawlessVictoriesText.text = statsManager.GetFlawlessVictories().ToString();
        
        if (abilityUsesText != null)
            abilityUsesText.text = statsManager.GetTotalAbilitiesUsed().ToString();
        
        if (favoriteAbilityText != null)
            favoriteAbilityText.text = statsManager.GetFavoriteAbility();
        
        if (mostDefeatedEnemyText != null)
            mostDefeatedEnemyText.text = statsManager.GetMostDefeatedEnemy();
    }

    /// <summary>
    /// Refreshes the exploration tab with world exploration statistics.
    /// </summary>
    private void RefreshExplorationTab()
    {
        if (statsManager == null) return;
        
        if (locationsVisitedText != null)
            locationsVisitedText.text = statsManager.GetTotalLocationsVisited().ToString();
        
        if (worldProgressText != null)
            worldProgressText.text = $"{statsManager.GetWorldCompletionPercentage():F1}%";
        
        if (travelCountText != null)
            travelCountText.text = statsManager.GetTotalTravelCount().ToString();
        
        if (questsStartedText != null)
            questsStartedText.text = statsManager.GetTotalQuestsStarted().ToString();
        
        if (questsCompletedText != null)
            questsCompletedText.text = statsManager.GetTotalQuestsCompleted().ToString();
        
        if (questCompletionRateText != null)
            questCompletionRateText.text = $"{statsManager.GetQuestCompletionRate():F1}%";
        
        if (worldCompletionSlider != null)
            worldCompletionSlider.value = statsManager.GetWorldCompletionPercentage() / 100f;
    }

    /// <summary>
    /// Refreshes the economy tab with gold and trading statistics.
    /// </summary>
    private void RefreshEconomyTab()
    {
        if (statsManager == null) return;
        
        if (goldEarnedText != null)
            goldEarnedText.text = statsManager.GetTotalGoldEarned().ToString("N0");
        
        if (goldSpentText != null)
            goldSpentText.text = statsManager.GetTotalGoldSpent().ToString("N0");
        
        if (netGoldText != null)
        {
            long netGold = statsManager.GetNetGold();
            netGoldText.text = netGold.ToString("N0");
            
            // Color based on positive/negative
            if (netGold >= 0)
                netGoldText.color = new Color(0.2f, 0.8f, 0.2f); // Green
            else
                netGoldText.color = new Color(0.8f, 0.2f, 0.2f); // Red
        }
        
        if (itemsCraftedText != null)
            itemsCraftedText.text = statsManager.GetTotalItemsCrafted().ToString();
        
        if (itemsPurchasedText != null)
            itemsPurchasedText.text = statsManager.GetTotalItemsPurchased().ToString();
        
        if (itemsSoldText != null)
            itemsSoldText.text = statsManager.GetTotalItemsSold().ToString();
        
        if (craftingEfficiencyText != null)
            craftingEfficiencyText.text = $"{statsManager.GetCraftingEfficiency():F1}%";
        
        if (merchantInteractionsText != null)
            merchantInteractionsText.text = statsManager.GetMerchantInteractions().ToString();
        
        if (craftingEfficiencySlider != null)
            craftingEfficiencySlider.value = statsManager.GetCraftingEfficiency() / 100f;
    }

    /// <summary>
    /// Refreshes the character tab with character progression statistics.
    /// </summary>
    private void RefreshCharacterTab()
    {
        if (statsManager == null) return;
        
        if (currentClassText != null)
            currentClassText.text = statsManager.GetCurrentClass();
        
        if (favoriteClassDetailedText != null)
            favoriteClassDetailedText.text = statsManager.GetFavoriteClass();
        
        if (levelsGainedText != null)
            levelsGainedText.text = statsManager.GetTotalLevelsGained().ToString();
        
        if (totalXPText != null)
            totalXPText.text = statsManager.GetTotalXPEarned().ToString("N0");
        
        if (deathsText != null)
            deathsText.text = statsManager.GetTotalDeaths().ToString();
        
        if (companionsRecruitedText != null)
            companionsRecruitedText.text = statsManager.GetCompanionsRecruited().ToString();
        
        if (favoriteCompanionText != null)
            favoriteCompanionText.text = statsManager.GetFavoriteCompanion();
        
        if (dialogueChoicesText != null)
            dialogueChoicesText.text = statsManager.GetDialogueChoicesMade().ToString();
        
        if (achievementsUnlockedText != null)
            achievementsUnlockedText.text = statsManager.GetAchievementsUnlocked().ToString();
        
        if (achievementPointsText != null)
            achievementPointsText.text = statsManager.GetTotalAchievementPoints().ToString("N0");
    }

    /// <summary>
    /// Exports statistics as a text file.
    /// </summary>
    private void ExportStatistics()
    {
        if (statsManager == null) return;
        
        string exportText = statsManager.ExportStatisticsAsText();
        string filename = $"ACOTAR_Statistics_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        string path = System.IO.Path.Combine(Application.persistentDataPath, filename);
        
        try
        {
            System.IO.File.WriteAllText(path, exportText);
            Debug.Log($"Statistics exported to: {path}");
            
            // Show success message to player
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowNotification($"Statistics exported to:\n{filename}", 5f);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to export statistics: {ex.Message}");
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowNotification("Failed to export statistics", 3f);
            }
        }
    }

    /// <summary>
    /// Shows confirmation dialog before resetting statistics.
    /// </summary>
    private void ShowResetConfirmation()
    {
        // In a real implementation, this would show a confirmation dialog
        // For now, we'll just log a warning
        Debug.LogWarning("Reset statistics requested - implement confirmation dialog");
        
        // Example of how it might work:
        // DialogueSystem.Instance.ShowConfirmation(
        //     "Reset Statistics",
        //     "Are you sure you want to reset all statistics? This action cannot be undone.",
        //     OnConfirmReset
        // );
    }

    /// <summary>
    /// Callback for confirmed statistics reset.
    /// </summary>
    private void OnConfirmReset()
    {
        if (statsManager != null)
        {
            statsManager.ResetAllStatistics();
            RefreshAllStatistics();
            Debug.Log("Statistics reset successfully");
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowNotification("Statistics have been reset", 3f);
            }
        }
    }

    /// <summary>
    /// Public method to refresh statistics without opening panel.
    /// Useful for updating stats in the background.
    /// </summary>
    public void RefreshStatistics()
    {
        if (statisticsPanel != null && statisticsPanel.activeSelf)
        {
            RefreshAllStatistics();
        }
    }

    /// <summary>
    /// Toggles the statistics panel open/closed.
    /// </summary>
    public void TogglePanel()
    {
        if (statisticsPanel != null)
        {
            if (statisticsPanel.activeSelf)
                ClosePanel();
            else
                OpenPanel();
        }
    }
}
