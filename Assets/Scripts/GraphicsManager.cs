using UnityEngine;
using System.Collections.Generic;

namespace ACOTAR
{
    /// <summary>
    /// Quality presets for graphics settings
    /// </summary>
    public enum GraphicsQuality
    {
        Low,
        Medium,
        High,
        Ultra
    }

    /// <summary>
    /// Resolution presets for display
    /// </summary>
    public enum ResolutionPreset
    {
        HD_720p,      // 1280x720
        FullHD_1080p, // 1920x1080
        QHD_1440p,    // 2560x1440
        UHD_4K        // 3840x2160
    }

    /// <summary>
    /// Centralized graphics management system for ACOTAR RPG
    /// Handles visual quality settings, post-processing, and rendering options
    /// </summary>
    public class GraphicsManager : MonoBehaviour
    {
        public static GraphicsManager Instance { get; private set; }

        [Header("Quality Settings")]
        [SerializeField] private GraphicsQuality currentQuality = GraphicsQuality.High;
        [SerializeField] private bool vSyncEnabled = true;
        [SerializeField] private int targetFrameRate = 60;

        [Header("Display Settings")]
        [SerializeField] private bool fullscreen = true;
        [SerializeField] private ResolutionPreset resolutionPreset = ResolutionPreset.FullHD_1080p;

        [Header("Visual Effects")]
        [SerializeField] private bool particleEffectsEnabled = true;
        [SerializeField] private bool screenEffectsEnabled = true;
        [SerializeField] private bool shadowsEnabled = true;
        [SerializeField] private bool ambientOcclusionEnabled = true;
        [SerializeField] private bool bloomEnabled = true;
        [SerializeField] private bool antiAliasingEnabled = true;

        [Header("Performance")]
        [SerializeField] private float renderScale = 1.0f;
        [SerializeField] private int shadowResolution = 2048;
        [SerializeField] private float shadowDistance = 100f;
        [SerializeField] private int particleDensity = 100;

        [Header("UI Scaling")]
        [SerializeField] private float uiScale = 1.0f;
        [SerializeField] private bool adaptiveUI = true;

        // Events for graphics changes
        public event System.Action<GraphicsQuality> OnQualityChanged;
        public event System.Action<bool> OnFullscreenChanged;
        public event System.Action<ResolutionPreset> OnResolutionChanged;

        // Cached settings
        private Dictionary<GraphicsQuality, QualityPreset> qualityPresets;

        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGraphicsManager();
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        /// <summary>
        /// Initialize graphics system with default settings
        /// </summary>
        private void InitializeGraphicsManager()
        {
            InitializeQualityPresets();
            LoadGraphicsSettings();
            ApplyCurrentSettings();

            Debug.Log($"GraphicsManager initialized - Quality: {currentQuality}, Resolution: {resolutionPreset}");
        }

        /// <summary>
        /// Initialize quality preset configurations
        /// </summary>
        private void InitializeQualityPresets()
        {
            qualityPresets = new Dictionary<GraphicsQuality, QualityPreset>
            {
                {
                    GraphicsQuality.Low, new QualityPreset
                    {
                        ShadowResolution = 512,
                        ShadowDistance = 30f,
                        ParticleDensity = 25,
                        RenderScale = 0.75f,
                        AntiAliasing = 0,
                        ShadowsEnabled = false,
                        BloomEnabled = false,
                        AmbientOcclusionEnabled = false,
                        TextureQuality = 2
                    }
                },
                {
                    GraphicsQuality.Medium, new QualityPreset
                    {
                        ShadowResolution = 1024,
                        ShadowDistance = 60f,
                        ParticleDensity = 50,
                        RenderScale = 1.0f,
                        AntiAliasing = 2,
                        ShadowsEnabled = true,
                        BloomEnabled = false,
                        AmbientOcclusionEnabled = false,
                        TextureQuality = 1
                    }
                },
                {
                    GraphicsQuality.High, new QualityPreset
                    {
                        ShadowResolution = 2048,
                        ShadowDistance = 100f,
                        ParticleDensity = 100,
                        RenderScale = 1.0f,
                        AntiAliasing = 4,
                        ShadowsEnabled = true,
                        BloomEnabled = true,
                        AmbientOcclusionEnabled = true,
                        TextureQuality = 0
                    }
                },
                {
                    GraphicsQuality.Ultra, new QualityPreset
                    {
                        ShadowResolution = 4096,
                        ShadowDistance = 150f,
                        ParticleDensity = 150,
                        RenderScale = 1.25f,
                        AntiAliasing = 8,
                        ShadowsEnabled = true,
                        BloomEnabled = true,
                        AmbientOcclusionEnabled = true,
                        TextureQuality = 0
                    }
                }
            };
        }

