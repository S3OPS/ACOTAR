using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Types of status effects in the game
    /// </summary>
    public enum StatusEffectType
    {
        // Damage over time
        Bleeding,
        Burning,
        Poisoned,
        
        // Control effects
        Frozen,
        Stunned,
        Silenced,
        
        // Debuffs
        Weakened,
        Slowed,
        Cursed,
        
        // Buffs
        Strengthened,
        Hastened,
        Shielded,
        Regenerating,
        MagicBoosted
    }

    /// <summary>
    /// Represents a status effect applied to a character
    /// </summary>
    [Serializable]
    public class StatusEffect
    {
        public StatusEffectType type;
        public string name;
        public string description;
        public int duration;           // Turns remaining
        public int potency;            // Effect strength
        public bool isDebuff;          // True for negative effects
        public int tickDamage;         // Damage per turn (for DoT effects)
        public float statModifier;     // Multiplier for stat modifications

        public StatusEffect(StatusEffectType type, int duration, int potency = 1)
        {
            this.type = type;
            this.duration = duration;
            this.potency = potency;
            InitializeEffect();
        }

        /// <summary>
        /// Initialize effect properties based on type
        /// </summary>
        private void InitializeEffect()
        {
            switch (type)
            {
                case StatusEffectType.Bleeding:
                    name = "Bleeding";
                    description = "Losing health each turn";
                    isDebuff = true;
                    tickDamage = 5 * potency;
                    statModifier = 1.0f;
                    break;
                    
                case StatusEffectType.Burning:
                    name = "Burning";
                    description = "Fire damage each turn, reduced defense";
                    isDebuff = true;
                    tickDamage = 8 * potency;
                    statModifier = 0.9f;
                    break;
                    
                case StatusEffectType.Poisoned:
                    name = "Poisoned";
                    description = "Poison damage each turn";
                    isDebuff = true;
                    tickDamage = 3 * potency;
                    statModifier = 1.0f;
                    break;
                    
                case StatusEffectType.Frozen:
                    name = "Frozen";
                    description = "Cannot act, increased damage taken";
                    isDebuff = true;
                    tickDamage = 0;
                    statModifier = 1.25f; // Takes 25% more damage
                    break;
                    
                case StatusEffectType.Stunned:
                    name = "Stunned";
                    description = "Cannot act for this turn";
                    isDebuff = true;
                    tickDamage = 0;
                    statModifier = 1.0f;
                    break;
                    
                case StatusEffectType.Silenced:
                    name = "Silenced";
                    description = "Cannot use magic abilities";
                    isDebuff = true;
                    tickDamage = 0;
                    statModifier = 1.0f;
                    break;
                    
                case StatusEffectType.Weakened:
                    name = "Weakened";
                    description = "Reduced attack damage";
                    isDebuff = true;
                    tickDamage = 0;
                    statModifier = 0.7f; // 30% damage reduction
                    break;
                    
                case StatusEffectType.Slowed:
                    name = "Slowed";
                    description = "Reduced agility";
                    isDebuff = true;
                    tickDamage = 0;
                    statModifier = 0.5f; // 50% agility reduction
                    break;
                    
                case StatusEffectType.Cursed:
                    name = "Cursed";
                    description = "All stats reduced";
                    isDebuff = true;
                    tickDamage = 2 * potency;
                    statModifier = 0.8f; // 20% all stats reduction
                    break;
                    
                case StatusEffectType.Strengthened:
                    name = "Strengthened";
                    description = "Increased attack damage";
                    isDebuff = false;
                    tickDamage = 0;
                    statModifier = 1.3f; // 30% damage boost
                    break;
                    
                case StatusEffectType.Hastened:
                    name = "Hastened";
                    description = "Increased agility";
                    isDebuff = false;
                    tickDamage = 0;
                    statModifier = 1.5f; // 50% agility boost
                    break;
                    
                case StatusEffectType.Shielded:
                    name = "Shielded";
                    description = "Reduced damage taken";
                    isDebuff = false;
                    tickDamage = 0;
                    statModifier = 0.7f; // Takes 30% less damage
                    break;
                    
                case StatusEffectType.Regenerating:
                    name = "Regenerating";
                    description = "Healing each turn";
                    isDebuff = false;
                    tickDamage = -10 * potency; // Negative = healing
                    statModifier = 1.0f;
                    break;
                    
                case StatusEffectType.MagicBoosted:
                    name = "Magic Boosted";
                    description = "Increased magic power";
                    isDebuff = false;
                    tickDamage = 0;
                    statModifier = 1.4f; // 40% magic boost
                    break;
            }
        }

        /// <summary>
        /// Tick the effect (reduce duration, apply tick damage)
        /// Returns true if effect is still active
        /// </summary>
        public bool Tick()
        {
            duration--;
            return duration > 0;
        }

        /// <summary>
        /// Check if effect prevents actions
        /// </summary>
        public bool PreventsAction()
        {
            return type == StatusEffectType.Frozen || type == StatusEffectType.Stunned;
        }

        /// <summary>
        /// Check if effect prevents magic
        /// </summary>
        public bool PreventsMagic()
        {
            return type == StatusEffectType.Silenced || PreventsAction();
        }
    }

    /// <summary>
    /// Manages status effects on characters
    /// </summary>
    public class StatusEffectManager
    {
        private Dictionary<Character, List<StatusEffect>> characterEffects;
        private Dictionary<Enemy, List<StatusEffect>> enemyEffects;

        public StatusEffectManager()
        {
            characterEffects = new Dictionary<Character, List<StatusEffect>>();
            enemyEffects = new Dictionary<Enemy, List<StatusEffect>>();
        }

        /// <summary>
        /// Apply a status effect to a character
        /// </summary>
        /// <param name="target">Character to apply the effect to</param>
        /// <param name="effectType">Type of status effect to apply</param>
        /// <param name="duration">Duration in turns</param>
        /// <param name="potency">Strength of the effect</param>
        /// <remarks>
        /// v2.6.3: Added comprehensive error handling, structured logging, and XML documentation
        /// 
        /// This method applies a status effect to a character. If the effect already exists,
        /// it refreshes the duration and potency to the maximum values.
        /// 
        /// Error handling prevents null reference exceptions and ensures the character effects
        /// dictionary is properly initialized before use.
        /// </remarks>
        public void ApplyEffect(Character target, StatusEffectType effectType, int duration, int potency = 1)
        {
            try
            {
                // Input validation
                if (target == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "StatusEffect", "Cannot apply effect: target character is null");
                    return;
                }

                if (duration <= 0)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "StatusEffect", $"Cannot apply effect to {target.name}: duration must be positive ({duration})");
                    return;
                }

                if (potency <= 0)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "StatusEffect", $"Cannot apply effect to {target.name}: potency must be positive ({potency})");
                    return;
                }

                // Initialize character effects list if needed
                if (characterEffects == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "StatusEffect", "Character effects dictionary is null, initializing");
                    characterEffects = new Dictionary<Character, List<StatusEffect>>();
                }

                if (!characterEffects.ContainsKey(target))
                {
                    characterEffects[target] = new List<StatusEffect>();
                }

                // Check if effect already exists - refresh duration if so
                var existingEffect = characterEffects[target].Find(e => e != null && e.type == effectType);
                if (existingEffect != null)
                {
                    existingEffect.duration = Mathf.Max(existingEffect.duration, duration);
                    existingEffect.potency = Mathf.Max(existingEffect.potency, potency);
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                        "StatusEffect", $"{target.name}'s {effectType} effect refreshed! Duration: {existingEffect.duration}, Potency: {existingEffect.potency}");
                }
                else
                {
                    StatusEffect effect = new StatusEffect(effectType, duration, potency);
                    if (effect != null)
                    {
                        characterEffects[target].Add(effect);
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                            "StatusEffect", $"{target.name} is now {effect.name}! ({duration} turns)");
                    }
                    else
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                            "StatusEffect", $"Failed to create status effect {effectType} for {target.name}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "StatusEffect", $"Exception in ApplyEffect (Character): {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Apply a status effect to an enemy
        /// </summary>
        public void ApplyEffect(Enemy target, StatusEffectType effectType, int duration, int potency = 1)
        {
            if (!enemyEffects.ContainsKey(target))
            {
                enemyEffects[target] = new List<StatusEffect>();
            }

            var existingEffect = enemyEffects[target].Find(e => e.type == effectType);
            if (existingEffect != null)
            {
                existingEffect.duration = Mathf.Max(existingEffect.duration, duration);
                existingEffect.potency = Mathf.Max(existingEffect.potency, potency);
                Debug.Log($"{target.name}'s {effectType} effect refreshed!");
            }
            else
            {
                StatusEffect effect = new StatusEffect(effectType, duration, potency);
                enemyEffects[target].Add(effect);
                Debug.Log($"{target.name} is now {effect.name}! ({duration} turns)");
            }
        }

        /// <summary>
        /// Process turn start effects for a character
        /// </summary>
        /// <param name="target">Character to process effects for</param>
        /// <returns>Total damage dealt by status effects</returns>
        /// <remarks>
        /// v2.6.3: Added comprehensive error handling, structured logging, and XML documentation
        /// 
        /// CRITICAL: This method is called every combat turn. Any exception here would break
        /// the combat flow. The try-catch blocks protect against failures in TakeDamage/Heal
        /// calls and effect processing.
        /// 
        /// The method processes all active effects, applies their damage/healing, decrements
        /// duration, and removes expired effects.
        /// </remarks>
        public int ProcessTurnStart(Character target)
        {
            try
            {
                // Input validation
                if (target == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                        "StatusEffect", "Cannot process turn: target character is null");
                    return 0;
                }

                if (characterEffects == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                        "StatusEffect", "Character effects dictionary is null");
                    return 0;
                }

                if (!characterEffects.ContainsKey(target))
                    return 0;

                int totalDamage = 0;
                List<StatusEffect> toRemove = new List<StatusEffect>();

                foreach (var effect in characterEffects[target])
                {
                    if (effect == null)
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, 
                            "StatusEffect", $"Null effect found for {target.name}, skipping");
                        toRemove.Add(effect);
                        continue;
                    }

                    try
                    {
                        // Apply tick damage
                        if (effect.tickDamage != 0)
                        {
                            if (effect.tickDamage > 0)
                            {
                                target.TakeDamage(effect.tickDamage);
                                totalDamage += effect.tickDamage;
                                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                                    "StatusEffect", $"{target.name} takes {effect.tickDamage} {effect.name} damage!");
                            }
                            else
                            {
                                target.Heal(-effect.tickDamage);
                                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                                    "StatusEffect", $"{target.name} heals {-effect.tickDamage} from {effect.name}!");
                            }
                        }

                        // Tick duration
                        if (!effect.Tick())
                        {
                            toRemove.Add(effect);
                        }
                    }
                    catch (System.Exception effectEx)
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                            "StatusEffect", $"Exception processing effect {effect.name} for {target.name}: {effectEx.Message}");
                        toRemove.Add(effect); // Remove problematic effect
                    }
                }

                // Remove expired effects
                foreach (var effect in toRemove)
                {
                    if (effect != null)
                    {
                        characterEffects[target].Remove(effect);
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, 
                            "StatusEffect", $"{target.name}'s {effect.name} effect has worn off.");
                    }
                    else
                    {
                        characterEffects[target].Remove(effect); // Remove null effect
                    }
                }

                return totalDamage;
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, 
                    "StatusEffect", $"Exception in ProcessTurnStart (Character): {ex.Message}\nStack: {ex.StackTrace}");
                return 0;
            }
        }

        /// <summary>
        /// Process turn start effects for an enemy
        /// </summary>
        public int ProcessTurnStart(Enemy target)
        {
            if (!enemyEffects.ContainsKey(target))
                return 0;

            int totalDamage = 0;
            List<StatusEffect> toRemove = new List<StatusEffect>();

            foreach (var effect in enemyEffects[target])
            {
                if (effect.tickDamage != 0)
                {
                    if (effect.tickDamage > 0)
                    {
                        target.TakeDamage(effect.tickDamage);
                        totalDamage += effect.tickDamage;
                        Debug.Log($"{target.name} takes {effect.tickDamage} {effect.name} damage!");
                    }
                    else
                    {
                        target.Heal(-effect.tickDamage);
                        Debug.Log($"{target.name} heals {-effect.tickDamage} from {effect.name}!");
                    }
                }

                if (!effect.Tick())
                {
                    toRemove.Add(effect);
                }
            }

            foreach (var effect in toRemove)
            {
                enemyEffects[target].Remove(effect);
                Debug.Log($"{target.name}'s {effect.name} effect has worn off.");
            }

            return totalDamage;
        }

        /// <summary>
        /// Check if character can act
        /// </summary>
        public bool CanAct(Character target)
        {
            if (!characterEffects.ContainsKey(target))
                return true;

            return !characterEffects[target].Exists(e => e.PreventsAction());
        }

        /// <summary>
        /// Check if enemy can act
        /// </summary>
        public bool CanAct(Enemy target)
        {
            if (!enemyEffects.ContainsKey(target))
                return true;

            return !enemyEffects[target].Exists(e => e.PreventsAction());
        }

        /// <summary>
        /// Check if character can use magic
        /// </summary>
        public bool CanUseMagic(Character target)
        {
            if (!characterEffects.ContainsKey(target))
                return true;

            return !characterEffects[target].Exists(e => e.PreventsMagic());
        }

        /// <summary>
        /// Get damage modifier for a character
        /// </summary>
        public float GetDamageModifier(Character target)
        {
            if (!characterEffects.ContainsKey(target))
                return 1.0f;

            float modifier = 1.0f;
            foreach (var effect in characterEffects[target])
            {
                if (effect.type == StatusEffectType.Strengthened || 
                    effect.type == StatusEffectType.Weakened)
                {
                    modifier *= effect.statModifier;
                }
            }
            return modifier;
        }

        /// <summary>
        /// Get damage taken modifier for a character (for effects like Frozen, Shielded)
        /// </summary>
        public float GetDamageTakenModifier(Character target)
        {
            if (!characterEffects.ContainsKey(target))
                return 1.0f;

            float modifier = 1.0f;
            foreach (var effect in characterEffects[target])
            {
                if (effect.type == StatusEffectType.Frozen || 
                    effect.type == StatusEffectType.Shielded)
                {
                    modifier *= effect.statModifier;
                }
            }
            return modifier;
        }

        /// <summary>
        /// Get all active effects on a character
        /// </summary>
        public List<StatusEffect> GetActiveEffects(Character target)
        {
            if (!characterEffects.ContainsKey(target))
                return new List<StatusEffect>();

            return new List<StatusEffect>(characterEffects[target]);
        }

        /// <summary>
        /// Get all active effects on an enemy
        /// </summary>
        public List<StatusEffect> GetActiveEffects(Enemy target)
        {
            if (!enemyEffects.ContainsKey(target))
                return new List<StatusEffect>();

            return new List<StatusEffect>(enemyEffects[target]);
        }

        /// <summary>
        /// Remove a specific effect from a character
        /// </summary>
        public bool RemoveEffect(Character target, StatusEffectType effectType)
        {
            if (!characterEffects.ContainsKey(target))
                return false;

            var effect = characterEffects[target].Find(e => e.type == effectType);
            if (effect != null)
            {
                characterEffects[target].Remove(effect);
                Debug.Log($"{target.name}'s {effect.name} effect was removed!");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove all debuffs from a character
        /// </summary>
        public void ClearDebuffs(Character target)
        {
            if (!characterEffects.ContainsKey(target))
                return;

            characterEffects[target].RemoveAll(e => e.isDebuff);
            Debug.Log($"All debuffs removed from {target.name}!");
        }

        /// <summary>
        /// Remove all buffs from a character
        /// </summary>
        public void ClearBuffs(Character target)
        {
            if (!characterEffects.ContainsKey(target))
                return;

            characterEffects[target].RemoveAll(e => !e.isDebuff);
            Debug.Log($"All buffs removed from {target.name}!");
        }

        /// <summary>
        /// Clear all effects when combat ends
        /// </summary>
        public void ClearAllEffects()
        {
            characterEffects.Clear();
            enemyEffects.Clear();
        }
    }
}
