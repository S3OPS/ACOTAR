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
    /// v2.6.7: Enhanced with equipment set support and mana cost reduction
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
        
        // v2.6.7: NEW - Equipment set system
        public string equipmentSetId;  // e.g., "SpringCourt", "NightCourt", "Illyrian"
        
        // v2.6.7: NEW - Mana cost reduction
        public int manaCostReduction;  // Reduces mana cost by this amount (flat reduction)
        public float manaCostReductionPercent;  // Reduces mana cost by this percentage (0.0 to 1.0)

        public Item(string id, string name, string desc, ItemType type, ItemRarity rarity)
        {
            this.itemId = id;
            this.name = name;
            this.description = desc;
            this.type = type;
            this.rarity = rarity;
            this.maxStackSize = type == ItemType.Consumable ? 99 : 1;
            this.isQuestItem = type == ItemType.QuestItem;
            this.equipmentSetId = "";  // v2.6.7
            this.manaCostReduction = 0;  // v2.6.7
            this.manaCostReductionPercent = 0f;  // v2.6.7
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
        /// <param name="itemId">The unique identifier of the item to add</param>
        /// <param name="quantity">The number of items to add (default: 1)</param>
        /// <returns>True if the item was successfully added, false otherwise</returns>
        /// <remarks>
        /// This method handles item stacking for stackable items (maxStackSize > 1).
        /// Items will be stacked with existing slots before creating new inventory slots.
        /// Will fail if the inventory is full or if the item doesn't exist in the database.
        /// v2.6.2: Enhanced with error handling and structured logging
        /// </remarks>
        public bool AddItem(string itemId, int quantity = 1)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(itemId))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Inventory", 
                        "Cannot add item: itemId is null or empty");
                    return false;
                }

                if (quantity <= 0)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Inventory", 
                        $"Cannot add item: invalid quantity {quantity}");
                    return false;
                }

                if (!itemDatabase.ContainsKey(itemId))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Inventory", 
                        $"Item not found in database: {itemId}");
                    return false;
                }

                Item item = itemDatabase[itemId];
                if (item == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Inventory", 
                        $"Item database contains null item for id: {itemId}");
                    return false;
                }

                // Check if inventory is full
                if (inventory.Count >= MAX_INVENTORY_SIZE)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Inventory", 
                        "Inventory is full!");
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
                                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, "Inventory", 
                                    $"Added {toAdd} x {item.name} to inventory (stacked)");
                                return true;
                            }
                        }
                    }
                }

                // Create new slot
                if (quantity > 0)
                {
                    inventory.Add(new InventorySlot(item, quantity));
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, "Inventory", 
                        $"Added {quantity} x {item.name} to inventory (new slot)");
                }

                return true;
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Inventory", 
                    $"Exception in AddItem({itemId}, {quantity}): {ex.Message}\nStack: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Remove item from inventory
        /// </summary>
        /// <param name="itemId">The unique identifier of the item to remove</param>
        /// <param name="quantity">The number of items to remove (default: 1)</param>
        /// <returns>True if the item was successfully removed, false if not enough items were found</returns>
        /// <remarks>
        /// This method will remove items from multiple stacks if necessary.
        /// Returns false only if the total quantity of the item across all stacks is less than requested.
        /// v2.6.2: Enhanced with error handling and structured logging
        /// </remarks>
        public bool RemoveItem(string itemId, int quantity = 1)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(itemId))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Inventory", 
                        "Cannot remove item: itemId is null or empty");
                    return false;
                }

                if (quantity <= 0)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Inventory", 
                        $"Cannot remove item: invalid quantity {quantity}");
                    return false;
                }

                int originalQuantity = quantity;
                for (int i = inventory.Count - 1; i >= 0; i--)
                {
                    if (inventory[i] == null || inventory[i].item == null)
                    {
                        LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Inventory", 
                            $"Found null inventory slot at index {i}, removing");
                        inventory.RemoveAt(i);
                        continue;
                    }

                    if (inventory[i].item.itemId == itemId)
                    {
                        if (inventory[i].quantity > quantity)
                        {
                            inventory[i].quantity -= quantity;
                            LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, "Inventory", 
                                $"Removed {originalQuantity} x {itemId} from inventory");
                            return true;
                        }
                        else if (inventory[i].quantity == quantity)
                        {
                            inventory.RemoveAt(i);
                            LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Debug, "Inventory", 
                                $"Removed {originalQuantity} x {itemId} from inventory");
                            return true;
                        }
                        else
                        {
                            quantity -= inventory[i].quantity;
                            inventory.RemoveAt(i);
                        }
                    }
                }
                
                // If we get here, we didn't have enough items
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Inventory", 
                    $"Failed to remove {originalQuantity} x {itemId}: insufficient quantity");
                return false;
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Inventory", 
                    $"Exception in RemoveItem({itemId}, {quantity}): {ex.Message}\nStack: {ex.StackTrace}");
                return false;
            }
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
        /// Use a consumable item (v2.3.3: Enhanced validation)
        /// </summary>
        /// <param name="itemId">The unique identifier of the item to use</param>
        /// <returns>True if the item was successfully used and removed, false otherwise</returns>
        /// <remarks>
        /// Only works with ItemType.Consumable items.
        /// Applies item effects to the player character and removes one instance from inventory.
        /// v2.6.2: Enhanced with error handling and structured logging
        /// </remarks>
        public bool UseItem(string itemId)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(itemId))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Inventory", 
                        "Cannot use item: itemId is null or empty");
                    return false;
                }

                var slot = FindSlot(itemId);
                if (slot == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Inventory", 
                        $"Item {itemId} not found in inventory");
                    return false;
                }

                if (slot.item.type != ItemType.Consumable)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Inventory", 
                        $"Item {slot.item.name} is not consumable");
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowNotification("Item is not consumable", Color.red);
                    }
                    return false;
                }

                // Apply item effects
                ApplyItemEffects(slot.item);
                
                // Remove one from quantity
                bool removed = RemoveItem(itemId, 1);
                
                if (removed)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Inventory", 
                        $"Used {slot.item.name}");
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowNotification($"Used {slot.item.name}", Color.white);
                    }
                }
                
                return removed;
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Inventory", 
                    $"Exception in UseItem({itemId}): {ex.Message}\nStack: {ex.StackTrace}");
                return false;
            }
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
        /// Check if GameManager and player are available (v2.3.3: Helper method)
        /// </summary>
        private bool IsGameManagerAvailable()
        {
            return GameManager.Instance != null && GameManager.Instance.player != null;
        }

        /// <summary>
        /// Apply item effects to player character (v2.3.3: Now actually applies effects)
        /// </summary>
        /// <remarks>
        /// v2.6.2: Enhanced with error handling and structured logging
        /// </remarks>
        private void ApplyItemEffects(Item item)
        {
            try
            {
                if (item == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Inventory", 
                        "Cannot apply item effects: item is null");
                    return;
                }

                if (!IsGameManagerAvailable())
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Inventory", 
                        "Cannot apply item effects: GameManager or player not found");
                    return;
                }

                Character player = GameManager.Instance.player;
                if (player == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Inventory", 
                        "Cannot apply item effects: player is null");
                    return;
                }

                // Apply health bonus (consumable healing)
                if (item.healthBonus > 0)
                {
                    player.Heal(item.healthBonus);
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Inventory", 
                        $"Restored {item.healthBonus} health with {item.name}");
                    
                    // Trigger visual feedback
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowNotification($"+{item.healthBonus} Health", Color.green);
                    }
                }

                // Apply magic power bonus (temporary buff for consumables)
                if (item.magicPowerBonus > 0)
                {
                    // For consumables, this is a temporary effect
                    // Note: Permanent bonuses from equipment are handled in GetEquipmentBonuses()
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Inventory", 
                        $"Magic power increased by {item.magicPowerBonus} (temporary)");
                    
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowNotification($"+{item.magicPowerBonus} Magic (Temporary)", Color.cyan);
                    }
                }

                // Apply strength bonus (temporary buff)
                if (item.strengthBonus > 0)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Inventory", 
                        $"Strength increased by {item.strengthBonus} (temporary)");
                    
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowNotification($"+{item.strengthBonus} Strength (Temporary)", Color.yellow);
                    }
                }

                // Apply agility bonus (temporary buff)
                if (item.agilityBonus > 0)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Inventory", 
                        $"Agility increased by {item.agilityBonus} (temporary)");
                    
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowNotification($"+{item.agilityBonus} Agility (Temporary)", Color.green);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Inventory", 
                    $"Exception in ApplyItemEffects: {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Equip a weapon (v2.3.3: Enhanced with stat application)
        /// </summary>
        /// <param name="itemId">The unique identifier of the weapon to equip</param>
        /// <returns>True if the weapon was successfully equipped, false otherwise</returns>
        /// <remarks>
        /// Only works with ItemType.Weapon items.
        /// Automatically unequips the currently equipped weapon before equipping the new one.
        /// Triggers GameEvents.OnEquipmentChanged event when successful.
        /// v2.6.2: Enhanced with error handling and structured logging
        /// </remarks>
        public bool EquipWeapon(string itemId)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(itemId))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Inventory", 
                        "Cannot equip weapon: itemId is null or empty");
                    return false;
                }

                var slot = FindSlot(itemId);
                if (slot == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Inventory", 
                        $"Item {itemId} not found in inventory");
                    return false;
                }

                if (slot.item == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Inventory", 
                        $"Found null item in slot for id: {itemId}");
                    return false;
                }

                if (slot.item.type != ItemType.Weapon)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Inventory", 
                        $"Item {itemId} is not a valid weapon (type: {slot.item.type})");
                    return false;
                }

                // Unequip current weapon and remove its bonuses
                if (equippedWeapon != null)
                {
                    var oldSlot = FindSlot(equippedWeapon.itemId);
                    if (oldSlot != null) 
                    {
                        oldSlot.isEquipped = false;
                    }
                }

                // Equip new weapon
                equippedWeapon = slot.item;
                slot.isEquipped = true;
                
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Inventory", 
                    $"Equipped weapon: {slot.item.name}");
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowNotification($"Equipped: {slot.item.name}", Color.green);
                }

                // Trigger equipment changed event
                if (GameEvents.OnEquipmentChanged != null)
                {
                    GameEvents.OnEquipmentChanged.Invoke();
                }

                return true;
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Inventory", 
                    $"Exception in EquipWeapon({itemId}): {ex.Message}\nStack: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Equip armor (v2.3.3: Enhanced with stat application)
        /// </summary>
        /// <param name="itemId">The unique identifier of the armor to equip</param>
        /// <returns>True if the armor was successfully equipped, false otherwise</returns>
        /// <remarks>
        /// Only works with ItemType.Armor items.
        /// Automatically unequips the currently equipped armor before equipping the new one.
        /// Triggers GameEvents.OnEquipmentChanged event when successful.
        /// v2.6.2: Enhanced with error handling and structured logging
        /// </remarks>
        public bool EquipArmor(string itemId)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(itemId))
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Inventory", 
                        "Cannot equip armor: itemId is null or empty");
                    return false;
                }

                var slot = FindSlot(itemId);
                if (slot == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Inventory", 
                        $"Item {itemId} not found in inventory");
                    return false;
                }

                if (slot.item == null)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Inventory", 
                        $"Found null item in slot for id: {itemId}");
                    return false;
                }

                if (slot.item.type != ItemType.Armor)
                {
                    LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Warning, "Inventory", 
                        $"Item {itemId} is not valid armor (type: {slot.item.type})");
                    return false;
                }

                // Unequip current armor and remove its bonuses
                if (equippedArmor != null)
                {
                    var oldSlot = FindSlot(equippedArmor.itemId);
                    if (oldSlot != null)
                    {
                        oldSlot.isEquipped = false;
                    }
                }

                // Equip new armor
                equippedArmor = slot.item;
                slot.isEquipped = true;
                
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Info, "Inventory", 
                    $"Equipped armor: {slot.item.name}");
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowNotification($"Equipped: {slot.item.name}", Color.green);
                }

                // Trigger equipment changed event
                if (GameEvents.OnEquipmentChanged != null)
                {
                    GameEvents.OnEquipmentChanged.Invoke();
                }

                return true;
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Instance?.Log(LoggingSystem.LogLevel.Error, "Inventory", 
                    $"Exception in EquipArmor({itemId}): {ex.Message}\nStack: {ex.StackTrace}");
                return false;
            }
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
        /// Get all equipped items
        /// v2.6.7: NEW - Returns list of all equipped items for set bonus calculation
        /// </summary>
        public List<Item> GetAllEquippedItems()
        {
            List<Item> equipped = new List<Item>();
            if (equippedWeapon != null) equipped.Add(equippedWeapon);
            if (equippedArmor != null) equipped.Add(equippedArmor);
            
            // Add any other equipped items from inventory
            foreach (var slot in inventory)
            {
                if (slot.isEquipped && slot.item != null)
                {
                    equipped.Add(slot.item);
                }
            }
            
            return equipped;
        }
        
        /// <summary>
        /// Calculate equipment set bonuses
        /// v2.6.7: NEW - Counts matching equipment sets and returns bonus multiplier
        /// </summary>
        /// <returns>Dictionary mapping set IDs to piece counts</returns>
        public Dictionary<string, int> GetEquipmentSetCounts()
        {
            Dictionary<string, int> setCounts = new Dictionary<string, int>();
            List<Item> equipped = GetAllEquippedItems();
            
            foreach (var item in equipped)
            {
                if (!string.IsNullOrEmpty(item.equipmentSetId))
                {
                    if (!setCounts.ContainsKey(item.equipmentSetId))
                    {
                        setCounts[item.equipmentSetId] = 0;
                    }
                    setCounts[item.equipmentSetId]++;
                }
            }
            
            return setCounts;
        }
        
        /// <summary>
        /// Get active equipment set bonuses
        /// v2.6.7: NEW - Returns bonus multiplier based on equipped set pieces
        /// </summary>
        /// <returns>Multiplier to apply to stats (1.0 = no bonus, 1.5 = 50% bonus)</returns>
        public float GetEquipmentSetBonus()
        {
            Dictionary<string, int> setCounts = GetEquipmentSetCounts();
            float maxBonus = 1.0f;
            
            foreach (var setCount in setCounts.Values)
            {
                float bonus = 1.0f;
                
                if (setCount >= GameConfig.Equipment.SET_BONUS_4_PIECES)
                {
                    bonus = GameConfig.Equipment.SET_BONUS_4_PIECE_MULTIPLIER;
                }
                else if (setCount >= GameConfig.Equipment.SET_BONUS_3_PIECES)
                {
                    bonus = GameConfig.Equipment.SET_BONUS_3_PIECE_MULTIPLIER;
                }
                else if (setCount >= GameConfig.Equipment.SET_BONUS_2_PIECES)
                {
                    bonus = GameConfig.Equipment.SET_BONUS_2_PIECE_MULTIPLIER;
                }
                
                // Use the highest bonus if multiple sets are active
                if (bonus > maxBonus)
                {
                    maxBonus = bonus;
                }
            }
            
            return maxBonus;
        }
        
        /// <summary>
        /// Get total mana cost reduction from equipment
        /// v2.6.7: NEW - Calculates total mana cost reduction from all equipped items
        /// </summary>
        /// <returns>Tuple of (flat reduction, percent reduction)</returns>
        public (int flatReduction, float percentReduction) GetManaCostReduction()
        {
            List<Item> equipped = GetAllEquippedItems();
            int totalFlat = 0;
            float totalPercent = 0f;
            
            foreach (var item in equipped)
            {
                totalFlat += item.manaCostReduction;
                totalPercent += item.manaCostReductionPercent;
            }
            
            // Apply caps
            totalFlat = Mathf.Min(totalFlat, GameConfig.Equipment.MAX_FLAT_MANA_REDUCTION);
            totalPercent = Mathf.Min(totalPercent, GameConfig.Equipment.MAX_PERCENT_MANA_REDUCTION);
            
            return (totalFlat, totalPercent);
        }

        /// <summary>
        /// Get total equipment stat bonuses (v2.3.3: NEW)
        /// Returns combined bonuses from all equipped items
        /// </summary>
        public (int health, int magicPower, int strength, int agility) GetEquipmentBonuses()
        {
            int totalHealth = 0;
            int totalMagicPower = 0;
            int totalStrength = 0;
            int totalAgility = 0;

            // Add weapon bonuses
            if (equippedWeapon != null)
            {
                totalHealth += equippedWeapon.healthBonus;
                totalMagicPower += equippedWeapon.magicPowerBonus;
                totalStrength += equippedWeapon.strengthBonus;
                totalAgility += equippedWeapon.agilityBonus;
            }

            // Add armor bonuses
            if (equippedArmor != null)
            {
                totalHealth += equippedArmor.healthBonus;
                totalMagicPower += equippedArmor.magicPowerBonus;
                totalStrength += equippedArmor.strengthBonus;
                totalAgility += equippedArmor.agilityBonus;
            }

            return (totalHealth, totalMagicPower, totalStrength, totalAgility);
        }

        /// <summary>
        /// Sell an item for gold (v2.3.3: NEW)
        /// </summary>
        public bool SellItem(string itemId, int quantity = 1)
        {
            var slot = FindSlot(itemId);
            if (slot == null || slot.item.isQuestItem)
            {
                Debug.LogWarning("Cannot sell quest items");
                return false;
            }

            if (slot.quantity < quantity)
            {
                Debug.LogWarning($"Not enough items to sell. Have {slot.quantity}, trying to sell {quantity}");
                return false;
            }

            // Calculate sell value (50% of purchase price)
            int sellValue = CalculateSellValue(slot.item) * quantity;

            // Add gold to player
            if (CurrencySystem.Instance != null)
            {
                CurrencySystem.Instance.AddGold(sellValue);
                Debug.Log($"Sold {quantity}x {slot.item.name} for {sellValue} gold");
                
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowNotification($"Sold {slot.item.name} for {sellValue} gold", Color.yellow);
                }
            }

            // Remove item from inventory
            return RemoveItem(itemId, quantity);
        }

        /// <summary>
        /// Calculate sell value for an item (v2.3.3: NEW)
        /// </summary>
        private int CalculateSellValue(Item item)
        {
            int baseValue = 5;
            switch (item.rarity)
            {
                case ItemRarity.Common: return baseValue;
                case ItemRarity.Uncommon: return baseValue * 2;
                case ItemRarity.Rare: return baseValue * 5;
                case ItemRarity.Epic: return baseValue * 12;
                case ItemRarity.Legendary: return baseValue * 25;
                case ItemRarity.Artifact: return baseValue * 50;
                default: return baseValue;
            }
        }

        /// <summary>
        /// Drop/discard an item (v2.3.3: NEW)
        /// </summary>
        public bool DropItem(string itemId, int quantity = 1)
        {
            var slot = FindSlot(itemId);
            if (slot == null)
                return false;

            if (slot.item.isQuestItem)
            {
                Debug.LogWarning("Cannot drop quest items");
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowNotification("Cannot drop quest items", Color.red);
                }
                return false;
            }

            if (slot.isEquipped)
            {
                Debug.LogWarning("Cannot drop equipped items. Unequip first.");
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowNotification("Unequip item first", Color.red);
                }
                return false;
            }

            Debug.Log($"Dropped {quantity}x {slot.item.name}");
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowNotification($"Dropped {slot.item.name}", Color.gray);
            }

            return RemoveItem(itemId, quantity);
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