        /// <summary>
        /// Set graphics quality preset
        /// </summary>
        public void SetQuality(GraphicsQuality quality)
        {
            if (currentQuality == quality) return;

            currentQuality = quality;
            ApplyQualityPreset(quality);
            OnQualityChanged?.Invoke(quality);
            SaveGraphicsSettings();

            Debug.Log($"Graphics quality set to: {quality}");
        }

        /// <summary>
        /// Apply a quality preset
        /// </summary>
        private void ApplyQualityPreset(GraphicsQuality quality)
        {
            if (!qualityPresets.ContainsKey(quality)) return;

            QualityPreset preset = qualityPresets[quality];

            shadowResolution = preset.ShadowResolution;
            shadowDistance = preset.ShadowDistance;
            particleDensity = preset.ParticleDensity;
            renderScale = preset.RenderScale;
            shadowsEnabled = preset.ShadowsEnabled;
            bloomEnabled = preset.BloomEnabled;
            ambientOcclusionEnabled = preset.AmbientOcclusionEnabled;

            // Apply Unity quality settings
            QualitySettings.SetQualityLevel((int)quality, true);
            QualitySettings.shadowResolution = GetShadowResolution(preset.ShadowResolution);
            QualitySettings.shadowDistance = preset.ShadowDistance;
            QualitySettings.antiAliasing = preset.AntiAliasing;
            QualitySettings.globalTextureMipmapLimit = preset.TextureQuality;
        }

        /// <summary>
        /// Get Unity shadow resolution enum from int
        /// </summary>
        private ShadowResolution GetShadowResolution(int resolution)
        {
            return resolution switch
            {
                <= 512 => ShadowResolution.Low,
                <= 1024 => ShadowResolution.Medium,
                <= 2048 => ShadowResolution.High,
                _ => ShadowResolution.VeryHigh
            };
        }

        /// <summary>
        /// Set display resolution
        /// </summary>
        public void SetResolution(ResolutionPreset preset)
        {
            resolutionPreset = preset;
            var (width, height) = GetResolutionDimensions(preset);
            Screen.SetResolution(width, height, fullscreen);
            OnResolutionChanged?.Invoke(preset);
            SaveGraphicsSettings();

            Debug.Log($"Resolution set to: {width}x{height} ({preset})");
        }

        /// <summary>
        /// Get resolution dimensions from preset
        /// </summary>
        private (int width, int height) GetResolutionDimensions(ResolutionPreset preset)
        {
            return preset switch
            {
                ResolutionPreset.HD_720p => (1280, 720),
                ResolutionPreset.FullHD_1080p => (1920, 1080),
                ResolutionPreset.QHD_1440p => (2560, 1440),
                ResolutionPreset.UHD_4K => (3840, 2160),
                _ => (1920, 1080)
            };
        }

        /// <summary>
        /// Toggle fullscreen mode
        /// </summary>
        public void SetFullscreen(bool enabled)
        {
            fullscreen = enabled;
            Screen.fullScreen = enabled;
            OnFullscreenChanged?.Invoke(enabled);
            SaveGraphicsSettings();

            Debug.Log($"Fullscreen: {enabled}");
        }

        /// <summary>
        /// Toggle VSync
        /// </summary>
        public void SetVSync(bool enabled)
        {
            vSyncEnabled = enabled;
            QualitySettings.vSyncCount = enabled ? 1 : 0;
            SaveGraphicsSettings();

            Debug.Log($"VSync: {enabled}");
        }

        /// <summary>
        /// Set target frame rate
        /// </summary>
        public void SetTargetFrameRate(int fps)
        {
            targetFrameRate = Mathf.Clamp(fps, 30, 240);
            Application.targetFrameRate = targetFrameRate;
            SaveGraphicsSettings();

            Debug.Log($"Target frame rate: {fps}");
        }

        /// <summary>
        /// Toggle particle effects
        /// </summary>
        public void SetParticleEffectsEnabled(bool enabled)
        {
            particleEffectsEnabled = enabled;
            SaveGraphicsSettings();
        }

        /// <summary>
        /// Toggle screen effects (shake, flash, etc.)
        /// </summary>
        public void SetScreenEffectsEnabled(bool enabled)
        {
            screenEffectsEnabled = enabled;
            SaveGraphicsSettings();
        }

        /// <summary>
        /// Set UI scale factor
        /// </summary>
        public void SetUIScale(float scale)
        {
            uiScale = Mathf.Clamp(scale, 0.5f, 2.0f);
            SaveGraphicsSettings();
        }

        /// <summary>
        /// Apply all current settings
        /// </summary>
        private void ApplyCurrentSettings()
        {
            ApplyQualityPreset(currentQuality);
            
            var (width, height) = GetResolutionDimensions(resolutionPreset);
            Screen.SetResolution(width, height, fullscreen);
            
            QualitySettings.vSyncCount = vSyncEnabled ? 1 : 0;
            Application.targetFrameRate = targetFrameRate;
        }

