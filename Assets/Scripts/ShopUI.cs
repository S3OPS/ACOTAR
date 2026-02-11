using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ACOTAR
{
    /// <summary>
    /// Shop UI for buying and selling items from merchants
    /// Integrates with reputation system for dynamic pricing
    /// </summary>
    public class ShopUI : MonoBehaviour
    {
        [Header("Shop Panel")]
        public GameObject shopPanel;
        public Text merchantNameText;
        public Text merchantTypeText;
        public Text playerGoldText;

        [Header("Merchant Inventory")]
        public Transform merchantInventoryContainer;
        public GameObject shopItemSlotPrefab;

        [Header("Player Inventory")]
        public Transform playerInventoryContainer;
        public GameObject playerItemSlotPrefab;

        [Header("Item Details")]
        public GameObject itemDetailsPanel;
        public Text itemNameText;
        public Text itemDescriptionText;
        public Text itemPriceText;
        public Button buyButton;
        public Button sellButton;
        public Text reputationDiscountText;

        private MerchantSystem merchantSystem;
        private InventorySystem inventorySystem;
        private CurrencySystem currencySystem;
        private Merchant currentMerchant;
        private Item selectedItem;
        private bool isPlayerInventory; // True if selecting from player inventory to sell

        void Start()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(false);
            }

            if (itemDetailsPanel != null)
            {
                itemDetailsPanel.SetActive(false);
            }

            SetupEventListeners();
        }

        /// <summary>
        /// Setup button listeners
        /// </summary>
        private void SetupEventListeners()
        {
            if (buyButton != null)
            {
                buyButton.onClick.AddListener(OnBuyClicked);
            }

            if (sellButton != null)
            {
                sellButton.onClick.AddListener(OnSellClicked);
            }
        }

        /// <summary>
        /// Initialize systems
        /// </summary>
        public void Initialize(MerchantSystem merchant, InventorySystem inventory, CurrencySystem currency)
        {
            this.merchantSystem = merchant;
            this.inventorySystem = inventory;
            this.currencySystem = currency;
        }

        /// <summary>
        /// Open shop with specific merchant
        /// </summary>
        public void OpenShop(string merchantId)
        {
            if (merchantSystem == null)
            {
                Debug.LogWarning("MerchantSystem not initialized!");
                return;
            }

            currentMerchant = merchantSystem.GetMerchant(merchantId);
            if (currentMerchant == null)
            {
                Debug.LogWarning($"Merchant {merchantId} not found!");
                return;
            }

            if (!merchantSystem.CanAccessMerchant(currentMerchant))
            {
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowNotification($"You need better reputation with {currentMerchant.court} Court to access this merchant.", 4f);
                }
                return;
            }

            if (shopPanel != null)
            {
                shopPanel.SetActive(true);
            }

            RefreshShop();
        }

        /// <summary>
        /// Close the shop
        /// </summary>
        public void CloseShop()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(false);
            }

            if (itemDetailsPanel != null)
            {
                itemDetailsPanel.SetActive(false);
            }

            currentMerchant = null;
            selectedItem = null;
        }

        /// <summary>
        /// Refresh all shop displays
        /// </summary>
        private void RefreshShop()
        {
            if (currentMerchant == null)
                return;

            // Update merchant info
            if (merchantNameText != null)
            {
                merchantNameText.text = currentMerchant.name;
            }

            if (merchantTypeText != null)
            {
                merchantTypeText.text = $"{currentMerchant.type} - {currentMerchant.location}";
            }

            // Update player gold
            if (playerGoldText != null && currencySystem != null)
            {
                playerGoldText.text = $"Gold: {currencySystem.Gold}";
            }

            // Refresh inventories
            RefreshMerchantInventory();
            RefreshPlayerInventory();

            // Hide item details
            if (itemDetailsPanel != null)
            {
                itemDetailsPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Refresh merchant's inventory display
        /// </summary>
        private void RefreshMerchantInventory()
        {
            if (merchantInventoryContainer == null || shopItemSlotPrefab == null)
                return;

            // Clear existing slots
            foreach (Transform child in merchantInventoryContainer)
            {
                Destroy(child.gameObject);
            }

            if (currentMerchant == null || inventorySystem == null)
                return;

            // Create slots for each item
            foreach (string itemId in currentMerchant.inventory)
            {
                Item item = inventorySystem.GetItemFromDatabase(itemId);
                if (item != null)
                {
                    CreateMerchantItemSlot(item);
                }
            }
        }

        /// <summary>
        /// Create a slot for a merchant item
        /// </summary>
        private void CreateMerchantItemSlot(Item item)
        {
            GameObject slot = Instantiate(shopItemSlotPrefab, merchantInventoryContainer);

            // Set item icon
            Image iconImage = slot.transform.Find("ItemIcon")?.GetComponent<Image>();
            if (iconImage != null)
            {
                // Would set sprite here if we had item sprites
                iconImage.color = GetRarityColor(item.rarity);
            }

            // Set item name
            Text nameText = slot.transform.Find("ItemName")?.GetComponent<Text>();
            if (nameText != null)
            {
                nameText.text = item.name;
            }

            // Set price
            int price = merchantSystem.CalculatePrice(item, currentMerchant, false);
            Text priceText = slot.transform.Find("PriceText")?.GetComponent<Text>();
            if (priceText != null)
            {
                priceText.text = $"{price} Gold";
            }

            // Add click listener
            Button slotButton = slot.GetComponent<Button>();
            if (slotButton != null)
            {
                slotButton.onClick.AddListener(() => OnMerchantItemClicked(item));
            }
        }

        /// <summary>
        /// Refresh player's inventory display
        /// </summary>
        private void RefreshPlayerInventory()
        {
            if (playerInventoryContainer == null || playerItemSlotPrefab == null)
                return;

            // Clear existing slots
            foreach (Transform child in playerInventoryContainer)
            {
                Destroy(child.gameObject);
            }

            if (inventorySystem == null)
                return;

            // Create slots for each item
            List<InventoryItem> items = inventorySystem.GetAllItems();
            foreach (InventoryItem invItem in items)
            {
                Item item = inventorySystem.GetItemFromDatabase(invItem.itemId);
                if (item != null && !item.isQuestItem)
                {
                    CreatePlayerItemSlot(item, invItem.quantity);
                }
            }
        }

        /// <summary>
        /// Create a slot for a player item
        /// </summary>
        private void CreatePlayerItemSlot(Item item, int quantity)
        {
            GameObject slot = Instantiate(playerItemSlotPrefab, playerInventoryContainer);

            // Set item icon
            Image iconImage = slot.transform.Find("ItemIcon")?.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.color = GetRarityColor(item.rarity);
            }

            // Set item name
            Text nameText = slot.transform.Find("ItemName")?.GetComponent<Text>();
            if (nameText != null)
            {
                nameText.text = $"{item.name} x{quantity}";
            }

            // Set sell price
            int sellPrice = merchantSystem.CalculatePrice(item, currentMerchant, true) / 2;
            Text priceText = slot.transform.Find("PriceText")?.GetComponent<Text>();
            if (priceText != null)
            {
                priceText.text = $"{sellPrice} Gold";
            }

            // Add click listener
            Button slotButton = slot.GetComponent<Button>();
            if (slotButton != null)
            {
                slotButton.onClick.AddListener(() => OnPlayerItemClicked(item));
            }
        }

        /// <summary>
        /// Handle merchant item clicked
        /// </summary>
        private void OnMerchantItemClicked(Item item)
        {
            selectedItem = item;
            isPlayerInventory = false;
            ShowItemDetails(item, false);
        }

        /// <summary>
        /// Handle player item clicked
        /// </summary>
        private void OnPlayerItemClicked(Item item)
        {
            selectedItem = item;
            isPlayerInventory = true;
            ShowItemDetails(item, true);
        }

        /// <summary>
        /// Show item details panel
        /// </summary>
        private void ShowItemDetails(Item item, bool isSelling)
        {
            if (itemDetailsPanel == null)
                return;

            itemDetailsPanel.SetActive(true);

            // Set item name
            if (itemNameText != null)
            {
                itemNameText.text = GetColoredItemName(item);
            }

            // Set item description
            if (itemDescriptionText != null)
            {
                string description = item.description;
                description += $"\n\nType: {item.type}";
                description += $"\nRarity: {item.rarity}";
                
                if (item.healthBonus > 0)
                    description += $"\n+{item.healthBonus} Health";
                if (item.magicPowerBonus > 0)
                    description += $"\n+{item.magicPowerBonus} Magic Power";
                if (item.strengthBonus > 0)
                    description += $"\n+{item.strengthBonus} Strength";
                if (item.agilityBonus > 0)
                    description += $"\n+{item.agilityBonus} Agility";

                itemDescriptionText.text = description;
            }

            // Set price
            int price;
            if (isSelling)
            {
                price = merchantSystem.CalculatePrice(item, currentMerchant, true) / 2;
            }
            else
            {
                price = merchantSystem.CalculatePrice(item, currentMerchant, false);
            }

            if (itemPriceText != null)
            {
                itemPriceText.text = isSelling ? $"Sell for: {price} Gold" : $"Buy for: {price} Gold";
            }

            // Show reputation discount info
            if (reputationDiscountText != null && currentMerchant.court != Court.None)
            {
                ReputationSystem repSystem = GameManager.Instance?.GetComponent<ReputationSystem>();
                if (repSystem != null)
                {
                    float multiplier = repSystem.GetPriceMultiplier(currentMerchant.court.ToString());
                    int discount = Mathf.RoundToInt((1f - multiplier) * 100);
                    
                    if (discount > 0)
                    {
                        reputationDiscountText.text = $"<color=green>{discount}% Reputation Discount</color>";
                    }
                    else if (discount < 0)
                    {
                        reputationDiscountText.text = $"<color=red>{Mathf.Abs(discount)}% Reputation Penalty</color>";
                    }
                    else
                    {
                        reputationDiscountText.text = "No reputation modifier";
                    }
                }
            }

            // Enable/disable buttons
            if (buyButton != null)
            {
                buyButton.gameObject.SetActive(!isSelling);
                buyButton.interactable = currencySystem != null && currencySystem.Gold >= price;
            }

            if (sellButton != null)
            {
                sellButton.gameObject.SetActive(isSelling);
                sellButton.interactable = true;
            }
        }

        /// <summary>
        /// Handle buy button clicked
        /// </summary>
        private void OnBuyClicked()
        {
            if (selectedItem == null || currentMerchant == null || isPlayerInventory)
                return;

            bool success = merchantSystem.PurchaseItem(currentMerchant.merchantId, selectedItem.itemId);

            if (success)
            {
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowNotification($"Purchased {selectedItem.name}!", 2f);
                }
                RefreshShop();
            }
            else
            {
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowNotification("Cannot complete purchase!", 2f);
                }
            }
        }

        /// <summary>
        /// Handle sell button clicked
        /// </summary>
        private void OnSellClicked()
        {
            if (selectedItem == null || currentMerchant == null || !isPlayerInventory)
                return;

            bool success = merchantSystem.SellItem(currentMerchant.merchantId, selectedItem.itemId);

            if (success)
            {
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowNotification($"Sold {selectedItem.name}!", 2f);
                }
                RefreshShop();
            }
            else
            {
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowNotification("Cannot complete sale!", 2f);
                }
            }
        }

        /// <summary>
        /// Get colored item name by rarity
        /// </summary>
        private string GetColoredItemName(Item item)
        {
            string colorCode = GetRarityColorCode(item.rarity);
            return $"<color={colorCode}>{item.name}</color>";
        }

        /// <summary>
        /// Get color code for item rarity
        /// </summary>
        private string GetRarityColorCode(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common:
                    return "#FFFFFF";
                case ItemRarity.Uncommon:
                    return "#00FF00";
                case ItemRarity.Rare:
                    return "#0080FF";
                case ItemRarity.Epic:
                    return "#A020F0";
                case ItemRarity.Legendary:
                    return "#FF8000";
                case ItemRarity.Artifact:
                    return "#FFD700";
                default:
                    return "#FFFFFF";
            }
        }

        /// <summary>
        /// Get color for item rarity
        /// </summary>
        private Color GetRarityColor(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common:
                    return Color.white;
                case ItemRarity.Uncommon:
                    return Color.green;
                case ItemRarity.Rare:
                    return Color.blue;
                case ItemRarity.Epic:
                    return new Color(0.63f, 0.13f, 0.94f); // Purple
                case ItemRarity.Legendary:
                    return new Color(1f, 0.5f, 0f); // Orange
                case ItemRarity.Artifact:
                    return new Color(1f, 0.84f, 0f); // Gold
                default:
                    return Color.white;
            }
        }
    }
}
