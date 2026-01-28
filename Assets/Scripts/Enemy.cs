using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Enemy difficulty tiers
    /// </summary>
    public enum EnemyDifficulty
    {
        Trivial,
        Easy,
        Normal,
        Hard,
        Elite,
        Boss
    }

    /// <summary>
    /// Enemy behavior patterns
    /// </summary>
    public enum EnemyBehavior
    {
        Aggressive,     // Always attacks
        Defensive,      // Prefers to defend
        Balanced,       // Mix of attack and defense
        Tactical,       // Uses abilities strategically
        Berserker,      // High damage, low defense
        Support         // Focuses on buffs/debuffs
    }

    /// <summary>
    /// Enemy character class extending the base Character system
    /// Represents hostile creatures and NPCs in ACOTAR world
    /// </summary>
    [System.Serializable]
    public class Enemy : Character
    {
        public EnemyDifficulty difficulty;
        public EnemyBehavior behavior;
        public int experienceReward;
        public List<string> lootTable;
        public float aggroRange;
        public bool isBoss;

        /// <summary>
        /// Create an enemy with specified properties
        /// </summary>
        public Enemy(string name, CharacterClass charClass, Court court, EnemyDifficulty difficulty)
            : base(name, charClass, court)
        {
            this.difficulty = difficulty;
            this.behavior = EnemyBehavior.Balanced;
            this.lootTable = new List<string>();
            this.aggroRange = 10f;
            this.isBoss = false;

            // Scale stats based on difficulty
            ApplyDifficultyScaling();
            CalculateExperienceReward();
        }

        /// <summary>
        /// Scale enemy stats based on difficulty
        /// </summary>
        private void ApplyDifficultyScaling()
        {
            float multiplier = 1.0f;

            switch (difficulty)
            {
                case EnemyDifficulty.Trivial:
                    multiplier = 0.5f;
                    break;
                case EnemyDifficulty.Easy:
                    multiplier = 0.75f;
                    break;
                case EnemyDifficulty.Normal:
                    multiplier = 1.0f;
                    break;
                case EnemyDifficulty.Hard:
                    multiplier = 1.5f;
                    break;
                case EnemyDifficulty.Elite:
                    multiplier = 2.0f;
                    break;
                case EnemyDifficulty.Boss:
                    multiplier = 3.0f;
                    isBoss = true;
                    break;
            }

            // Apply scaling to stats
            maxHealth = Mathf.RoundToInt(maxHealth * multiplier);
            health = maxHealth;
            magicPower = Mathf.RoundToInt(magicPower * multiplier);
            strength = Mathf.RoundToInt(strength * multiplier);
            agility = Mathf.RoundToInt(agility * multiplier);
        }

        /// <summary>
        /// Calculate experience reward based on difficulty and stats
        /// </summary>
        private void CalculateExperienceReward()
        {
            int baseXP = 50;

            switch (difficulty)
            {
                case EnemyDifficulty.Trivial:
                    baseXP = 25;
                    break;
                case EnemyDifficulty.Easy:
                    baseXP = 50;
                    break;
                case EnemyDifficulty.Normal:
                    baseXP = 100;
                    break;
                case EnemyDifficulty.Hard:
                    baseXP = 200;
                    break;
                case EnemyDifficulty.Elite:
                    baseXP = 400;
                    break;
                case EnemyDifficulty.Boss:
                    baseXP = 1000;
                    break;
            }

            // Scale XP with level
            experienceReward = baseXP * level;
        }

        /// <summary>
        /// Add item to loot table
        /// </summary>
        public void AddLoot(string itemId)
        {
            if (!lootTable.Contains(itemId))
            {
                lootTable.Add(itemId);
            }
        }

        /// <summary>
        /// Get random loot from enemy's loot table
        /// </summary>
        public List<string> DropLoot()
        {
            List<string> droppedItems = new List<string>();

            foreach (string itemId in lootTable)
            {
                // 50% chance to drop each item (can be adjusted per item later)
                if (Random.value < 0.5f)
                {
                    droppedItems.Add(itemId);
                }
            }

            return droppedItems;
        }
    }

    /// <summary>
    /// Factory for creating pre-defined ACOTAR enemies
    /// </summary>
    public static class EnemyFactory
    {
        /// <summary>
        /// Create a Bogge (shapeshifter from ACOTAR)
        /// </summary>
        public static Enemy CreateBogge(EnemyDifficulty difficulty = EnemyDifficulty.Normal)
        {
            Enemy bogge = new Enemy("Bogge", CharacterClass.LesserFae, Court.Spring, difficulty);
            bogge.behavior = EnemyBehavior.Tactical;
            bogge.LearnAbility(MagicType.Shapeshifting);
            bogge.AddLoot("potion_healing");
            return bogge;
        }

        /// <summary>
        /// Create a Naga (serpent creature from ACOTAR)
        /// </summary>
        public static Enemy CreateNaga(EnemyDifficulty difficulty = EnemyDifficulty.Normal)
        {
            Enemy naga = new Enemy("Naga", CharacterClass.LesserFae, Court.Spring, difficulty);
            naga.behavior = EnemyBehavior.Aggressive;
            naga.AddLoot("crafting_naga_scale");
            naga.AddLoot("potion_healing");
            return naga;
        }

        /// <summary>
        /// Create an Attor (flying monster from ACOTAR)
        /// </summary>
        public static Enemy CreateAttor(EnemyDifficulty difficulty = EnemyDifficulty.Hard)
        {
            Enemy attor = new Enemy("Attor", CharacterClass.Attor, Court.Spring, difficulty);
            attor.behavior = EnemyBehavior.Aggressive;
            attor.AddLoot("crafting_attor_wing");
            attor.AddLoot("weapon_attor_claw");
            return attor;
        }

        /// <summary>
        /// Create a Suriel (prophetic creature)
        /// </summary>
        public static Enemy CreateSuriel(EnemyDifficulty difficulty = EnemyDifficulty.Easy)
        {
            Enemy suriel = new Enemy("Suriel", CharacterClass.Suriel, Court.Spring, difficulty);
            suriel.behavior = EnemyBehavior.Defensive;
            suriel.LearnAbility(MagicType.Seer);
            suriel.AddLoot("magical_suriel_blessing");
            return suriel;
        }

        /// <summary>
        /// Create Amarantha (main antagonist, boss fight)
        /// </summary>
        public static Enemy CreateAmarantha()
        {
            Enemy amarantha = new Enemy("Amarantha", CharacterClass.HighFae, Court.Spring, EnemyDifficulty.Boss);
            amarantha.behavior = EnemyBehavior.Tactical;
            amarantha.level = 10;
            amarantha.LearnAbility(MagicType.DarknessManipulation);
            amarantha.LearnAbility(MagicType.Daemati);
            amarantha.LearnAbility(MagicType.ShieldCreation);
            amarantha.AddLoot("quest_cursed_crown");
            amarantha.AddLoot("magical_amarantha_power");
            return amarantha;
        }

        /// <summary>
        /// Create a generic Fae Guard
        /// </summary>
        public static Enemy CreateFaeGuard(Court court, EnemyDifficulty difficulty = EnemyDifficulty.Normal)
        {
            Enemy guard = new Enemy($"{court} Court Guard", CharacterClass.HighFae, court, difficulty);
            guard.behavior = EnemyBehavior.Balanced;
            guard.AddLoot("weapon_fae_sword");
            guard.AddLoot("potion_healing");
            return guard;
        }

        /// <summary>
        /// Create an Illyrian Warrior
        /// </summary>
        public static Enemy CreateIllyrianWarrior(EnemyDifficulty difficulty = EnemyDifficulty.Hard)
        {
            Enemy warrior = new Enemy("Illyrian Warrior", CharacterClass.Illyrian, Court.Night, difficulty);
            warrior.behavior = EnemyBehavior.Aggressive;
            warrior.AddLoot("weapon_illyrian_blade");
            warrior.AddLoot("armor_illyrian_leathers");
            return warrior;
        }
    }
}
