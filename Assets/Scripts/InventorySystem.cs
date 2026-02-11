using System;
using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Item types in ACOTAR world
    /// </summary>
    public enum ItemType
    {
        Weapon,
        Armor,
        Consumable,
        QuestItem,
        Crafting,
        Magical
    }

    /// <summary>
    /// Item rarity tiers
    /// </summary>
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Artifact
    }

    /// <summary>
    /// Represents an item in the game world
    /// </summary>
    [Serializable]
    public class Item
    {
        public string itemId;
        public string name;
        public string description;
        public ItemType type;
        public ItemRarity rarity;
        public int maxStackSize;
        public bool isQuestItem;

        // Item effects
        public int healthBonus;
        public int magicPowerBonus;
        public int strengthBonus;
        public int agilityBonus;

        public Item(string id, string name, string desc, ItemType type, ItemRarity rarity)
        {
            this.itemId = id;
            this.name = name;
            this.description = desc;
            this.type = type;
            this.rarity = rarity;
            this.maxStackSize = type == ItemType.Consumable ? 99 : 1;
            this.isQuestItem = type == ItemType.QuestItem;
        }
    }

    /// <summary>
    /// Represents an item instance in inventory
    /// </summary>
    [Serializable]
    public class InventorySlot
    {
        public Item item;
        public int quantity;
        public bool isEquipped;

        public InventorySlot(Item item, int quantity = 1)
        {
            this.item = item;
            this.quantity = quantity;
            this.isEquipped = false;
        }
    }

    /// <summary>
    /// View model for inventory items used by UI
    /// Combines Item data with inventory slot information
    /// </summary>
    [Serializable]
    public class InventoryItem
    {
        public string itemId;
        public string name;
        public string description;
        public ItemType itemType;
        public ItemRarity rarity;
        public int quantity;
        public int value;
        public int power;
        public bool isEquipped;

        public InventoryItem(InventorySlot slot)
        {
            if (slot == null || slot.item == null)
            {
                throw new System.ArgumentNullException(nameof(slot), "InventorySlot and its item cannot be null");
            }

            this.itemId = slot.item.itemId;
            this.name = slot.item.name;
            this.description = slot.item.description;
            this.itemType = slot.item.type;
            this.rarity = slot.item.rarity;
            this.quantity = slot.quantity;
            this.isEquipped = slot.isEquipped;
            
            // Calculate value based on rarity
            this.value = CalculateValue(slot.item);
            
            // Calculate power based on stat bonuses
            this.power = slot.item.strengthBonus + slot.item.magicPowerBonus;
        }

        private int CalculateValue(Item item)
        {
            int baseValue = 10;
            switch (item.rarity)
            {
                case ItemRarity.Common: return baseValue;
                case ItemRarity.Uncommon: return baseValue * 3;
                case ItemRarity.Rare: return baseValue * 10;
                case ItemRarity.Epic: return baseValue * 25;
                case ItemRarity.Legendary: return baseValue * 50;
                case ItemRarity.Artifact: return baseValue * 100;
                default: return baseValue;
            }
        }
    }

    /// <summary>
    /// Manages character inventory and equipment
    /// Foundation for item collection and management
    /// </summary>
    public class InventorySystem
    {
        private List<InventorySlot> inventory;
        private Dictionary<string, Item> itemDatabase;
        private const int MAX_INVENTORY_SIZE = 50;

        // Equipment slots
        private Item equippedWeapon;
        private Item equippedArmor;

        public InventorySystem()
        {
            inventory = new List<InventorySlot>();
            itemDatabase = new Dictionary<string, Item>();
            InitializeItemDatabase();
        }

        /// <summary>
        /// Initialize sample items for the game
        /// </summary>
        private void InitializeItemDatabase()
        {
            // Weapons
            AddItemToDatabase(new Item(
                "weapon_ash_dagger",
                "Ash Wood Dagger",
                "A simple dagger made of ash wood, effective against Fae",
                ItemType.Weapon,
                ItemRarity.Common
            ) { strengthBonus = 5 });

            AddItemToDatabase(new Item(
                "weapon_illyrian_blade",
                "Illyrian Blade",
                "A finely crafted Illyrian sword, sharp and deadly",
                ItemType.Weapon,
                ItemRarity.Rare
            ) { strengthBonus = 20 });

            // NEW BASE GAME WEAPONS
            AddItemToDatabase(new Item(
                "weapon_spring_blade",
                "Spring Court Sword",
                "An elegant blade forged in the Spring Court, decorated with rose motifs",
                ItemType.Weapon,
                ItemRarity.Uncommon
            ) { strengthBonus = 12, agilityBonus = 5 });

            AddItemToDatabase(new Item(
                "weapon_hunters_bow",
                "Hunter's Longbow",
                "Your trusty bow from your days hunting in the mortal lands",
                ItemType.Weapon,
                ItemRarity.Common
            ) { strengthBonus = 8, agilityBonus = 8 });

            AddItemToDatabase(new Item(
                "weapon_fae_spear",
                "Enchanted Fae Spear",
                "A spear imbued with ancient Fae magic",
                ItemType.Weapon,
                ItemRarity.Epic
            ) { strengthBonus = 25, magicPowerBonus = 10 });

            AddItemToDatabase(new Item(
                "weapon_shadow_dagger",
                "Dagger of Shadows",
                "A mysterious blade that seems to absorb light itself",
                ItemType.Weapon,
                ItemRarity.Legendary
            ) { strengthBonus = 30, agilityBonus = 15, magicPowerBonus = 10 });

            // NEW BASE GAME ARMOR
            AddItemToDatabase(new Item(
                "armor_spring_leather",
                "Spring Court Leathers",
                "Light leather armor in Spring Court colors",
                ItemType.Armor,
                ItemRarity.Common
            ) { healthBonus = 25, agilityBonus = 5 });

            AddItemToDatabase(new Item(
                "armor_fae_cloak",
                "Enchanted Fae Cloak",
                "A cloak woven with protective magic",
                ItemType.Armor,
                ItemRarity.Rare
            ) { healthBonus = 40, magicPowerBonus = 15 });

            AddItemToDatabase(new Item(
                "armor_high_fae_plate",
                "High Fae Battle Armor",
                "Ornate armor fit for a High Fae warrior",
                ItemType.Armor,
                ItemRarity.Epic
            ) { healthBonus = 75, strengthBonus = 10 });

            AddItemToDatabase(new Item(
                "armor_night_silk",
                "Night Court Silk Armor",
                "Lightweight but incredibly strong armor from the Night Court",
                ItemType.Armor,
                ItemRarity.Legendary
            ) { healthBonus = 100, agilityBonus = 20, magicPowerBonus = 20 });

            // Consumables
            AddItemToDatabase(new Item(
                "potion_healing",
                "Healing Potion",
                "Restores 50 health points",
                ItemType.Consumable,
                ItemRarity.Common
            ) { healthBonus = 50 });

            AddItemToDatabase(new Item(
                "potion_magic",
                "Magic Elixir",
                "Temporarily increases magic power",
                ItemType.Consumable,
                ItemRarity.Uncommon
            ) { magicPowerBonus = 20 });

            // NEW BASE GAME CONSUMABLES
            AddItemToDatabase(new Item(
                "potion_greater_healing",
                "Greater Healing Potion",
                "Restores 100 health points - powerful Fae remedy",
                ItemType.Consumable,
                ItemRarity.Rare
            ) { healthBonus = 100 });

            AddItemToDatabase(new Item(
                "potion_stamina",
                "Stamina Draught",
                "Restores energy and increases agility temporarily",
                ItemType.Consumable,
                ItemRarity.Common
            ) { agilityBonus = 10 });

            AddItemToDatabase(new Item(
                "potion_strength",
                "Potion of the Bear",
                "Grants temporary strength boost",
                ItemType.Consumable,
                ItemRarity.Uncommon
            ) { strengthBonus = 15 });

            AddItemToDatabase(new Item(
                "food_fae_bread",
                "Fae Bread",
                "Delicious and nourishing bread from the Spring Court kitchens",
                ItemType.Consumable,
                ItemRarity.Common
            ) { healthBonus = 25 });

            AddItemToDatabase(new Item(
                "food_starlight_wine",
                "Starlight Wine",
                "A rare wine from the Night Court that enhances magic",
                ItemType.Consumable,
                ItemRarity.Rare
            ) { magicPowerBonus = 25 });

            // Quest Items
            AddItemToDatabase(new Item(
                "quest_book_breathings",
                "Book of Breathings",
                "An ancient and powerful tome from the Summer Court",
                ItemType.QuestItem,
                ItemRarity.Artifact
            ));

            AddItemToDatabase(new Item(
                "quest_rose_petal",
                "Spring Court Rose Petal",
                "A magical rose petal from Tamlin's manor",
                ItemType.QuestItem,
                ItemRarity.Rare
            ));

            // Magical Items
            AddItemToDatabase(new Item(
                "magical_glamour_stone",
                "Glamour Stone",
                "Allows user to create illusions and disguises",
                ItemType.Magical,
                ItemRarity.Epic
            ) { magicPowerBonus = 15 });

            // NEW BASE GAME MAGICAL ITEMS
            AddItemToDatabase(new Item(
                "magical_ash_warding",
                "Ash Wood Charm",
                "A protective charm made from ash wood, wards against Fae magic",
                ItemType.Magical,
                ItemRarity.Uncommon
            ) { magicPowerBonus = 10 });

            AddItemToDatabase(new Item(
                "magical_spring_blessing",
                "Spring Court Blessing",
                "A magical token blessed by the Spring Court, enhances vitality",
                ItemType.Magical,
                ItemRarity.Rare
            ) { healthBonus = 50, magicPowerBonus = 20 });

            AddItemToDatabase(new Item(
                "magical_suriel_vision",
                "Suriel's Eye Amulet",
                "Grants glimpses of possible futures",
                ItemType.Magical,
                ItemRarity.Epic
            ) { magicPowerBonus = 30 });

            AddItemToDatabase(new Item(
                "magical_rhysand_gift",
                "Star-blessed Pendant",
                "A gift from the High Lord of Night, channels starlight magic",
                ItemType.Magical,
                ItemRarity.Legendary
            ) { magicPowerBonus = 40, healthBonus = 30 });

            AddItemToDatabase(new Item(
                "magical_paint_set",
                "Enchanted Paint Set",
                "Your painting supplies, now imbued with a touch of Fae magic",
                ItemType.Magical,
                ItemRarity.Uncommon
            ) { magicPowerBonus = 5 });

            // Crafting Materials
            AddCraftingMaterials();
        }

        /// <summary>
        /// Add all crafting materials to the database
        /// </summary>
        private void AddCraftingMaterials()
        {
            // Basic Materials
            AddItemToDatabase(new Item(
                "crafting_ash_wood",
                "Ash Wood",
                "Wood from an ash tree, effective against Fae creatures",
                ItemType.Crafting,
                ItemRarity.Common
            ));

            AddItemToDatabase(new Item(
                "crafting_iron_ingot",
                "Iron Ingot",
                "A refined bar of iron for crafting",
                ItemType.Crafting,
                ItemRarity.Common
            ));

            AddItemToDatabase(new Item(
                "crafting_leather",
                "Leather",
                "Tanned animal hide for armor and accessories",
                ItemType.Crafting,
                ItemRarity.Common
            ));

            AddItemToDatabase(new Item(
                "crafting_silk",
                "Fae Silk",
                "Delicate silk woven by Fae artisans",
                ItemType.Crafting,
                ItemRarity.Uncommon
            ));

            // Alchemy Materials
            AddItemToDatabase(new Item(
                "crafting_healing_herb",
                "Healing Herb",
                "A common herb with restorative properties",
                ItemType.Crafting,
                ItemRarity.Common
            ));

            AddItemToDatabase(new Item(
                "crafting_water_vial",
                "Pure Water Vial",
                "Pristine water for potion brewing",
                ItemType.Crafting,
                ItemRarity.Common
            ));

            AddItemToDatabase(new Item(
                "crafting_moonflower",
                "Moonflower",
                "A rare flower that blooms only under moonlight",
                ItemType.Crafting,
                ItemRarity.Rare
            ));

            AddItemToDatabase(new Item(
                "crafting_starlight_essence",
                "Starlight Essence",
                "Captured essence of starlight, precious to the Night Court",
                ItemType.Crafting,
                ItemRarity.Epic
            ));

            // Advanced Materials
            AddItemToDatabase(new Item(
                "crafting_illyrian_steel",
                "Illyrian Steel",
                "Legendary steel forged by Illyrian warriors",
                ItemType.Crafting,
                ItemRarity.Rare
            ));

            AddItemToDatabase(new Item(
                "crafting_moonstone",
                "Moonstone",
                "A gem that glows with inner light",
                ItemType.Crafting,
                ItemRarity.Rare
            ));

            AddItemToDatabase(new Item(
                "crafting_mithril",
                "Mithril",
                "A rare, lightweight magical metal",
                ItemType.Crafting,
                ItemRarity.Epic
            ));

            AddItemToDatabase(new Item(
                "crafting_silver",
                "Silver Bar",
                "Refined silver for enchanting",
                ItemType.Crafting,
                ItemRarity.Uncommon
            ));

            // Magical Essences
            AddItemToDatabase(new Item(
                "magical_fae_essence",
                "Fae Essence",
                "Concentrated magical energy from the Fae realm",
                ItemType.Magical,
                ItemRarity.Epic
            ));

            AddItemToDatabase(new Item(
                "crafting_suriel_feather",
                "Suriel Feather",
                "A rare feather from a Suriel, imbued with prophetic power",
                ItemType.Crafting,
                ItemRarity.Legendary
            ));

            // Monster Drops
            AddItemToDatabase(new Item(
                "crafting_naga_scale",
                "Naga Scale",
                "A tough scale from a Naga serpent",
                ItemType.Crafting,
                ItemRarity.Uncommon
            ));

            AddItemToDatabase(new Item(
                "crafting_attor_wing",
                "Attor Wing Fragment",
                "Leathery wing membrane from an Attor",
                ItemType.Crafting,
                ItemRarity.Rare
            ));
        }

        private void AddItemToDatabase(Item item)
        {
            itemDatabase[item.itemId] = item;
        }

        /// <summary>
        /// Add item to inventory
        /// </summary>
        public bool AddItem(string itemId, int quantity = 1)
        {
            if (!itemDatabase.ContainsKey(itemId))
            {
                Debug.LogWarning($"Item not found in database: {itemId}");
                return false;
            }

            Item item = itemDatabase[itemId];

            // Check if inventory is full
            if (inventory.Count >= MAX_INVENTORY_SIZE)
            {
                Debug.LogWarning("Inventory is full!");
                return false;
            }

            // Try to stack with existing item
            if (item.maxStackSize > 1)
            {
                foreach (var slot in inventory)
                {
                    if (slot.item.itemId == itemId && slot.quantity < item.maxStackSize)
                    {
                        int spaceLeft = item.maxStackSize - slot.quantity;
                        int toAdd = Mathf.Min(quantity, spaceLeft);
                        slot.quantity += toAdd;
                        quantity -= toAdd;

                        if (quantity == 0)
                        {
                            Debug.Log($"Added {toAdd} x {item.name} to inventory");
                            return true;
                        }
                    }
                }
            }

            // Create new slot
            if (quantity > 0)
            {
                inventory.Add(new InventorySlot(item, quantity));
                Debug.Log($"Added {quantity} x {item.name} to inventory");
            }

            return true;
        }

        /// <summary>
        /// Remove item from inventory
        /// </summary>
        public bool RemoveItem(string itemId, int quantity = 1)
        {
            for (int i = inventory.Count - 1; i >= 0; i--)
            {
                if (inventory[i].item.itemId == itemId)
                {
                    if (inventory[i].quantity > quantity)
                    {
                        inventory[i].quantity -= quantity;
                        return true;
                    }
                    else if (inventory[i].quantity == quantity)
                    {
                        inventory.RemoveAt(i);
                        return true;
                    }
                    else
                    {
                        quantity -= inventory[i].quantity;
                        inventory.RemoveAt(i);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Check if inventory has item
        /// </summary>
        public bool HasItem(string itemId, int quantity = 1)
        {
            int count = 0;
            foreach (var slot in inventory)
            {
                if (slot.item.itemId == itemId)
                {
                    count += slot.quantity;
                }
            }
            return count >= quantity;
        }

        /// <summary>
        /// Get all items in inventory as InventoryItem view models
        /// </summary>
        public List<InventoryItem> GetAllItems()
        {
            var items = new List<InventoryItem>();
            foreach (var slot in inventory)
            {
                items.Add(new InventoryItem(slot));
            }
            return items;
        }

        /// <summary>
        /// Set inventory order based on sorted items list
        /// Used by UI sorting functionality
        /// </summary>
        public void SetSortedOrder(List<InventoryItem> sortedItems)
        {
            if (sortedItems == null || sortedItems.Count == 0)
                return;

            // Create a new sorted inventory list
            var sortedInventory = new List<InventorySlot>();
            
            foreach (var sortedItem in sortedItems)
            {
                // Find the corresponding slot in current inventory
                var slot = FindSlot(sortedItem.itemId);
                if (slot != null)
                {
                    sortedInventory.Add(slot);
                }
            }
            
            // Replace current inventory with sorted version
            inventory = sortedInventory;
        }

        /// <summary>
        /// Get total inventory slot count
        /// </summary>
        public int GetItemCount()
        {
            return inventory.Count;
        }

        /// <summary>
        /// Get count of a specific item by ID
        /// </summary>
        public int GetItemCount(string itemId)
        {
            int count = 0;
            foreach (var slot in inventory)
            {
                if (slot.item != null && slot.item.itemId == itemId)
                {
                    count += slot.quantity;
                }
            }
            return count;
        }

        /// <summary>
        /// Get a specific item from inventory by ID
        /// </summary>
        public InventoryItem GetItem(string itemId)
        {
            foreach (var slot in inventory)
            {
                if (slot.item != null && slot.item.itemId == itemId)
                {
                    return new InventoryItem(slot);
                }
            }
            return null;
        }

        /// <summary>
        /// Use a consumable item
        /// </summary>
        public bool UseItem(string itemId)
        {
            var slot = FindSlot(itemId);
            if (slot == null || slot.item.type != ItemType.Consumable)
                return false;

            // Apply item effects
            ApplyItemEffects(slot.item);
            
            // Remove one from quantity
            return RemoveItem(itemId, 1);
        }

        /// <summary>
        /// Find an inventory slot by item ID
        /// </summary>
        private InventorySlot FindSlot(string itemId)
        {
            foreach (var slot in inventory)
            {
                if (slot.item != null && slot.item.itemId == itemId)
                    return slot;
            }
            return null;
        }

        /// <summary>
        /// Apply item effects (placeholder for actual game logic)
        /// </summary>
        private void ApplyItemEffects(Item item)
        {
            if (item.healthBonus > 0)
            {
                Debug.Log($"Restored {item.healthBonus} health");
            }
            if (item.magicPowerBonus > 0)
            {
                Debug.Log($"Magic power increased by {item.magicPowerBonus}");
            }
        }

        /// <summary>
        /// Equip a weapon
        /// </summary>
        public bool EquipWeapon(string itemId)
        {
            var slot = FindSlot(itemId);
            if (slot == null || slot.item.type != ItemType.Weapon)
                return false;

            // Unequip current weapon
            if (equippedWeapon != null)
            {
                var oldSlot = FindSlot(equippedWeapon.itemId);
                if (oldSlot != null) oldSlot.isEquipped = false;
            }

            equippedWeapon = slot.item;
            slot.isEquipped = true;
            Debug.Log($"Equipped weapon: {slot.item.name}");
            return true;
        }

        /// <summary>
        /// Equip armor
        /// </summary>
        public bool EquipArmor(string itemId)
        {
            var slot = FindSlot(itemId);
            if (slot == null || slot.item.type != ItemType.Armor)
                return false;

            // Unequip current armor
            if (equippedArmor != null)
            {
                var oldSlot = FindSlot(equippedArmor.itemId);
                if (oldSlot != null) oldSlot.isEquipped = false;
            }

            equippedArmor = slot.item;
            slot.isEquipped = true;
            Debug.Log($"Equipped armor: {slot.item.name}");
            return true;
        }

        /// <summary>
        /// Get equipped weapon ID
        /// </summary>
        public string GetEquippedWeapon()
        {
            return equippedWeapon?.itemId;
        }

        /// <summary>
        /// Get equipped armor ID
        /// </summary>
        public string GetEquippedArmor()
        {
            return equippedArmor?.itemId;
        }

        /// <summary>
        /// Get item from database
        /// </summary>
        public Item GetItemFromDatabase(string itemId)
        {
            return itemDatabase.ContainsKey(itemId) ? itemDatabase[itemId] : null;
        }
    }
}
