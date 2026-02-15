using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ACOTAR
{
    /// <summary>
    /// Enhanced accessibility settings panel
    /// Phase 8: Accessibility Features
    /// </summary>
    public class AccessibilitySettingsUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject accessibilityPanel;
        [SerializeField] private Dropdown colorBlindModeDropdown;
        [SerializeField] private Toggle highContrastToggle;
        [SerializeField] private Slider textScaleSlider;
        [SerializeField] private Text textScaleLabel;
        [SerializeField] private Toggle damageNumbersToggle;
        [SerializeField] private Toggle screenReaderToggle;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button closeButton;

        [Header("Difficulty Explanation")]
        [SerializeField] private Text storyModeText;
        [SerializeField] private Text normalModeText;
        [SerializeField] private Text hardModeText;
        [SerializeField] private Text nightmareModeText;

        private void Start()
        {
            // Setup dropdown options
            if (colorBlindModeDropdown != null)
            {
                colorBlindModeDropdown.ClearOptions();
                List<string> options = new List<string>
                {
                    "None",
                    "Protanopia (Red-Weak)",
                    "Deuteranopia (Green-Weak)",
                    "Tritanopia (Blue-Weak)"
                };
                colorBlindModeDropdown.AddOptions(options);
                colorBlindModeDropdown.onValueChanged.AddListener(OnColorBlindModeChanged);
            }

            // Setup toggles
            if (highContrastToggle != null)
            {
                highContrastToggle.onValueChanged.AddListener(OnHighContrastChanged);
            }

            if (damageNumbersToggle != null)
            {
                damageNumbersToggle.onValueChanged.AddListener(OnDamageNumbersChanged);
            }

            if (screenReaderToggle != null)
            {
                screenReaderToggle.onValueChanged.AddListener(OnScreenReaderChanged);
            }

            // Setup slider
            if (textScaleSlider != null)
            {
                textScaleSlider.minValue = 0.8f;
                textScaleSlider.maxValue = 1.5f;
                textScaleSlider.value = 1.0f;
                textScaleSlider.onValueChanged.AddListener(OnTextScaleChanged);
            }

            // Setup buttons
            if (resetButton != null)
            {
                resetButton.onClick.AddListener(OnResetClicked);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseClicked);
            }

            // Setup difficulty explanations
            if (storyModeText != null)
            {
                storyModeText.text = AccessibilityManager.Instance.GetDifficultyExplanation(DifficultyMode.Story);
            }

            if (normalModeText != null)
            {
                normalModeText.text = AccessibilityManager.Instance.GetDifficultyExplanation(DifficultyMode.Normal);
            }

            if (hardModeText != null)
            {
                hardModeText.text = AccessibilityManager.Instance.GetDifficultyExplanation(DifficultyMode.Hard);
            }

            if (nightmareModeText != null)
            {
                nightmareModeText.text = AccessibilityManager.Instance.GetDifficultyExplanation(DifficultyMode.Nightmare);
            }

            // Load current settings
            LoadCurrentSettings();

            // Hide panel initially
            if (accessibilityPanel != null)
            {
                accessibilityPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Show the accessibility panel
        /// </summary>
        public void Show()
        {
            if (accessibilityPanel != null)
            {
                accessibilityPanel.SetActive(true);
                LoadCurrentSettings();
            }
        }

        /// <summary>
        /// Hide the accessibility panel
        /// </summary>
        public void Hide()
        {
            if (accessibilityPanel != null)
            {
                accessibilityPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Load current settings from AccessibilityManager
        /// </summary>
        private void LoadCurrentSettings()
        {
            if (AccessibilityManager.Instance == null)
            {
                return;
            }

            if (colorBlindModeDropdown != null)
            {
                colorBlindModeDropdown.value = (int)AccessibilityManager.Instance.ColorBlindMode;
            }

            if (highContrastToggle != null)
            {
                highContrastToggle.isOn = AccessibilityManager.Instance.HighContrastMode;
            }

            if (textScaleSlider != null)
            {
                textScaleSlider.value = AccessibilityManager.Instance.TextScale;
            }

            if (damageNumbersToggle != null)
            {
                damageNumbersToggle.isOn = AccessibilityManager.Instance.ShowDamageNumbers;
            }

            if (screenReaderToggle != null)
            {
                screenReaderToggle.isOn = AccessibilityManager.Instance.ScreenReaderMode;
            }

            UpdateTextScaleLabel();
        }

        /// <summary>
        /// Handle colorblind mode dropdown change
        /// </summary>
        private void OnColorBlindModeChanged(int value)
        {
            AccessibilityManager.Instance.SetColorBlindMode((ColorBlindMode)value);
            
            // Announce change for screen readers
            AccessibilityManager.Instance.AnnounceForScreenReader(
                $"Colorblind mode changed to {(ColorBlindMode)value}"
            );
        }

        /// <summary>
        /// Handle high contrast toggle change
        /// </summary>
        private void OnHighContrastChanged(bool value)
        {
            AccessibilityManager.Instance.SetHighContrastMode(value);
            
            AccessibilityManager.Instance.AnnounceForScreenReader(
                value ? "High contrast mode enabled" : "High contrast mode disabled"
            );
        }

        /// <summary>
        /// Handle text scale slider change
        /// </summary>
        private void OnTextScaleChanged(float value)
        {
            AccessibilityManager.Instance.SetTextScale(value);
            UpdateTextScaleLabel();
            
            AccessibilityManager.Instance.AnnounceForScreenReader(
                $"Text scale set to {Mathf.RoundToInt(value * 100)}%"
            );
        }

        /// <summary>
        /// Handle damage numbers toggle change
        /// </summary>
        private void OnDamageNumbersChanged(bool value)
        {
            AccessibilityManager.Instance.SetShowDamageNumbers(value);
            
            AccessibilityManager.Instance.AnnounceForScreenReader(
                value ? "Damage numbers enabled" : "Damage numbers disabled"
            );
        }

        /// <summary>
        /// Handle screen reader toggle change
        /// </summary>
        private void OnScreenReaderChanged(bool value)
        {
            AccessibilityManager.Instance.SetScreenReaderMode(value);
            
            if (value)
            {
                AccessibilityManager.Instance.AnnounceForScreenReader(
                    "Screen reader mode enabled. Important game events will be announced."
                );
            }
        }

        /// <summary>
        /// Update text scale label
        /// </summary>
        private void UpdateTextScaleLabel()
        {
            if (textScaleLabel != null && textScaleSlider != null)
            {
                int percentage = Mathf.RoundToInt(textScaleSlider.value * 100);
                textScaleLabel.text = $"Text Scale: {percentage}%";
            }
        }

        /// <summary>
        /// Handle reset button click
        /// </summary>
        private void OnResetClicked()
        {
            AccessibilityManager.Instance.ResetToDefaults();
            LoadCurrentSettings();
            
            AccessibilityManager.Instance.AnnounceForScreenReader(
                "All accessibility settings reset to defaults"
            );

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowNotification("Accessibility settings reset to defaults", Color.white);
            }
        }

        /// <summary>
        /// Handle close button click
        /// </summary>
        private void OnCloseClicked()
        {
            Hide();
        }

        /// <summary>
        /// Get help text for accessibility features
        /// </summary>
        public static string GetHelpText()
        {
            return @"ACCESSIBILITY FEATURES

Colorblind Modes:
• Protanopia (Red-Weak) - Most common type
• Deuteranopia (Green-Weak) - Common type
• Tritanopia (Blue-Weak) - Rare type

These modes adjust colors and add text indicators to help distinguish elements.

High Contrast Mode:
Increases contrast between UI elements for better visibility.

Text Scale:
Adjusts the size of all text in the game (80% - 150%).

Damage Numbers:
Toggle floating damage numbers during combat.

Screen Reader Mode:
Announces important game events for visually impaired players.

Difficulty Explanations:
View detailed explanations of what each difficulty mode means in terms of enemy stats.";
        }
    }
}
