using UnityEngine;
using UnityEngine.UI;

namespace ACOTAR
{
    /// <summary>
    /// Manages tooltip display for UI elements
    /// Shows item details, ability descriptions, and other contextual information
    /// </summary>
    public class TooltipSystem : MonoBehaviour
    {
        public static TooltipSystem Instance { get; private set; }

        [Header("Tooltip Panel")]
        public GameObject tooltipPanel;
        public Text tooltipTitleText;
        public Text tooltipDescriptionText;
        public Text tooltipStatsText;
        public RectTransform tooltipRect;

        [Header("Settings")]
        public float showDelay = 0.5f;
        public Vector2 offset = new Vector2(10, 10);

        private float hoverTimer = 0f;
        private bool isShowingTooltip = false;
        private string currentTooltipId = "";

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            HideTooltip();
        }

        void Update()
        {
            if (isShowingTooltip && tooltipPanel != null)
            {
                // Update tooltip position to follow mouse
                UpdateTooltipPosition();
            }
        }

        /// <summary>
        /// Show tooltip for an item
        /// </summary>
        public void ShowItemTooltip(InventoryItem item)
        {
            if (item == null || tooltipPanel == null)
                return;

            // Set tooltip content
            if (tooltipTitleText != null)
            {
                tooltipTitleText.text = GetColoredItemName(item);
            }

            if (tooltipDescriptionText != null)
            {
                tooltipDescriptionText.text = item.description;
            }

            if (tooltipStatsText != null)
            {
                tooltipStatsText.text = GetItemStats(item);
            }

            // Show the tooltip
            tooltipPanel.SetActive(true);
            isShowingTooltip = true;
            currentTooltipId = item.itemId;
            UpdateTooltipPosition();
        }

        /// <summary>
        /// Show tooltip with custom content
        /// </summary>
        public void ShowTooltip(string title, string description, string stats = "")
        {
            if (tooltipPanel == null)
                return;

            if (tooltipTitleText != null)
            {
                tooltipTitleText.text = title;
            }

            if (tooltipDescriptionText != null)
            {
                tooltipDescriptionText.text = description;
            }

            if (tooltipStatsText != null)
            {
                tooltipStatsText.text = stats;
            }

            tooltipPanel.SetActive(true);
            isShowingTooltip = true;
            UpdateTooltipPosition();
        }

        /// <summary>
        /// Hide the tooltip
        /// </summary>
        public void HideTooltip()
        {
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(false);
            }
            isShowingTooltip = false;
            currentTooltipId = "";
            hoverTimer = 0f;
        }

        /// <summary>
        /// Update tooltip position to follow mouse cursor
        /// </summary>
        private void UpdateTooltipPosition()
        {
            if (tooltipRect == null)
                return;

            Vector2 mousePosition = Input.mousePosition;
            
            // Add offset to avoid cursor overlap
            Vector2 position = mousePosition + offset;
            
            // Get screen bounds
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            // Prevent tooltip from going off screen
            float tooltipWidth = tooltipRect.rect.width;
            float tooltipHeight = tooltipRect.rect.height;
            
            if (position.x + tooltipWidth > screenWidth)
            {
                position.x = mousePosition.x - tooltipWidth - offset.x;
            }
            
            if (position.y + tooltipHeight > screenHeight)
            {
                position.y = mousePosition.y - tooltipHeight - offset.y;
            }
            
            tooltipRect.position = position;
        }

        /// <summary>
        /// Get colored item name based on rarity
        /// </summary>
        private string GetColoredItemName(InventoryItem item)
        {
            string colorCode = GetRarityColor(item.rarity);
            return $"<color={colorCode}>{item.name}</color>";
        }

        /// <summary>
        /// Get color code for item rarity
        /// </summary>
        private string GetRarityColor(ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common:
                    return "#FFFFFF"; // White
                case ItemRarity.Uncommon:
                    return "#00FF00"; // Green
                case ItemRarity.Rare:
                    return "#0080FF"; // Blue
                case ItemRarity.Epic:
                    return "#A020F0"; // Purple
                case ItemRarity.Legendary:
                    return "#FF8000"; // Orange
                case ItemRarity.Artifact:
                    return "#FFD700"; // Gold
                default:
                    return "#FFFFFF";
            }
        }

        /// <summary>
        /// Get formatted item stats text
        /// </summary>
        private string GetItemStats(InventoryItem item)
        {
            System.Text.StringBuilder stats = new System.Text.StringBuilder();
            
            stats.AppendLine($"Type: {item.itemType}");
            stats.AppendLine($"Rarity: {item.rarity}");
            
            if (item.power > 0)
            {
                stats.AppendLine($"Power: +{item.power}");
            }
            
            if (item.value > 0)
            {
                stats.AppendLine($"Value: {item.value} gold");
            }
            
            if (item.quantity > 1)
            {
                stats.AppendLine($"Quantity: {item.quantity}");
            }
            
            if (item.isEquipped)
            {
                stats.AppendLine("<color=yellow>[EQUIPPED]</color>");
            }
            
            return stats.ToString();
        }

        /// <summary>
        /// Check if tooltip is currently being shown
        /// </summary>
        public bool IsTooltipVisible()
        {
            return isShowingTooltip;
        }
    }
}
