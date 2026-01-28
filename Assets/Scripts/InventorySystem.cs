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
                return;

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
                if (slot.item.itemId == itemId)
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
                if (slot.item.itemId == itemId)
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
                if (slot.item.itemId == itemId)
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
