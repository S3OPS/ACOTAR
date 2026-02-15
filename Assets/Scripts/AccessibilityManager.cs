using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Manages accessibility features for the game
    /// Phase 8 Enhancement - Accessibility Features
    /// </summary>
    public class AccessibilityManager : MonoBehaviour
    {
        [Header("Accessibility Settings")]
        [SerializeField] private ColorBlindMode colorBlindMode = ColorBlindMode.None;
        [SerializeField] private bool highContrastMode = false;
        [SerializeField] private float textScale = 1.0f;
        [SerializeField] private bool showDamageNumbers = true;
        [SerializeField] private bool screenReaderMode = false;

        // Singleton pattern
        private static AccessibilityManager _instance;
        public static AccessibilityManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("AccessibilityManager");
                    _instance = go.AddComponent<AccessibilityManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        // Properties
        public ColorBlindMode ColorBlindMode => colorBlindMode;
        public bool HighContrastMode => highContrastMode;
        public float TextScale => textScale;
        public bool ShowDamageNumbers => showDamageNumbers;
        public bool ScreenReaderMode => screenReaderMode;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Load saved settings
            LoadSettings();
        }

        /// <summary>
        /// Set colorblind mode
        /// </summary>
        public void SetColorBlindMode(ColorBlindMode mode)
        {
            colorBlindMode = mode;
            SaveSettings();
            ApplyColorBlindMode();
        }

        /// <summary>
        /// Toggle high contrast mode
        /// </summary>
        public void SetHighContrastMode(bool enabled)
        {
            highContrastMode = enabled;
            SaveSettings();
            ApplyHighContrastMode();
        }

        /// <summary>
        /// Set text scale multiplier
        /// </summary>
        public void SetTextScale(float scale)
        {
            textScale = Mathf.Clamp(scale, 0.8f, 1.5f);
            SaveSettings();
            ApplyTextScale();
        }

        /// <summary>
        /// Toggle damage numbers display
        /// </summary>
        public void SetShowDamageNumbers(bool show)
        {
            showDamageNumbers = show;
            SaveSettings();
        }

        /// <summary>
        /// Toggle screen reader mode
        /// </summary>
        public void SetScreenReaderMode(bool enabled)
        {
            screenReaderMode = enabled;
            SaveSettings();
        }

        /// <summary>
        /// Get adjusted color based on colorblind mode
        /// </summary>
        public Color GetAdjustedColor(Color originalColor, ColorPurpose purpose)
        {
            if (colorBlindMode == ColorBlindMode.None)
            {
                return originalColor;
            }

            // Apply colorblind adjustments based on mode and purpose
            switch (colorBlindMode)
            {
                case ColorBlindMode.Protanopia: // Red-weak
                    return AdjustForProtanopia(originalColor, purpose);
                
                case ColorBlindMode.Deuteranopia: // Green-weak
                    return AdjustForDeuteranopia(originalColor, purpose);
                
                case ColorBlindMode.Tritanopia: // Blue-weak
                    return AdjustForTritanopia(originalColor, purpose);
                
                default:
                    return originalColor;
            }
        }

        /// <summary>
        /// Get element type indicator for colorblind users
        /// </summary>
        public string GetElementTypeIndicator(DamageType damageType)
        {
            switch (damageType)
            {
                case DamageType.Fire:
                    return "[FIRE]";
                case DamageType.Ice:
                    return "[ICE]";
                case DamageType.Darkness:
                    return "[DARK]";
                case DamageType.Light:
                    return "[LIGHT]";
                case DamageType.Magical:
                    return "[MAGIC]";
                case DamageType.Physical:
                    return "[PHYS]";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Adjust color for protanopia (red-weak vision)
        /// </summary>
        private Color AdjustForProtanopia(Color color, ColorPurpose purpose)
        {
            // Red appears darker, shift to blue-yellow spectrum
            switch (purpose)
            {
                case ColorPurpose.Damage:
                    return new Color(0.8f, 0.8f, 0.2f); // Yellow for damage
                case ColorPurpose.Healing:
                    return new Color(0.2f, 0.8f, 0.8f); // Cyan for healing
                case ColorPurpose.Fire:
                    return new Color(1f, 0.7f, 0.2f); // Bright yellow-orange
                case ColorPurpose.Enemy:
                    return new Color(0.8f, 0.8f, 0.3f); // Yellow-green
                case ColorPurpose.Ally:
                    return new Color(0.3f, 0.7f, 0.9f); // Bright blue
                default:
                    return color;
            }
        }

        /// <summary>
        /// Adjust color for deuteranopia (green-weak vision)
        /// </summary>
        private Color AdjustForDeuteranopia(Color color, ColorPurpose purpose)
        {
            // Green appears similar to red, use blue-yellow spectrum
            switch (purpose)
            {
                case ColorPurpose.Damage:
                    return new Color(0.9f, 0.7f, 0.3f); // Orange for damage
                case ColorPurpose.Healing:
                    return new Color(0.4f, 0.6f, 0.9f); // Blue for healing
                case ColorPurpose.Fire:
                    return new Color(1f, 0.6f, 0.2f); // Bright orange
                case ColorPurpose.Enemy:
                    return new Color(0.8f, 0.7f, 0.4f); // Tan/brown
                case ColorPurpose.Ally:
                    return new Color(0.3f, 0.6f, 1f); // Bright blue
                default:
                    return color;
            }
        }

        /// <summary>
        /// Adjust color for tritanopia (blue-weak vision)
        /// </summary>
        private Color AdjustForTritanopia(Color color, ColorPurpose purpose)
        {
            // Blue appears greenish, shift to red-green spectrum
            switch (purpose)
            {
                case ColorPurpose.Damage:
                    return new Color(1f, 0.3f, 0.3f); // Bright red for damage
                case ColorPurpose.Healing:
                    return new Color(0.2f, 0.9f, 0.4f); // Bright green for healing
                case ColorPurpose.Fire:
                    return new Color(1f, 0.2f, 0.2f); // Bright red
                case ColorPurpose.Enemy:
                    return new Color(0.9f, 0.4f, 0.4f); // Light red
                case ColorPurpose.Ally:
                    return new Color(0.4f, 0.9f, 0.4f); // Bright green
                default:
                    return color;
            }
        }

        /// <summary>
        /// Apply colorblind mode to UI elements
        /// </summary>
        private void ApplyColorBlindMode()
        {
            // Trigger UI update event
            GameEvents.TriggerAccessibilityChanged();
            Debug.Log($"Applied colorblind mode: {colorBlindMode}");
        }

        /// <summary>
        /// Apply high contrast mode
        /// </summary>
        private void ApplyHighContrastMode()
        {
            // Trigger UI update event
            GameEvents.TriggerAccessibilityChanged();
            Debug.Log($"High contrast mode: {highContrastMode}");
        }

        /// <summary>
        /// Apply text scale to UI
        /// </summary>
        private void ApplyTextScale()
        {
            // Trigger UI update event
            GameEvents.TriggerAccessibilityChanged();
            Debug.Log($"Text scale: {textScale}");
        }

        /// <summary>
        /// Save accessibility settings
        /// </summary>
        private void SaveSettings()
        {
            PlayerPrefs.SetInt("Accessibility_ColorBlindMode", (int)colorBlindMode);
            PlayerPrefs.SetInt("Accessibility_HighContrast", highContrastMode ? 1 : 0);
            PlayerPrefs.SetFloat("Accessibility_TextScale", textScale);
            PlayerPrefs.SetInt("Accessibility_ShowDamageNumbers", showDamageNumbers ? 1 : 0);
            PlayerPrefs.SetInt("Accessibility_ScreenReader", screenReaderMode ? 1 : 0);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Load accessibility settings
        /// </summary>
        private void LoadSettings()
        {
            colorBlindMode = (ColorBlindMode)PlayerPrefs.GetInt("Accessibility_ColorBlindMode", 0);
            highContrastMode = PlayerPrefs.GetInt("Accessibility_HighContrast", 0) == 1;
            textScale = PlayerPrefs.GetFloat("Accessibility_TextScale", 1.0f);
            showDamageNumbers = PlayerPrefs.GetInt("Accessibility_ShowDamageNumbers", 1) == 1;
            screenReaderMode = PlayerPrefs.GetInt("Accessibility_ScreenReader", 0) == 1;
        }

        /// <summary>
        /// Reset all accessibility settings to default
        /// </summary>
        public void ResetToDefaults()
        {
            colorBlindMode = ColorBlindMode.None;
            highContrastMode = false;
            textScale = 1.0f;
            showDamageNumbers = true;
            screenReaderMode = false;
            SaveSettings();
            ApplyColorBlindMode();
            ApplyHighContrastMode();
            ApplyTextScale();
        }

        /// <summary>
        /// Get difficulty mode explanation for accessibility
        /// </summary>
        public string GetDifficultyExplanation(DifficultyMode mode)
        {
            switch (mode)
            {
                case DifficultyMode.Story:
                    return "Story Mode: Focus on the narrative. Enemies have 0.5x health and deal 0.5x damage. Perfect for experiencing the story without challenge.";
                
                case DifficultyMode.Normal:
                    return "Normal Mode: Balanced gameplay. Standard enemy stats. Recommended for most players.";
                
                case DifficultyMode.Hard:
                    return "Hard Mode: Challenging combat. Enemies have 1.5x health and deal 1.3x damage. For experienced players.";
                
                case DifficultyMode.Nightmare:
                    return "Nightmare Mode: Extreme challenge. Enemies have 2.0x health and deal 1.5x damage. Only for veteran players.";
                
                default:
                    return "Unknown difficulty mode.";
            }
        }

        /// <summary>
        /// Announce text for screen readers
        /// </summary>
        public void AnnounceForScreenReader(string message)
        {
            if (screenReaderMode)
            {
                Debug.Log($"[SCREEN READER] {message}");
                // In a real implementation, this would interface with platform screen reader APIs
            }
        }
    }

    /// <summary>
    /// Colorblind mode options
    /// </summary>
    public enum ColorBlindMode
    {
        None = 0,           // No adjustment
        Protanopia = 1,     // Red-weak (most common)
        Deuteranopia = 2,   // Green-weak (common)
        Tritanopia = 3      // Blue-weak (rare)
    }

    /// <summary>
    /// Color purpose for accessibility adjustments
    /// </summary>
    public enum ColorPurpose
    {
        Damage,
        Healing,
        Fire,
        Ice,
        Darkness,
        Light,
        Enemy,
        Ally,
        Neutral,
        Positive,
        Negative
    }
}
