using System;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Currency types in Prythian
    /// </summary>
    public enum CurrencyType
    {
        Gold,           // Standard currency
        FaeCrystals,    // Premium/rare currency
        CourtTokens     // Court-specific currency
    }

    /// <summary>
    /// Manages player currencies and transactions
    /// </summary>
    public class CurrencySystem
    {
        private int gold;
        private int faeCrystals;
        private int[] courtTokens;

        // Events
        public event Action<CurrencyType, int, int> OnCurrencyChanged; // type, oldAmount, newAmount
        public event Action<string, int> OnPurchase; // itemName, cost

        public CurrencySystem()
        {
            gold = GameConfig.CurrencySettings.STARTING_GOLD;
            faeCrystals = 0;
            courtTokens = new int[7]; // One for each court (excluding None)
        }

        /// <summary>
        /// Get current gold amount
        /// </summary>
        public int Gold
        {
            get { return gold; }
        }

        /// <summary>
        /// Get current fae crystals amount
        /// </summary>
        public int FaeCrystals
        {
            get { return faeCrystals; }
        }

        /// <summary>
        /// Add gold to player's wallet
        /// </summary>
        public void AddGold(int amount)
        {
            if (amount <= 0) return;

            int oldAmount = gold;
            gold += amount;
            Debug.Log($"+{amount} Gold (Total: {gold})");
            OnCurrencyChanged?.Invoke(CurrencyType.Gold, oldAmount, gold);
        }

        /// <summary>
        /// Remove gold from player's wallet
        /// Returns true if successful, false if insufficient funds
        /// </summary>
        public bool SpendGold(int amount)
        {
            if (amount <= 0) return false;
            if (gold < amount)
            {
                Debug.LogWarning($"Insufficient gold! Need {amount}, have {gold}");
                return false;
            }

            int oldAmount = gold;
            gold -= amount;
            Debug.Log($"-{amount} Gold (Remaining: {gold})");
            OnCurrencyChanged?.Invoke(CurrencyType.Gold, oldAmount, gold);
            return true;
        }

        /// <summary>
        /// Check if player can afford an amount
        /// </summary>
        public bool CanAfford(int amount)
        {
            return gold >= amount;
        }

        /// <summary>
        /// Add fae crystals
        /// </summary>
        public void AddFaeCrystals(int amount)
        {
            if (amount <= 0) return;

            int oldAmount = faeCrystals;
            faeCrystals += amount;
            Debug.Log($"+{amount} Fae Crystals (Total: {faeCrystals})");
            OnCurrencyChanged?.Invoke(CurrencyType.FaeCrystals, oldAmount, faeCrystals);
        }

        /// <summary>
        /// Spend fae crystals
        /// </summary>
        public bool SpendFaeCrystals(int amount)
        {
            if (amount <= 0) return false;
            if (faeCrystals < amount)
            {
                Debug.LogWarning($"Insufficient Fae Crystals! Need {amount}, have {faeCrystals}");
                return false;
            }

            int oldAmount = faeCrystals;
            faeCrystals -= amount;
            Debug.Log($"-{amount} Fae Crystals (Remaining: {faeCrystals})");
            OnCurrencyChanged?.Invoke(CurrencyType.FaeCrystals, oldAmount, faeCrystals);
            return true;
        }

        /// <summary>
        /// Get court token count for a specific court
        /// </summary>
        public int GetCourtTokens(Court court)
        {
            if (court == Court.None) return 0;
            int index = (int)court - 1; // Skip None enum
            if (index >= 0 && index < courtTokens.Length)
            {
                return courtTokens[index];
            }
            return 0;
        }

        /// <summary>
        /// Add court tokens for a specific court
        /// </summary>
        public void AddCourtTokens(Court court, int amount)
        {
            if (court == Court.None || amount <= 0) return;

            int index = (int)court - 1;
            if (index >= 0 && index < courtTokens.Length)
            {
                int oldAmount = courtTokens[index];
                courtTokens[index] += amount;
                Debug.Log($"+{amount} {court} Court Tokens (Total: {courtTokens[index]})");
                OnCurrencyChanged?.Invoke(CurrencyType.CourtTokens, oldAmount, courtTokens[index]);
            }
        }

        /// <summary>
        /// Spend court tokens for a specific court
        /// </summary>
        public bool SpendCourtTokens(Court court, int amount)
        {
            if (court == Court.None || amount <= 0) return false;

            int index = (int)court - 1;
            if (index >= 0 && index < courtTokens.Length)
            {
                if (courtTokens[index] < amount)
                {
                    Debug.LogWarning($"Insufficient {court} Court Tokens! Need {amount}, have {courtTokens[index]}");
                    return false;
                }

                int oldAmount = courtTokens[index];
                courtTokens[index] -= amount;
                Debug.Log($"-{amount} {court} Court Tokens (Remaining: {courtTokens[index]})");
                OnCurrencyChanged?.Invoke(CurrencyType.CourtTokens, oldAmount, courtTokens[index]);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculate gold from enemy based on difficulty and enemy type
        /// </summary>
        public static int CalculateEnemyGoldDrop(Enemy enemy)
        {
            int baseGold = 10;

            // Scale by difficulty tier
            switch (enemy.difficulty)
            {
                case EnemyDifficulty.Trivial:
                    baseGold = 5;
                    break;
                case EnemyDifficulty.Easy:
                    baseGold = 10;
                    break;
                case EnemyDifficulty.Normal:
                    baseGold = 25;
                    break;
                case EnemyDifficulty.Hard:
                    baseGold = 50;
                    break;
                case EnemyDifficulty.Elite:
                    baseGold = 100;
                    break;
                case EnemyDifficulty.Boss:
                    baseGold = 500;
                    break;
            }

            // Scale by enemy level
            baseGold *= enemy.level;

            // Apply difficulty setting multiplier
            baseGold = Mathf.RoundToInt(baseGold * DifficultySettings.GetGoldMultiplier());

            // Add some variance (80-120%)
            float variance = UnityEngine.Random.Range(0.8f, 1.2f);
            baseGold = Mathf.RoundToInt(baseGold * variance);

            return Mathf.Max(1, baseGold);
        }

        /// <summary>
        /// Calculate sell price for an item
        /// </summary>
        public static int CalculateSellPrice(Item item, ReputationSystem reputation = null, Court shopCourt = Court.None)
        {
            int basePrice = GetItemBasePrice(item);

            // Rarity multiplier
            float rarityMult = GetRarityMultiplier(item.rarity);
            basePrice = Mathf.RoundToInt(basePrice * rarityMult);

            // Apply reputation discount if available
            if (reputation != null && shopCourt != Court.None)
            {
                float reputationMult = reputation.GetCourtPriceMultiplier(shopCourt);
                basePrice = Mathf.RoundToInt(basePrice * reputationMult);
            }

            // Sell price is 50% of buy price
            return Mathf.Max(1, basePrice / 2);
        }

        /// <summary>
        /// Calculate buy price for an item
        /// </summary>
        public static int CalculateBuyPrice(Item item, ReputationSystem reputation = null, Court shopCourt = Court.None)
        {
            int basePrice = GetItemBasePrice(item);

            // Rarity multiplier
            float rarityMult = GetRarityMultiplier(item.rarity);
            basePrice = Mathf.RoundToInt(basePrice * rarityMult);

            // Apply reputation discount if available
            if (reputation != null && shopCourt != Court.None)
            {
                float reputationMult = reputation.GetCourtPriceMultiplier(shopCourt);
                basePrice = Mathf.RoundToInt(basePrice * reputationMult);
            }

            return Mathf.Max(1, basePrice);
        }

        /// <summary>
        /// Get base price for an item based on type
        /// </summary>
        private static int GetItemBasePrice(Item item)
        {
            switch (item.type)
            {
                case ItemType.Weapon:
                    return 50 + (item.strengthBonus * 5);
                case ItemType.Armor:
                    return 40 + (item.healthBonus / 2);
                case ItemType.Consumable:
                    return 10 + (item.healthBonus / 5) + (item.magicPowerBonus * 2);
                case ItemType.Crafting:
                    return 5;
                case ItemType.Magical:
                    return 100 + (item.magicPowerBonus * 10);
                case ItemType.QuestItem:
                    return 0; // Quest items can't be sold
                default:
                    return 10;
            }
        }

        /// <summary>
        /// Get price multiplier based on rarity
        /// </summary>
        private static float GetRarityMultiplier(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common:
                    return 1.0f;
                case ItemRarity.Uncommon:
                    return 1.5f;
                case ItemRarity.Rare:
                    return 3.0f;
                case ItemRarity.Epic:
                    return 6.0f;
                case ItemRarity.Legendary:
                    return 15.0f;
                case ItemRarity.Artifact:
                    return 50.0f;
                default:
                    return 1.0f;
            }
        }

        /// <summary>
        /// Purchase an item from a shop
        /// </summary>
        public bool PurchaseItem(Item item, InventorySystem inventory, ReputationSystem reputation = null, Court shopCourt = Court.None)
        {
            int price = CalculateBuyPrice(item, reputation, shopCourt);

            if (!CanAfford(price))
            {
                Debug.LogWarning($"Cannot afford {item.name}! Price: {price}, Gold: {gold}");
                return false;
            }

            if (SpendGold(price))
            {
                inventory.AddItem(item.itemId);
                Debug.Log($"Purchased {item.name} for {price} gold!");
                OnPurchase?.Invoke(item.name, price);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sell an item to a shop
        /// </summary>
        public bool SellItem(string itemId, InventorySystem inventory, ReputationSystem reputation = null, Court shopCourt = Court.None)
        {
            Item item = inventory.GetItemFromDatabase(itemId);
            if (item == null)
            {
                Debug.LogWarning($"Item not found: {itemId}");
                return false;
            }

            if (item.isQuestItem)
            {
                Debug.LogWarning("Cannot sell quest items!");
                return false;
            }

            if (!inventory.HasItem(itemId))
            {
                Debug.LogWarning($"You don't have {item.name} to sell!");
                return false;
            }

            int sellPrice = CalculateSellPrice(item, reputation, shopCourt);

            if (inventory.RemoveItem(itemId))
            {
                AddGold(sellPrice);
                Debug.Log($"Sold {item.name} for {sellPrice} gold!");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Display current currency amounts
        /// </summary>
        public void DisplayCurrency()
        {
            Debug.Log("\n=== Currency ===");
            Debug.Log($"Gold: {gold}");
            Debug.Log($"Fae Crystals: {faeCrystals}");
            
            Debug.Log("\nCourt Tokens:");
            Debug.Log($"  Spring: {GetCourtTokens(Court.Spring)}");
            Debug.Log($"  Summer: {GetCourtTokens(Court.Summer)}");
            Debug.Log($"  Autumn: {GetCourtTokens(Court.Autumn)}");
            Debug.Log($"  Winter: {GetCourtTokens(Court.Winter)}");
            Debug.Log($"  Night: {GetCourtTokens(Court.Night)}");
            Debug.Log($"  Dawn: {GetCourtTokens(Court.Dawn)}");
            Debug.Log($"  Day: {GetCourtTokens(Court.Day)}");
            Debug.Log("================\n");
        }
    }
}
