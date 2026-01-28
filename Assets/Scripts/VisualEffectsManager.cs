using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ACOTAR
{
    /// <summary>
    /// Types of visual effects
    /// </summary>
    public enum VFXType
    {
        // Combat effects
        PhysicalHit,
        CriticalHit,
        MagicCast,
        MagicImpact,
        Heal,
        Shield,
        Buff,
        Debuff,
        
        // Elemental effects
        FireBurst,
        FireTrail,
        IceShatter,
        IceCrystals,
        LightningStrike,
        WaterSplash,
        WindGust,
        DarkTendrils,
        LightBeam,
        NatureGrowth,
        DeathAura,
        
        // Status effects
        Bleeding,
        Burning,
        Frozen,
        Poisoned,
        Stunned,
        
        // UI effects
        LevelUp,
        QuestComplete,
        ItemPickup,
        AbilityUnlock,
        
        // Environmental
        Teleport,
        CourtTransition,
        MoonMagic
    }

    /// <summary>
    /// Configuration for a visual effect
    /// </summary>
    [System.Serializable]
    public class VFXConfig
    {
        public VFXType Type;
        public Color PrimaryColor;
        public Color SecondaryColor;
        public float Duration;
        public float Scale;
        public bool Loop;
        public int ParticleCount;
        public AudioClip Sound;
    }

    /// <summary>
    /// Visual effects system for ACOTAR RPG
    /// Manages particle effects, screen effects, and visual feedback
    /// </summary>
    public class VisualEffectsManager : MonoBehaviour
    {
        public static VisualEffectsManager Instance { get; private set; }

        [Header("Effect Prefabs")]
        [SerializeField] private GameObject genericParticlePrefab;
        [SerializeField] private GameObject impactParticlePrefab;
        [SerializeField] private GameObject trailParticlePrefab;
        [SerializeField] private GameObject auraParticlePrefab;

        [Header("Court Color Themes")]
        private Dictionary<Court, CourtColorTheme> courtColors;

        [Header("Element Colors")]
        private Dictionary<Element, ElementColorTheme> elementColors;

        [Header("Effect Pool")]
        private Dictionary<VFXType, Queue<GameObject>> effectPools;
        private int poolSize = 10;

        [Header("Active Effects")]
        private List<ActiveVFX> activeEffects;

        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeVFXSystem();
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        /// <summary>
        /// Initialize the VFX system
        /// </summary>
        private void InitializeVFXSystem()
        {
            effectPools = new Dictionary<VFXType, Queue<GameObject>>();
            activeEffects = new List<ActiveVFX>();
            
            InitializeCourtColors();
            InitializeElementColors();
            
            Debug.Log("VisualEffectsManager initialized");
        }

        /// <summary>
        /// Initialize court-themed color palettes
        /// </summary>
        private void InitializeCourtColors()
        {
            courtColors = new Dictionary<Court, CourtColorTheme>
            {
                {
                    Court.Spring, new CourtColorTheme
                    {
                        Primary = new Color(0.4f, 0.8f, 0.4f),      // Green
                        Secondary = new Color(1f, 0.8f, 0.9f),      // Pink
                        Accent = new Color(0.9f, 0.95f, 0.8f),      // Cream
                        Glow = new Color(0.5f, 1f, 0.5f, 0.5f)      // Soft green glow
                    }
                },
                {
                    Court.Summer, new CourtColorTheme
                    {
                        Primary = new Color(0f, 0.6f, 1f),          // Ocean blue
                        Secondary = new Color(1f, 0.85f, 0.4f),     // Sandy gold
                        Accent = new Color(0f, 0.9f, 0.9f),         // Turquoise
                        Glow = new Color(0.3f, 0.7f, 1f, 0.5f)      // Blue glow
                    }
                },
                {
                    Court.Autumn, new CourtColorTheme
                    {
                        Primary = new Color(0.9f, 0.4f, 0.1f),      // Orange
                        Secondary = new Color(0.8f, 0.2f, 0.1f),    // Deep red
                        Accent = new Color(1f, 0.7f, 0.2f),         // Gold
                        Glow = new Color(1f, 0.5f, 0.2f, 0.5f)      // Orange glow
                    }
                },
                {
                    Court.Winter, new CourtColorTheme
                    {
                        Primary = new Color(0.8f, 0.9f, 1f),        // Ice blue
                        Secondary = new Color(1f, 1f, 1f),          // White
                        Accent = new Color(0.6f, 0.8f, 1f),         // Pale blue
                        Glow = new Color(0.7f, 0.9f, 1f, 0.5f)      // Blue-white glow
                    }
                },
                {
                    Court.Night, new CourtColorTheme
                    {
                        Primary = new Color(0.1f, 0.05f, 0.2f),     // Deep purple
                        Secondary = new Color(0.6f, 0.4f, 0.8f),    // Amethyst
                        Accent = new Color(0.9f, 0.9f, 1f),         // Starlight
                        Glow = new Color(0.5f, 0.3f, 0.8f, 0.5f)    // Purple glow
                    }
                },
                {
                    Court.Dawn, new CourtColorTheme
                    {
                        Primary = new Color(1f, 0.7f, 0.5f),        // Peach
                        Secondary = new Color(1f, 0.85f, 0.7f),     // Soft gold
                        Accent = new Color(1f, 0.6f, 0.6f),         // Rose
                        Glow = new Color(1f, 0.8f, 0.6f, 0.5f)      // Warm glow
                    }
                },
                {
                    Court.Day, new CourtColorTheme
                    {
                        Primary = new Color(1f, 0.95f, 0.8f),       // Bright gold
                        Secondary = new Color(1f, 1f, 0.9f),        // White gold
                        Accent = new Color(1f, 0.85f, 0.4f),        // Sun gold
                        Glow = new Color(1f, 0.95f, 0.7f, 0.6f)     // Brilliant glow
                    }
                },
                {
                    Court.None, new CourtColorTheme
                    {
                        Primary = new Color(0.5f, 0.5f, 0.5f),      // Gray
                        Secondary = new Color(0.7f, 0.7f, 0.7f),    // Light gray
                        Accent = new Color(0.9f, 0.9f, 0.9f),       // Near white
                        Glow = new Color(0.6f, 0.6f, 0.6f, 0.4f)    // Neutral glow
                    }
                }
            };
        }

        /// <summary>
        /// Initialize element-themed color palettes
        /// </summary>
        private void InitializeElementColors()
        {
            elementColors = new Dictionary<Element, ElementColorTheme>
            {
                {
                    Element.Fire, new ElementColorTheme
                    {
                        Core = new Color(1f, 0.8f, 0.2f),
                        Outer = new Color(1f, 0.3f, 0f),
                        Particles = new Color(1f, 0.6f, 0.1f)
                    }
                },
                {
                    Element.Ice, new ElementColorTheme
                    {
                        Core = new Color(1f, 1f, 1f),
                        Outer = new Color(0.6f, 0.9f, 1f),
                        Particles = new Color(0.8f, 0.95f, 1f)
                    }
                },
                {
                    Element.Water, new ElementColorTheme
                    {
                        Core = new Color(0.4f, 0.7f, 1f),
                        Outer = new Color(0.2f, 0.4f, 0.9f),
                        Particles = new Color(0.5f, 0.8f, 1f)
                    }
                },
                {
                    Element.Wind, new ElementColorTheme
                    {
                        Core = new Color(0.9f, 1f, 0.95f),
                        Outer = new Color(0.7f, 0.9f, 0.8f),
                        Particles = new Color(0.85f, 1f, 0.9f)
                    }
                },
                {
                    Element.Light, new ElementColorTheme
                    {
                        Core = new Color(1f, 1f, 0.9f),
                        Outer = new Color(1f, 0.95f, 0.7f),
                        Particles = new Color(1f, 1f, 0.8f)
                    }
                },
                {
                    Element.Darkness, new ElementColorTheme
                    {
                        Core = new Color(0.2f, 0.1f, 0.3f),
                        Outer = new Color(0.4f, 0.2f, 0.5f),
                        Particles = new Color(0.1f, 0.05f, 0.2f)
                    }
                },
                {
                    Element.Nature, new ElementColorTheme
                    {
                        Core = new Color(0.3f, 0.8f, 0.3f),
                        Outer = new Color(0.2f, 0.6f, 0.2f),
                        Particles = new Color(0.5f, 0.9f, 0.4f)
                    }
                },
                {
                    Element.Death, new ElementColorTheme
                    {
                        Core = new Color(0.1f, 0.1f, 0.1f),
                        Outer = new Color(0.5f, 0f, 0.1f),
                        Particles = new Color(0.3f, 0f, 0.05f)
                    }
                },
                {
                    Element.None, new ElementColorTheme
                    {
                        Core = new Color(0.8f, 0.8f, 0.8f),
                        Outer = new Color(0.6f, 0.6f, 0.6f),
                        Particles = new Color(0.9f, 0.9f, 0.9f)
                    }
                }
            };
        }

        /// <summary>
        /// Spawn a visual effect at a position
        /// </summary>
        public void SpawnEffect(VFXType effectType, Vector3 position, Quaternion rotation = default)
        {
            if (GraphicsManager.Instance != null && !GraphicsManager.Instance.AreParticleEffectsEnabled)
                return;

            if (rotation == default)
                rotation = Quaternion.identity;

            VFXConfig config = GetEffectConfig(effectType);
            StartCoroutine(PlayEffectCoroutine(effectType, position, rotation, config));
        }

        /// <summary>
        /// Spawn an elemental effect
        /// </summary>
        public void SpawnElementalEffect(Element element, Vector3 position, float scale = 1f)
        {
            if (GraphicsManager.Instance != null && !GraphicsManager.Instance.AreParticleEffectsEnabled)
                return;

            VFXType effectType = GetEffectTypeForElement(element);
            ElementColorTheme colors = GetElementColors(element);

            Debug.Log($"Spawning {element} effect at {position}");
            
            // Would instantiate actual particle system here
            // For now, log the effect
            StartCoroutine(SimulateEffect(effectType, position, colors.Core, 1f));
        }

        /// <summary>
        /// Spawn a magic ability effect
        /// </summary>
        public void SpawnMagicEffect(MagicType magic, Vector3 casterPosition, Vector3 targetPosition)
        {
            if (GraphicsManager.Instance != null && !GraphicsManager.Instance.AreParticleEffectsEnabled)
                return;

            Element element = ElementalSystem.GetElementFromMagic(magic);
            ElementColorTheme colors = GetElementColors(element);

            Debug.Log($"Spawning {magic} effect from {casterPosition} to {targetPosition}");

            // Cast effect at caster
            SpawnEffect(VFXType.MagicCast, casterPosition);

            // Trail to target
            StartCoroutine(SpawnTrailEffect(casterPosition, targetPosition, colors.Core, 0.3f));

            // Impact at target
            StartCoroutine(DelayedEffect(VFXType.MagicImpact, targetPosition, 0.3f));
        }

        /// <summary>
        /// Spawn combat hit effect
        /// </summary>
        public void SpawnHitEffect(Vector3 position, bool isCritical = false, DamageType damageType = DamageType.Physical)
        {
            if (GraphicsManager.Instance != null && !GraphicsManager.Instance.AreParticleEffectsEnabled)
                return;

            VFXType effectType = isCritical ? VFXType.CriticalHit : VFXType.PhysicalHit;
            Color effectColor = GetDamageTypeColor(damageType);

            Debug.Log($"Spawning {(isCritical ? "CRITICAL " : "")}hit effect ({damageType})");
            
            StartCoroutine(SimulateEffect(effectType, position, effectColor, isCritical ? 1.5f : 1f));
        }

        /// <summary>
        /// Spawn status effect visual
        /// </summary>
        public void SpawnStatusEffect(StatusEffectType status, Vector3 position)
        {
            if (GraphicsManager.Instance != null && !GraphicsManager.Instance.AreParticleEffectsEnabled)
                return;

            VFXType effectType = GetEffectTypeForStatus(status);
            Color effectColor = GetStatusEffectColor(status);

            Debug.Log($"Spawning {status} status effect visual");
            
            StartCoroutine(SimulateEffect(effectType, position, effectColor, 1f));
        }

        /// <summary>
        /// Spawn court-themed effect
        /// </summary>
        public void SpawnCourtEffect(Court court, Vector3 position)
        {
            if (GraphicsManager.Instance != null && !GraphicsManager.Instance.AreParticleEffectsEnabled)
                return;

            CourtColorTheme colors = GetCourtColors(court);
            
            Debug.Log($"Spawning {court} Court themed effect");
            
            StartCoroutine(SimulateEffect(VFXType.CourtTransition, position, colors.Primary, 2f));
        }

        /// <summary>
        /// Get configuration for an effect type
        /// </summary>
        private VFXConfig GetEffectConfig(VFXType effectType)
        {
            // Default configurations for effect types
            return effectType switch
            {
                VFXType.PhysicalHit => new VFXConfig { Duration = 0.3f, Scale = 1f, ParticleCount = 15 },
                VFXType.CriticalHit => new VFXConfig { Duration = 0.5f, Scale = 1.5f, ParticleCount = 30 },
                VFXType.MagicCast => new VFXConfig { Duration = 0.5f, Scale = 1.2f, ParticleCount = 25 },
                VFXType.MagicImpact => new VFXConfig { Duration = 0.4f, Scale = 1.3f, ParticleCount = 20 },
                VFXType.Heal => new VFXConfig { Duration = 1f, Scale = 1f, ParticleCount = 20, Loop = false },
                VFXType.Shield => new VFXConfig { Duration = 2f, Scale = 1.5f, ParticleCount = 10, Loop = true },
                VFXType.LevelUp => new VFXConfig { Duration = 2f, Scale = 2f, ParticleCount = 50 },
                VFXType.Teleport => new VFXConfig { Duration = 0.8f, Scale = 1.5f, ParticleCount = 40 },
                _ => new VFXConfig { Duration = 0.5f, Scale = 1f, ParticleCount = 10 }
            };
        }

        /// <summary>
        /// Get VFX type for element
        /// </summary>
        private VFXType GetEffectTypeForElement(Element element)
        {
            return element switch
            {
                Element.Fire => VFXType.FireBurst,
                Element.Ice => VFXType.IceShatter,
                Element.Water => VFXType.WaterSplash,
                Element.Wind => VFXType.WindGust,
                Element.Light => VFXType.LightBeam,
                Element.Darkness => VFXType.DarkTendrils,
                Element.Nature => VFXType.NatureGrowth,
                Element.Death => VFXType.DeathAura,
                _ => VFXType.MagicImpact
            };
        }

        /// <summary>
        /// Get VFX type for status effect
        /// </summary>
        private VFXType GetEffectTypeForStatus(StatusEffectType status)
        {
            return status switch
            {
                StatusEffectType.Bleeding => VFXType.Bleeding,
                StatusEffectType.Burning => VFXType.Burning,
                StatusEffectType.Frozen => VFXType.Frozen,
                StatusEffectType.Poisoned => VFXType.Poisoned,
                StatusEffectType.Stunned => VFXType.Stunned,
                StatusEffectType.Strengthened => VFXType.Buff,
                StatusEffectType.Hastened => VFXType.Buff,
                StatusEffectType.Shielded => VFXType.Shield,
                StatusEffectType.Weakened => VFXType.Debuff,
                StatusEffectType.Cursed => VFXType.Debuff,
                _ => VFXType.Debuff
            };
        }

        /// <summary>
        /// Get color for damage type
        /// </summary>
        private Color GetDamageTypeColor(DamageType damageType)
        {
            return damageType switch
            {
                DamageType.Physical => new Color(1f, 1f, 1f),
                DamageType.Magical => new Color(0.6f, 0.4f, 1f),
                DamageType.Fire => new Color(1f, 0.5f, 0f),
                DamageType.Ice => new Color(0.7f, 0.9f, 1f),
                DamageType.Darkness => new Color(0.3f, 0.1f, 0.4f),
                DamageType.Light => new Color(1f, 1f, 0.8f),
                DamageType.Death => new Color(0.2f, 0f, 0.1f),
                _ => Color.white
            };
        }

        /// <summary>
        /// Get color for status effect
        /// </summary>
        private Color GetStatusEffectColor(StatusEffectType status)
        {
            return status switch
            {
                StatusEffectType.Bleeding => new Color(0.8f, 0.1f, 0.1f),
                StatusEffectType.Burning => new Color(1f, 0.5f, 0f),
                StatusEffectType.Frozen => new Color(0.7f, 0.9f, 1f),
                StatusEffectType.Poisoned => new Color(0.4f, 0.8f, 0.2f),
                StatusEffectType.Stunned => new Color(1f, 1f, 0.5f),
                StatusEffectType.Strengthened => new Color(1f, 0.8f, 0.2f),
                StatusEffectType.Hastened => new Color(0.2f, 1f, 0.8f),
                StatusEffectType.Shielded => new Color(0.4f, 0.6f, 1f),
                StatusEffectType.Weakened => new Color(0.5f, 0.3f, 0.5f),
                StatusEffectType.Cursed => new Color(0.3f, 0f, 0.3f),
                _ => Color.white
            };
        }

        /// <summary>
        /// Get court color theme
        /// </summary>
        public CourtColorTheme GetCourtColors(Court court)
        {
            return courtColors.ContainsKey(court) ? courtColors[court] : courtColors[Court.None];
        }

        /// <summary>
        /// Get element color theme
        /// </summary>
        public ElementColorTheme GetElementColors(Element element)
        {
            return elementColors.ContainsKey(element) ? elementColors[element] : elementColors[Element.None];
        }

        // Coroutines for effect playback

        private IEnumerator PlayEffectCoroutine(VFXType effectType, Vector3 position, Quaternion rotation, VFXConfig config)
        {
            Debug.Log($"Playing effect: {effectType} at {position}");
            
            // In a full implementation, this would instantiate and configure particle systems
            yield return new WaitForSeconds(config.Duration);
            
            Debug.Log($"Effect {effectType} completed");
        }

        private IEnumerator SimulateEffect(VFXType effectType, Vector3 position, Color color, float duration)
        {
            Debug.Log($"VFX: {effectType} - Color: {color} - Duration: {duration}s");
            yield return new WaitForSeconds(duration);
        }

        private IEnumerator SpawnTrailEffect(Vector3 start, Vector3 end, Color color, float duration)
        {
            Debug.Log($"Trail effect from {start} to {end}");
            yield return new WaitForSeconds(duration);
        }

        private IEnumerator DelayedEffect(VFXType effectType, Vector3 position, float delay)
        {
            yield return new WaitForSeconds(delay);
            SpawnEffect(effectType, position);
        }

        /// <summary>
        /// Clear all active effects
        /// </summary>
        public void ClearAllEffects()
        {
            StopAllCoroutines();
            activeEffects.Clear();
            Debug.Log("All visual effects cleared");
        }
    }

    /// <summary>
    /// Court color theme configuration
    /// </summary>
    [System.Serializable]
    public class CourtColorTheme
    {
        public Color Primary;
        public Color Secondary;
        public Color Accent;
        public Color Glow;
    }

    /// <summary>
    /// Element color theme configuration
    /// </summary>
    [System.Serializable]
    public class ElementColorTheme
    {
        public Color Core;
        public Color Outer;
        public Color Particles;
    }

    /// <summary>
    /// Active VFX tracking
    /// </summary>
    public class ActiveVFX
    {
        public VFXType Type;
        public GameObject Instance;
        public float StartTime;
        public float Duration;
    }
}
