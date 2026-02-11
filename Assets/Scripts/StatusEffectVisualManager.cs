using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace ACOTAR
{
    /// <summary>
    /// Manages visual indicators for status effects on characters
    /// Displays active buffs and debuffs with icons and timers
    /// </summary>
    public class StatusEffectVisualManager : MonoBehaviour
    {
        public static StatusEffectVisualManager Instance { get; private set; }

        [Header("Status Effect Icons")]
        public Sprite bleedingIcon;
        public Sprite burningIcon;
        public Sprite poisonedIcon;
        public Sprite frozenIcon;
        public Sprite stunnedIcon;
        public Sprite strengthBuffIcon;
        public Sprite defenseBuffIcon;
        public Sprite speedBuffIcon;
        public Sprite magicBuffIcon;
        public Sprite weakenedIcon;
        public Sprite slowedIcon;
        public Sprite silencedIcon;
        public Sprite blindedIcon;
        public Sprite regeneratingIcon;

        [Header("UI Elements")]
        public GameObject statusEffectIconPrefab;
        public Transform playerStatusContainer;
        public Transform enemyStatusContainer;

        // Track active status effect icons
        private Dictionary<string, List<GameObject>> activeStatusIcons = new Dictionary<string, List<GameObject>>();

        void Awake()
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

        /// <summary>
        /// Update status effect display for a character
        /// </summary>
        public void UpdateStatusEffects(Character character, List<StatusEffect> activeEffects, Transform container)
        {
            if (character == null || container == null)
                return;

            string characterId = character.name;
            
            // Clear existing icons for this character
            ClearStatusIcons(characterId);
            
            if (activeEffects == null || activeEffects.Count == 0)
                return;
            
            // Create new icons for active effects
            foreach (StatusEffect effect in activeEffects)
            {
                CreateStatusIcon(characterId, effect, container);
            }
        }

        /// <summary>
        /// Create a status effect icon
        /// </summary>
        private void CreateStatusIcon(string characterId, StatusEffect effect, Transform container)
        {
            if (statusEffectIconPrefab == null)
            {
                Debug.LogWarning("Status effect icon prefab not set!");
                return;
            }

            GameObject iconObj = Instantiate(statusEffectIconPrefab, container);
            Image iconImage = iconObj.GetComponent<Image>();
            
            if (iconImage != null)
            {
                // Set appropriate icon based on effect type
                Sprite icon = GetStatusEffectIcon(effect.type);
                if (icon != null)
                {
                    iconImage.sprite = icon;
                }
                
                // Set color based on effect category
                iconImage.color = GetStatusEffectColor(effect);
            }

            // Add tooltip with effect details
            var tooltipTrigger = iconObj.AddComponent<StatusEffectTooltipTrigger>();
            tooltipTrigger.statusEffect = effect;

            // Track the icon
            if (!activeStatusIcons.ContainsKey(characterId))
            {
                activeStatusIcons[characterId] = new List<GameObject>();
            }
            activeStatusIcons[characterId].Add(iconObj);
        }

        /// <summary>
        /// Get icon sprite for a status effect type
        /// </summary>
        private Sprite GetStatusEffectIcon(StatusEffectType type)
        {
            switch (type)
            {
                case StatusEffectType.Bleeding:
                    return bleedingIcon;
                case StatusEffectType.Burning:
                    return burningIcon;
                case StatusEffectType.Poisoned:
                    return poisonedIcon;
                case StatusEffectType.Frozen:
                    return frozenIcon;
                case StatusEffectType.Stunned:
                    return stunnedIcon;
                case StatusEffectType.StrengthBuff:
                    return strengthBuffIcon;
                case StatusEffectType.DefenseBuff:
                    return defenseBuffIcon;
                case StatusEffectType.SpeedBuff:
                    return speedBuffIcon;
                case StatusEffectType.MagicBuff:
                    return magicBuffIcon;
                case StatusEffectType.Weakened:
                    return weakenedIcon;
                case StatusEffectType.Slowed:
                    return slowedIcon;
                case StatusEffectType.Silenced:
                    return silencedIcon;
                case StatusEffectType.Blinded:
                    return blindedIcon;
                case StatusEffectType.Regenerating:
                    return regeneratingIcon;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get color tint for a status effect
        /// </summary>
        private Color GetStatusEffectColor(StatusEffect effect)
        {
            // Debuffs are red-tinted, buffs are green-tinted
            if (effect.isDebuff)
            {
                return new Color(1f, 0.6f, 0.6f, 1f); // Light red
            }
            else
            {
                return new Color(0.6f, 1f, 0.6f, 1f); // Light green
            }
        }

        /// <summary>
        /// Clear all status icons for a character
        /// </summary>
        private void ClearStatusIcons(string characterId)
        {
            if (activeStatusIcons.ContainsKey(characterId))
            {
                foreach (GameObject icon in activeStatusIcons[characterId])
                {
                    if (icon != null)
                    {
                        Destroy(icon);
                    }
                }
                activeStatusIcons[characterId].Clear();
            }
        }

        /// <summary>
        /// Clear all status icons
        /// </summary>
        public void ClearAllStatusIcons()
        {
            foreach (var kvp in activeStatusIcons)
            {
                foreach (GameObject icon in kvp.Value)
                {
                    if (icon != null)
                    {
                        Destroy(icon);
                    }
                }
            }
            activeStatusIcons.Clear();
        }

        /// <summary>
        /// Add a floating text indicator for status effect application
        /// </summary>
        public void ShowStatusEffectApplied(Vector3 worldPosition, StatusEffectType effectType, bool isDebuff)
        {
            string effectName = effectType.ToString();
            Color textColor = isDebuff ? Color.red : Color.green;
            
            // This would integrate with a floating text system if available
            Debug.Log($"Status effect applied: {effectName} at {worldPosition}");
        }
    }

    /// <summary>
    /// Component to handle tooltip display for status effect icons
    /// </summary>
    public class StatusEffectTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public StatusEffect statusEffect;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (statusEffect != null && TooltipSystem.Instance != null)
            {
                string title = statusEffect.type.ToString();
                string description = GetStatusEffectDescription(statusEffect.type);
                string stats = $"Duration: {statusEffect.duration} turns\n";
                
                if (statusEffect.value != 0)
                {
                    stats += $"Effect: {(statusEffect.value > 0 ? "+" : "")}{statusEffect.value}\n";
                }
                
                stats += statusEffect.isDebuff ? "<color=red>[DEBUFF]</color>" : "<color=green>[BUFF]</color>";
                
                TooltipSystem.Instance.ShowTooltip(title, description, stats);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (TooltipSystem.Instance != null)
            {
                TooltipSystem.Instance.HideTooltip();
            }
        }

        private string GetStatusEffectDescription(StatusEffectType type)
        {
            switch (type)
            {
                case StatusEffectType.Bleeding:
                    return "Takes damage over time from blood loss";
                case StatusEffectType.Burning:
                    return "Takes fire damage each turn";
                case StatusEffectType.Poisoned:
                    return "Takes poison damage over time";
                case StatusEffectType.Frozen:
                    return "Movement and actions slowed by ice";
                case StatusEffectType.Stunned:
                    return "Cannot act for duration";
                case StatusEffectType.StrengthBuff:
                    return "Increased physical attack power";
                case StatusEffectType.DefenseBuff:
                    return "Reduced damage taken";
                case StatusEffectType.SpeedBuff:
                    return "Increased agility and dodge chance";
                case StatusEffectType.MagicBuff:
                    return "Enhanced magic power";
                case StatusEffectType.Weakened:
                    return "Reduced attack damage";
                case StatusEffectType.Slowed:
                    return "Reduced movement speed";
                case StatusEffectType.Silenced:
                    return "Cannot use magic abilities";
                case StatusEffectType.Blinded:
                    return "Reduced accuracy";
                case StatusEffectType.Regenerating:
                    return "Restores health each turn";
                default:
                    return "Status effect";
            }
        }
    }
}
