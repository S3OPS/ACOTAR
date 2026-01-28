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
    /// </summary>
    public class CombatSystem
    {
        // Use centralized config values
        private static float CRITICAL_HIT_CHANCE => GameConfig.CombatSettings.CRITICAL_HIT_CHANCE;
        private static float CRITICAL_HIT_MULTIPLIER => GameConfig.CombatSettings.CRITICAL_HIT_MULTIPLIER;
        private static float BASE_DODGE_CHANCE => GameConfig.CombatSettings.BASE_DODGE_CHANCE;
        private static float AGILITY_DODGE_FACTOR => GameConfig.CombatSettings.AGILITY_DODGE_FACTOR;
        private static float DEFEND_DAMAGE_REDUCTION => GameConfig.CombatSettings.DEFEND_DAMAGE_REDUCTION;

        // Combo system tracking
        private static int currentCombo = 0;
        private static Character lastAttacker = null;

        /// <summary>
        /// Calculate physical attack damage with difficulty and elemental modifiers
        /// </summary>
        public static CombatResult CalculatePhysicalAttack(Character attacker, Character defender, bool isPlayerAttack = true)
        {
            if (attacker == null || defender == null)
            {
                return new CombatResult(0, DamageType.Physical, "Invalid combat participants");
            }

            // Base damage from strength
            int baseDamage = attacker.strength;

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

            // Apply defender's agility for dodge chance
            float dodgeChance = BASE_DODGE_CHANCE + (defender.agility * AGILITY_DODGE_FACTOR);
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

            string comboText = currentCombo > 1 ? $" [{currentCombo}x COMBO!]" : "";
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

        /// <summary>
        /// Calculate magic attack damage with elemental system integration
        /// </summary>
        public static CombatResult CalculateMagicAttack(Character attacker, Character defender, MagicType magicType, bool isPlayerAttack = true)
        {
            if (attacker == null || defender == null)
            {
                return new CombatResult(0, DamageType.Magical, "Invalid combat participants");
            }

            // Check if attacker has the ability
            if (!attacker.HasAbility(magicType))
            {
                return new CombatResult(0, DamageType.Magical, $"{attacker.name} doesn't know {magicType}!");
            }

            // Base damage from magic power
            int baseDamage = attacker.magicPower;

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

            string comboText = currentCombo > 1 ? $" [{currentCombo}x COMBO!]" : "";
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

        /// <summary>
        /// Calculate attack against enemy with element consideration
        /// </summary>
        public static CombatResult CalculatePhysicalAttack(Character attacker, Enemy defender)
        {
            CombatResult result = CalculatePhysicalAttack(attacker, defender as Character, true);
            
            // Apply enemy-specific difficulty scaling to damage taken
            if (result.damage > 0)
            {
                // Enemies have health scaled by difficulty, damage remains as calculated
            }
            
            return result;
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
        /// </summary>
        private static float GetComboMultiplier(Character attacker)
        {
            if (attacker == lastAttacker && currentCombo > 0)
            {
                return 1.0f + (currentCombo * GameConfig.CombatSettings.COMBO_DAMAGE_BONUS_PER_HIT);
            }
            return 1.0f;
        }

        /// <summary>
        /// Update combo counter
        /// </summary>
        private static void UpdateCombo(Character attacker)
        {
            if (attacker == lastAttacker)
            {
                currentCombo = Mathf.Min(currentCombo + 1, GameConfig.CombatSettings.MAX_COMBO_COUNT);
            }
            else
            {
                currentCombo = 1;
                lastAttacker = attacker;
            }
        }

        /// <summary>
        /// Reset combo counter (on miss, flee, or turn change)
        /// </summary>
        public static void ResetCombo()
        {
            currentCombo = 0;
            lastAttacker = null;
        }

        /// <summary>
        /// Get current combo count
        /// </summary>
        public static int GetCurrentCombo()
        {
            return currentCombo;
        }
    }
}
