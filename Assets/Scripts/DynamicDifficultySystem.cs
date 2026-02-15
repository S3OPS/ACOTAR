using UnityEngine;
using System.Collections.Generic;
using ACOTAR;

/// <summary>
/// Dynamic Difficulty Scaling System
/// Adapts game difficulty based on player performance in real-time.
/// Provides custom difficulty tuning and optional Ironman mode.
/// </summary>
public class DynamicDifficultySystem : MonoBehaviour
{
    private static DynamicDifficultySystem instance;
    public static DynamicDifficultySystem Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("DynamicDifficultySystem");
                instance = go.AddComponent<DynamicDifficultySystem>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    [Header("Adaptive Difficulty Settings")]
    [SerializeField] private bool enableAdaptiveDifficulty = false;
    [SerializeField] private float performanceCheckInterval = 300f; // 5 minutes
    [SerializeField] private int combatsToTrack = 10; // Last N combats to consider
    
    [Header("Difficulty Thresholds")]
    [SerializeField] private float easyThreshold = 0.8f; // 80%+ win rate = too easy
    [SerializeField] private float hardThreshold = 0.3f; // 30%- win rate = too hard
    [SerializeField] private float adjustmentRate = 0.05f; // 5% adjustment per check
    
    [Header("Custom Difficulty Multipliers")]
    [SerializeField] private float enemyHealthMultiplier = 1.0f;
    [SerializeField] private float enemyDamageMultiplier = 1.0f;
    [SerializeField] private float xpMultiplier = 1.0f;
    [SerializeField] private float goldMultiplier = 1.0f;
    
    [Header("Ironman Mode")]
    [SerializeField] private bool ironmanMode = false;
    [SerializeField] private bool allowContinueAfterDeath = false;
    [SerializeField] private int livesRemaining = 1;
    
    // Performance tracking
    private Queue<bool> recentCombatResults = new Queue<bool>(); // true = win, false = loss
    private float lastPerformanceCheck = 0f;
    private int totalAdaptiveAdjustments = 0;
    
    // Difficulty profile
    private string difficultyProfile = "Normal";
    private Dictionary<string, DifficultyPreset> difficultyPresets;
    
