using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages comprehensive player statistics and analytics.
/// Tracks playtime, combat performance, exploration progress, economic activity, and more.
/// Singleton pattern for global access.
/// </summary>
public class StatisticsManager : MonoBehaviour
{
    private static StatisticsManager instance;
    public static StatisticsManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("StatisticsManager");
                instance = go.AddComponent<StatisticsManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    // Playtime Statistics
    private float totalPlaytimeSeconds = 0f;
    private float currentSessionStartTime = 0f;
    private int sessionCount = 0;
    private DateTime lastPlayDate;
    
    // Combat Statistics
    private int totalCombatsWon = 0;
    private int totalCombatsLost = 0;
    private int totalCombatsFled = 0;
    private long totalDamageDealt = 0;
    private long totalDamageTaken = 0;
    private long totalHealing = 0;
    private int totalCriticalHits = 0;
    private int totalAbilitiesUsed = 0;
    private Dictionary<string, int> abilityUsageCount = new Dictionary<string, int>();
    private Dictionary<string, int> enemiesDefeated = new Dictionary<string, int>();
    private int flawlessVictories = 0;
    private int perfectDefenses = 0;
    
    // Exploration Statistics
    private int totalLocationsVisited = 0;
    private HashSet<string> visitedLocations = new HashSet<string>();
    private int totalTravelCount = 0;
    private int totalQuestsCompleted = 0;
    private int totalQuestsStarted = 0;
    private Dictionary<string, bool> locationDiscovery = new Dictionary<string, bool>();
    private float worldCompletionPercentage = 0f;
    
    // Economic Statistics
    private long totalGoldEarned = 0;
    private long totalGoldSpent = 0;
    private int totalItemsCrafted = 0;
    private int totalItemsPurchased = 0;
    private int totalItemsSold = 0;
    private Dictionary<string, int> craftedItems = new Dictionary<string, int>();
    private int merchantInteractions = 0;
    
    // Character Statistics
    private int totalLevelsGained = 0;
    private int totalXPEarned = 0;
    private int totalDeaths = 0;
    private Dictionary<string, int> classPlaytime = new Dictionary<string, int>();
    private string currentClass = "";
    private string favoriteClass = "";
    
    // Achievement Statistics
    private int achievementsUnlocked = 0;
    private int totalAchievementPoints = 0;
    private DateTime firstAchievementDate;
    private DateTime lastAchievementDate;
    
    // Social Statistics
    private int companionsRecruited = 0;
    private Dictionary<string, int> companionBattles = new Dictionary<string, int>();
    private int dialogueChoicesMade = 0;
    
