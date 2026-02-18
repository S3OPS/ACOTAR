using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Combat action types available in ACOTAR
    /// </summary>
    public enum CombatAction
    {
        PhysicalAttack,
        MagicAttack,
        Defend,
        UseAbility,
        UseItem,
        Flee
    }

    /// <summary>
    /// Combat damage types
    /// </summary>
    public enum DamageType
    {
        Physical,
        Magical,
        Fire,
        Ice,
        Darkness,
        Light,
        Nature,
        Death
    }

    /// <summary>
    /// Result of a combat action
    /// Enhanced with elemental effectiveness and status effect triggers
    /// </summary>
    public class CombatResult
    {
        public int damage;
        public DamageType damageType;
        public bool isCritical;
        public bool wasBlocked;
        public bool wasDodged;
        public bool wasElementalEffective;
        public bool wasElementalResisted;
        public StatusEffectType? appliedEffect;
        public string description;
        public string effectivenessMessage;

        public CombatResult(int dmg, DamageType type, string desc)
        {
            damage = dmg;
            damageType = type;
            description = desc;
            isCritical = false;
            wasBlocked = false;
            wasDodged = false;
            wasElementalEffective = false;
            wasElementalResisted = false;
            appliedEffect = null;
            effectivenessMessage = "";
        }
    }

    /// <summary>
    /// Enhanced turn-based combat system
    /// Integrates with difficulty settings, elemental system, and status effects
    /// v2.6.1: Enhanced with comprehensive error handling and logging
    /// </summary>
    public class CombatSystem
    {
        // Use centralized config values
        private static float CRITICAL_HIT_CHANCE => GameConfig.CombatSettings.CRITICAL_HIT_CHANCE;
        private static float CRITICAL_HIT_MULTIPLIER => GameConfig.CombatSettings.CRITICAL_HIT_MULTIPLIER;
        private static float BASE_DODGE_CHANCE => GameConfig.CombatSettings.BASE_DODGE_CHANCE;
        private static float AGILITY_DODGE_FACTOR => GameConfig.CombatSettings.AGILITY_DODGE_FACTOR;
        private static float DEFEND_DAMAGE_REDUCTION => GameConfig.CombatSettings.DEFEND_DAMAGE_REDUCTION;

        // Combo system tracking (v2.6.7: Enhanced with cascade support)
        private static int currentCombo = 0;
        private static int totalCombos = 0;  // NEW: Track total hits for cascade
        private static Character lastAttacker = null;
        private static bool lastAttackWasCascade = false;  // NEW: Track cascade state

        /// <summary>
        /// Calculate physical attack damage with difficulty and elemental modifiers
        /// v2.6.1: Enhanced with error handling and comprehensive logging
        /// </summary>
        /// <param name="attacker">The character performing the physical attack</param>
        /// <param name="defender">The character receiving the attack</param>
        /// <param name="isPlayerAttack">True if player is attacking (affects difficulty modifiers)</param>
        /// <returns>CombatResult containing damage, type, and descriptive text</returns>
        /// <remarks>
        /// Calculates damage using:
        /// - Base damage from effective strength (includes equipment bonuses)
        /// - Difficulty multipliers (player/enemy)
        /// - Critical hit system (15% base chance, 2x multiplier)
        /// - Dodge mechanics based on defender agility
        /// - Combo system (increases with consecutive hits)
        /// - Random variance (80-120%)
        /// - Status effects (30% chance of bleeding on critical)
        /// </remarks>
        public static CombatResult CalculatePhysicalAttack(Character attacker, Character defender, bool isPlayerAttack = true)
        {
            try
            {
                if (attacker == null || defender == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Combat", "Invalid combat participants in CalculatePhysicalAttack");
                    return new CombatResult(0, DamageType.Physical, "Invalid combat participants");
                }
                
                if (attacker.stats == null || defender.stats == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Combat", $"Missing character stats: attacker={attacker.name}, defender={defender.name}");
                    return new CombatResult(0, DamageType.Physical, "Character stats not initialized");
                }

            // Base damage from strength (v2.3.3: Use effective strength with equipment)
            int baseDamage = attacker.stats.EffectiveStrength;

            // Apply difficulty modifiers
            if (isPlayerAttack)
            {
                baseDamage = Mathf.RoundToInt(baseDamage * DifficultySettings.GetPlayerDamageMultiplier());
            }
            else
            {
                baseDamage = Mathf.RoundToInt(baseDamage * DifficultySettings.GetEnemyDamageMultiplier());
            }

            // Check for critical hit
            float critChance = CRITICAL_HIT_CHANCE;
            if (!isPlayerAttack)
            {
                critChance += DifficultySettings.GetEnemyCritChanceModifier();
            }
            bool isCritical = Random.value < critChance;
            if (isCritical)
            {
                baseDamage = Mathf.RoundToInt(baseDamage * CRITICAL_HIT_MULTIPLIER);
            }

            // Apply defender's agility for dodge chance (v2.3.3: Use effective agility)
            float dodgeChance = BASE_DODGE_CHANCE + (defender.stats.EffectiveAgility * AGILITY_DODGE_FACTOR);
            bool dodged = Random.value < dodgeChance;
            if (dodged)
            {
                ResetCombo();
                CombatResult dodgeResult = new CombatResult(0, DamageType.Physical, $"{defender.name} dodged the attack!");
                dodgeResult.wasDodged = true;
                return dodgeResult;
            }

            // Apply combo bonus
            float comboMultiplier = GetComboMultiplier(attacker);
            baseDamage = Mathf.RoundToInt(baseDamage * comboMultiplier);

            // Apply random variance (80-120% of base damage)
            float variance = Random.Range(0.8f, 1.2f);
            int finalDamage = Mathf.RoundToInt(baseDamage * variance);

            // Increment combo
            UpdateCombo(attacker);

            // v2.6.7: Enhanced combo display with cascade notification
            string comboText = "";
            if (currentCombo > 1)
            {
                if (lastAttackWasCascade)
                {
                    comboText = $" [⚡{totalCombos}x COMBO CASCADE!⚡ +{GameConfig.CombatSettings.COMBO_CASCADE_BONUS*100}% POWER!]";
                }
                else
                {
                    comboText = $" [{currentCombo}x COMBO!]";
                }
            }
            
            string description = isCritical 
                ? $"{attacker.name} landed a CRITICAL HIT for {finalDamage} damage!{comboText}"
                : $"{attacker.name} attacked {defender.name} for {finalDamage} damage{comboText}";

            CombatResult result = new CombatResult(finalDamage, DamageType.Physical, description);
            result.isCritical = isCritical;

            // Chance to apply bleeding on physical attacks
            if (isCritical && Random.value < 0.3f)
            {
                result.appliedEffect = StatusEffectType.Bleeding;
                result.description += " and caused BLEEDING!";
                }

                return result;
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Combat", 
                    $"Exception in CalculatePhysicalAttack: {ex.Message}\nStack: {ex.StackTrace}");
                return new CombatResult(0, DamageType.Physical, "Combat error occurred");
            }
        }

        /// <summary>
        /// Calculate magic attack damage with elemental system integration
        /// v2.3.3: Enhanced with mana cost system
        /// v2.6.1: Enhanced with error handling and comprehensive logging
        /// </summary>
        /// <param name="attacker">The character casting the magic spell</param>
        /// <param name="defender">The character receiving the magic attack</param>
        /// <param name="magicType">Type of magic spell being cast</param>
        /// <param name="isPlayerAttack">True if player is attacking (affects difficulty modifiers)</param>
        /// <returns>CombatResult containing damage, elemental effectiveness, and status effects</returns>
        /// <remarks>
        /// Calculates magic damage using:
        /// - Mana cost validation and consumption (15-60 mana per spell)
        /// - Base damage from effective magic power
        /// - Difficulty multipliers
        /// - Magic type multipliers (different spells have different power)
        /// - Elemental effectiveness (e.g., Fire vs Ice)
        /// - Critical hit system
        /// - Combo bonuses
        /// - Status effects (25% chance based on magic type)
        /// </remarks>
        public static CombatResult CalculateMagicAttack(Character attacker, Character defender, MagicType magicType, bool isPlayerAttack = true)
        {
            try
            {
                if (attacker == null || defender == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Combat", "Invalid combat participants in CalculateMagicAttack");
                    return new CombatResult(0, DamageType.Magical, "Invalid combat participants");
                }
                
                if (attacker.stats == null || defender.stats == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Combat", $"Missing character stats in magic attack");
                    return new CombatResult(0, DamageType.Magical, "Character stats not initialized");
                }

            // Check if attacker has the ability
            if (!attacker.HasAbility(magicType))
            {
                return new CombatResult(0, DamageType.Magical, $"{attacker.name} doesn't know {magicType}!");
            }

            // v2.3.3: Check and consume mana
            int manaCost = ManaSystem.GetManaCost(magicType);
            if (!attacker.manaSystem.HasEnoughMana(manaCost))
            {
                return new CombatResult(0, DamageType.Magical, $"{attacker.name} doesn't have enough mana! Need {manaCost}, have {attacker.manaSystem.CurrentMana}");
            }

            if (!attacker.manaSystem.TryConsumeMana(manaCost))
            {
                return new CombatResult(0, DamageType.Magical, $"{attacker.name} failed to consume mana!");
            }

            // Base damage from magic power (v2.3.3: Use effective magic power)
            int baseDamage = attacker.stats.EffectiveMagicPower;

            // Apply difficulty modifiers
            if (isPlayerAttack)
            {
                baseDamage = Mathf.RoundToInt(baseDamage * DifficultySettings.GetPlayerDamageMultiplier());
            }
            else
            {
                baseDamage = Mathf.RoundToInt(baseDamage * DifficultySettings.GetEnemyDamageMultiplier());
            }

            // Apply magic type modifiers
            DamageType damageType = GetDamageTypeForMagic(magicType);
            float typeMultiplier = GetMagicTypeMultiplier(magicType);
            baseDamage = Mathf.RoundToInt(baseDamage * typeMultiplier);

            // Apply elemental effectiveness
            Element attackElement = ElementalSystem.GetElementFromMagic(magicType);
            Element defenderElement = ElementalSystem.GetElementFromCourt(defender.allegiance);
            float elementalMultiplier = ElementalSystem.GetDamageMultiplier(attackElement, defenderElement);
            baseDamage = Mathf.RoundToInt(baseDamage * elementalMultiplier);

            string effectivenessMessage = ElementalSystem.GetEffectivenessMessage(attackElement, defenderElement);

            // Check for critical hit
            float critChance = CRITICAL_HIT_CHANCE;
            if (!isPlayerAttack)
            {
                critChance += DifficultySettings.GetEnemyCritChanceModifier();
            }
            bool isCritical = Random.value < critChance;
            if (isCritical)
            {
                baseDamage = Mathf.RoundToInt(baseDamage * CRITICAL_HIT_MULTIPLIER);
            }

            // Apply combo bonus for magic too
            float comboMultiplier = GetComboMultiplier(attacker);
            baseDamage = Mathf.RoundToInt(baseDamage * comboMultiplier);

            // Apply random variance
            float variance = Random.Range(0.8f, 1.2f);
            int finalDamage = Mathf.RoundToInt(baseDamage * variance);

            // Increment combo
            UpdateCombo(attacker);

            // v2.6.7: Enhanced combo display with cascade notification
            string comboText = "";
            if (currentCombo > 1)
            {
                if (lastAttackWasCascade)
                {
                    comboText = $" [⚡{totalCombos}x COMBO CASCADE!⚡ +{GameConfig.CombatSettings.COMBO_CASCADE_BONUS*100}% POWER!]";
                }
                else
                {
                    comboText = $" [{currentCombo}x COMBO!]";
                }
            }
            
            string description = isCritical
                ? $"{attacker.name} unleashed {magicType} - CRITICAL HIT for {finalDamage} damage!{comboText}"
                : $"{attacker.name} used {magicType} on {defender.name} for {finalDamage} damage{comboText}";

            CombatResult result = new CombatResult(finalDamage, damageType, description);
            result.isCritical = isCritical;
            result.effectivenessMessage = effectivenessMessage;
            result.wasElementalEffective = elementalMultiplier > 1.0f;
            result.wasElementalResisted = elementalMultiplier < 1.0f;

            // Chance to apply status effects based on magic type
            StatusEffectType? potentialEffect = GetPotentialStatusEffect(magicType);
            if (potentialEffect.HasValue && Random.value < 0.25f)
            {
                result.appliedEffect = potentialEffect;
                    result.description += $" and applied {potentialEffect.Value}!";
            }

            return result;
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Combat", 
                    $"Exception in CalculateMagicAttack: {ex.Message}\nStack: {ex.StackTrace}");
                return new CombatResult(0, DamageType.Magical, "Magic attack error occurred");
            }
        }

        /// <summary>
        /// Calculate attack against enemy with element consideration
        /// v2.6.1: Enhanced with error handling
        /// </summary>
        public static CombatResult CalculatePhysicalAttack(Character attacker, Enemy defender)
        {
            try
            {
                CombatResult result = CalculatePhysicalAttack(attacker, defender as Character, true);
            
            // Apply enemy-specific difficulty scaling to damage taken
            if (result.damage > 0)
            {
                // Enemies have health scaled by difficulty, damage remains as calculated
            }
            
            return result;
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Combat", 
                    $"Exception in CalculatePhysicalAttack (Enemy): {ex.Message}");
                return new CombatResult(0, DamageType.Physical, "Combat error occurred");
            }
        }

        /// <summary>
        /// Calculate magic attack against enemy
        /// </summary>
        public static CombatResult CalculateMagicAttack(Character attacker, Enemy defender, MagicType magicType)
        {
            return CalculateMagicAttack(attacker, defender as Character, magicType, true);
        }

        /// <summary>
        /// Calculate enemy attack against player
        /// </summary>
        public static CombatResult CalculateEnemyAttack(Enemy attacker, Character defender)
        {
            return CalculatePhysicalAttack(attacker as Character, defender, false);
        }

        /// <summary>
        /// Execute defend action (reduces incoming damage)
        /// </summary>
        public static int ApplyDefense(int incomingDamage)
        {
            return Mathf.RoundToInt(incomingDamage * DEFEND_DAMAGE_REDUCTION);
        }

        /// <summary>
        /// Calculate flee success chance with difficulty modifier
        /// </summary>
        public static bool AttemptFlee(Character character, Character enemy)
        {
            if (character == null || enemy == null) return false;

            // Base flee chance modified by agility and difficulty
            float baseChance = GameConfig.CombatSettings.BASE_FLEE_CHANCE;
            float agilityDiff = (character.agility - enemy.agility) * 0.01f;
            float difficultyMod = DifficultySettings.GetFleeChanceModifier();
            
            float fleeChance = Mathf.Clamp(
                baseChance + agilityDiff + difficultyMod, 
                GameConfig.CombatSettings.MIN_FLEE_CHANCE, 
                GameConfig.CombatSettings.MAX_FLEE_CHANCE
            );

            ResetCombo();
            return Random.value < fleeChance;
        }

        /// <summary>
        /// Calculate XP reward with difficulty multiplier
        /// </summary>
        public static int CalculateXPReward(int baseXP)
        {
            return Mathf.RoundToInt(baseXP * DifficultySettings.GetXPMultiplier());
        }

        /// <summary>
        /// Map magic type to damage type
        /// </summary>
        public static DamageType GetDamageTypeForMagic(MagicType magicType)
        {
            switch (magicType)
            {
                case MagicType.FireManipulation:
                    return DamageType.Fire;
                case MagicType.IceManipulation:
                    return DamageType.Ice;
                case MagicType.DarknessManipulation:
                    return DamageType.Darkness;
                case MagicType.LightManipulation:
                    return DamageType.Light;
                case MagicType.DeathManifestation:
                    return DamageType.Death;
                default:
                    return DamageType.Magical;
            }
        }

        /// <summary>
        /// Get damage multiplier for magic type
        /// </summary>
        private static float GetMagicTypeMultiplier(MagicType magicType)
        {
            switch (magicType)
            {
                case MagicType.FireManipulation:
                case MagicType.IceManipulation:
                case MagicType.DarknessManipulation:
                case MagicType.LightManipulation:
                    return 1.5f; // Elemental magic deals more damage
                case MagicType.Daemati:
                    return 2.0f; // Mind attacks are powerful
                case MagicType.DeathManifestation:
                    return 2.5f; // Death magic is extremely powerful
                case MagicType.ShieldCreation:
                    return 0.5f; // Shields are defensive
                case MagicType.Healing:
                    return 0f; // Healing doesn't deal damage
                default:
                    return 1.0f;
            }
        }

        /// <summary>
        /// Get potential status effect from magic type
        /// </summary>
        private static StatusEffectType? GetPotentialStatusEffect(MagicType magicType)
        {
            switch (magicType)
            {
                case MagicType.FireManipulation:
                    return StatusEffectType.Burning;
                case MagicType.IceManipulation:
                    return StatusEffectType.Frozen;
                case MagicType.DarknessManipulation:
                    return StatusEffectType.Weakened;
                case MagicType.LightManipulation:
                    return StatusEffectType.Stunned;
                case MagicType.Daemati:
                    return StatusEffectType.Silenced;
                case MagicType.DeathManifestation:
                    return StatusEffectType.Cursed;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get combo multiplier based on consecutive hits
        /// v2.6.7: Enhanced with cascade bonuses - every 5 hits triggers a power surge
        /// </summary>
        private static float GetComboMultiplier(Character attacker)
        {
            if (attacker == lastAttacker && currentCombo > 0)
            {
                float baseMultiplier = 1.0f + (currentCombo * GameConfig.CombatSettings.COMBO_DAMAGE_BONUS_PER_HIT);
                
                // v2.6.7: Apply cascade bonus if we hit the threshold
                if (lastAttackWasCascade)
                {
                    baseMultiplier += GameConfig.CombatSettings.COMBO_CASCADE_BONUS;
                }
                
                return baseMultiplier;
            }
            return 1.0f;
        }

        /// <summary>
        /// Update combo counter
        /// v2.6.7: Enhanced to track cascades for extra rewards
        /// </summary>
        private static void UpdateCombo(Character attacker)
        {
            if (attacker == lastAttacker)
            {
                currentCombo = Mathf.Min(currentCombo + 1, GameConfig.CombatSettings.MAX_COMBO_COUNT);
                totalCombos++;
                
                // v2.6.7: Check for cascade - every CASCADE_THRESHOLD hits
                if (totalCombos > 0 && totalCombos % GameConfig.CombatSettings.COMBO_CASCADE_THRESHOLD == 0)
                {
                    lastAttackWasCascade = true;
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Combat", 
                        $"COMBO CASCADE! {attacker.name} triggered power surge at {totalCombos} consecutive hits!");
                }
                else
                {
                    lastAttackWasCascade = false;
                }
            }
            else
            {
                currentCombo = 1;
                totalCombos = 1;
                lastAttacker = attacker;
                lastAttackWasCascade = false;
            }
        }

        /// <summary>
        /// Reset combo counter (on miss, flee, or turn change)
        /// v2.6.7: Also resets cascade tracking
        /// </summary>
        public static void ResetCombo()
        {
            currentCombo = 0;
            totalCombos = 0;
            lastAttacker = null;
            lastAttackWasCascade = false;
        }

        /// <summary>
        /// Get current combo count
        /// </summary>
        public static int GetCurrentCombo()
        {
            return currentCombo;
        }
        
        /// <summary>
        /// Get total combo hits (for cascade tracking)
        /// v2.6.7: NEW - Returns total hits in current combo chain
        /// </summary>
        public static int GetTotalComboHits()
        {
            return totalCombos;
        }
        
        /// <summary>
        /// Check if last attack was a cascade
        /// v2.6.7: NEW - Used for visual effects and bonus XP
        /// </summary>
        public static bool WasLastAttackCascade()
        {
            return lastAttackWasCascade;
        }

        /// <summary>
        /// Apply party synergy bonuses to combat result
        /// v2.6.0: NEW - Integrates party synergy system with combat
        /// </summary>
        /// <param name="baseResult">The base combat result before synergy bonuses</param>
        /// <param name="attacker">The attacking character</param>
        /// <param name="synergySystem">Reference to the party synergy system</param>
        /// <returns>Modified CombatResult with synergy bonuses applied</returns>
        /// <remarks>
        /// Applies synergy bonuses based on active party composition:
        /// - Damage synergy: Adds percentage-based bonus damage
        /// - Critical rate synergy: Chance to upgrade hits to critical
        /// - Magic power synergy: Boosts magical damage types
        /// All bonuses stack with other combat modifiers
        /// </remarks>
        public static CombatResult ApplySynergyBonuses(CombatResult baseResult, Character attacker, PartySynergySystem synergySystem)
        {
            // Defensive check (v2.6.0)
            if (baseResult == null || attacker == null || synergySystem == null || !synergySystem.IsInitialized)
            {
                return baseResult;
            }

            // Apply damage synergy bonus
            float damageBonus = synergySystem.GetSynergyBonus(SynergyType.Damage);
            if (damageBonus > 0)
            {
                int bonusDamage = Mathf.RoundToInt(baseResult.damage * damageBonus);
                baseResult.damage += bonusDamage;
                baseResult.description += $" [+{bonusDamage} Synergy Bonus!]";
            }

            // Apply critical rate synergy bonus
            float critBonus = synergySystem.GetSynergyBonus(SynergyType.CriticalRate);
            if (critBonus > 0 && !baseResult.isCritical)
            {
                // Chance to upgrade to critical based on synergy
                if (Random.value < critBonus)
                {
                    baseResult.isCritical = true;
                    int critDamage = Mathf.RoundToInt(baseResult.damage * 0.5f);
                    baseResult.damage += critDamage;
                    baseResult.description += " [Synergy CRITICAL!]";
                }
            }

            // Apply magic power synergy bonus (for magic attacks)
            if (baseResult.damageType == DamageType.Magical || 
                baseResult.damageType == DamageType.Fire || 
                baseResult.damageType == DamageType.Ice ||
                baseResult.damageType == DamageType.Darkness ||
                baseResult.damageType == DamageType.Light)
            {
                float magicBonus = synergySystem.GetSynergyBonus(SynergyType.MagicPower);
                if (magicBonus > 0)
                {
                    int bonusMagicDamage = Mathf.RoundToInt(baseResult.damage * magicBonus);
                    baseResult.damage += bonusMagicDamage;
                    baseResult.description += $" [+{bonusMagicDamage} Magic Synergy!]";
                }
            }

            return baseResult;
        }

        /// <summary>
        /// Apply defensive synergy bonuses to reduce incoming damage
        /// v2.6.0: NEW - Reduce damage based on defense synergies
        /// </summary>
        /// <param name="incomingDamage">The amount of damage before defense synergy</param>
        /// <param name="synergySystem">Reference to the party synergy system</param>
        /// <returns>Reduced damage amount after applying defense synergy</returns>
        /// <remarks>
        /// Defensive synergies reduce incoming damage by a percentage.
        /// Logs the reduction amount when damage is reduced.
        /// Always returns at least 0 damage (never negative).
        /// </remarks>
        public static int ApplyDefensiveSynergy(int incomingDamage, PartySynergySystem synergySystem)
        {
            // Defensive check (v2.6.0)
            if (synergySystem == null || !synergySystem.IsInitialized)
            {
                return incomingDamage;
            }

            float defenseBonus = synergySystem.GetSynergyBonus(SynergyType.Defense);
            if (defenseBonus > 0)
            {
                int reduction = Mathf.RoundToInt(incomingDamage * defenseBonus);
                int reducedDamage = Mathf.Max(0, incomingDamage - reduction);
                
                if (reduction > 0)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, "Combat", 
                        $"Defense Synergy reduced damage by {reduction}!");
                }
                
                return reducedDamage;
            }

            return incomingDamage;
        }
    }
}
