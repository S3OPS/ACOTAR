using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Extension to the crafting system that enables batch crafting of items.
    /// Allows players to craft multiple items at once with a single action.
    /// </summary>
    public class BatchCraftingSystem : MonoBehaviour
    {
        #region Singleton
        private static BatchCraftingSystem _instance;
        public static BatchCraftingSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("BatchCraftingSystem");
                    _instance = go.AddComponent<BatchCraftingSystem>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        #endregion

        #region Configuration
        [Header("Batch Crafting Settings")]
        [SerializeField] private int maxBatchSize = 99;
        [SerializeField] private bool allowBackgroundCrafting = true;
        [SerializeField] private float craftingSpeedMultiplier = 1.0f;
        [SerializeField] private bool showProgressNotifications = true;
        #endregion

        #region Batch Craft Request
        [Serializable]
        public class BatchCraftRequest
        {
            public string recipeId;
            public int quantity;
            public int itemsCrafted;
            public float progress;
            public bool isComplete;
            public DateTime startTime;
            public float estimatedTimeRemaining;

            public BatchCraftRequest(string recipeId, int quantity)
            {
                this.recipeId = recipeId;
                this.quantity = quantity;
                this.itemsCrafted = 0;
                this.progress = 0f;
                this.isComplete = false;
                this.startTime = DateTime.Now;
            }
        }
        #endregion

        #region Private Fields
        private Queue<BatchCraftRequest> craftingQueue = new Queue<BatchCraftRequest>();
        private BatchCraftRequest currentRequest = null;
        private Coroutine craftingCoroutine = null;
        private CraftingSystem craftingSystem;
        private Dictionary<string, int> totalCraftedItems = new Dictionary<string, int>();
        #endregion

        #region Events
        public delegate void BatchCraftEventHandler(string recipeId, int quantity);
        public event BatchCraftEventHandler OnBatchCraftStarted;
        public event BatchCraftEventHandler OnBatchCraftCompleted;
        public event BatchCraftEventHandler OnBatchCraftProgress;
        public event BatchCraftEventHandler OnBatchCraftCancelled;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();
        }

        private void Update()
        {
            if (currentRequest != null)
            {
                // Update progress notifications
                if (showProgressNotifications && currentRequest.itemsCrafted > 0)
                {
                    // Calculate time remaining
                    TimeSpan elapsed = DateTime.Now - currentRequest.startTime;
                    float craftedRatio = (float)currentRequest.itemsCrafted / currentRequest.quantity;
                    if (craftedRatio > 0)
                    {
                        float totalEstimatedSeconds = (float)elapsed.TotalSeconds / craftedRatio;
                        currentRequest.estimatedTimeRemaining = totalEstimatedSeconds - (float)elapsed.TotalSeconds;
                    }
                }
            }
        }
        #endregion

        #region Initialization
        private void Initialize()
        {
            craftingSystem = CraftingSystem.Instance;
            LoggingSystem.Info("BatchCrafting", "BatchCraftingSystem initialized");
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Queue a batch craft request
        /// </summary>
        public static bool QueueBatchCraft(string recipeId, int quantity)
        {
            if (quantity <= 0 || quantity > Instance.maxBatchSize)
            {
                NotificationSystem.ShowError($"Invalid batch size. Maximum is {Instance.maxBatchSize}");
                LoggingSystem.Warning("BatchCrafting", $"Invalid batch size: {quantity}");
                return false;
            }

            // Validate that we have enough materials for the full batch
            if (!Instance.ValidateBatchMaterials(recipeId, quantity))
            {
                NotificationSystem.ShowError("Insufficient materials for batch crafting");
                return false;
            }

            var request = new BatchCraftRequest(recipeId, quantity);
            Instance.craftingQueue.Enqueue(request);

            LoggingSystem.Info("BatchCrafting", $"Queued batch craft: {recipeId} x{quantity}");
            NotificationSystem.ShowInfo($"Queued crafting: {quantity} items");

            // Start crafting if not already in progress
            if (Instance.currentRequest == null)
            {
                Instance.StartNextBatch();
            }

            return true;
        }

        /// <summary>
        /// Calculate maximum craftable quantity based on available materials
        /// </summary>
        public static int CalculateMaxCraftable(string recipeId)
        {
            var recipe = CraftingSystem.Instance.GetRecipe(recipeId);
            if (recipe == null)
            {
                LoggingSystem.Warning("BatchCrafting", $"Recipe not found: {recipeId}");
                return 0;
            }

            var inventory = InventorySystem.Instance;
            int maxCraftable = int.MaxValue;

            // Check each required material
            foreach (var material in recipe.requiredMaterials)
            {
                int available = inventory.GetItemCount(material.Key);
                int possible = available / material.Value;
                
                if (possible < maxCraftable)
                {
                    maxCraftable = possible;
                }
            }

            // Cap at max batch size
            return Math.Min(maxCraftable, Instance.maxBatchSize);
        }

        /// <summary>
        /// Craft maximum possible quantity
        /// </summary>
        public static bool CraftMaxBatch(string recipeId)
        {
            int maxQuantity = CalculateMaxCraftable(recipeId);
            
            if (maxQuantity <= 0)
            {
                NotificationSystem.ShowWarning("No materials available for crafting");
                return false;
            }

            return QueueBatchCraft(recipeId, maxQuantity);
        }

        /// <summary>
        /// Cancel the current batch crafting operation
        /// </summary>
        public static void CancelCurrentBatch()
        {
            if (Instance.currentRequest == null)
            {
                NotificationSystem.ShowWarning("No active batch crafting to cancel");
                return;
            }

            Instance.StopBatchCrafting();
            NotificationSystem.ShowInfo("Batch crafting cancelled");
            LoggingSystem.Info("BatchCrafting", "Current batch crafting cancelled");
        }

        /// <summary>
        /// Clear all queued batch craft requests
        /// </summary>
        public static void ClearQueue()
        {
            Instance.craftingQueue.Clear();
            NotificationSystem.ShowInfo("Crafting queue cleared");
            LoggingSystem.Info("BatchCrafting", "Crafting queue cleared");
        }

        /// <summary>
        /// Get current batch crafting progress
        /// </summary>
        public static BatchCraftRequest GetCurrentRequest()
        {
            return Instance.currentRequest;
        }

        /// <summary>
        /// Get number of queued batch requests
        /// </summary>
        public static int GetQueueLength()
        {
            return Instance.craftingQueue.Count;
        }

        /// <summary>
        /// Get total crafted items statistics
        /// </summary>
        public static Dictionary<string, int> GetCraftingStatistics()
        {
            return new Dictionary<string, int>(Instance.totalCraftedItems);
        }

        /// <summary>
        /// Get total items crafted for a specific recipe
        /// </summary>
        public static int GetTotalCrafted(string recipeId)
        {
            return Instance.totalCraftedItems.ContainsKey(recipeId) ? Instance.totalCraftedItems[recipeId] : 0;
        }
        #endregion

        #region Private Methods
        private bool ValidateBatchMaterials(string recipeId, int quantity)
        {
            var recipe = craftingSystem.GetRecipe(recipeId);
            if (recipe == null)
            {
                LoggingSystem.Error("BatchCrafting", $"Recipe not found: {recipeId}");
                return false;
            }

            var inventory = InventorySystem.Instance;

            // Check each required material
            foreach (var material in recipe.requiredMaterials)
            {
                int required = material.Value * quantity;
                int available = inventory.GetItemCount(material.Key);

                if (available < required)
                {
                    LoggingSystem.Warning("BatchCrafting", 
                        $"Insufficient materials: {material.Key} (need {required}, have {available})");
                    return false;
                }
            }

            return true;
        }

        private void StartNextBatch()
        {
            if (craftingQueue.Count == 0)
            {
                currentRequest = null;
                return;
            }

            currentRequest = craftingQueue.Dequeue();
            OnBatchCraftStarted?.Invoke(currentRequest.recipeId, currentRequest.quantity);

            LoggingSystem.Info("BatchCrafting", $"Starting batch craft: {currentRequest.recipeId} x{currentRequest.quantity}");
            NotificationSystem.ShowInfo($"Crafting {currentRequest.quantity} items...");

            craftingCoroutine = StartCoroutine(ProcessBatchCraft());
        }

        private IEnumerator ProcessBatchCraft()
        {
            var recipe = craftingSystem.GetRecipe(currentRequest.recipeId);
            if (recipe == null)
            {
                LoggingSystem.Error("BatchCrafting", $"Recipe not found: {currentRequest.recipeId}");
                FinishCurrentBatch();
                yield break;
            }

            // Craft items one by one
            while (currentRequest.itemsCrafted < currentRequest.quantity)
            {
                // Check if we still have materials
                if (!ValidateBatchMaterials(currentRequest.recipeId, 1))
                {
                    LoggingSystem.Warning("BatchCrafting", "Ran out of materials during batch craft");
                    NotificationSystem.ShowWarning("Batch crafting stopped: insufficient materials");
                    break;
                }

                // Craft one item
                bool success = craftingSystem.CraftItem(currentRequest.recipeId);
                
                if (success)
                {
                    currentRequest.itemsCrafted++;
                    currentRequest.progress = (float)currentRequest.itemsCrafted / currentRequest.quantity;

                    // Update statistics
                    if (!totalCraftedItems.ContainsKey(currentRequest.recipeId))
                    {
                        totalCraftedItems[currentRequest.recipeId] = 0;
                    }
                    totalCraftedItems[currentRequest.recipeId]++;

                    // Fire progress event
                    OnBatchCraftProgress?.Invoke(currentRequest.recipeId, currentRequest.itemsCrafted);

                    // Show progress notification (every 10% or for small batches)
                    if (currentRequest.quantity >= 10 && currentRequest.itemsCrafted % (currentRequest.quantity / 10) == 0)
                    {
                        NotificationSystem.ShowInfo($"Crafting progress: {currentRequest.itemsCrafted}/{currentRequest.quantity}");
                    }
                }
                else
                {
                    LoggingSystem.Error("BatchCrafting", $"Failed to craft item: {currentRequest.recipeId}");
                    NotificationSystem.ShowError("Crafting failed");
                    break;
                }

                // Wait for crafting time (adjusted by speed multiplier)
                float waitTime = recipe.craftingTime / craftingSpeedMultiplier;
                yield return new WaitForSeconds(waitTime);
            }

            currentRequest.isComplete = true;
            FinishCurrentBatch();
        }

        private void FinishCurrentBatch()
        {
            if (currentRequest == null)
                return;

            int itemsCrafted = currentRequest.itemsCrafted;
            string recipeId = currentRequest.recipeId;

            if (itemsCrafted > 0)
            {
                OnBatchCraftCompleted?.Invoke(recipeId, itemsCrafted);
                NotificationSystem.ShowSuccess($"Crafted {itemsCrafted} items!");
                LoggingSystem.Info("BatchCrafting", $"Completed batch craft: {recipeId} x{itemsCrafted}");
            }

            currentRequest = null;
            craftingCoroutine = null;

            // Start next batch if available
            if (craftingQueue.Count > 0)
            {
                StartNextBatch();
            }
        }

        private void StopBatchCrafting()
        {
            if (craftingCoroutine != null)
            {
                StopCoroutine(craftingCoroutine);
                craftingCoroutine = null;
            }

            if (currentRequest != null)
            {
                OnBatchCraftCancelled?.Invoke(currentRequest.recipeId, currentRequest.itemsCrafted);
            }

            currentRequest = null;
        }
        #endregion

        #region Configuration
        /// <summary>
        /// Configure batch crafting settings
        /// </summary>
        public static void Configure(int maxBatchSize, float speedMultiplier, bool showNotifications)
        {
            Instance.maxBatchSize = maxBatchSize;
            Instance.craftingSpeedMultiplier = speedMultiplier;
            Instance.showProgressNotifications = showNotifications;

            LoggingSystem.Info("BatchCrafting", 
                $"Configured: maxBatch={maxBatchSize}, speed={speedMultiplier}, notifications={showNotifications}");
        }
        #endregion
    }
}
