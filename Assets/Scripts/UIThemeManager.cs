using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ACOTAR
{
    /// <summary>
    /// UI color themes matching court aesthetics
    /// </summary>
    public enum UITheme
    {
        Default,
        Spring,
        Summer,
        Autumn,
        Winter,
        Night,
        Dawn,
        Day,
        Dark,
        Light
    }

    /// <summary>
    /// UI element types for theming
    /// </summary>
    public enum UIElementType
    {
        Panel,
        Button,
        ButtonHighlight,
        ButtonPressed,
        ButtonDisabled,
        Text,
        TextHighlight,
        TextMuted,
        Header,
        Border,
        Accent,
        Health,
        Magic,
        Experience,
        Warning,
        Success,
        Error
    }

    /// <summary>
    /// Manages UI theming and styling across the game
    /// Provides consistent visual appearance with court-themed colors
    /// </summary>
    public class UIThemeManager : MonoBehaviour
    {
        public static UIThemeManager Instance { get; private set; }

        [Header("Current Theme")]
        [SerializeField] private UITheme currentTheme = UITheme.Night;

        [Header("Font Settings")]
        [SerializeField] private Font primaryFont;
        [SerializeField] private Font headerFont;
        [SerializeField] private int baseFontSize = 14;

        [Header("Animation Settings")]
        [SerializeField] private float buttonTransitionDuration = 0.1f;
        [SerializeField] private float panelFadeDuration = 0.3f;

        // Theme color palettes
        private Dictionary<UITheme, UIColorPalette> themePalettes;

        // Tracked UI elements for dynamic theming
        private List<ThemedUIElement> trackedElements;

        // Events
        public event System.Action<UITheme> OnThemeChanged;

        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeThemeManager();
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        /// <summary>
        /// Initialize the theme manager
        /// </summary>
        private void InitializeThemeManager()
        {
            trackedElements = new List<ThemedUIElement>();
            InitializeThemePalettes();
            
            Debug.Log($"UIThemeManager initialized with {currentTheme} theme");
        }

        /// <summary>
        /// Initialize all theme color palettes
        /// </summary>
        private void InitializeThemePalettes()
        {
            themePalettes = new Dictionary<UITheme, UIColorPalette>
            {
                {
                    UITheme.Default, new UIColorPalette
                    {
                        Panel = new Color(0.15f, 0.15f, 0.15f, 0.95f),
                        PanelHeader = new Color(0.2f, 0.2f, 0.2f, 1f),
                        Button = new Color(0.3f, 0.3f, 0.3f, 1f),
                        ButtonHighlight = new Color(0.4f, 0.4f, 0.4f, 1f),
                        ButtonPressed = new Color(0.25f, 0.25f, 0.25f, 1f),
                        ButtonDisabled = new Color(0.2f, 0.2f, 0.2f, 0.5f),
                        Text = new Color(0.9f, 0.9f, 0.9f, 1f),
                        TextHighlight = new Color(1f, 1f, 1f, 1f),
                        TextMuted = new Color(0.6f, 0.6f, 0.6f, 1f),
                        Header = new Color(1f, 0.9f, 0.7f, 1f),
                        Border = new Color(0.4f, 0.4f, 0.4f, 1f),
                        Accent = new Color(0.6f, 0.6f, 0.8f, 1f),
                        Health = new Color(0.8f, 0.2f, 0.2f, 1f),
                        Magic = new Color(0.3f, 0.5f, 0.9f, 1f),
                        Experience = new Color(0.8f, 0.7f, 0.2f, 1f),
                        Warning = new Color(1f, 0.7f, 0.2f, 1f),
                        Success = new Color(0.3f, 0.8f, 0.3f, 1f),
                        Error = new Color(0.9f, 0.2f, 0.2f, 1f)
                    }
                },
                {
                    UITheme.Night, new UIColorPalette
                    {
                        Panel = new Color(0.08f, 0.05f, 0.15f, 0.95f),
                        PanelHeader = new Color(0.15f, 0.1f, 0.25f, 1f),
                        Button = new Color(0.2f, 0.15f, 0.35f, 1f),
                        ButtonHighlight = new Color(0.35f, 0.25f, 0.55f, 1f),
                        ButtonPressed = new Color(0.15f, 0.1f, 0.25f, 1f),
                        ButtonDisabled = new Color(0.15f, 0.1f, 0.2f, 0.5f),
                        Text = new Color(0.9f, 0.9f, 1f, 1f),
                        TextHighlight = new Color(1f, 1f, 1f, 1f),
                        TextMuted = new Color(0.5f, 0.5f, 0.7f, 1f),
                        Header = new Color(0.9f, 0.85f, 1f, 1f),
                        Border = new Color(0.4f, 0.3f, 0.6f, 1f),
                        Accent = new Color(0.6f, 0.4f, 0.9f, 1f),
                        Health = new Color(0.9f, 0.3f, 0.4f, 1f),
                        Magic = new Color(0.5f, 0.3f, 0.9f, 1f),
                        Experience = new Color(0.9f, 0.8f, 0.4f, 1f),
                        Warning = new Color(1f, 0.6f, 0.3f, 1f),
                        Success = new Color(0.4f, 0.9f, 0.6f, 1f),
                        Error = new Color(1f, 0.3f, 0.4f, 1f)
                    }
                },
                {
                    UITheme.Spring, new UIColorPalette
                    {
                        Panel = new Color(0.15f, 0.2f, 0.15f, 0.95f),
                        PanelHeader = new Color(0.2f, 0.3f, 0.2f, 1f),
                        Button = new Color(0.25f, 0.4f, 0.25f, 1f),
                        ButtonHighlight = new Color(0.35f, 0.55f, 0.35f, 1f),
                        ButtonPressed = new Color(0.2f, 0.3f, 0.2f, 1f),
                        ButtonDisabled = new Color(0.2f, 0.25f, 0.2f, 0.5f),
                        Text = new Color(0.95f, 1f, 0.95f, 1f),
                        TextHighlight = new Color(1f, 1f, 1f, 1f),
                        TextMuted = new Color(0.6f, 0.7f, 0.6f, 1f),
                        Header = new Color(1f, 0.95f, 0.85f, 1f),
                        Border = new Color(0.4f, 0.6f, 0.4f, 1f),
                        Accent = new Color(1f, 0.7f, 0.8f, 1f),
                        Health = new Color(0.9f, 0.3f, 0.3f, 1f),
                        Magic = new Color(0.5f, 0.8f, 0.5f, 1f),
                        Experience = new Color(1f, 0.9f, 0.5f, 1f),
                        Warning = new Color(1f, 0.7f, 0.3f, 1f),
                        Success = new Color(0.4f, 0.9f, 0.4f, 1f),
                        Error = new Color(0.9f, 0.3f, 0.3f, 1f)
                    }
                },
                {
                    UITheme.Summer, new UIColorPalette
                    {
                        Panel = new Color(0.1f, 0.15f, 0.2f, 0.95f),
                        PanelHeader = new Color(0.15f, 0.25f, 0.35f, 1f),
                        Button = new Color(0.2f, 0.35f, 0.5f, 1f),
                        ButtonHighlight = new Color(0.3f, 0.5f, 0.7f, 1f),
                        ButtonPressed = new Color(0.15f, 0.25f, 0.35f, 1f),
                        ButtonDisabled = new Color(0.15f, 0.2f, 0.25f, 0.5f),
                        Text = new Color(0.95f, 0.98f, 1f, 1f),
                        TextHighlight = new Color(1f, 1f, 1f, 1f),
                        TextMuted = new Color(0.6f, 0.7f, 0.8f, 1f),
                        Header = new Color(1f, 0.95f, 0.8f, 1f),
                        Border = new Color(0.4f, 0.6f, 0.8f, 1f),
                        Accent = new Color(0f, 0.8f, 0.9f, 1f),
                        Health = new Color(0.9f, 0.4f, 0.4f, 1f),
                        Magic = new Color(0.3f, 0.6f, 1f, 1f),
                        Experience = new Color(1f, 0.85f, 0.4f, 1f),
                        Warning = new Color(1f, 0.7f, 0.3f, 1f),
                        Success = new Color(0.3f, 0.9f, 0.6f, 1f),
                        Error = new Color(1f, 0.4f, 0.4f, 1f)
                    }
                },
                {
                    UITheme.Autumn, new UIColorPalette
                    {
                        Panel = new Color(0.18f, 0.1f, 0.08f, 0.95f),
                        PanelHeader = new Color(0.3f, 0.15f, 0.1f, 1f),
                        Button = new Color(0.5f, 0.25f, 0.1f, 1f),
                        ButtonHighlight = new Color(0.7f, 0.35f, 0.15f, 1f),
                        ButtonPressed = new Color(0.35f, 0.18f, 0.08f, 1f),
                        ButtonDisabled = new Color(0.25f, 0.15f, 0.1f, 0.5f),
                        Text = new Color(1f, 0.95f, 0.9f, 1f),
                        TextHighlight = new Color(1f, 1f, 1f, 1f),
                        TextMuted = new Color(0.7f, 0.6f, 0.5f, 1f),
                        Header = new Color(1f, 0.85f, 0.6f, 1f),
                        Border = new Color(0.7f, 0.4f, 0.2f, 1f),
                        Accent = new Color(1f, 0.6f, 0.2f, 1f),
                        Health = new Color(0.9f, 0.3f, 0.2f, 1f),
                        Magic = new Color(0.9f, 0.5f, 0.2f, 1f),
                        Experience = new Color(1f, 0.8f, 0.3f, 1f),
                        Warning = new Color(1f, 0.6f, 0.2f, 1f),
                        Success = new Color(0.6f, 0.8f, 0.3f, 1f),
                        Error = new Color(0.9f, 0.2f, 0.1f, 1f)
                    }
                },
                {
                    UITheme.Winter, new UIColorPalette
                    {
                        Panel = new Color(0.15f, 0.18f, 0.22f, 0.95f),
                        PanelHeader = new Color(0.2f, 0.25f, 0.3f, 1f),
                        Button = new Color(0.3f, 0.38f, 0.45f, 1f),
                        ButtonHighlight = new Color(0.45f, 0.55f, 0.65f, 1f),
                        ButtonPressed = new Color(0.2f, 0.25f, 0.3f, 1f),
                        ButtonDisabled = new Color(0.2f, 0.22f, 0.25f, 0.5f),
                        Text = new Color(0.95f, 0.98f, 1f, 1f),
                        TextHighlight = new Color(1f, 1f, 1f, 1f),
                        TextMuted = new Color(0.6f, 0.7f, 0.8f, 1f),
                        Header = new Color(0.85f, 0.95f, 1f, 1f),
                        Border = new Color(0.5f, 0.7f, 0.9f, 1f),
                        Accent = new Color(0.7f, 0.9f, 1f, 1f),
                        Health = new Color(0.9f, 0.4f, 0.5f, 1f),
                        Magic = new Color(0.5f, 0.7f, 1f, 1f),
                        Experience = new Color(0.9f, 0.95f, 0.6f, 1f),
                        Warning = new Color(1f, 0.8f, 0.4f, 1f),
                        Success = new Color(0.5f, 0.9f, 0.7f, 1f),
                        Error = new Color(1f, 0.4f, 0.5f, 1f)
                    }
                },
                {
                    UITheme.Dawn, new UIColorPalette
                    {
                        Panel = new Color(0.2f, 0.15f, 0.15f, 0.95f),
                        PanelHeader = new Color(0.35f, 0.25f, 0.22f, 1f),
                        Button = new Color(0.5f, 0.35f, 0.3f, 1f),
                        ButtonHighlight = new Color(0.7f, 0.5f, 0.4f, 1f),
                        ButtonPressed = new Color(0.35f, 0.25f, 0.22f, 1f),
                        ButtonDisabled = new Color(0.25f, 0.2f, 0.18f, 0.5f),
                        Text = new Color(1f, 0.97f, 0.95f, 1f),
                        TextHighlight = new Color(1f, 1f, 1f, 1f),
                        TextMuted = new Color(0.7f, 0.6f, 0.55f, 1f),
                        Header = new Color(1f, 0.9f, 0.8f, 1f),
                        Border = new Color(0.8f, 0.6f, 0.5f, 1f),
                        Accent = new Color(1f, 0.7f, 0.5f, 1f),
                        Health = new Color(1f, 0.5f, 0.5f, 1f),
                        Magic = new Color(1f, 0.7f, 0.6f, 1f),
                        Experience = new Color(1f, 0.9f, 0.5f, 1f),
                        Warning = new Color(1f, 0.7f, 0.4f, 1f),
                        Success = new Color(0.6f, 0.9f, 0.6f, 1f),
                        Error = new Color(1f, 0.4f, 0.4f, 1f)
                    }
                },
                {
                    UITheme.Day, new UIColorPalette
                    {
                        Panel = new Color(0.25f, 0.22f, 0.15f, 0.95f),
                        PanelHeader = new Color(0.4f, 0.35f, 0.2f, 1f),
                        Button = new Color(0.55f, 0.5f, 0.3f, 1f),
                        ButtonHighlight = new Color(0.75f, 0.68f, 0.4f, 1f),
                        ButtonPressed = new Color(0.4f, 0.35f, 0.2f, 1f),
                        ButtonDisabled = new Color(0.3f, 0.27f, 0.18f, 0.5f),
                        Text = new Color(1f, 0.98f, 0.92f, 1f),
                        TextHighlight = new Color(1f, 1f, 1f, 1f),
                        TextMuted = new Color(0.7f, 0.65f, 0.5f, 1f),
                        Header = new Color(1f, 0.95f, 0.75f, 1f),
                        Border = new Color(0.85f, 0.75f, 0.4f, 1f),
                        Accent = new Color(1f, 0.9f, 0.4f, 1f),
                        Health = new Color(0.95f, 0.4f, 0.35f, 1f),
                        Magic = new Color(1f, 0.85f, 0.4f, 1f),
                        Experience = new Color(1f, 0.95f, 0.5f, 1f),
                        Warning = new Color(1f, 0.75f, 0.35f, 1f),
                        Success = new Color(0.6f, 0.9f, 0.5f, 1f),
                        Error = new Color(0.95f, 0.35f, 0.3f, 1f)
                    }
                },
                {
                    UITheme.Dark, new UIColorPalette
                    {
                        Panel = new Color(0.1f, 0.1f, 0.1f, 0.98f),
                        PanelHeader = new Color(0.15f, 0.15f, 0.15f, 1f),
                        Button = new Color(0.25f, 0.25f, 0.25f, 1f),
                        ButtonHighlight = new Color(0.35f, 0.35f, 0.35f, 1f),
                        ButtonPressed = new Color(0.18f, 0.18f, 0.18f, 1f),
                        ButtonDisabled = new Color(0.15f, 0.15f, 0.15f, 0.5f),
                        Text = new Color(0.85f, 0.85f, 0.85f, 1f),
                        TextHighlight = new Color(1f, 1f, 1f, 1f),
                        TextMuted = new Color(0.5f, 0.5f, 0.5f, 1f),
                        Header = new Color(0.9f, 0.9f, 0.9f, 1f),
                        Border = new Color(0.3f, 0.3f, 0.3f, 1f),
                        Accent = new Color(0.4f, 0.6f, 0.9f, 1f),
                        Health = new Color(0.8f, 0.25f, 0.25f, 1f),
                        Magic = new Color(0.3f, 0.5f, 0.9f, 1f),
                        Experience = new Color(0.9f, 0.8f, 0.3f, 1f),
                        Warning = new Color(0.95f, 0.7f, 0.25f, 1f),
                        Success = new Color(0.3f, 0.8f, 0.4f, 1f),
                        Error = new Color(0.9f, 0.3f, 0.3f, 1f)
                    }
                },
                {
                    UITheme.Light, new UIColorPalette
                    {
                        Panel = new Color(0.95f, 0.95f, 0.95f, 0.98f),
                        PanelHeader = new Color(0.9f, 0.9f, 0.9f, 1f),
                        Button = new Color(0.85f, 0.85f, 0.85f, 1f),
                        ButtonHighlight = new Color(0.75f, 0.75f, 0.75f, 1f),
                        ButtonPressed = new Color(0.7f, 0.7f, 0.7f, 1f),
                        ButtonDisabled = new Color(0.8f, 0.8f, 0.8f, 0.5f),
                        Text = new Color(0.15f, 0.15f, 0.15f, 1f),
                        TextHighlight = new Color(0f, 0f, 0f, 1f),
                        TextMuted = new Color(0.4f, 0.4f, 0.4f, 1f),
                        Header = new Color(0.1f, 0.1f, 0.1f, 1f),
                        Border = new Color(0.6f, 0.6f, 0.6f, 1f),
                        Accent = new Color(0.2f, 0.5f, 0.8f, 1f),
                        Health = new Color(0.85f, 0.2f, 0.2f, 1f),
                        Magic = new Color(0.2f, 0.4f, 0.85f, 1f),
                        Experience = new Color(0.85f, 0.75f, 0.2f, 1f),
                        Warning = new Color(0.9f, 0.65f, 0.15f, 1f),
                        Success = new Color(0.2f, 0.7f, 0.3f, 1f),
                        Error = new Color(0.85f, 0.2f, 0.2f, 1f)
                    }
                }
            };
        }

        /// <summary>
        /// Set the current UI theme
        /// </summary>
        public void SetTheme(UITheme theme)
        {
            if (currentTheme == theme) return;

            currentTheme = theme;
            ApplyThemeToTrackedElements();
            OnThemeChanged?.Invoke(theme);

            Debug.Log($"UI Theme changed to: {theme}");
        }

        /// <summary>
        /// Set theme based on court allegiance
        /// </summary>
        public void SetThemeForCourt(Court court)
        {
            UITheme theme = court switch
            {
                Court.Spring => UITheme.Spring,
                Court.Summer => UITheme.Summer,
                Court.Autumn => UITheme.Autumn,
                Court.Winter => UITheme.Winter,
                Court.Night => UITheme.Night,
                Court.Dawn => UITheme.Dawn,
                Court.Day => UITheme.Day,
                _ => UITheme.Default
            };

            SetTheme(theme);
        }

        /// <summary>
        /// Get a color for a specific UI element type
        /// </summary>
        public Color GetColor(UIElementType elementType)
        {
            UIColorPalette palette = GetCurrentPalette();
            
            return elementType switch
            {
                UIElementType.Panel => palette.Panel,
                UIElementType.Button => palette.Button,
                UIElementType.ButtonHighlight => palette.ButtonHighlight,
                UIElementType.ButtonPressed => palette.ButtonPressed,
                UIElementType.ButtonDisabled => palette.ButtonDisabled,
                UIElementType.Text => palette.Text,
                UIElementType.TextHighlight => palette.TextHighlight,
                UIElementType.TextMuted => palette.TextMuted,
                UIElementType.Header => palette.Header,
                UIElementType.Border => palette.Border,
                UIElementType.Accent => palette.Accent,
                UIElementType.Health => palette.Health,
                UIElementType.Magic => palette.Magic,
                UIElementType.Experience => palette.Experience,
                UIElementType.Warning => palette.Warning,
                UIElementType.Success => palette.Success,
                UIElementType.Error => palette.Error,
                _ => Color.white
            };
        }

        /// <summary>
        /// Get the current color palette
        /// </summary>
        public UIColorPalette GetCurrentPalette()
        {
            return themePalettes.ContainsKey(currentTheme) ? themePalettes[currentTheme] : themePalettes[UITheme.Default];
        }

        /// <summary>
        /// Register a UI element for automatic theming
        /// </summary>
        public void RegisterElement(ThemedUIElement element)
        {
            if (!trackedElements.Contains(element))
            {
                trackedElements.Add(element);
                ApplyThemeToElement(element);
            }
        }

        /// <summary>
        /// Unregister a UI element from automatic theming
        /// </summary>
        public void UnregisterElement(ThemedUIElement element)
        {
            trackedElements.Remove(element);
        }

        /// <summary>
        /// Apply theme to all tracked elements
        /// </summary>
        private void ApplyThemeToTrackedElements()
        {
            foreach (var element in trackedElements)
            {
                ApplyThemeToElement(element);
            }
        }

        /// <summary>
        /// Apply theme to a specific element
        /// </summary>
        private void ApplyThemeToElement(ThemedUIElement element)
        {
            if (element == null || element.Target == null) return;

            Color color = GetColor(element.ElementType);

            // Apply color based on component type
            if (element.Target is Image image)
            {
                image.color = color;
            }
            else if (element.Target is Text text)
            {
                text.color = color;
            }
            else if (element.Target is Button button)
            {
                var colors = button.colors;
                colors.normalColor = GetColor(UIElementType.Button);
                colors.highlightedColor = GetColor(UIElementType.ButtonHighlight);
                colors.pressedColor = GetColor(UIElementType.ButtonPressed);
                colors.disabledColor = GetColor(UIElementType.ButtonDisabled);
                colors.fadeDuration = buttonTransitionDuration;
                button.colors = colors;
            }
            else if (element.Target is Slider slider)
            {
                // Apply to slider fill
                if (slider.fillRect != null)
                {
                    Image fillImage = slider.fillRect.GetComponent<Image>();
                    if (fillImage != null)
                    {
                        fillImage.color = color;
                    }
                }
            }
        }

        /// <summary>
        /// Apply theme to a button
        /// </summary>
        public void ApplyButtonTheme(Button button)
        {
            if (button == null) return;

            var colors = button.colors;
            colors.normalColor = GetColor(UIElementType.Button);
            colors.highlightedColor = GetColor(UIElementType.ButtonHighlight);
            colors.pressedColor = GetColor(UIElementType.ButtonPressed);
            colors.disabledColor = GetColor(UIElementType.ButtonDisabled);
            colors.fadeDuration = buttonTransitionDuration;
            button.colors = colors;

            // Apply text color if present
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.color = GetColor(UIElementType.Text);
            }
        }

        /// <summary>
        /// Apply theme to a panel
        /// </summary>
        public void ApplyPanelTheme(Image panel, Text header = null)
        {
            if (panel != null)
            {
                panel.color = GetColor(UIElementType.Panel);
            }

            if (header != null)
            {
                header.color = GetColor(UIElementType.Header);
            }
        }

        /// <summary>
        /// Apply theme to a slider (health bar, etc.)
        /// </summary>
        public void ApplySliderTheme(Slider slider, UIElementType fillType)
        {
            if (slider == null) return;

            if (slider.fillRect != null)
            {
                Image fillImage = slider.fillRect.GetComponent<Image>();
                if (fillImage != null)
                {
                    fillImage.color = GetColor(fillType);
                }
            }
        }

        // Public accessors
        public UITheme CurrentTheme => currentTheme;
        public float ButtonTransitionDuration => buttonTransitionDuration;
        public float PanelFadeDuration => panelFadeDuration;
    }

    /// <summary>
    /// Color palette for a UI theme
    /// </summary>
    [System.Serializable]
    public class UIColorPalette
    {
        public Color Panel;
        public Color PanelHeader;
        public Color Button;
        public Color ButtonHighlight;
        public Color ButtonPressed;
        public Color ButtonDisabled;
        public Color Text;
        public Color TextHighlight;
        public Color TextMuted;
        public Color Header;
        public Color Border;
        public Color Accent;
        public Color Health;
        public Color Magic;
        public Color Experience;
        public Color Warning;
        public Color Success;
        public Color Error;
    }

    /// <summary>
    /// Reference to a themed UI element
    /// </summary>
    [System.Serializable]
    public class ThemedUIElement
    {
        public Component Target;
        public UIElementType ElementType;
    }
}