    private const string STATS_SAVE_KEY = "PlayerStatistics_v1";

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        LoadStatistics();
        StartNewSession();
    }

    void OnApplicationQuit()
    {
        EndCurrentSession();
        SaveStatistics();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            EndCurrentSession();
            SaveStatistics();
        }
        else
        {
            StartNewSession();
        }
    }

    #region Session Management

    /// <summary>
    /// Starts a new play session.
    /// </summary>
    private void StartNewSession()
    {
        currentSessionStartTime = Time.realtimeSinceStartup;
        sessionCount++;
        lastPlayDate = DateTime.Now;
    }

    /// <summary>
    /// Ends the current play session and updates total playtime.
    /// </summary>
    private void EndCurrentSession()
    {
        if (currentSessionStartTime > 0)
        {
            float sessionDuration = Time.realtimeSinceStartup - currentSessionStartTime;
            totalPlaytimeSeconds += sessionDuration;
            currentSessionStartTime = 0;
        }
    }

    /// <summary>
    /// Gets the current session playtime in seconds.
    /// </summary>
    public float GetCurrentSessionTime()
    {
        if (currentSessionStartTime > 0)
        {
            return Time.realtimeSinceStartup - currentSessionStartTime;
        }
        return 0f;
    }

    /// <summary>
    /// Gets total playtime in seconds including current session.
    /// </summary>
    public float GetTotalPlaytime()
    {
        return totalPlaytimeSeconds + GetCurrentSessionTime();
    }

    /// <summary>
    /// Gets total playtime formatted as "HH:MM:SS".
    /// </summary>
    public string GetFormattedPlaytime()
    {
        float totalSeconds = GetTotalPlaytime();
        int hours = Mathf.FloorToInt(totalSeconds / 3600f);
        int minutes = Mathf.FloorToInt((totalSeconds % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    /// <summary>
    /// Gets average session length in seconds.
    /// </summary>
    public float GetAverageSessionLength()
    {
        if (sessionCount == 0) return 0f;
        return totalPlaytimeSeconds / sessionCount;
    }

    #endregion

    #region Combat Statistics

    /// <summary>
    /// Records a combat victory.
    /// </summary>
    public void RecordCombatWin(bool flawless = false)
    {
        totalCombatsWon++;
        if (flawless)
        {
            flawlessVictories++;
        }
    }

    /// <summary>
    /// Records a combat loss.
    /// </summary>
    public void RecordCombatLoss()
    {
        totalCombatsLost++;
    }

    /// <summary>
    /// Records a successful flee from combat.
    /// </summary>
    public void RecordCombatFlee()
    {
        totalCombatsFled++;
    }

    /// <summary>
    /// Records damage dealt in combat.
    /// </summary>
    public void RecordDamageDealt(int amount)
    {
        totalDamageDealt += amount;
    }

    /// <summary>
    /// Records damage taken in combat.
    /// </summary>
    public void RecordDamageTaken(int amount)
    {
        totalDamageTaken += amount;
    }

    /// <summary>
    /// Records healing received.
    /// </summary>
    public void RecordHealing(int amount)
    {
        totalHealing += amount;
    }

    /// <summary>
    /// Records a critical hit.
    /// </summary>
    public void RecordCriticalHit()
    {
        totalCriticalHits++;
    }

    /// <summary>
    /// Records ability usage.
    /// </summary>
    public void RecordAbilityUsed(string abilityName)
    {
        totalAbilitiesUsed++;
        if (!abilityUsageCount.ContainsKey(abilityName))
        {
            abilityUsageCount[abilityName] = 0;
        }
        abilityUsageCount[abilityName]++;
    }

    /// <summary>
    /// Records an enemy defeat.
    /// </summary>
    public void RecordEnemyDefeated(string enemyType)
    {
        if (!enemiesDefeated.ContainsKey(enemyType))
        {
            enemiesDefeated[enemyType] = 0;
        }
        enemiesDefeated[enemyType]++;
    }

    /// <summary>
    /// Records a perfect defense (blocked/dodged all attacks in combat).
    /// </summary>
    public void RecordPerfectDefense()
    {
        perfectDefenses++;
    }

    /// <summary>
    /// Gets combat win rate as a percentage (0-100).
    /// </summary>
    public float GetWinRate()
    {
        int totalCombats = totalCombatsWon + totalCombatsLost;
        if (totalCombats == 0) return 0f;
        return (totalCombatsWon / (float)totalCombats) * 100f;
    }

    /// <summary>
    /// Gets the most used ability name.
    /// </summary>
    public string GetFavoriteAbility()
    {
        string favorite = "None";
        int maxUses = 0;
        foreach (var kvp in abilityUsageCount)
        {
            if (kvp.Value > maxUses)
            {
                maxUses = kvp.Value;
                favorite = kvp.Key;
            }
        }
        return favorite;
    }

    /// <summary>
    /// Gets the most defeated enemy type.
    /// </summary>
    public string GetMostDefeatedEnemy()
    {
        string mostDefeated = "None";
        int maxDefeats = 0;
        foreach (var kvp in enemiesDefeated)
        {
            if (kvp.Value > maxDefeats)
            {
                maxDefeats = kvp.Value;
                mostDefeated = kvp.Key;
            }
        }
        return mostDefeated;
    }

    #endregion

    #region Exploration Statistics

    /// <summary>
    /// Records a location visit.
    /// </summary>
    public void RecordLocationVisit(string locationName)
    {
        if (!visitedLocations.Contains(locationName))
        {
            visitedLocations.Add(locationName);
            totalLocationsVisited++;
        }
    }

    /// <summary>
    /// Records a travel action.
    /// </summary>
    public void RecordTravel()
    {
        totalTravelCount++;
    }

    /// <summary>
    /// Records a quest start.
    /// </summary>
    public void RecordQuestStarted()
    {
        totalQuestsStarted++;
    }

    /// <summary>
    /// Records a quest completion.
    /// </summary>
    public void RecordQuestCompleted()
    {
        totalQuestsCompleted++;
    }

    /// <summary>
    /// Updates world completion percentage (should be called when locations are unlocked).
    /// </summary>
    public void UpdateWorldCompletion(float percentage)
    {
        worldCompletionPercentage = Mathf.Clamp(percentage, 0f, 100f);
    }

    /// <summary>
    /// Gets quest completion rate as a percentage.
    /// </summary>
    public float GetQuestCompletionRate()
    {
        if (totalQuestsStarted == 0) return 0f;
        return (totalQuestsCompleted / (float)totalQuestsStarted) * 100f;
    }

    #endregion

    #region Economic Statistics

    /// <summary>
    /// Records gold earned.
    /// </summary>
    public void RecordGoldEarned(int amount)
    {
        totalGoldEarned += amount;
    }

    /// <summary>
    /// Records gold spent.
    /// </summary>
    public void RecordGoldSpent(int amount)
    {
        totalGoldSpent += amount;
    }

    /// <summary>
    /// Records an item craft.
    /// </summary>
    public void RecordItemCrafted(string itemName)
    {
        totalItemsCrafted++;
        if (!craftedItems.ContainsKey(itemName))
        {
            craftedItems[itemName] = 0;
        }
        craftedItems[itemName]++;
    }

    /// <summary>
    /// Records an item purchase.
    /// </summary>
    public void RecordItemPurchased()
    {
        totalItemsPurchased++;
    }

    /// <summary>
    /// Records an item sale.
    /// </summary>
    public void RecordItemSold()
    {
        totalItemsSold++;
    }

    /// <summary>
    /// Records a merchant interaction.
    /// </summary>
    public void RecordMerchantInteraction()
    {
        merchantInteractions++;
    }

    /// <summary>
    /// Gets net gold (earned - spent).
    /// </summary>
    public long GetNetGold()
    {
        return totalGoldEarned - totalGoldSpent;
    }

    /// <summary>
    /// Gets crafting efficiency (% of items crafted vs purchased).
    /// </summary>
    public float GetCraftingEfficiency()
    {
        int totalAcquired = totalItemsCrafted + totalItemsPurchased;
        if (totalAcquired == 0) return 0f;
        return (totalItemsCrafted / (float)totalAcquired) * 100f;
    }

    #endregion

    #region Character Statistics

    /// <summary>
    /// Records a level gain.
    /// </summary>
    public void RecordLevelGain()
    {
        totalLevelsGained++;
    }

    /// <summary>
    /// Records XP earned.
    /// </summary>
    public void RecordXPEarned(int amount)
    {
        totalXPEarned += amount;
    }

    /// <summary>
    /// Records a character death.
    /// </summary>
    public void RecordDeath()
    {
        totalDeaths++;
    }

    /// <summary>
    /// Sets the current character class being played.
    /// </summary>
    public void SetCurrentClass(string className)
    {
        currentClass = className;
        if (!classPlaytime.ContainsKey(className))
        {
            classPlaytime[className] = 0;
        }
    }

    /// <summary>
    /// Gets the most played class.
    /// </summary>
    public string GetFavoriteClass()
    {
        string favorite = "None";
        int maxTime = 0;
        foreach (var kvp in classPlaytime)
        {
            if (kvp.Value > maxTime)
            {
                maxTime = kvp.Value;
                favorite = kvp.Key;
            }
        }
        return favorite;
    }

    #endregion

    #region Achievement Statistics

    /// <summary>
    /// Records an achievement unlock.
    /// </summary>
    public void RecordAchievementUnlocked(int points)
    {
        achievementsUnlocked++;
        totalAchievementPoints += points;
        
        if (achievementsUnlocked == 1)
        {
            firstAchievementDate = DateTime.Now;
        }
        lastAchievementDate = DateTime.Now;
    }

    #endregion

    #region Social Statistics

    /// <summary>
    /// Records a companion recruitment.
    /// </summary>
    public void RecordCompanionRecruited(string companionName)
    {
        companionsRecruited++;
        if (!companionBattles.ContainsKey(companionName))
        {
            companionBattles[companionName] = 0;
        }
    }

    /// <summary>
    /// Records a battle with a companion.
    /// </summary>
    public void RecordCompanionBattle(string companionName)
    {
        if (!companionBattles.ContainsKey(companionName))
        {
            companionBattles[companionName] = 0;
        }
        companionBattles[companionName]++;
    }

    /// <summary>
    /// Records a dialogue choice made.
    /// </summary>
    public void RecordDialogueChoice()
    {
        dialogueChoicesMade++;
    }

    /// <summary>
    /// Gets the most used companion in battles.
    /// </summary>
    public string GetFavoriteCompanion()
    {
        string favorite = "None";
        int maxBattles = 0;
        foreach (var kvp in companionBattles)
        {
            if (kvp.Value > maxBattles)
            {
                maxBattles = kvp.Value;
                favorite = kvp.Key;
            }
        }
        return favorite;
    }

    #endregion

    #region Data Access Methods

    /// <summary>
    /// Gets all statistics as a formatted string for export.
    /// </summary>
    public string ExportStatisticsAsText()
    {
        string export = "=== ACOTAR RPG - Player Statistics ===\n\n";
        
        export += "--- PLAYTIME ---\n";
        export += $"Total Playtime: {GetFormattedPlaytime()}\n";
        export += $"Session Count: {sessionCount}\n";
        export += $"Average Session: {GetFormattedTime(GetAverageSessionLength())}\n";
        export += $"Last Played: {lastPlayDate:yyyy-MM-dd HH:mm}\n\n";
        
        export += "--- COMBAT ---\n";
        export += $"Wins: {totalCombatsWon}\n";
        export += $"Losses: {totalCombatsLost}\n";
        export += $"Flees: {totalCombatsFled}\n";
        export += $"Win Rate: {GetWinRate():F1}%\n";
        export += $"Damage Dealt: {totalDamageDealt:N0}\n";
        export += $"Damage Taken: {totalDamageTaken:N0}\n";
        export += $"Healing: {totalHealing:N0}\n";
        export += $"Critical Hits: {totalCriticalHits}\n";
        export += $"Flawless Victories: {flawlessVictories}\n";
        export += $"Favorite Ability: {GetFavoriteAbility()}\n";
        export += $"Most Defeated Enemy: {GetMostDefeatedEnemy()}\n\n";
        
        export += "--- EXPLORATION ---\n";
        export += $"Locations Visited: {totalLocationsVisited}\n";
        export += $"Travel Count: {totalTravelCount}\n";
        export += $"Quests Started: {totalQuestsStarted}\n";
        export += $"Quests Completed: {totalQuestsCompleted}\n";
        export += $"Completion Rate: {GetQuestCompletionRate():F1}%\n";
        export += $"World Completion: {worldCompletionPercentage:F1}%\n\n";
        
        export += "--- ECONOMY ---\n";
        export += $"Gold Earned: {totalGoldEarned:N0}\n";
        export += $"Gold Spent: {totalGoldSpent:N0}\n";
        export += $"Net Gold: {GetNetGold():N0}\n";
        export += $"Items Crafted: {totalItemsCrafted}\n";
        export += $"Items Purchased: {totalItemsPurchased}\n";
        export += $"Items Sold: {totalItemsSold}\n";
        export += $"Crafting Efficiency: {GetCraftingEfficiency():F1}%\n";
        export += $"Merchant Interactions: {merchantInteractions}\n\n";
        
        export += "--- CHARACTER ---\n";
        export += $"Current Class: {currentClass}\n";
        export += $"Favorite Class: {GetFavoriteClass()}\n";
        export += $"Levels Gained: {totalLevelsGained}\n";
        export += $"Total XP: {totalXPEarned:N0}\n";
        export += $"Deaths: {totalDeaths}\n\n";
        
        export += "--- ACHIEVEMENTS ---\n";
        export += $"Unlocked: {achievementsUnlocked}\n";
        export += $"Total Points: {totalAchievementPoints}\n\n";
        
        export += "--- SOCIAL ---\n";
        export += $"Companions Recruited: {companionsRecruited}\n";
        export += $"Favorite Companion: {GetFavoriteCompanion()}\n";
        export += $"Dialogue Choices: {dialogueChoicesMade}\n\n";
        
        export += $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n";
        
        return export;
    }

    private string GetFormattedTime(float seconds)
    {
        int hours = Mathf.FloorToInt(seconds / 3600f);
        int minutes = Mathf.FloorToInt((seconds % 3600f) / 60f);
        return string.Format("{0:00}:{1:00}", hours, minutes);
    }

    #endregion

    #region Save/Load

    /// <summary>
    /// Saves statistics to PlayerPrefs.
    /// </summary>
    public void SaveStatistics()
    {
        PlayerPrefs.SetFloat("Stats_TotalPlaytime", totalPlaytimeSeconds);
        PlayerPrefs.SetInt("Stats_SessionCount", sessionCount);
        PlayerPrefs.SetString("Stats_LastPlayDate", lastPlayDate.ToString("o"));
        
        PlayerPrefs.SetInt("Stats_CombatsWon", totalCombatsWon);
        PlayerPrefs.SetInt("Stats_CombatsLost", totalCombatsLost);
        PlayerPrefs.SetInt("Stats_CombatsFled", totalCombatsFled);
        PlayerPrefs.SetString("Stats_DamageDealt", totalDamageDealt.ToString());
        PlayerPrefs.SetString("Stats_DamageTaken", totalDamageTaken.ToString());
        PlayerPrefs.SetString("Stats_Healing", totalHealing.ToString());
        PlayerPrefs.SetInt("Stats_CriticalHits", totalCriticalHits);
        PlayerPrefs.SetInt("Stats_AbilitiesUsed", totalAbilitiesUsed);
        PlayerPrefs.SetInt("Stats_FlawlessVictories", flawlessVictories);
        
        PlayerPrefs.SetInt("Stats_LocationsVisited", totalLocationsVisited);
        PlayerPrefs.SetInt("Stats_TravelCount", totalTravelCount);
        PlayerPrefs.SetInt("Stats_QuestsStarted", totalQuestsStarted);
        PlayerPrefs.SetInt("Stats_QuestsCompleted", totalQuestsCompleted);
        PlayerPrefs.SetFloat("Stats_WorldCompletion", worldCompletionPercentage);
        
        PlayerPrefs.SetString("Stats_GoldEarned", totalGoldEarned.ToString());
        PlayerPrefs.SetString("Stats_GoldSpent", totalGoldSpent.ToString());
        PlayerPrefs.SetInt("Stats_ItemsCrafted", totalItemsCrafted);
        PlayerPrefs.SetInt("Stats_ItemsPurchased", totalItemsPurchased);
        PlayerPrefs.SetInt("Stats_ItemsSold", totalItemsSold);
        PlayerPrefs.SetInt("Stats_MerchantInteractions", merchantInteractions);
        
        PlayerPrefs.SetInt("Stats_LevelsGained", totalLevelsGained);
        PlayerPrefs.SetInt("Stats_XPEarned", totalXPEarned);
        PlayerPrefs.SetInt("Stats_Deaths", totalDeaths);
        PlayerPrefs.SetString("Stats_CurrentClass", currentClass);
        
        PlayerPrefs.SetInt("Stats_AchievementsUnlocked", achievementsUnlocked);
        PlayerPrefs.SetInt("Stats_AchievementPoints", totalAchievementPoints);
        
        PlayerPrefs.SetInt("Stats_CompanionsRecruited", companionsRecruited);
        PlayerPrefs.SetInt("Stats_DialogueChoices", dialogueChoicesMade);
        
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads statistics from PlayerPrefs.
    /// </summary>
    public void LoadStatistics()
    {
        totalPlaytimeSeconds = PlayerPrefs.GetFloat("Stats_TotalPlaytime", 0f);
        sessionCount = PlayerPrefs.GetInt("Stats_SessionCount", 0);
        
        string dateString = PlayerPrefs.GetString("Stats_LastPlayDate", DateTime.Now.ToString("o"));
        DateTime.TryParse(dateString, out lastPlayDate);
        
        totalCombatsWon = PlayerPrefs.GetInt("Stats_CombatsWon", 0);
        totalCombatsLost = PlayerPrefs.GetInt("Stats_CombatsLost", 0);
        totalCombatsFled = PlayerPrefs.GetInt("Stats_CombatsFled", 0);
        long.TryParse(PlayerPrefs.GetString("Stats_DamageDealt", "0"), out totalDamageDealt);
        long.TryParse(PlayerPrefs.GetString("Stats_DamageTaken", "0"), out totalDamageTaken);
        long.TryParse(PlayerPrefs.GetString("Stats_Healing", "0"), out totalHealing);
        totalCriticalHits = PlayerPrefs.GetInt("Stats_CriticalHits", 0);
        totalAbilitiesUsed = PlayerPrefs.GetInt("Stats_AbilitiesUsed", 0);
        flawlessVictories = PlayerPrefs.GetInt("Stats_FlawlessVictories", 0);
        
        totalLocationsVisited = PlayerPrefs.GetInt("Stats_LocationsVisited", 0);
        totalTravelCount = PlayerPrefs.GetInt("Stats_TravelCount", 0);
        totalQuestsStarted = PlayerPrefs.GetInt("Stats_QuestsStarted", 0);
        totalQuestsCompleted = PlayerPrefs.GetInt("Stats_QuestsCompleted", 0);
        worldCompletionPercentage = PlayerPrefs.GetFloat("Stats_WorldCompletion", 0f);
        
        long.TryParse(PlayerPrefs.GetString("Stats_GoldEarned", "0"), out totalGoldEarned);
        long.TryParse(PlayerPrefs.GetString("Stats_GoldSpent", "0"), out totalGoldSpent);
        totalItemsCrafted = PlayerPrefs.GetInt("Stats_ItemsCrafted", 0);
        totalItemsPurchased = PlayerPrefs.GetInt("Stats_ItemsPurchased", 0);
        totalItemsSold = PlayerPrefs.GetInt("Stats_ItemsSold", 0);
        merchantInteractions = PlayerPrefs.GetInt("Stats_MerchantInteractions", 0);
        
        totalLevelsGained = PlayerPrefs.GetInt("Stats_LevelsGained", 0);
        totalXPEarned = PlayerPrefs.GetInt("Stats_XPEarned", 0);
        totalDeaths = PlayerPrefs.GetInt("Stats_Deaths", 0);
        currentClass = PlayerPrefs.GetString("Stats_CurrentClass", "");
        
        achievementsUnlocked = PlayerPrefs.GetInt("Stats_AchievementsUnlocked", 0);
        totalAchievementPoints = PlayerPrefs.GetInt("Stats_AchievementPoints", 0);
        
        companionsRecruited = PlayerPrefs.GetInt("Stats_CompanionsRecruited", 0);
        dialogueChoicesMade = PlayerPrefs.GetInt("Stats_DialogueChoices", 0);
    }

    /// <summary>
    /// Resets all statistics to default values.
    /// </summary>
    public void ResetAllStatistics()
    {
        totalPlaytimeSeconds = 0f;
        sessionCount = 0;
        totalCombatsWon = 0;
        totalCombatsLost = 0;
        totalCombatsFled = 0;
        totalDamageDealt = 0;
        totalDamageTaken = 0;
        totalHealing = 0;
        totalCriticalHits = 0;
        totalAbilitiesUsed = 0;
        abilityUsageCount.Clear();
        enemiesDefeated.Clear();
        flawlessVictories = 0;
        
        totalLocationsVisited = 0;
        visitedLocations.Clear();
        totalTravelCount = 0;
        totalQuestsCompleted = 0;
        totalQuestsStarted = 0;
        worldCompletionPercentage = 0f;
        
        totalGoldEarned = 0;
        totalGoldSpent = 0;
        totalItemsCrafted = 0;
        totalItemsPurchased = 0;
        totalItemsSold = 0;
        craftedItems.Clear();
        merchantInteractions = 0;
        
        totalLevelsGained = 0;
        totalXPEarned = 0;
        totalDeaths = 0;
        classPlaytime.Clear();
        currentClass = "";
        
        achievementsUnlocked = 0;
        totalAchievementPoints = 0;
        
        companionsRecruited = 0;
        companionBattles.Clear();
        dialogueChoicesMade = 0;
        
        SaveStatistics();
    }

    #endregion

    #region Public Getters for UI

    public int GetTotalCombatsWon() => totalCombatsWon;
    public int GetTotalCombatsLost() => totalCombatsLost;
    public int GetTotalCombatsFled() => totalCombatsFled;
    public long GetTotalDamageDealt() => totalDamageDealt;
    public long GetTotalDamageTaken() => totalDamageTaken;
    public long GetTotalHealing() => totalHealing;
    public int GetTotalCriticalHits() => totalCriticalHits;
    public int GetTotalAbilitiesUsed() => totalAbilitiesUsed;
    public int GetFlawlessVictories() => flawlessVictories;
    public int GetPerfectDefenses() => perfectDefenses;
    
    public int GetTotalLocationsVisited() => totalLocationsVisited;
    public int GetTotalTravelCount() => totalTravelCount;
    public int GetTotalQuestsStarted() => totalQuestsStarted;
    public int GetTotalQuestsCompleted() => totalQuestsCompleted;
    public float GetWorldCompletionPercentage() => worldCompletionPercentage;
    
    public long GetTotalGoldEarned() => totalGoldEarned;
    public long GetTotalGoldSpent() => totalGoldSpent;
    public int GetTotalItemsCrafted() => totalItemsCrafted;
    public int GetTotalItemsPurchased() => totalItemsPurchased;
    public int GetTotalItemsSold() => totalItemsSold;
    public int GetMerchantInteractions() => merchantInteractions;
    
    public int GetTotalLevelsGained() => totalLevelsGained;
    public int GetTotalXPEarned() => totalXPEarned;
    public int GetTotalDeaths() => totalDeaths;
    public string GetCurrentClass() => currentClass;
    
    public int GetAchievementsUnlocked() => achievementsUnlocked;
    public int GetTotalAchievementPoints() => totalAchievementPoints;
    
    public int GetCompanionsRecruited() => companionsRecruited;
    public int GetDialogueChoicesMade() => dialogueChoicesMade;
    public int GetSessionCount() => sessionCount;

    #endregion
}