        /// <summary>
        /// Save graphics settings to PlayerPrefs
        /// </summary>
        private void SaveGraphicsSettings()
        {
            PlayerPrefs.SetInt("GraphicsQuality", (int)currentQuality);
            PlayerPrefs.SetInt("ResolutionPreset", (int)resolutionPreset);
            PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
            PlayerPrefs.SetInt("VSync", vSyncEnabled ? 1 : 0);
            PlayerPrefs.SetInt("TargetFPS", targetFrameRate);
            PlayerPrefs.SetInt("ParticleEffects", particleEffectsEnabled ? 1 : 0);
            PlayerPrefs.SetInt("ScreenEffects", screenEffectsEnabled ? 1 : 0);
            PlayerPrefs.SetFloat("UIScale", uiScale);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Load graphics settings from PlayerPrefs
        /// </summary>
        private void LoadGraphicsSettings()
        {
            if (PlayerPrefs.HasKey("GraphicsQuality"))
            {
                currentQuality = (GraphicsQuality)PlayerPrefs.GetInt("GraphicsQuality");
            }
            if (PlayerPrefs.HasKey("ResolutionPreset"))
            {
                resolutionPreset = (ResolutionPreset)PlayerPrefs.GetInt("ResolutionPreset");
            }
            if (PlayerPrefs.HasKey("Fullscreen"))
            {
                fullscreen = PlayerPrefs.GetInt("Fullscreen") == 1;
            }
            if (PlayerPrefs.HasKey("VSync"))
            {
                vSyncEnabled = PlayerPrefs.GetInt("VSync") == 1;
            }
            if (PlayerPrefs.HasKey("TargetFPS"))
            {
                targetFrameRate = PlayerPrefs.GetInt("TargetFPS");
            }
            if (PlayerPrefs.HasKey("ParticleEffects"))
            {
                particleEffectsEnabled = PlayerPrefs.GetInt("ParticleEffects") == 1;
            }
            if (PlayerPrefs.HasKey("ScreenEffects"))
            {
                screenEffectsEnabled = PlayerPrefs.GetInt("ScreenEffects") == 1;
            }
            if (PlayerPrefs.HasKey("UIScale"))
            {
                uiScale = PlayerPrefs.GetFloat("UIScale");
            }
        }

        /// <summary>
        /// Reset to default settings
        /// </summary>
        public void ResetToDefaults()
        {
            currentQuality = GraphicsQuality.High;
            resolutionPreset = ResolutionPreset.FullHD_1080p;
            fullscreen = true;
            vSyncEnabled = true;
            targetFrameRate = 60;
            particleEffectsEnabled = true;
            screenEffectsEnabled = true;
            uiScale = 1.0f;

            ApplyCurrentSettings();
            SaveGraphicsSettings();

            Debug.Log("Graphics settings reset to defaults");
        }

        // Public accessors for settings
        public GraphicsQuality CurrentQuality => currentQuality;
        public ResolutionPreset CurrentResolution => resolutionPreset;
        public bool IsFullscreen => fullscreen;
        public bool IsVSyncEnabled => vSyncEnabled;
        public int TargetFrameRate => targetFrameRate;
        public bool AreParticleEffectsEnabled => particleEffectsEnabled;
        public bool AreScreenEffectsEnabled => screenEffectsEnabled;
        public float UIScale => uiScale;
        public int ParticleDensity => particleDensity;
        public float RenderScale => renderScale;

        /// <summary>
        /// Display current graphics settings
        /// </summary>
        public void DisplaySettings()
        {
            Debug.Log("\n=== Graphics Settings ===");
            Debug.Log($"Quality: {currentQuality}");
            Debug.Log($"Resolution: {resolutionPreset}");
            Debug.Log($"Fullscreen: {fullscreen}");
            Debug.Log($"VSync: {vSyncEnabled}");
            Debug.Log($"Target FPS: {targetFrameRate}");
            Debug.Log($"Particle Effects: {particleEffectsEnabled}");
            Debug.Log($"Screen Effects: {screenEffectsEnabled}");
            Debug.Log($"UI Scale: {uiScale}");
            Debug.Log("==========================\n");
        }
    }

    /// <summary>
    /// Quality preset configuration
    /// </summary>
    [System.Serializable]
    public class QualityPreset
    {
        public int ShadowResolution;
        public float ShadowDistance;
        public int ParticleDensity;
        public float RenderScale;
        public int AntiAliasing;
        public bool ShadowsEnabled;
        public bool BloomEnabled;
        public bool AmbientOcclusionEnabled;
        public int TextureQuality;
    }
}
