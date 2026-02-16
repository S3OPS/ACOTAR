using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Item rarity tiers affecting stats and appearance
    /// </summary>
    public enum ItemRarity
    {
        Common,      // White - Basic items
        Uncommon,    // Green - Slightly better
        Rare,        // Blue - Significantly better
        Epic,        // Purple - Very powerful
        Legendary,   // Orange - Extremely rare and powerful
        Mythic       // Red - Unique one-of-a-kind items
    }

    /// <summary>
    /// Affix types that can be applied to equipment
    /// </summary>
    public enum AffixType
    {
        // Damage affixes
        OfPower,        // +Strength
        OfMight,        // +Physical Damage
        OfFlame,        // +Fire Damage
        OfFrost,        // +Ice Damage
        OfShadow,       // +Darkness Damage
        OfLight,        // +Light Damage
        
        // Defensive affixes
        OfProtection,   // +Defense
        OfVitality,     // +Max Health
        OfResilience,   // +Damage Reduction
        OfWarding,      // +Magic Resistance
        
        // Utility affixes
        OfSwiftness,    // +Agility
        OfWisdom,       // +Magic Power
        OfRegeneration, // +Health Regen
        OfClarity,      // +Mana Regen
        OfLuck,         // +Critical Chance
        
        // Special affixes (rare)
        OfTheFae,       // +All Stats
        OfTheHighLord,  // +Massive Power
        OfTheCauldron,  // +Unique Effect
        OfStarfall      // +Combo with magic
    }

    /// <summary>
    /// Equipment set definitions
    /// </summary>
    public enum EquipmentSet
    {
        None,
        NightCourtRegalia,    // Night Court themed set
        SpringCourtArmor,     // Spring Court themed set
        IllyrianWarGear,      // Illyrian warrior set
        ArcheronHeirlooms,    // Archeron sisters set
        InnerCircleRelics,    // Inner Circle set
        CauldronForged,       // Items made by the Cauldron
        StarfallCollection    // Starfall event items
    }

    /// <summary>
    /// Enhanced item with procedural affixes and set bonuses
    /// v2.6.0: NEW - Advanced loot system
    /// </summary>
    [System.Serializable]
    public class EnhancedItem
    {
        public string baseItemId;
        public string displayName;
        public ItemType itemType;
        public ItemRarity rarity;
        public EquipmentSet set;
        public List<AffixType> affixes;
        public Dictionary<string, int> statBonuses;
        
        public int strengthBonus;
        public int agilityBonus;
        public int magicPowerBonus;
        public int healthBonus;
        public int defenseBonus;
        
        public int level;              // Item level requirement
        public int goldValue;          // Sell/buy price
        public string description;
        
        public EnhancedItem(string baseId, ItemType type, ItemRarity itemRarity)
        {
            baseItemId = baseId;
            itemType = type;
            rarity = itemRarity;
            set = EquipmentSet.None;
            affixes = new List<AffixType>();
            statBonuses = new Dictionary<string, int>();
            
            strengthBonus = 0;
            agilityBonus = 0;
            magicPowerBonus = 0;
            healthBonus = 0;
            defenseBonus = 0;
            
            level = 1;
            goldValue = 100;
            description = "";
        }

        /// <summary>
        /// Generate display name with affixes
        /// </summary>
        public string GetFullDisplayName()
        {
            string prefix = "";
            string suffix = "";
            
            if (affixes.Count > 0)
            {
                // Add first affix as suffix
                suffix = $" {affixes[0].ToString()}";
            }
            
            return $"{prefix}{displayName}{suffix}";
        }

        /// <summary>
        /// Get rarity color for UI display
        /// </summary>
        public Color GetRarityColor()
        {
            switch (rarity)
            {
                case ItemRarity.Common: return Color.white;
                case ItemRarity.Uncommon: return Color.green;
                case ItemRarity.Rare: return Color.blue;
                case ItemRarity.Epic: return new Color(0.6f, 0.2f, 0.8f); // Purple
                case ItemRarity.Legendary: return new Color(1f, 0.5f, 0f); // Orange
                case ItemRarity.Mythic: return Color.red;
                default: return Color.white;
            }
        }
    }

    /// <summary>
    /// Advanced Loot System - Procedural item generation with rarity and affixes
    /// Version 2.6.0 - New Feature
    /// 
    /// Generates dynamic loot with:
    /// - Rarity tiers (Common -> Mythic)
    /// - Random affixes for equipment
    /// - Set bonuses for themed equipment
    /// - Level-appropriate loot tables
    /// </summary>
    public class AdvancedLootSystem
    {
        private Dictionary<EquipmentSet, List<string>> setItems;
        private Dictionary<EquipmentSet, SetBonus> setBonuses;
        
        // Property accessors (v2.6.0: Following v2.5.x patterns)
        public bool IsInitialized => setItems != null && setBonuses != null;
        
        /// <summary>
        /// Set bonus definition
        /// </summary>
        [System.Serializable]
        public class SetBonus
        {
            public string setName;
            public int requiredPieces;
            public string bonusDescription;
            public int strengthBonus;
            public int agilityBonus;
            public int magicPowerBonus;
            public int healthBonus;
            public float damageMultiplier;
            
            public SetBonus(string name, int pieces, string desc)
            {
                setName = name;
                requiredPieces = pieces;
                bonusDescription = desc;
                strengthBonus = 0;
                agilityBonus = 0;
                magicPowerBonus = 0;
                healthBonus = 0;
                damageMultiplier = 1.0f;
            }
        }

        public AdvancedLootSystem()
        {
            setItems = new Dictionary<EquipmentSet, List<string>>();
            setBonuses = new Dictionary<EquipmentSet, SetBonus>();
            InitializeSetBonuses();
            Debug.Log("AdvancedLootSystem initialized with procedural generation");
        }

        /// <summary>
        /// Initialize equipment set bonuses
        /// </summary>
        private void InitializeSetBonuses()
        {
            // Night Court Regalia (3 pieces)
            var nightCourtBonus = new SetBonus("Night Court Regalia", 3, "Darkness and starlight empower you");
            nightCourtBonus.magicPowerBonus = 15;
            nightCourtBonus.agilityBonus = 10;
            nightCourtBonus.damageMultiplier = 1.15f;
            setBonuses[EquipmentSet.NightCourtRegalia] = nightCourtBonus;

            // Spring Court Armor (3 pieces)
            var springCourtBonus = new SetBonus("Spring Court Armor", 3, "Nature's blessing enhances vitality");
            springCourtBonus.healthBonus = 50;
            springCourtBonus.strengthBonus = 12;
            setBonuses[EquipmentSet.SpringCourtArmor] = springCourtBonus;

            // Illyrian War Gear (3 pieces)
            var illyrianBonus = new SetBonus("Illyrian War Gear", 3, "Warrior spirit increases combat prowess");
            illyrianBonus.strengthBonus = 20;
            illyrianBonus.agilityBonus = 15;
            illyrianBonus.damageMultiplier = 1.20f;
            setBonuses[EquipmentSet.IllyrianWarGear] = illyrianBonus;

            // Archeron Heirlooms (2 pieces)
            var archeronBonus = new SetBonus("Archeron Heirlooms", 2, "Sisterhood provides unique power");
            archeronBonus.magicPowerBonus = 18;
            archeronBonus.healthBonus = 30;
            setBonuses[EquipmentSet.ArcheronHeirlooms] = archeronBonus;

            // Inner Circle Relics (4 pieces)
            var innerCircleBonus = new SetBonus("Inner Circle Relics", 4, "Bond of loyalty grants supreme power");
            innerCircleBonus.strengthBonus = 15;
            innerCircleBonus.magicPowerBonus = 20;
            innerCircleBonus.agilityBonus = 12;
            innerCircleBonus.damageMultiplier = 1.25f;
            setBonuses[EquipmentSet.InnerCircleRelics] = innerCircleBonus;

            // Cauldron Forged (2 pieces)
            var cauldronBonus = new SetBonus("Cauldron Forged", 2, "Power stolen from the Cauldron itself");
            cauldronBonus.magicPowerBonus = 30;
            cauldronBonus.damageMultiplier = 1.30f;
            setBonuses[EquipmentSet.CauldronForged] = cauldronBonus;

            // Starfall Collection (3 pieces)
            var starfallBonus = new SetBonus("Starfall Collection", 3, "Blessed by falling stars");
            starfallBonus.magicPowerBonus = 25;
            starfallBonus.healthBonus = 40;
            starfallBonus.damageMultiplier = 1.18f;
            setBonuses[EquipmentSet.StarfallCollection] = starfallBonus;
        }

        /// <summary>
        /// Generate a random enhanced item based on player level
        /// </summary>
        public EnhancedItem GenerateLoot(int playerLevel, ItemType itemType = ItemType.Weapon)
        {
            // Defensive check (v2.6.0)
            if (!IsInitialized)
            {
                Debug.LogWarning("AdvancedLootSystem: Cannot generate loot - system not initialized");
                return null;
            }

            // Determine rarity based on level and random chance
            ItemRarity rarity = DetermineRarity(playerLevel);
            
            // Create base item
            string baseId = $"generated_{itemType}_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
            EnhancedItem item = new EnhancedItem(baseId, itemType, rarity);
            item.level = playerLevel;
            
            // Set base name based on type
            item.displayName = GetBaseItemName(itemType);
            
            // Generate affixes based on rarity
            int affixCount = GetAffixCountForRarity(rarity);
            for (int i = 0; i < affixCount; i++)
            {
                AffixType affix = GenerateRandomAffix();
                if (!item.affixes.Contains(affix))
                {
                    item.affixes.Add(affix);
                    ApplyAffixStats(item, affix, rarity);
                }
            }
            
            // Chance for set item (higher at higher rarities)
            if (ShouldBeSetItem(rarity))
            {
                item.set = GenerateRandomSet();
            }
            
            // Calculate gold value
            item.goldValue = CalculateItemValue(item);
            
            // Generate description
            item.description = GenerateItemDescription(item);
            
            Debug.Log($"Generated {rarity} {item.GetFullDisplayName()} (Level {item.level})");
            return item;
        }

        /// <summary>
        /// Determine item rarity based on player level and RNG
        /// Higher levels have better chance for rare items
        /// </summary>
        private ItemRarity DetermineRarity(int playerLevel)
        {
            float roll = Random.value * 100f;
            float levelBonus = playerLevel * 0.5f; // 0.5% better chance per level
            
            // Mythic: 1% + level bonus (max 6%)
            if (roll < 1f + Mathf.Min(levelBonus, 5f))
                return ItemRarity.Mythic;
            
            // Legendary: 5% + level bonus (max 15%)
            if (roll < 5f + Mathf.Min(levelBonus, 10f))
                return ItemRarity.Legendary;
            
            // Epic: 15% + level bonus
            if (roll < 15f + levelBonus)
                return ItemRarity.Epic;
            
            // Rare: 25%
            if (roll < 40f)
                return ItemRarity.Rare;
            
            // Uncommon: 30%
            if (roll < 70f)
                return ItemRarity.Uncommon;
            
            // Common: 30%
            return ItemRarity.Common;
        }

        /// <summary>
        /// Get number of affixes based on rarity
        /// </summary>
        private int GetAffixCountForRarity(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common: return 0;
                case ItemRarity.Uncommon: return 1;
                case ItemRarity.Rare: return Random.Range(1, 3);
                case ItemRarity.Epic: return Random.Range(2, 4);
                case ItemRarity.Legendary: return Random.Range(3, 5);
                case ItemRarity.Mythic: return Random.Range(4, 6);
                default: return 0;
            }
        }

        /// <summary>
        /// Generate a random affix
        /// </summary>
        private AffixType GenerateRandomAffix()
        {
            var allAffixes = System.Enum.GetValues(typeof(AffixType));
            return (AffixType)allAffixes.GetValue(Random.Range(0, allAffixes.Length));
        }

        /// <summary>
        /// Apply stat bonuses from an affix
        /// </summary>
        private void ApplyAffixStats(EnhancedItem item, AffixType affix, ItemRarity rarity)
        {
            int rarityMultiplier = (int)rarity + 1; // 1-6 based on rarity
            
            switch (affix)
            {
                case AffixType.OfPower:
                    item.strengthBonus += 3 * rarityMultiplier;
                    break;
                case AffixType.OfMight:
                    item.strengthBonus += 5 * rarityMultiplier;
                    break;
                case AffixType.OfProtection:
                    item.defenseBonus += 4 * rarityMultiplier;
                    break;
                case AffixType.OfVitality:
                    item.healthBonus += 10 * rarityMultiplier;
                    break;
                case AffixType.OfSwiftness:
                    item.agilityBonus += 3 * rarityMultiplier;
                    break;
                case AffixType.OfWisdom:
                    item.magicPowerBonus += 4 * rarityMultiplier;
                    break;
                case AffixType.OfFlame:
                case AffixType.OfFrost:
                case AffixType.OfShadow:
                case AffixType.OfLight:
                    item.magicPowerBonus += 6 * rarityMultiplier;
                    break;
                case AffixType.OfTheFae:
                    item.strengthBonus += 2 * rarityMultiplier;
                    item.agilityBonus += 2 * rarityMultiplier;
                    item.magicPowerBonus += 2 * rarityMultiplier;
                    break;
                case AffixType.OfTheHighLord:
                    item.strengthBonus += 10 * rarityMultiplier;
                    item.magicPowerBonus += 10 * rarityMultiplier;
                    break;
                case AffixType.OfTheCauldron:
                    item.magicPowerBonus += 15 * rarityMultiplier;
                    item.healthBonus += 15 * rarityMultiplier;
                    break;
                case AffixType.OfStarfall:
                    item.magicPowerBonus += 8 * rarityMultiplier;
                    item.agilityBonus += 4 * rarityMultiplier;
                    break;
            }
        }

        /// <summary>
        /// Determine if item should be part of a set
        /// </summary>
        private bool ShouldBeSetItem(ItemRarity rarity)
        {
            float chance = 0f;
            switch (rarity)
            {
                case ItemRarity.Rare: chance = 0.1f; break;
                case ItemRarity.Epic: chance = 0.3f; break;
                case ItemRarity.Legendary: chance = 0.5f; break;
                case ItemRarity.Mythic: chance = 0.8f; break;
            }
            return Random.value < chance;
        }

        /// <summary>
        /// Generate random equipment set
        /// </summary>
        private EquipmentSet GenerateRandomSet()
        {
            var allSets = System.Enum.GetValues(typeof(EquipmentSet));
            EquipmentSet set;
            do
            {
                set = (EquipmentSet)allSets.GetValue(Random.Range(0, allSets.Length));
            } while (set == EquipmentSet.None);
            
            return set;
        }

        /// <summary>
        /// Get base item name for type
        /// </summary>
        private string GetBaseItemName(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                    string[] weapons = { "Blade", "Sword", "Dagger", "Staff", "Bow", "Axe" };
                    return weapons[Random.Range(0, weapons.Length)];
                    
                case ItemType.Armor:
                    string[] armor = { "Breastplate", "Chainmail", "Robes", "Leather Armor", "Scale Mail" };
                    return armor[Random.Range(0, armor.Length)];
                    
                default:
                    return "Item";
            }
        }

        /// <summary>
        /// Calculate item gold value based on stats and rarity
        /// </summary>
        private int CalculateItemValue(EnhancedItem item)
        {
            int baseValue = 100;
            int rarityValue = ((int)item.rarity + 1) * 100;
            int affixValue = item.affixes.Count * 50;
            int setBonus = item.set != EquipmentSet.None ? 200 : 0;
            
            return baseValue + rarityValue + affixValue + setBonus;
        }

        /// <summary>
        /// Generate item description
        /// </summary>
        private string GenerateItemDescription(EnhancedItem item)
        {
            string desc = $"A {item.rarity} {item.itemType}";
            
            if (item.set != EquipmentSet.None)
            {
                desc += $" from the {item.set} set";
            }
            
            desc += $". Requires level {item.level}.";
            
            return desc;
        }

        /// <summary>
        /// Check active set bonuses for equipped items
        /// </summary>
        public Dictionary<EquipmentSet, int> CheckSetBonuses(List<EnhancedItem> equippedItems)
        {
            // Defensive check (v2.6.0)
            if (equippedItems == null)
            {
                return new Dictionary<EquipmentSet, int>();
            }

            var setCounts = new Dictionary<EquipmentSet, int>();
            
            foreach (var item in equippedItems)
            {
                if (item.set != EquipmentSet.None)
                {
                    if (!setCounts.ContainsKey(item.set))
                    {
                        setCounts[item.set] = 0;
                    }
                    setCounts[item.set]++;
                }
            }
            
            return setCounts;
        }

        /// <summary>
        /// Get active set bonuses based on equipped items
        /// </summary>
        public List<SetBonus> GetActiveSetBonuses(List<EnhancedItem> equippedItems)
        {
            var activeBonuses = new List<SetBonus>();
            var setCounts = CheckSetBonuses(equippedItems);
            
            foreach (var setCount in setCounts)
            {
                if (setBonuses.ContainsKey(setCount.Key))
                {
                    var bonus = setBonuses[setCount.Key];
                    if (setCount.Value >= bonus.requiredPieces)
                    {
                        activeBonuses.Add(bonus);
                        Debug.Log($"✨ Set Bonus Active: {bonus.setName} ({setCount.Value}/{bonus.requiredPieces} pieces)");
                    }
                }
            }
            
            return activeBonuses;
        }
    }
}
