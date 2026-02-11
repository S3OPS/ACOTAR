using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Merchant type determines what they sell
    /// </summary>
    public enum MerchantType
    {
        GeneralGoods,    // Potions, consumables
        Weaponsmith,     // Weapons and weapon upgrades
        Armorer,         // Armor and protective gear
        Enchanter,       // Magical items
        Alchemist,       // Potions and elixirs
        Provisions,      // Food and supplies
        Curiosities      // Rare and unique items
    }

    /// <summary>
    /// Individual merchant NPC data
    /// </summary>
    [System.Serializable]
    public class Merchant
    {
        public string merchantId;
        public string name;
        public MerchantType type;
        public Court court;
        public string location;
        public List<string> inventory; // Item IDs
        public int reputationRequired; // Minimum reputation to access
        public bool isUnlocked;

        public Merchant(string id, string name, MerchantType type, Court court, string location)
        {
            this.merchantId = id;
            this.name = name;
            this.type = type;
            this.court = court;
            this.location = location;
            this.inventory = new List<string>();
            this.reputationRequired = 0;
            this.isUnlocked = true;
        }
    }

    /// <summary>
    /// Manages all merchants in Prythian
    /// Handles inventory, pricing, and availability based on reputation
    /// </summary>
    public class MerchantSystem : MonoBehaviour
    {
        public static MerchantSystem Instance { get; private set; }

        private Dictionary<string, Merchant> merchants;
        private ReputationSystem reputationSystem;
        private CurrencySystem currencySystem;
        private InventorySystem inventorySystem;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeMerchants();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Set required systems
        /// </summary>
        public void Initialize(ReputationSystem reputation, CurrencySystem currency, InventorySystem inventory)
        {
            this.reputationSystem = reputation;
            this.currencySystem = currency;
            this.inventorySystem = inventory;
        }

        /// <summary>
        /// Initialize all merchants in the game
        /// </summary>
        private void InitializeMerchants()
        {
            merchants = new Dictionary<string, Merchant>();

            // Spring Court Merchants
            AddMerchant("spring_general", "Alis", MerchantType.GeneralGoods, Court.Spring, "Spring Court Manor");
            AddMerchantInventory("spring_general", new string[] { 
                "potion_healing", "potion_magic", "food_fae_bread", "potion_stamina", "potion_strength"
            });

            AddMerchant("spring_weaponsmith", "Andras", MerchantType.Weaponsmith, Court.Spring, "Spring Court");
            AddMerchantInventory("spring_weaponsmith", new string[] {
                "weapon_spring_sword", "weapon_hunter_bow", "weapon_ash_dagger"
            });

            // Night Court Merchants
            AddMerchant("night_enchanter", "Clotho", MerchantType.Enchanter, Court.Night, "Velaris");
            AddMerchantInventory("night_enchanter", new string[] {
                "magical_suriel_amulet", "magical_star_pendant", "magical_spring_blessing"
            });

            AddMerchant("night_provisions", "Cerridwen", MerchantType.Provisions, Court.Night, "Velaris");
            AddMerchantInventory("night_provisions", new string[] {
                "potion_greater_healing", "food_starlight_wine", "food_fae_bread"
            });

            // Summer Court Merchants
            AddMerchant("summer_armorer", "Tarquin's Armorer", MerchantType.Armorer, Court.Summer, "Adriata");
            AddMerchantInventory("summer_armorer", new string[] {
                "armor_spring_leather", "armor_fae_cloak", "armor_battle_armor"
            });

            // Day Court Merchants
            AddMerchant("day_alchemist", "Helion's Alchemist", MerchantType.Alchemist, Court.Day, "Day Court");
            AddMerchantInventory("day_alchemist", new string[] {
                "potion_greater_healing", "potion_magic", "potion_stamina", "potion_strength"
            });

            // Traveling Merchant (No court affiliation)
            AddMerchant("traveling_merchant", "The Suriel", MerchantType.Curiosities, Court.None, "Wandering");
            AddMerchantInventory("traveling_merchant", new string[] {
                "magical_suriel_amulet", "weapon_enchanted_spear", "armor_night_silk"
            });

            Debug.Log($"Initialized {merchants.Count} merchants across Prythian");
        }

        /// <summary>
        /// Add a merchant to the system
        /// </summary>
        private void AddMerchant(string id, string name, MerchantType type, Court court, string location)
        {
            Merchant merchant = new Merchant(id, name, type, court, location);
            merchants[id] = merchant;
        }

        /// <summary>
        /// Set merchant's inventory
        /// </summary>
        private void AddMerchantInventory(string merchantId, string[] itemIds)
        {
            if (merchants.ContainsKey(merchantId))
            {
                merchants[merchantId].inventory.AddRange(itemIds);
            }
        }

        /// <summary>
        /// Get a merchant by ID
        /// </summary>
        public Merchant GetMerchant(string merchantId)
        {
            if (merchants.ContainsKey(merchantId))
            {
                return merchants[merchantId];
            }
            return null;
        }

        /// <summary>
        /// Get all merchants at a location
        /// </summary>
        public List<Merchant> GetMerchantsAtLocation(string location)
        {
            List<Merchant> result = new List<Merchant>();
            foreach (Merchant merchant in merchants.Values)
            {
                if (merchant.location == location && CanAccessMerchant(merchant))
                {
                    result.Add(merchant);
                }
            }
            return result;
        }

        /// <summary>
        /// Check if player can access a merchant
        /// </summary>
        public bool CanAccessMerchant(Merchant merchant)
        {
            if (!merchant.isUnlocked)
                return false;

            if (merchant.court == Court.None)
                return true; // Traveling merchants always accessible

            if (reputationSystem == null)
                return true; // If no reputation system, allow access

            int reputation = reputationSystem.GetReputation(merchant.court.ToString());
            return reputation >= merchant.reputationRequired;
        }

        /// <summary>
        /// Calculate final price for an item at a merchant
        /// </summary>
        public int CalculatePrice(Item item, Merchant merchant, bool isSelling)
        {
            int basePrice = item.type == ItemType.Consumable ? 50 : 100;
            
            // Adjust by rarity
            switch (item.rarity)
            {
                case ItemRarity.Common:
                    basePrice *= 1;
                    break;
                case ItemRarity.Uncommon:
                    basePrice *= 2;
                    break;
                case ItemRarity.Rare:
                    basePrice *= 5;
                    break;
                case ItemRarity.Epic:
                    basePrice *= 10;
                    break;
                case ItemRarity.Legendary:
                    basePrice *= 25;
                    break;
                case ItemRarity.Artifact:
                    basePrice *= 50;
                    break;
            }

            // Apply reputation discount for buying (or penalty for selling)
            if (reputationSystem != null && merchant.court != Court.None)
            {
                float priceMultiplier = reputationSystem.GetPriceMultiplier(merchant.court.ToString());
                
                if (isSelling)
                {
                    // When selling TO merchant, player gets less if bad reputation
                    basePrice = Mathf.RoundToInt(basePrice * (2f - priceMultiplier));
                }
                else
                {
                    // When buying FROM merchant, player pays less with good reputation
                    basePrice = Mathf.RoundToInt(basePrice * priceMultiplier);
                }
            }

            return Mathf.Max(1, basePrice); // Never go below 1 gold
        }

        /// <summary>
        /// Purchase an item from a merchant
        /// </summary>
        public bool PurchaseItem(string merchantId, string itemId)
        {
            Merchant merchant = GetMerchant(merchantId);
            if (merchant == null)
            {
                Debug.LogWarning($"Merchant {merchantId} not found");
                return false;
            }

            if (!merchant.inventory.Contains(itemId))
            {
                Debug.LogWarning($"Merchant {merchant.name} doesn't sell {itemId}");
                return false;
            }

            // Get item from inventory system
            Item item = inventorySystem.GetItemFromDatabase(itemId);
            if (item == null)
            {
                Debug.LogWarning($"Item {itemId} not found in database");
                return false;
            }

            int price = CalculatePrice(item, merchant, false);

            // Check if player can afford it
            if (currencySystem == null || currencySystem.Gold < price)
            {
                Debug.LogWarning($"Cannot afford {item.name}! Need {price} gold, have {currencySystem?.Gold ?? 0}");
                return false;
            }

            // Complete the purchase
            if (currencySystem.SpendGold(price))
            {
                inventorySystem.AddItem(itemId, 1);
                Debug.Log($"Purchased {item.name} from {merchant.name} for {price} gold");
                
                // Trigger purchase event
                GameEvents.TriggerItemPurchased(itemId, price, merchant.name);
                
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sell an item to a merchant
        /// </summary>
        public bool SellItem(string merchantId, string itemId)
        {
            Merchant merchant = GetMerchant(merchantId);
            if (merchant == null)
            {
                Debug.LogWarning($"Merchant {merchantId} not found");
                return false;
            }

            // Check if player has the item
            if (!inventorySystem.HasItem(itemId, 1))
            {
                Debug.LogWarning($"Player doesn't have {itemId} to sell");
                return false;
            }

            // Get item from inventory system
            Item item = inventorySystem.GetItemFromDatabase(itemId);
            if (item == null)
            {
                Debug.LogWarning($"Item {itemId} not found in database");
                return false;
            }

            // Quest items cannot be sold
            if (item.isQuestItem)
            {
                Debug.LogWarning($"{item.name} is a quest item and cannot be sold");
                return false;
            }

            int sellPrice = CalculatePrice(item, merchant, true) / 2; // Merchants buy for 50% of sell price

            // Complete the sale
            if (inventorySystem.RemoveItem(itemId, 1))
            {
                currencySystem.AddGold(sellPrice);
                Debug.Log($"Sold {item.name} to {merchant.name} for {sellPrice} gold");
                
                // Trigger sale event
                GameEvents.TriggerItemSold(itemId, sellPrice, merchant.name);
                
                return true;
            }

            return false;
        }

        /// <summary>
        /// Unlock a merchant (for story progression)
        /// </summary>
        public void UnlockMerchant(string merchantId)
        {
            if (merchants.ContainsKey(merchantId))
            {
                merchants[merchantId].isUnlocked = true;
                Debug.Log($"Unlocked merchant: {merchants[merchantId].name}");
            }
        }

        /// <summary>
        /// Get all available merchants
        /// </summary>
        public List<Merchant> GetAllMerchants()
        {
            List<Merchant> result = new List<Merchant>();
            foreach (Merchant merchant in merchants.Values)
            {
                if (CanAccessMerchant(merchant))
                {
                    result.Add(merchant);
                }
            }
            return result;
        }
    }
}
