using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace ACOTAR
{
    /// <summary>
    /// Manages inventory UI display and interactions
    /// Handles item display, equipment, and drag-drop functionality
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("Inventory Grid")]
        public GameObject inventoryGrid;
        public GameObject itemSlotPrefab;
        
        [Header("Equipment Slots")]
        public GameObject weaponSlot;
        public GameObject armorSlot;

        [Header("Item Details")]
        public GameObject itemDetailsPanel;
        public Text itemNameText;
        public Text itemDescriptionText;
        public Text itemStatsText;
        public Button useItemButton;
        public Button dropItemButton;
        public Button equipItemButton;

        [Header("Sorting")]
        public Dropdown sortDropdown;
        public Button sortButton;

        private InventorySystem inventorySystem;
        private List<GameObject> itemSlots = new List<GameObject>();
        private InventoryItem selectedItem;

        void Start()
        {
            InitializeInventory();
            SetupEventListeners();
        }

        /// <summary>
        /// Initialize inventory UI
        /// </summary>
        private void InitializeInventory()
        {
            // Get inventory system from GameManager
            if (GameManager.Instance != null)
            {
                // inventorySystem will be set when GameManager initializes
                Debug.Log("InventoryUI initialized");
            }

            // Hide item details initially
            if (itemDetailsPanel != null)
            {
                itemDetailsPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Setup button event listeners
        /// </summary>
        private void SetupEventListeners()
        {
            if (useItemButton != null)
            {
                useItemButton.onClick.AddListener(OnUseItemClicked);
            }

            if (dropItemButton != null)
            {
                dropItemButton.onClick.AddListener(OnDropItemClicked);
            }

            if (equipItemButton != null)
            {
                equipItemButton.onClick.AddListener(OnEquipItemClicked);
            }

            if (sortButton != null)
            {
                sortButton.onClick.AddListener(OnSortClicked);
            }
        }

        /// <summary>
        /// Refresh inventory display
        /// </summary>
        public void RefreshInventory(InventorySystem inventory)
        {
            if (inventory == null || inventoryGrid == null)
                return;

            inventorySystem = inventory;

            // Clear existing slots
            ClearInventorySlots();

            // Create slots for each item
            List<InventoryItem> items = inventory.GetAllItems();
            foreach (InventoryItem item in items)
            {
                CreateItemSlot(item);
            }

            Debug.Log($"Inventory refreshed: {items.Count} items displayed");
        }

        /// <summary>
        /// Clear all inventory slots
        /// </summary>
        private void ClearInventorySlots()
        {
            foreach (GameObject slot in itemSlots)
            {
                if (slot != null)
                {
                    Destroy(slot);
                }
            }
            itemSlots.Clear();
        }

        /// <summary>
        /// Create a visual slot for an item
        /// </summary>
        private void CreateItemSlot(InventoryItem item)
        {
            if (itemSlotPrefab == null || inventoryGrid == null)
                return;

            GameObject slot = Instantiate(itemSlotPrefab, inventoryGrid.transform);
            
            // Setup slot visuals
            Text itemNameText = slot.GetComponentInChildren<Text>();
            if (itemNameText != null)
            {
                itemNameText.text = item.quantity > 1 ? 
                    $"{item.name} x{item.quantity}" : 
                    item.name;
            }

            // Add click listener to show item details
            Button slotButton = slot.GetComponent<Button>();
            if (slotButton != null)
            {
                slotButton.onClick.AddListener(() => ShowItemDetails(item));
            }

            // Add rarity color
            Image slotImage = slot.GetComponent<Image>();
            if (slotImage != null)
            {
                slotImage.color = GetRarityColor(item.rarity);
            }

            itemSlots.Add(slot);
        }

        /// <summary>
        /// Get color based on item rarity
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
                    return new Color(0.6f, 0f, 1f); // Purple
                case ItemRarity.Legendary:
                    return new Color(1f, 0.5f, 0f); // Orange
                case ItemRarity.Artifact:
                    return Color.red;
                default:
                    return Color.white;
            }
        }

        /// <summary>
        /// Show detailed information about an item
        /// </summary>
        private void ShowItemDetails(InventoryItem item)
        {
            selectedItem = item;

            if (itemDetailsPanel != null)
            {
                itemDetailsPanel.SetActive(true);
            }

            if (itemNameText != null)
            {
                itemNameText.text = item.name;
            }

            if (itemDescriptionText != null)
            {
                itemDescriptionText.text = item.description;
            }

            if (itemStatsText != null)
            {
                string stats = $"Type: {item.itemType}\n";
                stats += $"Rarity: {item.rarity}\n";
                stats += $"Value: {item.value} gold\n";
                
                if (item.itemType == ItemType.Weapon || item.itemType == ItemType.Armor)
                {
                    stats += $"Power: +{item.power}\n";
                }
                
                if (item.quantity > 1)
                {
                    stats += $"Quantity: {item.quantity}";
                }

                itemStatsText.text = stats;
            }

            // Show/hide buttons based on item type
            UpdateItemButtons(item);

            Debug.Log($"Showing details for: {item.name}");
        }

        /// <summary>
        /// Update which buttons are available for the item
        /// </summary>
        private void UpdateItemButtons(InventoryItem item)
        {
            if (useItemButton != null)
            {
                useItemButton.gameObject.SetActive(item.itemType == ItemType.Consumable);
            }

            if (equipItemButton != null)
            {
                bool canEquip = item.itemType == ItemType.Weapon || item.itemType == ItemType.Armor;
                equipItemButton.gameObject.SetActive(canEquip);
            }

            if (dropItemButton != null)
            {
                // Can drop anything except quest items
                dropItemButton.gameObject.SetActive(item.itemType != ItemType.QuestItem);
            }
        }

        /// <summary>
        /// Use selected item
        /// </summary>
        private void OnUseItemClicked()
        {
            if (selectedItem == null || inventorySystem == null)
                return;

            if (selectedItem.itemType == ItemType.Consumable)
            {
                bool used = inventorySystem.UseItem(selectedItem.itemId);
                
                if (used)
                {
                    Debug.Log($"Used item: {selectedItem.name}");
                    
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowNotification($"Used {selectedItem.name}");
                    }

                    // Refresh display
                    RefreshInventory(inventorySystem);
                    
                    // Hide details if item is gone
                    if (inventorySystem.GetItemCount(selectedItem.itemId) == 0)
                    {
                        if (itemDetailsPanel != null)
                        {
                            itemDetailsPanel.SetActive(false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Drop selected item
        /// </summary>
        private void OnDropItemClicked()
        {
            if (selectedItem == null || inventorySystem == null)
                return;

            if (selectedItem.itemType != ItemType.QuestItem)
            {
                bool removed = inventorySystem.RemoveItem(selectedItem.itemId, 1);
                
                if (removed)
                {
                    Debug.Log($"Dropped item: {selectedItem.name}");
                    
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowNotification($"Dropped {selectedItem.name}");
                    }

                    // Refresh display
                    RefreshInventory(inventorySystem);
                    
                    // Hide details
                    if (itemDetailsPanel != null)
                    {
                        itemDetailsPanel.SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// Equip selected item
        /// </summary>
        private void OnEquipItemClicked()
        {
            if (selectedItem == null || inventorySystem == null)
                return;

            if (selectedItem.itemType == ItemType.Weapon)
            {
                inventorySystem.EquipWeapon(selectedItem.itemId);
                Debug.Log($"Equipped weapon: {selectedItem.name}");
                UpdateEquipmentDisplay();
            }
            else if (selectedItem.itemType == ItemType.Armor)
            {
                inventorySystem.EquipArmor(selectedItem.itemId);
                Debug.Log($"Equipped armor: {selectedItem.name}");
                UpdateEquipmentDisplay();
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowNotification($"Equipped {selectedItem.name}");
            }
        }

        /// <summary>
        /// Update equipment slots display
        /// </summary>
        private void UpdateEquipmentDisplay()
        {
            if (inventorySystem == null)
                return;

            // Update weapon slot
            if (weaponSlot != null)
            {
                Text weaponText = weaponSlot.GetComponentInChildren<Text>();
                if (weaponText != null)
                {
                    string weaponId = inventorySystem.GetEquippedWeapon();
                    if (!string.IsNullOrEmpty(weaponId))
                    {
                        InventoryItem weapon = inventorySystem.GetItem(weaponId);
                        weaponText.text = weapon != null ? weapon.name : "Empty";
                    }
                    else
                    {
                        weaponText.text = "Empty";
                    }
                }
            }

            // Update armor slot
            if (armorSlot != null)
            {
                Text armorText = armorSlot.GetComponentInChildren<Text>();
                if (armorText != null)
                {
                    string armorId = inventorySystem.GetEquippedArmor();
                    if (!string.IsNullOrEmpty(armorId))
                    {
                        InventoryItem armor = inventorySystem.GetItem(armorId);
                        armorText.text = armor != null ? armor.name : "Empty";
                    }
                    else
                    {
                        armorText.text = "Empty";
                    }
                }
            }
        }

        /// <summary>
        /// Sort inventory based on dropdown selection
        /// </summary>
        private void OnSortClicked()
        {
            if (inventorySystem == null)
                return;

            // Get sort option from dropdown
            int sortOption = sortDropdown != null ? sortDropdown.value : 0;
            
            // Sort the inventory items
            List<InventoryItem> sortedItems = inventorySystem.GetAllItems();
            
            switch (sortOption)
            {
                case 0: // Name (A-Z)
                    sortedItems.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase));
                    Debug.Log("Sorted inventory by Name (A-Z)");
                    break;
                    
                case 1: // Type
                    sortedItems.Sort((a, b) => 
                    {
                        int typeCompare = a.itemType.CompareTo(b.itemType);
                        return typeCompare != 0 ? typeCompare : string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase);
                    });
                    Debug.Log("Sorted inventory by Type");
                    break;
                    
                case 2: // Rarity
                    sortedItems.Sort((a, b) => 
                    {
                        // Sort by rarity descending (Legendary first, Common last)
                        int rarityCompare = b.rarity.CompareTo(a.rarity);
                        return rarityCompare != 0 ? rarityCompare : string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase);
                    });
                    Debug.Log("Sorted inventory by Rarity (highest first)");
                    break;
                    
                case 3: // Value
                    sortedItems.Sort((a, b) => 
                    {
                        int valueCompare = b.value.CompareTo(a.value);
                        return valueCompare != 0 ? valueCompare : string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase);
                    });
                    Debug.Log("Sorted inventory by Value (highest first)");
                    break;
                    
                case 4: // Power
                    sortedItems.Sort((a, b) => 
                    {
                        int powerCompare = b.power.CompareTo(a.power);
                        return powerCompare != 0 ? powerCompare : string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase);
                    });
                    Debug.Log("Sorted inventory by Power (highest first)");
                    break;
                    
                default:
                    Debug.LogWarning("Unknown sort option: " + sortOption);
                    break;
            }
            
            // Update inventory system with sorted order and refresh display
            inventorySystem.SetSortedOrder(sortedItems);
            RefreshInventory(inventorySystem);
        }

        /// <summary>
        /// Close item details panel
        /// </summary>
        public void CloseItemDetails()
        {
            if (itemDetailsPanel != null)
            {
                itemDetailsPanel.SetActive(false);
            }
            selectedItem = null;
        }
    }
}