    // Save keys
    private const string ADAPTIVE_ENABLED_KEY = "DDS_AdaptiveEnabled";
    private const string ENEMY_HP_KEY = "DDS_EnemyHP";
    private const string ENEMY_DMG_KEY = "DDS_EnemyDMG";
    private const string XP_MULT_KEY = "DDS_XPMult";
    private const string GOLD_MULT_KEY = "DDS_GoldMult";
    private const string IRONMAN_KEY = "DDS_Ironman";
    private const string PROFILE_KEY = "DDS_Profile";

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeDifficultyPresets();
        LoadSettings();
    }

    void Update()
    {
        if (enableAdaptiveDifficulty)
        {
            // Check performance periodically
            if (Time.time - lastPerformanceCheck >= performanceCheckInterval)
            {
                EvaluatePerformanceAndAdjust();
                lastPerformanceCheck = Time.time;
            }
        }
    }

    #region Difficulty Presets

    /// <summary>
    /// Initializes default difficulty presets.
    /// </summary>
    private void InitializeDifficultyPresets()
    {
        difficultyPresets = new Dictionary<string, DifficultyPreset>
        {
            { "Story", new DifficultyPreset(0.5f, 0.5f, 1.5f, 1.5f) },
            { "Easy", new DifficultyPreset(0.75f, 0.75f, 1.25f, 1.25f) },
            { "Normal", new DifficultyPreset(1.0f, 1.0f, 1.0f, 1.0f) },
            { "Hard", new DifficultyPreset(1.5f, 1.3f, 0.8f, 0.8f) },
            { "Nightmare", new DifficultyPreset(2.0f, 1.5f, 0.6f, 0.6f) },
            { "Custom", new DifficultyPreset(1.0f, 1.0f, 1.0f, 1.0f) }
        };
    }

    /// <summary>
    /// Applies a difficulty preset.
    /// </summary>
    public void ApplyDifficultyPreset(string presetName)
    {
        if (difficultyPresets.ContainsKey(presetName))
        {
            DifficultyPreset preset = difficultyPresets[presetName];
            enemyHealthMultiplier = preset.enemyHealth;
            enemyDamageMultiplier = preset.enemyDamage;
            xpMultiplier = preset.xpReward;
            goldMultiplier = preset.goldReward;
            difficultyProfile = presetName;
            
            Debug.Log($"Applied difficulty preset: {presetName}");
            SaveSettings();
            
            // Trigger event for game systems to update
            GameEvents.TriggerDifficultyChanged();
        }
        else
        {
            Debug.LogWarning($"Difficulty preset '{presetName}' not found");
        }
    }

    /// <summary>
    /// Gets the current difficulty preset name.
    /// </summary>
    public string GetCurrentPreset()
    {
        return difficultyProfile;
    }

    #endregion

    #region Adaptive Difficulty

    /// <summary>
    /// Records a combat result for adaptive difficulty tracking.
    /// </summary>
    public void RecordCombatResult(bool playerWon)
    {
        if (!enableAdaptiveDifficulty) return;
        
        recentCombatResults.Enqueue(playerWon);
        
        // Keep only recent combats
        while (recentCombatResults.Count > combatsToTrack)
        {
            recentCombatResults.Dequeue();
        }
    }

    /// <summary>
    /// Evaluates player performance and adjusts difficulty if needed.
    /// </summary>
    private void EvaluatePerformanceAndAdjust()
    {
        if (recentCombatResults.Count < combatsToTrack)
        {
            // Not enough data yet
            return;
        }
        
        // Calculate win rate
        int wins = 0;
        foreach (bool won in recentCombatResults)
        {
            if (won) wins++;
        }
        float winRate = wins / (float)recentCombatResults.Count;
        
        // Adjust difficulty based on performance
        if (winRate >= easyThreshold)
        {
            // Player is winning too much - increase difficulty
            AdjustDifficulty(adjustmentRate);
            Debug.Log($"Adaptive Difficulty: Increased (Win rate: {winRate:P})");
        }
        else if (winRate <= hardThreshold)
        {
            // Player is struggling - decrease difficulty
            AdjustDifficulty(-adjustmentRate);
            Debug.Log($"Adaptive Difficulty: Decreased (Win rate: {winRate:P})");
        }
        else
        {
            Debug.Log($"Adaptive Difficulty: No change needed (Win rate: {winRate:P})");
        }
    }

    /// <summary>
    /// Adjusts difficulty multipliers by a percentage.
    /// </summary>
    private void AdjustDifficulty(float adjustment)
    {
        // Adjust enemy stats (inverse for making easier)
        enemyHealthMultiplier = Mathf.Clamp(enemyHealthMultiplier + adjustment, 0.5f, 3.0f);
        enemyDamageMultiplier = Mathf.Clamp(enemyDamageMultiplier + adjustment, 0.5f, 2.5f);
        
        // Adjust rewards (inverse for making harder)
        xpMultiplier = Mathf.Clamp(xpMultiplier - (adjustment * 0.5f), 0.5f, 2.0f);
        goldMultiplier = Mathf.Clamp(goldMultiplier - (adjustment * 0.5f), 0.5f, 2.0f);
        
        totalAdaptiveAdjustments++;
        difficultyProfile = "Adaptive";
        
        SaveSettings();
        
        // Notify game systems
        GameEvents.TriggerDifficultyChanged();
    }

    /// <summary>
    /// Enables or disables adaptive difficulty.
    /// </summary>
    public void SetAdaptiveDifficulty(bool enabled)
    {
        enableAdaptiveDifficulty = enabled;
        
        if (enabled)
        {
            recentCombatResults.Clear();
            lastPerformanceCheck = Time.time;
            Debug.Log("Adaptive Difficulty enabled");
        }
        else
        {
            Debug.Log("Adaptive Difficulty disabled");
        }
        
        SaveSettings();
    }

    /// <summary>
    /// Gets whether adaptive difficulty is enabled.
    /// </summary>
    public bool IsAdaptiveDifficultyEnabled()
    {
        return enableAdaptiveDifficulty;
    }

    #endregion

    #region Custom Difficulty

    /// <summary>
    /// Sets custom enemy health multiplier.
    /// </summary>
    public void SetEnemyHealthMultiplier(float multiplier)
    {
        enemyHealthMultiplier = Mathf.Clamp(multiplier, 0.25f, 5.0f);
        difficultyProfile = "Custom";
        SaveSettings();
        
        GameEvents.TriggerDifficultyChanged();
    }

    /// <summary>
    /// Sets custom enemy damage multiplier.
    /// </summary>
    public void SetEnemyDamageMultiplier(float multiplier)
    {
        enemyDamageMultiplier = Mathf.Clamp(multiplier, 0.25f, 3.0f);
        difficultyProfile = "Custom";
        SaveSettings();
        
        GameEvents.TriggerDifficultyChanged();
    }

    /// <summary>
    /// Sets custom XP multiplier.
    /// </summary>
    public void SetXPMultiplier(float multiplier)
    {
        xpMultiplier = Mathf.Clamp(multiplier, 0.1f, 5.0f);
        difficultyProfile = "Custom";
        SaveSettings();
        
        GameEvents.TriggerDifficultyChanged();
    }

    /// <summary>
    /// Sets custom gold multiplier.
    /// </summary>
    public void SetGoldMultiplier(float multiplier)
    {
        goldMultiplier = Mathf.Clamp(multiplier, 0.1f, 5.0f);
        difficultyProfile = "Custom";
        SaveSettings();
        
        GameEvents.TriggerDifficultyChanged();
    }

    #endregion

    #region Ironman Mode

    /// <summary>
    /// Enables or disables Ironman mode (permadeath).
    /// </summary>
    public void SetIronmanMode(bool enabled, int lives = 1)
    {
        ironmanMode = enabled;
        livesRemaining = lives;
        
        if (enabled)
        {
            Debug.Log($"Ironman Mode enabled with {lives} life(lives)");
        }
        else
        {
            Debug.Log("Ironman Mode disabled");
        }
        
        SaveSettings();
    }

    /// <summary>
    /// Gets whether Ironman mode is enabled.
    /// </summary>
    public bool IsIronmanModeEnabled()
    {
        return ironmanMode;
    }

    /// <summary>
    /// Handles a player death in Ironman mode.
    /// Returns true if the player can continue, false if game over.
    /// </summary>
    public bool HandleIronmanDeath()
    {
        if (!ironmanMode) return true;
        
        livesRemaining--;
        
        if (livesRemaining <= 0)
        {
            Debug.Log("Ironman Mode: Game Over - All lives lost");
            // Trigger game over
            GameEvents.TriggerGameOver();
            return false;
        }
        else
        {
            Debug.Log($"Ironman Mode: {livesRemaining} life(lives) remaining");
            return true;
        }
    }

    /// <summary>
    /// Gets remaining lives in Ironman mode.
    /// </summary>
    public int GetRemainingLives()
    {
        return livesRemaining;
    }

    #endregion

    #region Getters

    /// <summary>
    /// Gets the current enemy health multiplier.
    /// </summary>
    public float GetEnemyHealthMultiplier()
    {
        return enemyHealthMultiplier;
    }

    /// <summary>
    /// Gets the current enemy damage multiplier.
    /// </summary>
    public float GetEnemyDamageMultiplier()
    {
        return enemyDamageMultiplier;
    }

    /// <summary>
    /// Gets the current XP reward multiplier.
    /// </summary>
    public float GetXPMultiplier()
    {
        return xpMultiplier;
    }

    /// <summary>
    /// Gets the current gold reward multiplier.
    /// </summary>
    public float GetGoldMultiplier()
    {
        return goldMultiplier;
    }

    /// <summary>
    /// Gets total number of adaptive adjustments made.
    /// </summary>
    public int GetTotalAdaptiveAdjustments()
    {
        return totalAdaptiveAdjustments;
    }

    /// <summary>
    /// Gets current win rate from recent combats.
    /// </summary>
    public float GetCurrentWinRate()
    {
        if (recentCombatResults.Count == 0) return 0f;
        
        int wins = 0;
        foreach (bool won in recentCombatResults)
        {
            if (won) wins++;
        }
        
        return wins / (float)recentCombatResults.Count;
    }

    #endregion

    #region Save/Load

    /// <summary>
    /// Saves difficulty settings to PlayerPrefs.
    /// </summary>
    public void SaveSettings()
    {
        PlayerPrefs.SetInt(ADAPTIVE_ENABLED_KEY, enableAdaptiveDifficulty ? 1 : 0);
        PlayerPrefs.SetFloat(ENEMY_HP_KEY, enemyHealthMultiplier);
        PlayerPrefs.SetFloat(ENEMY_DMG_KEY, enemyDamageMultiplier);
        PlayerPrefs.SetFloat(XP_MULT_KEY, xpMultiplier);
        PlayerPrefs.SetFloat(GOLD_MULT_KEY, goldMultiplier);
        PlayerPrefs.SetInt(IRONMAN_KEY, ironmanMode ? 1 : 0);
        PlayerPrefs.SetString(PROFILE_KEY, difficultyProfile);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads difficulty settings from PlayerPrefs.
    /// </summary>
    public void LoadSettings()
    {
        enableAdaptiveDifficulty = PlayerPrefs.GetInt(ADAPTIVE_ENABLED_KEY, 0) == 1;
        enemyHealthMultiplier = PlayerPrefs.GetFloat(ENEMY_HP_KEY, 1.0f);
        enemyDamageMultiplier = PlayerPrefs.GetFloat(ENEMY_DMG_KEY, 1.0f);
        xpMultiplier = PlayerPrefs.GetFloat(XP_MULT_KEY, 1.0f);
        goldMultiplier = PlayerPrefs.GetFloat(GOLD_MULT_KEY, 1.0f);
        ironmanMode = PlayerPrefs.GetInt(IRONMAN_KEY, 0) == 1;
        difficultyProfile = PlayerPrefs.GetString(PROFILE_KEY, "Normal");
    }

    /// <summary>
    /// Resets difficulty settings to defaults (Normal preset).
    /// </summary>
    public void ResetToDefaults()
    {
        ApplyDifficultyPreset("Normal");
        SetAdaptiveDifficulty(false);
        SetIronmanMode(false);
        recentCombatResults.Clear();
        totalAdaptiveAdjustments = 0;
        
        Debug.Log("Difficulty settings reset to defaults");
    }

    #endregion

    #region Helper Classes

    /// <summary>
    /// Represents a difficulty preset configuration.
    /// </summary>
    private class DifficultyPreset
    {
        public float enemyHealth;
        public float enemyDamage;
        public float xpReward;
        public float goldReward;

        public DifficultyPreset(float hp, float dmg, float xp, float gold)
        {
            enemyHealth = hp;
            enemyDamage = dmg;
            xpReward = xp;
            goldReward = gold;
        }
    }

    #endregion
}
