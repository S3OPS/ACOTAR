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
        Light
    }

    /// <summary>
    /// Result of a combat action
    /// </summary>
    public class CombatResult
    {
        public int damage;
        public DamageType damageType;
        public bool isCritical;
        public bool wasBlocked;
        public string description;

        public CombatResult(int dmg, DamageType type, string desc)
        {
            damage = dmg;
            damageType = type;
            description = desc;
            isCritical = false;
            wasBlocked = false;
        }
    }

    /// <summary>
    /// Foundation for turn-based combat system
    /// Designed for future expansion with magic abilities and strategies
    /// </summary>
    public class CombatSystem
    {
        private const float CRITICAL_HIT_CHANCE = 0.15f;
        private const float CRITICAL_HIT_MULTIPLIER = 2.0f;
        private const float AGILITY_DODGE_FACTOR = 0.01f;
        private const float DEFEND_DAMAGE_REDUCTION = 0.5f;

        /// <summary>
        /// Calculate physical attack damage
        /// </summary>
        public static CombatResult CalculatePhysicalAttack(Character attacker, Character defender)
        {
            if (attacker == null || defender == null)
            {
                return new CombatResult(0, DamageType.Physical, "Invalid combat participants");
            }

            // Base damage from strength
            int baseDamage = attacker.strength;

            // Check for critical hit
            bool isCritical = Random.value < CRITICAL_HIT_CHANCE;
            if (isCritical)
            {
                baseDamage = Mathf.RoundToInt(baseDamage * CRITICAL_HIT_MULTIPLIER);
            }

            // Apply defender's agility for dodge chance
            float dodgeChance = defender.agility * AGILITY_DODGE_FACTOR;
            bool dodged = Random.value < dodgeChance;
            if (dodged)
            {
                return new CombatResult(0, DamageType.Physical, $"{defender.name} dodged the attack!");
            }

            // Apply random variance (80-120% of base damage)
            float variance = Random.Range(0.8f, 1.2f);
            int finalDamage = Mathf.RoundToInt(baseDamage * variance);

            string description = isCritical 
                ? $"{attacker.name} landed a CRITICAL HIT for {finalDamage} damage!"
                : $"{attacker.name} attacked {defender.name} for {finalDamage} damage";

            CombatResult result = new CombatResult(finalDamage, DamageType.Physical, description);
            result.isCritical = isCritical;
            return result;
        }

        /// <summary>
        /// Calculate magic attack damage
        /// </summary>
        public static CombatResult CalculateMagicAttack(Character attacker, Character defender, MagicType magicType)
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

            // Apply magic type modifiers
            DamageType damageType = GetDamageTypeForMagic(magicType);
            float typeMultiplier = GetMagicTypeMultiplier(magicType);
            baseDamage = Mathf.RoundToInt(baseDamage * typeMultiplier);

            // Check for critical hit
            bool isCritical = Random.value < CRITICAL_HIT_CHANCE;
            if (isCritical)
            {
                baseDamage = Mathf.RoundToInt(baseDamage * CRITICAL_HIT_MULTIPLIER);
            }

            // Apply random variance
            float variance = Random.Range(0.8f, 1.2f);
            int finalDamage = Mathf.RoundToInt(baseDamage * variance);

            string description = isCritical
                ? $"{attacker.name} unleashed {magicType} - CRITICAL HIT for {finalDamage} damage!"
                : $"{attacker.name} used {magicType} on {defender.name} for {finalDamage} damage";

            CombatResult result = new CombatResult(finalDamage, damageType, description);
            result.isCritical = isCritical;
            return result;
        }

        /// <summary>
        /// Execute defend action (reduces incoming damage)
        /// </summary>
        public static int ApplyDefense(int incomingDamage)
        {
            return Mathf.RoundToInt(incomingDamage * DEFEND_DAMAGE_REDUCTION);
        }

        /// <summary>
        /// Calculate flee success chance
        /// </summary>
        public static bool AttemptFlee(Character character, Character enemy)
        {
            if (character == null || enemy == null) return false;

            // Base 50% flee chance, modified by agility difference
            float baseChance = 0.5f;
            float agilityDiff = (character.agility - enemy.agility) * 0.01f;
            float fleeChance = Mathf.Clamp(baseChance + agilityDiff, 0.1f, 0.9f);

            return Random.value < fleeChance;
        }

        /// <summary>
        /// Map magic type to damage type
        /// </summary>
        private static DamageType GetDamageTypeForMagic(MagicType magicType)
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
                case MagicType.ShieldCreation:
                    return 0.5f; // Shields are defensive
                default:
                    return 1.0f;
            }
        }
    }
}
