using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ACOTAR;

/// <summary>
/// Enhanced Auto-Save System with backup management and corruption detection.
/// Extends the existing SaveSystem with automatic saving and backup functionality.
/// </summary>
public class AutoSaveSystem : MonoBehaviour
{
    private static AutoSaveSystem instance;
    public static AutoSaveSystem Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("AutoSaveSystem");
                instance = go.AddComponent<AutoSaveSystem>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    [Header("Auto-Save Settings")]
    [SerializeField] private bool autoSaveEnabled = true;
    [SerializeField] private float autoSaveInterval = 300f; // 5 minutes default
    [SerializeField] private int maxBackupCount = 3; // Keep last 3 backups
    
    [Header("Save Events")]
    [SerializeField] private bool saveOnQuestComplete = true;
    [SerializeField] private bool saveOnLevelUp = true;
    [SerializeField] private bool saveOnLocationChange = true;
    [SerializeField] private bool saveOnCombatEnd = false; // Optional - can be frequent
    
    [Header("Quick Save")]
    [SerializeField] private KeyCode quickSaveKey = KeyCode.F5;
    [SerializeField] private KeyCode quickLoadKey = KeyCode.F9;
    [SerializeField] private string quickSaveSlotName = "QuickSave";
    
    private float timeSinceLastAutoSave = 0f;
    private bool isSaving = false;
    private Queue<string> recentBackups = new Queue<string>();
    
