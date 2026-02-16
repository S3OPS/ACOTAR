using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Customizable keybindings system for the ACOTAR RPG.
    /// Allows players to remap controls to their preference.
    /// </summary>
    public class KeybindingSystem : MonoBehaviour
    {
        #region Singleton
        private static KeybindingSystem _instance;
        public static KeybindingSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("KeybindingSystem");
                    _instance = go.AddComponent<KeybindingSystem>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        #endregion

        #region Key Action Definitions
        public enum GameAction
        {
            // UI Navigation
            ToggleInventory,
            ToggleQuestLog,
            ToggleCharacterSheet,
            ToggleMap,
            ToggleSettings,
            TogglePauseMenu,
            ToggleStatistics,
            ToggleAchievements,

            // Combat
            Attack,
            Defend,
            UseAbility1,
            UseAbility2,
            UseAbility3,
            UseAbility4,
            Flee,
            TargetNext,
            TargetPrevious,

            // Gameplay
            Interact,
            QuickSave,
            QuickLoad,
            Screenshot,
            ToggleRun,
            
            // System
            ToggleDebugOverlay,
            TogglePerformanceOverlay,
            ExportLogs,
            
            // Quick Access
            QuickPotion,
            QuickManaPotion,
            QuickItem1,
            QuickItem2,
            QuickItem3,
            QuickItem4
        }
        #endregion

        #region Keybinding Data
        [Serializable]
        public class Keybinding
        {
            public GameAction action;
            public KeyCode primaryKey;
            public KeyCode secondaryKey;
            public bool requiresCtrl;
            public bool requiresShift;
            public bool requiresAlt;
            public string description;

            public Keybinding(GameAction action, KeyCode primary, string description)
            {
                this.action = action;
                this.primaryKey = primary;
                this.secondaryKey = KeyCode.None;
                this.description = description;
            }

            public bool IsPressed()
            {
                bool modifiersMatch = CheckModifiers();
                if (!modifiersMatch)
                    return false;

                return Input.GetKeyDown(primaryKey) || 
                       (secondaryKey != KeyCode.None && Input.GetKeyDown(secondaryKey));
            }

            public bool IsHeld()
            {
                bool modifiersMatch = CheckModifiers();
                if (!modifiersMatch)
                    return false;

                return Input.GetKey(primaryKey) || 
                       (secondaryKey != KeyCode.None && Input.GetKey(secondaryKey));
            }

            private bool CheckModifiers()
            {
                if (requiresCtrl && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
                    return false;
                if (requiresShift && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
                    return false;
                if (requiresAlt && !Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt))
                    return false;

                return true;
            }

            public string GetDisplayString()
            {
                StringBuilder sb = new StringBuilder();
                
                if (requiresCtrl) sb.Append("Ctrl+");
                if (requiresShift) sb.Append("Shift+");
                if (requiresAlt) sb.Append("Alt+");
                
                sb.Append(primaryKey.ToString());
                
                if (secondaryKey != KeyCode.None)
                {
                    sb.Append(" / ");
                    if (requiresCtrl) sb.Append("Ctrl+");
                    if (requiresShift) sb.Append("Shift+");
                    if (requiresAlt) sb.Append("Alt+");
                    sb.Append(secondaryKey.ToString());
                }
                
                return sb.ToString();
            }
        }
        #endregion

        #region Private Fields
        private Dictionary<GameAction, Keybinding> keybindings = new Dictionary<GameAction, Keybinding>();
        private bool isRemapping = false;
        private GameAction remappingAction;
        private bool remappingSecondary = false;
        private string keybindingsFilePath;
        #endregion

        #region Events
        public delegate void KeybindingEventHandler(GameAction action, KeyCode oldKey, KeyCode newKey);
        public event KeybindingEventHandler OnKeybindingChanged;
        public event Action OnKeybindingsLoaded;
        public event Action OnKeybindingsReset;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();
        }

        private void Update()
        {
            if (isRemapping)
            {
                CheckForKeyInput();
            }
            else
            {
                ProcessKeybindings();
            }
        }
        #endregion

        #region Initialization
        private void Initialize()
        {
            keybindingsFilePath = Path.Combine(Application.persistentDataPath, "keybindings.json");
            
            SetDefaultKeybindings();
            LoadKeybindings();
            
            LoggingSystem.Info("Keybinding", "KeybindingSystem initialized");
        }

        private void SetDefaultKeybindings()
        {
            // UI Navigation
            AddKeybinding(GameAction.ToggleInventory, KeyCode.I, "Open/Close Inventory");
            AddKeybinding(GameAction.ToggleQuestLog, KeyCode.Q, "Open/Close Quest Log");
            AddKeybinding(GameAction.ToggleCharacterSheet, KeyCode.C, "Open/Close Character Sheet");
            AddKeybinding(GameAction.ToggleMap, KeyCode.M, "Open/Close Map");
            AddKeybinding(GameAction.ToggleSettings, KeyCode.O, "Open/Close Settings");
            AddKeybinding(GameAction.TogglePauseMenu, KeyCode.Escape, "Pause Menu");
            AddKeybinding(GameAction.ToggleStatistics, KeyCode.T, "Open/Close Statistics");
            AddKeybinding(GameAction.ToggleAchievements, KeyCode.A, "Open/Close Achievements");

            // Combat
            AddKeybinding(GameAction.Attack, KeyCode.Space, "Attack");
            AddKeybinding(GameAction.Defend, KeyCode.D, "Defend");
            AddKeybinding(GameAction.UseAbility1, KeyCode.Alpha1, "Use Ability 1");
            AddKeybinding(GameAction.UseAbility2, KeyCode.Alpha2, "Use Ability 2");
            AddKeybinding(GameAction.UseAbility3, KeyCode.Alpha3, "Use Ability 3");
            AddKeybinding(GameAction.UseAbility4, KeyCode.Alpha4, "Use Ability 4");
            AddKeybinding(GameAction.Flee, KeyCode.F, "Flee Combat");
            AddKeybinding(GameAction.TargetNext, KeyCode.Tab, "Next Target");
            AddKeybinding(GameAction.TargetPrevious, KeyCode.Tab, "Previous Target").requiresShift = true;

            // Gameplay
            AddKeybinding(GameAction.Interact, KeyCode.E, "Interact");
            AddKeybinding(GameAction.QuickSave, KeyCode.F5, "Quick Save");
            AddKeybinding(GameAction.QuickLoad, KeyCode.F9, "Quick Load");
            AddKeybinding(GameAction.Screenshot, KeyCode.F12, "Take Screenshot");
            AddKeybinding(GameAction.ToggleRun, KeyCode.LeftShift, "Run");

            // System
            AddKeybinding(GameAction.ToggleDebugOverlay, KeyCode.F3, "Debug Overlay");
            AddKeybinding(GameAction.TogglePerformanceOverlay, KeyCode.F4, "Performance Overlay");
            var exportLogs = AddKeybinding(GameAction.ExportLogs, KeyCode.L, "Export Logs");
            exportLogs.requiresCtrl = true;
            exportLogs.requiresShift = true;

            // Quick Access
            AddKeybinding(GameAction.QuickPotion, KeyCode.H, "Use Health Potion");
            AddKeybinding(GameAction.QuickManaPotion, KeyCode.N, "Use Mana Potion");
            AddKeybinding(GameAction.QuickItem1, KeyCode.Alpha5, "Quick Item Slot 1");
            AddKeybinding(GameAction.QuickItem2, KeyCode.Alpha6, "Quick Item Slot 2");
            AddKeybinding(GameAction.QuickItem3, KeyCode.Alpha7, "Quick Item Slot 3");
            AddKeybinding(GameAction.QuickItem4, KeyCode.Alpha8, "Quick Item Slot 4");
        }

        private Keybinding AddKeybinding(GameAction action, KeyCode key, string description)
        {
            var binding = new Keybinding(action, key, description);
            keybindings[action] = binding;
            return binding;
        }
        #endregion

        #region Keybinding Management
        /// <summary>
        /// Get keybinding for an action
        /// </summary>
        public static Keybinding GetKeybinding(GameAction action)
        {
            return Instance.keybindings.ContainsKey(action) ? Instance.keybindings[action] : null;
        }

        /// <summary>
        /// Check if an action is pressed this frame
        /// </summary>
        public static bool IsActionPressed(GameAction action)
        {
            var binding = GetKeybinding(action);
            return binding != null && binding.IsPressed();
        }

        /// <summary>
        /// Check if an action key is being held
        /// </summary>
        public static bool IsActionHeld(GameAction action)
        {
            var binding = GetKeybinding(action);
            return binding != null && binding.IsHeld();
        }

        /// <summary>
        /// Start remapping a keybinding
        /// </summary>
        public static void StartRemapping(GameAction action, bool secondary = false)
        {
            Instance.isRemapping = true;
            Instance.remappingAction = action;
            Instance.remappingSecondary = secondary;
            
            LoggingSystem.Info("Keybinding", $"Started remapping {action} ({(secondary ? "secondary" : "primary")})");
            NotificationSystem.ShowInfo($"Press a key to bind to {action}");
        }

        /// <summary>
        /// Cancel current remapping operation
        /// </summary>
        public static void CancelRemapping()
        {
            Instance.isRemapping = false;
            LoggingSystem.Info("Keybinding", "Remapping cancelled");
            NotificationSystem.ShowInfo("Remapping cancelled");
        }

        /// <summary>
        /// Check if a key is already bound to another action
        /// </summary>
        public static GameAction? FindConflict(KeyCode key)
        {
            foreach (var kvp in Instance.keybindings)
            {
                if (kvp.Value.primaryKey == key || kvp.Value.secondaryKey == key)
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// Manually set a keybinding
        /// </summary>
        public static bool SetKeybinding(GameAction action, KeyCode key, bool secondary = false)
        {
            var binding = GetKeybinding(action);
            if (binding == null)
                return false;

            // Check for conflicts
            var conflict = FindConflict(key);
            if (conflict.HasValue && conflict.Value != action)
            {
                LoggingSystem.Warning("Keybinding", $"Key {key} is already bound to {conflict.Value}");
                NotificationSystem.ShowWarning($"Key {key} is already bound to {conflict.Value}. Clear that binding first.");
                return false;
            }

            KeyCode oldKey = secondary ? binding.secondaryKey : binding.primaryKey;

            if (secondary)
            {
                binding.secondaryKey = key;
            }
            else
            {
                binding.primaryKey = key;
            }

            LoggingSystem.Info("Keybinding", $"Set {action} {(secondary ? "secondary" : "primary")} key to {key}");
            NotificationSystem.ShowSuccess($"Bound {action} to {key}");
            
            Instance.OnKeybindingChanged?.Invoke(action, oldKey, key);
            Instance.SaveKeybindings();
            
            return true;
        }

        /// <summary>
        /// Clear a keybinding
        /// </summary>
        public static void ClearKeybinding(GameAction action, bool secondary = false)
        {
            var binding = GetKeybinding(action);
            if (binding == null)
                return;

            if (secondary)
            {
                binding.secondaryKey = KeyCode.None;
            }
            else
            {
                binding.primaryKey = KeyCode.None;
            }

            LoggingSystem.Info("Keybinding", $"Cleared {action} {(secondary ? "secondary" : "primary")} key");
            Instance.SaveKeybindings();
        }

        /// <summary>
        /// Reset all keybindings to defaults
        /// </summary>
        public static void ResetToDefaults()
        {
            Instance.keybindings.Clear();
            Instance.SetDefaultKeybindings();
            Instance.SaveKeybindings();
            
            LoggingSystem.Info("Keybinding", "Reset all keybindings to defaults");
            NotificationSystem.ShowInfo("Keybindings reset to defaults");
            Instance.OnKeybindingsReset?.Invoke();
        }

        /// <summary>
        /// Get all keybindings
        /// </summary>
        public static Dictionary<GameAction, Keybinding> GetAllKeybindings()
        {
            return new Dictionary<GameAction, Keybinding>(Instance.keybindings);
        }
        #endregion

        #region Input Processing
        private void ProcessKeybindings()
        {
            // Process each keybinding
            foreach (var kvp in keybindings)
            {
                if (kvp.Value.IsPressed())
                {
                    HandleAction(kvp.Key);
                }
            }
        }

        private void HandleAction(GameAction action)
        {
            // Log action
            LoggingSystem.Debug("Keybinding", $"Action triggered: {action}");

            // Broadcast event
            GameEvents.TriggerKeybindingAction(action);
        }

        private void CheckForKeyInput()
        {
            // Listen for any key press
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    // Ignore modifier keys and mouse buttons
                    if (IsValidKey(keyCode))
                    {
                        SetKeybinding(remappingAction, keyCode, remappingSecondary);
                        isRemapping = false;
                        return;
                    }
                }
            }

            // Allow cancelling with Escape
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelRemapping();
            }
        }

        private bool IsValidKey(KeyCode key)
        {
            // Exclude modifier keys
            if (key == KeyCode.LeftControl || key == KeyCode.RightControl ||
                key == KeyCode.LeftShift || key == KeyCode.RightShift ||
                key == KeyCode.LeftAlt || key == KeyCode.RightAlt)
            {
                return false;
            }

            // Exclude mouse buttons (we handle those separately)
            if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Save/Load
        private void SaveKeybindings()
        {
            try
            {
                var data = new KeybindingsData();
                foreach (var kvp in keybindings)
                {
                    data.bindings.Add(new SerializableKeybinding(kvp.Value));
                }

                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(keybindingsFilePath, json);
                
                LoggingSystem.Info("Keybinding", "Saved keybindings");
            }
            catch (Exception ex)
            {
                LoggingSystem.Error("Keybinding", "Failed to save keybindings", ex);
            }
        }

        private void LoadKeybindings()
        {
            try
            {
                if (!File.Exists(keybindingsFilePath))
                {
                    LoggingSystem.Info("Keybinding", "No saved keybindings found, using defaults");
                    return;
                }

                string json = File.ReadAllText(keybindingsFilePath);
                var data = JsonUtility.FromJson<KeybindingsData>(json);

                foreach (var binding in data.bindings)
                {
                    if (keybindings.ContainsKey(binding.action))
                    {
                        var kb = keybindings[binding.action];
                        kb.primaryKey = binding.primaryKey;
                        kb.secondaryKey = binding.secondaryKey;
                        kb.requiresCtrl = binding.requiresCtrl;
                        kb.requiresShift = binding.requiresShift;
                        kb.requiresAlt = binding.requiresAlt;
                    }
                }

                LoggingSystem.Info("Keybinding", "Loaded keybindings");
                OnKeybindingsLoaded?.Invoke();
            }
            catch (Exception ex)
            {
                LoggingSystem.Error("Keybinding", "Failed to load keybindings", ex);
            }
        }
        #endregion

        #region Serialization Classes
        [Serializable]
        private class KeybindingsData
        {
            public List<SerializableKeybinding> bindings = new List<SerializableKeybinding>();
        }

        [Serializable]
        private class SerializableKeybinding
        {
            public GameAction action;
            public KeyCode primaryKey;
            public KeyCode secondaryKey;
            public bool requiresCtrl;
            public bool requiresShift;
            public bool requiresAlt;

            public SerializableKeybinding(Keybinding binding)
            {
                this.action = binding.action;
                this.primaryKey = binding.primaryKey;
                this.secondaryKey = binding.secondaryKey;
                this.requiresCtrl = binding.requiresCtrl;
                this.requiresShift = binding.requiresShift;
                this.requiresAlt = binding.requiresAlt;
            }
        }
        #endregion
    }

    /// <summary>
    /// Extension to GameEvents for keybinding actions
    /// </summary>
    public static class KeybindingEvents
    {
        public delegate void KeybindingActionHandler(KeybindingSystem.GameAction action);
        public static event KeybindingActionHandler OnKeybindingAction;

        public static void TriggerKeybindingAction(KeybindingSystem.GameAction action)
        {
            OnKeybindingAction?.Invoke(action);
        }
    }
}
