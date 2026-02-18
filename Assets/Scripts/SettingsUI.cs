using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace ACOTAR
{
    /// <summary>
    /// Settings menu interface for game configuration
    /// Controls audio, graphics, accessibility, and controls
    /// </summary>
    public class SettingsUI : MonoBehaviour
    {
        public static SettingsUI Instance { get; private set; }

        [Header("UI Panels")]
        public GameObject settingsPanel;
        public GameObject audioPanel;
        public GameObject graphicsPanel;
        public GameObject accessibilityPanel;
        public GameObject controlsPanel;

        [Header("Audio Settings")]
        public Slider masterVolumeSlider;
        public Slider musicVolumeSlider;
        public Slider sfxVolumeSlider;
        public Slider ambientVolumeSlider;
        public Toggle muteToggle;
        public AudioMixer audioMixer;

        [Header("Graphics Settings")]
        public Dropdown qualityDropdown;
        public Dropdown resolutionDropdown;
        public Toggle fullscreenToggle;
        public Toggle vsyncToggle;
        public Slider fpsLimitSlider;
        public Text fpsLimitText;

        [Header("Accessibility Settings")]
        public Slider textSizeSlider;
        public Text textSizeText;
        public Dropdown colorblindModeDropdown;
        public Toggle subtitlesToggle;
        public Toggle screenReaderToggle;
        public Toggle highContrastToggle;
        public Slider uiScaleSlider;
        public Text uiScaleText;

        [Header("Controls Settings")]
        public Button inventoryKeyButton;
        public Button questLogKeyButton;
        public Button pauseKeyButton;
        public Button mapKeyButton;
        public Text inventoryKeyText;
        public Text questLogKeyText;
        public Text pauseKeyText;
        public Text mapKeyText;
        public Button resetDefaultsButton;

        [Header("Navigation")]
        public Button applyButton;
        public Button cancelButton;
        public Button audioTabButton;
        public Button graphicsTabButton;
        public Button accessibilityTabButton;
        public Button controlsTabButton;

        private SettingsData currentSettings;
        private SettingsData originalSettings;
        private bool isRemappingKey = false;
        private string keyBeingRemapped = "";

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
            InitializeSettings();
            SetupListeners();
            HidePanel();
        }

        /// <summary>
        /// Initialize settings from saved preferences
        /// </summary>
        private void InitializeSettings()
        {
            currentSettings = LoadSettings();
            originalSettings = currentSettings.Clone();
            ApplySettingsToUI();
        }

        /// <summary>
        /// Setup all UI event listeners
        /// </summary>
        private void SetupListeners()
        {
            // Audio
            if (masterVolumeSlider != null) masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            if (musicVolumeSlider != null) musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            if (sfxVolumeSlider != null) sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            if (ambientVolumeSlider != null) ambientVolumeSlider.onValueChanged.AddListener(OnAmbientVolumeChanged);
            if (muteToggle != null) muteToggle.onValueChanged.AddListener(OnMuteChanged);

            // Graphics
            if (qualityDropdown != null) qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
            if (resolutionDropdown != null) resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
            if (fullscreenToggle != null) fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
            if (vsyncToggle != null) vsyncToggle.onValueChanged.AddListener(OnVSyncChanged);
            if (fpsLimitSlider != null) fpsLimitSlider.onValueChanged.AddListener(OnFPSLimitChanged);

            // Accessibility
            if (textSizeSlider != null) textSizeSlider.onValueChanged.AddListener(OnTextSizeChanged);
            if (colorblindModeDropdown != null) colorblindModeDropdown.onValueChanged.AddListener(OnColorblindModeChanged);
            if (subtitlesToggle != null) subtitlesToggle.onValueChanged.AddListener(OnSubtitlesChanged);
            if (screenReaderToggle != null) screenReaderToggle.onValueChanged.AddListener(OnScreenReaderChanged);
            if (highContrastToggle != null) highContrastToggle.onValueChanged.AddListener(OnHighContrastChanged);
            if (uiScaleSlider != null) uiScaleSlider.onValueChanged.AddListener(OnUIScaleChanged);

            // Controls
            if (inventoryKeyButton != null) inventoryKeyButton.onClick.AddListener(() => StartKeyRemapping("Inventory"));
            if (questLogKeyButton != null) questLogKeyButton.onClick.AddListener(() => StartKeyRemapping("QuestLog"));
            if (pauseKeyButton != null) pauseKeyButton.onClick.AddListener(() => StartKeyRemapping("Pause"));
            if (mapKeyButton != null) mapKeyButton.onClick.AddListener(() => StartKeyRemapping("Map"));
            if (resetDefaultsButton != null) resetDefaultsButton.onClick.AddListener(ResetToDefaults);

            // Navigation
            if (applyButton != null) applyButton.onClick.AddListener(ApplySettings);
            if (cancelButton != null) cancelButton.onClick.AddListener(CancelSettings);
            if (audioTabButton != null) audioTabButton.onClick.AddListener(() => ShowTab("Audio"));
            if (graphicsTabButton != null) graphicsTabButton.onClick.AddListener(() => ShowTab("Graphics"));
            if (accessibilityTabButton != null) accessibilityTabButton.onClick.AddListener(() => ShowTab("Accessibility"));
            if (controlsTabButton != null) controlsTabButton.onClick.AddListener(() => ShowTab("Controls"));

            // Initialize resolution dropdown
            PopulateResolutionDropdown();
        }

        /// <summary>
        /// Apply current settings to UI elements
        /// </summary>
        private void ApplySettingsToUI()
        {
            // Audio
            if (masterVolumeSlider != null) masterVolumeSlider.value = currentSettings.masterVolume;
            if (musicVolumeSlider != null) musicVolumeSlider.value = currentSettings.musicVolume;
            if (sfxVolumeSlider != null) sfxVolumeSlider.value = currentSettings.sfxVolume;
            if (ambientVolumeSlider != null) ambientVolumeSlider.value = currentSettings.ambientVolume;
            if (muteToggle != null) muteToggle.isOn = currentSettings.isMuted;

            // Graphics
            if (qualityDropdown != null) qualityDropdown.value = currentSettings.qualityLevel;
            if (fullscreenToggle != null) fullscreenToggle.isOn = currentSettings.fullscreen;
            if (vsyncToggle != null) vsyncToggle.isOn = currentSettings.vsync;
            if (fpsLimitSlider != null) fpsLimitSlider.value = currentSettings.fpsLimit;

            // Accessibility
            if (textSizeSlider != null) textSizeSlider.value = currentSettings.textSize;
            if (colorblindModeDropdown != null) colorblindModeDropdown.value = (int)currentSettings.colorblindMode;
            if (subtitlesToggle != null) subtitlesToggle.isOn = currentSettings.subtitles;
            if (screenReaderToggle != null) screenReaderToggle.isOn = currentSettings.screenReader;
            if (highContrastToggle != null) highContrastToggle.isOn = currentSettings.highContrast;
            if (uiScaleSlider != null) uiScaleSlider.value = currentSettings.uiScale;

            // Controls
            UpdateKeyBindingTexts();

            // Update display texts
            UpdateFPSLimitText(currentSettings.fpsLimit);
            UpdateTextSizeText(currentSettings.textSize);
            UpdateUIScaleText(currentSettings.uiScale);
        }

        #region Audio Settings
        
        /// <summary>
        /// Handle master volume slider changes
        /// v2.6.6: Enhanced with error handling to prevent Mathf.Log10(0) issues
        /// </summary>
        private void OnMasterVolumeChanged(float value)
        {
            try
            {
                currentSettings.masterVolume = value;
                if (audioMixer != null)
                {
                    // Clamp value to prevent Log10(0) which returns -Infinity
                    float clampedValue = Mathf.Max(value, 0.0001f);
                    audioMixer.SetFloat("MasterVolume", Mathf.Log10(clampedValue) * 20);
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "SettingsUI", $"Exception in OnMasterVolumeChanged: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle music volume slider changes
        /// v2.6.6: Enhanced with error handling to prevent Mathf.Log10(0) issues
        /// </summary>
        private void OnMusicVolumeChanged(float value)
        {
            try
            {
                currentSettings.musicVolume = value;
                if (audioMixer != null)
                {
                    // Clamp value to prevent Log10(0) which returns -Infinity
                    float clampedValue = Mathf.Max(value, 0.0001f);
                    audioMixer.SetFloat("MusicVolume", Mathf.Log10(clampedValue) * 20);
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "SettingsUI", $"Exception in OnMusicVolumeChanged: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle SFX volume slider changes
        /// v2.6.6: Enhanced with error handling to prevent Mathf.Log10(0) issues
        /// </summary>
        private void OnSFXVolumeChanged(float value)
        {
            try
            {
                currentSettings.sfxVolume = value;
                if (audioMixer != null)
                {
                    // Clamp value to prevent Log10(0) which returns -Infinity
                    float clampedValue = Mathf.Max(value, 0.0001f);
                    audioMixer.SetFloat("SFXVolume", Mathf.Log10(clampedValue) * 20);
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "SettingsUI", $"Exception in OnSFXVolumeChanged: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle ambient volume slider changes
        /// v2.6.6: Enhanced with error handling to prevent Mathf.Log10(0) issues
        /// </summary>
        private void OnAmbientVolumeChanged(float value)
        {
            try
            {
                currentSettings.ambientVolume = value;
                if (audioMixer != null)
                {
                    // Clamp value to prevent Log10(0) which returns -Infinity
                    float clampedValue = Mathf.Max(value, 0.0001f);
                    audioMixer.SetFloat("AmbientVolume", Mathf.Log10(clampedValue) * 20);
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "SettingsUI", $"Exception in OnAmbientVolumeChanged: {ex.Message}");
            }
        }

        private void OnMuteChanged(bool value)
        {
            currentSettings.isMuted = value;
            AudioListener.volume = value ? 0 : 1;
        }

        #endregion

        #region Graphics Settings

        private void OnQualityChanged(int value)
        {
            currentSettings.qualityLevel = value;
        }

        private void OnResolutionChanged(int value)
        {
            if (value >= 0 && value < Screen.resolutions.Length)
            {
                Resolution resolution = Screen.resolutions[value];
                currentSettings.resolutionWidth = resolution.width;
                currentSettings.resolutionHeight = resolution.height;
            }
        }

        private void OnFullscreenChanged(bool value)
        {
            currentSettings.fullscreen = value;
        }

        private void OnVSyncChanged(bool value)
        {
            currentSettings.vsync = value;
        }

        private void OnFPSLimitChanged(float value)
        {
            currentSettings.fpsLimit = Mathf.RoundToInt(value);
            UpdateFPSLimitText(currentSettings.fpsLimit);
        }

        private void UpdateFPSLimitText(int fps)
        {
            if (fpsLimitText != null)
            {
                fpsLimitText.text = fps == 0 ? "Unlimited" : fps.ToString();
            }
        }

        private void PopulateResolutionDropdown()
        {
            if (resolutionDropdown == null) return;

            resolutionDropdown.ClearOptions();
            var options = new System.Collections.Generic.List<string>();
            
            int currentResolutionIndex = 0;
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                Resolution res = Screen.resolutions[i];
                options.Add($"{res.width} x {res.height} @ {res.refreshRate}Hz");
                
                if (res.width == Screen.currentResolution.width && 
                    res.height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
            
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
        }

        #endregion

        #region Accessibility Settings

        private void OnTextSizeChanged(float value)
        {
            currentSettings.textSize = value;
            UpdateTextSizeText(value);
        }

        private void UpdateTextSizeText(float size)
        {
            if (textSizeText != null)
            {
                textSizeText.text = $"{Mathf.RoundToInt(size * 100)}%";
            }
        }

        private void OnColorblindModeChanged(int value)
        {
            currentSettings.colorblindMode = (ColorblindMode)value;
            // Apply colorblind filter here
        }

        private void OnSubtitlesChanged(bool value)
        {
            currentSettings.subtitles = value;
        }

        private void OnScreenReaderChanged(bool value)
        {
            currentSettings.screenReader = value;
        }

        private void OnHighContrastChanged(bool value)
        {
            currentSettings.highContrast = value;
        }

        private void OnUIScaleChanged(float value)
        {
            currentSettings.uiScale = value;
            UpdateUIScaleText(value);
        }

        private void UpdateUIScaleText(float scale)
        {
            if (uiScaleText != null)
            {
                uiScaleText.text = $"{Mathf.RoundToInt(scale * 100)}%";
            }
        }

        #endregion

        #region Controls Settings

        private void StartKeyRemapping(string keyName)
        {
            isRemappingKey = true;
            keyBeingRemapped = keyName;
            
            // Update button text to show "Press any key..."
            UpdateKeyButtonText(keyName, "Press key...");
        }

        private void Update()
        {
            if (isRemappingKey && Input.anyKeyDown)
            {
                foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key))
                    {
                        // Assign new key
                        switch (keyBeingRemapped)
                        {
                            case "Inventory":
                                currentSettings.inventoryKey = key;
                                break;
                            case "QuestLog":
                                currentSettings.questLogKey = key;
                                break;
                            case "Pause":
                                currentSettings.pauseKey = key;
                                break;
                            case "Map":
                                currentSettings.mapKey = key;
                                break;
                        }
                        
                        isRemappingKey = false;
                        keyBeingRemapped = "";
                        UpdateKeyBindingTexts();
                        break;
                    }
                }
            }
        }

        private void UpdateKeyBindingTexts()
        {
            UpdateKeyButtonText("Inventory", currentSettings.inventoryKey.ToString());
            UpdateKeyButtonText("QuestLog", currentSettings.questLogKey.ToString());
            UpdateKeyButtonText("Pause", currentSettings.pauseKey.ToString());
            UpdateKeyButtonText("Map", currentSettings.mapKey.ToString());
        }

        private void UpdateKeyButtonText(string keyName, string text)
        {
            Text buttonText = null;
            switch (keyName)
            {
                case "Inventory":
                    buttonText = inventoryKeyText;
                    break;
                case "QuestLog":
                    buttonText = questLogKeyText;
                    break;
                case "Pause":
                    buttonText = pauseKeyText;
                    break;
                case "Map":
                    buttonText = mapKeyText;
                    break;
            }
            
            if (buttonText != null)
            {
                buttonText.text = text;
            }
        }

        private void ResetToDefaults()
        {
            currentSettings = SettingsData.GetDefaults();
            ApplySettingsToUI();
        }

        #endregion

        #region Panel Management

        private void ShowTab(string tabName)
        {
            // Hide all panels
            if (audioPanel != null) audioPanel.SetActive(false);
            if (graphicsPanel != null) graphicsPanel.SetActive(false);
            if (accessibilityPanel != null) accessibilityPanel.SetActive(false);
            if (controlsPanel != null) controlsPanel.SetActive(false);

            // Show selected panel
            switch (tabName)
            {
                case "Audio":
                    if (audioPanel != null) audioPanel.SetActive(true);
                    break;
                case "Graphics":
                    if (graphicsPanel != null) graphicsPanel.SetActive(true);
                    break;
                case "Accessibility":
                    if (accessibilityPanel != null) accessibilityPanel.SetActive(true);
                    break;
                case "Controls":
                    if (controlsPanel != null) controlsPanel.SetActive(true);
                    break;
            }
        }

        public void ShowPanel()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
                originalSettings = currentSettings.Clone();
                ShowTab("Audio"); // Default to audio tab
            }
        }

        public void HidePanel()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Apply current settings to the game
        /// v2.6.6: Enhanced with comprehensive error handling and structured logging
        /// </summary>
        /// <remarks>
        /// CRITICAL: Applies graphics and quality settings, then saves them to PlayerPrefs.
        /// Failures here could result in unsupported resolutions or corrupted settings.
        /// Includes validation and exception handling for each settings operation.
        /// </remarks>
        private void ApplySettings()
        {
            try
            {
                // Apply graphics settings with validation
                try
                {
                    // Validate quality level
                    int maxQualityLevel = QualitySettings.names.Length - 1;
                    if (currentSettings.qualityLevel < 0 || currentSettings.qualityLevel > maxQualityLevel)
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                            "SettingsUI", $"Invalid quality level {currentSettings.qualityLevel}, clamping to valid range");
                        currentSettings.qualityLevel = Mathf.Clamp(currentSettings.qualityLevel, 0, maxQualityLevel);
                    }
                    
                    QualitySettings.SetQualityLevel(currentSettings.qualityLevel);
                }
                catch (System.Exception qualityEx)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "SettingsUI", $"Exception setting quality level: {qualityEx.Message}");
                }

                try
                {
                    // Validate resolution before applying
                    if (currentSettings.resolutionWidth <= 0 || currentSettings.resolutionHeight <= 0)
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                            "SettingsUI", $"Invalid resolution: {currentSettings.resolutionWidth}x{currentSettings.resolutionHeight}, using current");
                    }
                    else
                    {
                        Screen.SetResolution(currentSettings.resolutionWidth, currentSettings.resolutionHeight, currentSettings.fullscreen);
                    }
                }
                catch (System.Exception resEx)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "SettingsUI", $"Exception setting resolution: {resEx.Message}");
                }

                try
                {
                    QualitySettings.vSyncCount = currentSettings.vsync ? 1 : 0;
                    Application.targetFrameRate = currentSettings.fpsLimit;
                }
                catch (System.Exception fpsEx)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "SettingsUI", $"Exception setting VSync/FPS: {fpsEx.Message}");
                }

                // Save settings
                SaveSettings(currentSettings);
                
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                    "SettingsUI", "Settings applied and saved");
                HidePanel();
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "SettingsUI", $"Exception in ApplySettings: {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        private void CancelSettings()
        {
            // Revert to original settings
            currentSettings = originalSettings.Clone();
            ApplySettingsToUI();
            HidePanel();
        }

        #endregion

        #region Save/Load Settings

        /// <summary>
        /// Load settings from PlayerPrefs
        /// v2.6.6: Enhanced with comprehensive error handling and structured logging
        /// </summary>
        /// <returns>Loaded settings or defaults if loading fails</returns>
        /// <remarks>
        /// CRITICAL: Deserializes settings from JSON stored in PlayerPrefs.
        /// Failures here would result in loss of user preferences and potential crashes.
        /// Includes exception handling for JSON deserialization errors.
        /// </remarks>
        private SettingsData LoadSettings()
        {
            try
            {
                if (PlayerPrefs.HasKey("GameSettings"))
                {
                    try
                    {
                        string json = PlayerPrefs.GetString("GameSettings");
                        
                        if (string.IsNullOrEmpty(json))
                        {
                            LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                                "SettingsUI", "Settings JSON was empty, using defaults");
                            return SettingsData.GetDefaults();
                        }
                        
                        SettingsData settings = JsonUtility.FromJson<SettingsData>(json);
                        
                        if (settings == null)
                        {
                            LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                                "SettingsUI", "JSON deserialization returned null, using defaults");
                            return SettingsData.GetDefaults();
                        }
                        
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                            "SettingsUI", "Settings loaded successfully from PlayerPrefs");
                        return settings;
                    }
                    catch (System.Exception jsonEx)
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                            "SettingsUI", $"Exception deserializing settings JSON: {jsonEx.Message}");
                        return SettingsData.GetDefaults();
                    }
                }
                
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                    "SettingsUI", "No saved settings found, using defaults");
                return SettingsData.GetDefaults();
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "SettingsUI", $"Exception in LoadSettings: {ex.Message}\nStack: {ex.StackTrace}");
                return SettingsData.GetDefaults();
            }
        }

        /// <summary>
        /// Save settings to PlayerPrefs
        /// v2.6.6: Enhanced with comprehensive error handling and structured logging
        /// </summary>
        /// <param name="settings">Settings data to save</param>
        /// <remarks>
        /// CRITICAL: Serializes settings to JSON and saves to PlayerPrefs.
        /// Failures here would result in loss of user preferences between sessions.
        /// Includes exception handling for JSON serialization and PlayerPrefs operations.
        /// </remarks>
        private void SaveSettings(SettingsData settings)
        {
            try
            {
                if (settings == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "SettingsUI", "Cannot save null settings");
                    return;
                }

                try
                {
                    string json = JsonUtility.ToJson(settings);
                    
                    if (string.IsNullOrEmpty(json))
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                            "SettingsUI", "JSON serialization produced empty string");
                        return;
                    }
                    
                    PlayerPrefs.SetString("GameSettings", json);
                    PlayerPrefs.Save();
                    
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                        "SettingsUI", "Settings saved successfully to PlayerPrefs");
                }
                catch (System.Exception saveEx)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "SettingsUI", $"Exception during PlayerPrefs save operation: {saveEx.Message}");
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "SettingsUI", $"Exception in SaveSettings: {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        #endregion
    }

    /// <summary>
    /// Serializable settings data structure
    /// </summary>
    [System.Serializable]
    public class SettingsData
    {
        // Audio
        public float masterVolume = 1.0f;
        public float musicVolume = 0.8f;
        public float sfxVolume = 1.0f;
        public float ambientVolume = 0.6f;
        public bool isMuted = false;

        // Graphics
        public int qualityLevel = 2; // Medium
        public int resolutionWidth = 1920;
        public int resolutionHeight = 1080;
        public bool fullscreen = true;
        public bool vsync = true;
        public int fpsLimit = 60;

        // Accessibility
        public float textSize = 1.0f;
        public ColorblindMode colorblindMode = ColorblindMode.None;
        public bool subtitles = true;
        public bool screenReader = false;
        public bool highContrast = false;
        public float uiScale = 1.0f;

        // Controls
        public KeyCode inventoryKey = KeyCode.I;
        public KeyCode questLogKey = KeyCode.Q;
        public KeyCode pauseKey = KeyCode.Escape;
        public KeyCode mapKey = KeyCode.M;

        public static SettingsData GetDefaults()
        {
            return new SettingsData();
        }

        public SettingsData Clone()
        {
            return (SettingsData)this.MemberwiseClone();
        }
    }

    /// <summary>
    /// Colorblind mode options
    /// </summary>
    public enum ColorblindMode
    {
        None = 0,
        Protanopia = 1,      // Red-blind
        Deuteranopia = 2,    // Green-blind
        Tritanopia = 3       // Blue-blind
    }
}