    private const string AUTO_SAVE_ENABLED_KEY = "AutoSave_Enabled";
    private const string AUTO_SAVE_INTERVAL_KEY = "AutoSave_Interval";
    private const string BACKUP_DIR = "Backups";

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        LoadSettings();
        SubscribeToEvents();
        EnsureBackupDirectoryExists();
    }

    void Update()
    {
        // Quick save/load hotkeys
        if (Input.GetKeyDown(quickSaveKey))
        {
            QuickSave();
        }
        
        if (Input.GetKeyDown(quickLoadKey))
        {
            QuickLoad();
        }
        
        // Auto-save timer
        if (autoSaveEnabled && !isSaving)
        {
            timeSinceLastAutoSave += Time.deltaTime;
            
            if (timeSinceLastAutoSave >= autoSaveInterval)
            {
                PerformAutoSave();
                timeSinceLastAutoSave = 0f;
            }
        }
    }

    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    #region Auto-Save

    /// <summary>
    /// Performs an automatic save with backup creation.
    /// </summary>
    private void PerformAutoSave()
    {
        if (isSaving) return;
        
        StartCoroutine(AutoSaveCoroutine());
    }

    private System.Collections.IEnumerator AutoSaveCoroutine()
    {
        isSaving = true;
        
        try
        {
            // Create backup before saving
            CreateBackup();
            
            // Perform actual save
            bool success = SaveSystem.SaveGame();
                
            if (success)
            {
                Debug.Log($"Auto-save successful at {DateTime.Now:HH:mm:ss}");
                    
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowNotification("Game auto-saved", 2f);
                }
            }
            else
            {
                Debug.LogWarning("Auto-save failed");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Auto-save error: {ex.Message}");
        }
        
        yield return new WaitForSeconds(0.1f);
        isSaving = false;
    }

    /// <summary>
    /// Enables or disables auto-save.
    /// </summary>
    public void SetAutoSaveEnabled(bool enabled)
    {
        autoSaveEnabled = enabled;
        timeSinceLastAutoSave = 0f;
        
        PlayerPrefs.SetInt(AUTO_SAVE_ENABLED_KEY, enabled ? 1 : 0);
        PlayerPrefs.Save();
        
        Debug.Log($"Auto-save {(enabled ? "enabled" : "disabled")}");
    }

    /// <summary>
    /// Sets the auto-save interval in seconds.
    /// </summary>
    public void SetAutoSaveInterval(float intervalSeconds)
    {
        autoSaveInterval = Mathf.Clamp(intervalSeconds, 60f, 1800f); // 1-30 minutes
        timeSinceLastAutoSave = 0f;
        
        PlayerPrefs.SetFloat(AUTO_SAVE_INTERVAL_KEY, autoSaveInterval);
        PlayerPrefs.Save();
        
        Debug.Log($"Auto-save interval set to {autoSaveInterval} seconds");
    }

    /// <summary>
    /// Gets whether auto-save is currently enabled.
    /// </summary>
    public bool IsAutoSaveEnabled()
    {
        return autoSaveEnabled;
    }

    /// <summary>
    /// Gets the current auto-save interval.
    /// </summary>
    public float GetAutoSaveInterval()
    {
        return autoSaveInterval;
    }

    /// <summary>
    /// Gets time remaining until next auto-save.
    /// </summary>
    public float GetTimeUntilNextAutoSave()
    {
        return Mathf.Max(0f, autoSaveInterval - timeSinceLastAutoSave);
    }

    #endregion

    #region Quick Save/Load

    /// <summary>
    /// Performs a quick save to a dedicated slot.
    /// </summary>
    public void QuickSave()
    {
        if (isSaving) return;
        
        try
        {
            bool success = SaveSystem.SaveGame(); // Could use a specific quick-save slot
                
            if (success)
            {
                Debug.Log("Quick-save successful");
                    
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowNotification("Quick-saved (F5)", 2f);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Quick-save error: {ex.Message}");
        }
    }

    /// <summary>
    /// Performs a quick load from the dedicated quick-save slot.
    /// </summary>
    public void QuickLoad()
    {
        try
        {
            bool success = SaveSystem.LoadGame(); // Could use a specific quick-save slot
                
            if (success)
            {
                Debug.Log("Quick-load successful");
                    
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowNotification("Quick-loaded (F9)", 2f);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Quick-load error: {ex.Message}");
        }
    }

    #endregion

    #region Backup Management

    /// <summary>
    /// Ensures the backup directory exists.
    /// </summary>
    private void EnsureBackupDirectoryExists()
    {
        string backupPath = GetBackupDirectory();
        if (!Directory.Exists(backupPath))
        {
            Directory.CreateDirectory(backupPath);
            Debug.Log($"Created backup directory: {backupPath}");
        }
    }

    /// <summary>
    /// Gets the backup directory path.
    /// </summary>
    private string GetBackupDirectory()
    {
        return Path.Combine(Application.persistentDataPath, BACKUP_DIR);
    }

    /// <summary>
    /// Creates a backup of the current save file.
    /// </summary>
    private void CreateBackup()
    {
        try
        {
            string savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
            
            if (!File.Exists(savePath))
            {
                // No save file to backup
                return;
            }
            
            string backupPath = GetBackupDirectory();
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string backupFile = Path.Combine(backupPath, $"savegame_backup_{timestamp}.json");
            
            File.Copy(savePath, backupFile, true);
            recentBackups.Enqueue(backupFile);
            
            // Keep only the most recent backups
            while (recentBackups.Count > maxBackupCount)
            {
                string oldBackup = recentBackups.Dequeue();
                if (File.Exists(oldBackup))
                {
                    File.Delete(oldBackup);
                    Debug.Log($"Deleted old backup: {Path.GetFileName(oldBackup)}");
                }
            }
            
            Debug.Log($"Created backup: {Path.GetFileName(backupFile)}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to create backup: {ex.Message}");
        }
    }

    /// <summary>
    /// Restores a save file from the most recent backup.
    /// </summary>
    public bool RestoreFromBackup(int backupIndex = 0)
    {
        try
        {
            string backupPath = GetBackupDirectory();
            var backupFiles = Directory.GetFiles(backupPath, "savegame_backup_*.json")
                                      .OrderByDescending(f => File.GetCreationTime(f))
                                      .ToList();
            
            if (backupIndex >= 0 && backupIndex < backupFiles.Count)
            {
                string backupFile = backupFiles[backupIndex];
                string savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
                
                File.Copy(backupFile, savePath, true);
                
                Debug.Log($"Restored from backup: {Path.GetFileName(backupFile)}");
                
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowNotification("Save restored from backup", 3f);
                }
                
                return true;
            }
            else
            {
                Debug.LogWarning($"Backup index {backupIndex} out of range");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to restore from backup: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Gets a list of available backup files.
    /// </summary>
    public List<string> GetAvailableBackups()
    {
        try
        {
            string backupPath = GetBackupDirectory();
            var backupFiles = Directory.GetFiles(backupPath, "savegame_backup_*.json")
                                      .OrderByDescending(f => File.GetCreationTime(f))
                                      .Select(f => Path.GetFileName(f))
                                      .ToList();
            
            return backupFiles;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to get backup list: {ex.Message}");
            return new List<string>();
        }
    }

    #endregion

    #region Save Validation

    /// <summary>
    /// Validates a save file for corruption.
    /// </summary>
    public bool ValidateSaveFile(string savePath)
    {
        try
        {
            if (!File.Exists(savePath))
            {
                Debug.LogWarning($"Save file not found: {savePath}");
                return false;
            }
            
            string jsonContent = File.ReadAllText(savePath);
            
            // Basic validation - check if it's valid JSON
            var saveData = JsonUtility.FromJson<object>(jsonContent);
            
            // Additional validation could check for required fields
            // For now, if we can parse it, it's considered valid
            
            Debug.Log($"Save file validated: {Path.GetFileName(savePath)}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Save file validation failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Attempts to repair a corrupted save file using backups.
    /// </summary>
    public bool RepairSaveFile()
    {
        Debug.Log("Attempting to repair save file from backups...");
        
        // Try each backup until we find a valid one
        for (int i = 0; i < maxBackupCount; i++)
        {
            if (RestoreFromBackup(i))
            {
                string savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
                if (ValidateSaveFile(savePath))
                {
                    Debug.Log("Save file repaired successfully");
                    return true;
                }
            }
        }
        
        Debug.LogError("Failed to repair save file - all backups invalid or missing");
        return false;
    }

    #endregion

    #region Event Subscriptions

    /// <summary>
    /// Subscribes to game events for automatic saving.
    /// </summary>
    private void SubscribeToEvents()
    {
        if (saveOnQuestComplete)
            GameEvents.OnQuestCompleted += HandleQuestComplete;
        
        if (saveOnLevelUp)
            GameEvents.OnCharacterLevelUp += HandleLevelUp;
        
        if (saveOnLocationChange)
            GameEvents.OnLocationChanged += HandleLocationChange;
        
        if (saveOnCombatEnd)
            GameEvents.OnCombatEnded += HandleCombatEnd;
    }

    /// <summary>
    /// Unsubscribes from game events.
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        GameEvents.OnQuestCompleted -= HandleQuestComplete;
        GameEvents.OnCharacterLevelUp -= HandleLevelUp;
        GameEvents.OnLocationChanged -= HandleLocationChange;
        GameEvents.OnCombatEnded -= HandleCombatEnd;
    }

    private void HandleQuestComplete(Quest quest)
    {
        PerformAutoSave();
    }

    private void HandleLevelUp(Character character, int newLevel)
    {
        PerformAutoSave();
    }

    private void HandleLocationChange(string from, string to)
    {
        PerformAutoSave();
    }

    private void HandleCombatEnd(Character player, List<Enemy> enemies, bool playerWon)
    {
        PerformAutoSave();
    }

    #endregion

    #region Settings

    /// <summary>
    /// Loads auto-save settings from PlayerPrefs.
    /// </summary>
    private void LoadSettings()
    {
        autoSaveEnabled = PlayerPrefs.GetInt(AUTO_SAVE_ENABLED_KEY, 1) == 1;
        autoSaveInterval = PlayerPrefs.GetFloat(AUTO_SAVE_INTERVAL_KEY, 300f);
    }

    /// <summary>
    /// Saves auto-save settings to PlayerPrefs.
    /// </summary>
    public void SaveSettings()
    {
        PlayerPrefs.SetInt(AUTO_SAVE_ENABLED_KEY, autoSaveEnabled ? 1 : 0);
        PlayerPrefs.SetFloat(AUTO_SAVE_INTERVAL_KEY, autoSaveInterval);
        PlayerPrefs.Save();
    }

    #endregion
}
