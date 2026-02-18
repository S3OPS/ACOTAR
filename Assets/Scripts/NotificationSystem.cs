using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ACOTAR
{
    /// <summary>
    /// Notification system for displaying temporary messages and alerts to the player.
    /// Supports different notification types, priorities, and customizable display duration.
    /// </summary>
    public class NotificationSystem : MonoBehaviour
    {
        #region Singleton
        private static NotificationSystem _instance;
        public static NotificationSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("NotificationSystem");
                    _instance = go.AddComponent<NotificationSystem>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        #endregion

        #region Notification Types
        public enum NotificationType
        {
            Info,           // General information
            Success,        // Success messages (quest complete, item obtained)
            Warning,        // Warnings (low health, low mana)
            Error,          // Errors (action failed)
            Achievement,    // Achievement unlocked
            Combat,         // Combat notifications
            Quest,          // Quest updates
            Loot,           // Item obtained
            Level,          // Level up
            System          // System messages
        }

        public enum NotificationPriority
        {
            Low = 0,
            Normal = 1,
            High = 2,
            Critical = 3
        }
        #endregion

        #region Notification Data
        [Serializable]
        public class Notification
        {
            public string id;
            public NotificationType type;
            public NotificationPriority priority;
            public string title;
            public string message;
            public Sprite icon;
            public float duration;
            public DateTime timestamp;
            public bool isRead;
            public Color customColor;
            public System.Action onClick;

            public Notification(NotificationType type, string title, string message, 
                               float duration = 3f, NotificationPriority priority = NotificationPriority.Normal)
            {
                this.id = Guid.NewGuid().ToString();
                this.type = type;
                this.priority = priority;
                this.title = title;
                this.message = message;
                this.duration = duration;
                this.timestamp = DateTime.Now;
                this.isRead = false;
            }
        }
        #endregion

        #region Configuration
        [Header("Display Settings")]
        [SerializeField] private int maxVisibleNotifications = 5;
        [SerializeField] private float defaultDuration = 3f;
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.5f;
        [SerializeField] private Vector2 notificationPosition = new Vector2(10, -10);
        [SerializeField] private float notificationSpacing = 10f;

        [Header("Notification Colors")]
        [SerializeField] private Color infoColor = new Color(0.2f, 0.6f, 1f);
        [SerializeField] private Color successColor = new Color(0.2f, 0.8f, 0.2f);
        [SerializeField] private Color warningColor = new Color(1f, 0.8f, 0.2f);
        [SerializeField] private Color errorColor = new Color(1f, 0.3f, 0.3f);
        [SerializeField] private Color achievementColor = new Color(1f, 0.8f, 0.2f);
        [SerializeField] private Color combatColor = new Color(1f, 0.4f, 0.2f);
        [SerializeField] private Color questColor = new Color(0.6f, 0.4f, 1f);
        [SerializeField] private Color lootColor = new Color(0.2f, 1f, 0.8f);
        [SerializeField] private Color levelColor = new Color(1f, 1f, 0.2f);

        [Header("Sound Settings")]
        [SerializeField] private bool playSound = true;
        [SerializeField] private float soundVolume = 0.7f;
        #endregion

        #region Private Fields
        private Queue<Notification> notificationQueue = new Queue<Notification>();
        private List<Notification> activeNotifications = new List<Notification>();
        private List<Notification> notificationHistory = new List<Notification>();
        private const int MAX_HISTORY = 100;
        private Dictionary<NotificationType, int> notificationCounts = new Dictionary<NotificationType, int>();
        private Canvas notificationCanvas;
        private bool isInitialized = false;
        #endregion

        #region Events
        public delegate void NotificationEventHandler(Notification notification);
        public event NotificationEventHandler OnNotificationShown;
        public event NotificationEventHandler OnNotificationDismissed;
        public event NotificationEventHandler OnNotificationClicked;
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
            // Process notification queue
            if (notificationQueue.Count > 0 && activeNotifications.Count < maxVisibleNotifications)
            {
                ShowNextNotification();
            }
        }
        #endregion

        #region Initialization
        private void Initialize()
        {
            try
            {
                // Initialize notification counts
                foreach (NotificationType type in Enum.GetValues(typeof(NotificationType)))
                {
                    notificationCounts[type] = 0;
                }

                isInitialized = true;
                LoggingSystem.Info("Notification", "NotificationSystem initialized successfully");
            }
            catch (Exception ex)
            {
                LoggingSystem.Error("Notification", "Failed to initialize NotificationSystem", ex);
            }
        }
        #endregion

        #region Public Methods - Show Notifications
        /// <summary>
        /// Show an info notification
        /// </summary>
        public static void ShowInfo(string message, float duration = 3f)
        {
            Show(NotificationType.Info, "Info", message, duration);
        }

        /// <summary>
        /// Show a success notification
        /// </summary>
        public static void ShowSuccess(string message, float duration = 3f)
        {
            Show(NotificationType.Success, "Success", message, duration);
        }

        /// <summary>
        /// Show a warning notification
        /// </summary>
        public static void ShowWarning(string message, float duration = 4f)
        {
            Show(NotificationType.Warning, "Warning", message, duration);
        }

        /// <summary>
        /// Show an error notification
        /// </summary>
        public static void ShowError(string message, float duration = 5f)
        {
            Show(NotificationType.Error, "Error", message, duration);
        }

        /// <summary>
        /// Show an achievement notification
        /// </summary>
        public static void ShowAchievement(string title, string description, float duration = 5f)
        {
            Show(NotificationType.Achievement, title, description, duration, NotificationPriority.High);
        }

        /// <summary>
        /// Show a combat notification
        /// </summary>
        public static void ShowCombat(string message, float duration = 2f)
        {
            Show(NotificationType.Combat, "Combat", message, duration);
        }

        /// <summary>
        /// Show a quest notification
        /// </summary>
        public static void ShowQuest(string title, string message, float duration = 4f)
        {
            Show(NotificationType.Quest, title, message, duration);
        }

        /// <summary>
        /// Show a loot notification
        /// </summary>
        public static void ShowLoot(string itemName, int quantity = 1, float duration = 3f)
        {
            string message = quantity > 1 ? $"Obtained {itemName} x{quantity}" : $"Obtained {itemName}";
            Show(NotificationType.Loot, "Item Obtained", message, duration);
        }

        /// <summary>
        /// Show a level up notification
        /// </summary>
        public static void ShowLevelUp(int newLevel, float duration = 4f)
        {
            Show(NotificationType.Level, "Level Up!", $"You reached level {newLevel}", duration, NotificationPriority.High);
        }

        /// <summary>
        /// Show a system notification
        /// </summary>
        public static void ShowSystem(string message, float duration = 3f)
        {
            Show(NotificationType.System, "System", message, duration);
        }

        /// <summary>
        /// Show a custom notification
        /// </summary>
        /// <summary>
        /// Show a notification with specified properties (static convenience method)
        /// v2.6.6: Enhanced with comprehensive error handling and structured logging
        /// </summary>
        /// <param name="type">Type of notification</param>
        /// <param name="title">Notification title</param>
        /// <param name="message">Notification message</param>
        /// <param name="duration">How long to display (default: 3 seconds)</param>
        /// <param name="priority">Notification priority (default: Normal)</param>
        /// <remarks>
        /// Static convenience method for showing notifications from anywhere in the code.
        /// Validates instance and initialization state before creating notification.
        /// </remarks>
        public static void Show(NotificationType type, string title, string message, 
                               float duration = 3f, NotificationPriority priority = NotificationPriority.Normal)
        {
            try
            {
                if (Instance == null)
                {
                    LoggingSystem.Warning("Notification", "NotificationSystem instance is null, cannot show notification");
                    return;
                }

                if (!Instance.isInitialized)
                {
                    LoggingSystem.Warning("Notification", "NotificationSystem not initialized, skipping notification");
                    return;
                }

                if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(message))
                {
                    LoggingSystem.Warning("Notification", "Both title and message are empty, skipping notification");
                    return;
                }

                var notification = new Notification(type, title, message, duration, priority);
                notification.customColor = Instance.GetColorForType(type);
                
                Instance.QueueNotification(notification);
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Error("Notification", $"Exception in Show: {ex.Message}");
            }
        }
        #endregion

        #region Notification Queue Management
        /// <summary>
        /// Queue a notification for display
        /// v2.6.6: Enhanced with comprehensive error handling and structured logging
        /// </summary>
        /// <param name="notification">Notification to queue</param>
        /// <remarks>
        /// Handles priority-based queue insertion and maintains notification history.
        /// High priority notifications are inserted at the front of the queue.
        /// Includes validation and exception handling for queue operations.
        /// </remarks>
        private void QueueNotification(Notification notification)
        {
            try
            {
                if (notification == null)
                {
                    LoggingSystem.Error("Notification", "Cannot queue null notification");
                    return;
                }

                // Update statistics with validation
                try
                {
                    if (notificationCounts == null)
                    {
                        LoggingSystem.Warning("Notification", "Notification counts dictionary was null, initializing");
                        notificationCounts = new Dictionary<NotificationType, int>();
                        foreach (NotificationType type in System.Enum.GetValues(typeof(NotificationType)))
                        {
                            notificationCounts[type] = 0;
                        }
                    }

                    if (!notificationCounts.ContainsKey(notification.type))
                    {
                        notificationCounts[notification.type] = 0;
                    }
                    notificationCounts[notification.type]++;
                }
                catch (System.Exception statsEx)
                {
                    LoggingSystem.Error("Notification", $"Exception updating notification statistics: {statsEx.Message}");
                }

                // Add to history with validation
                try
                {
                    if (notificationHistory == null)
                    {
                        LoggingSystem.Warning("Notification", "Notification history was null, initializing");
                        notificationHistory = new List<Notification>();
                    }

                    notificationHistory.Add(notification);
                    if (notificationHistory.Count > MAX_HISTORY)
                    {
                        notificationHistory.RemoveAt(0);
                    }
                }
                catch (System.Exception historyEx)
                {
                    LoggingSystem.Error("Notification", $"Exception updating notification history: {historyEx.Message}");
                }

                // Priority-based insertion
                try
                {
                    if (notificationQueue == null)
                    {
                        LoggingSystem.Warning("Notification", "Notification queue was null, initializing");
                        notificationQueue = new Queue<Notification>();
                    }

                    if (notification.priority >= NotificationPriority.High)
                    {
                        // High priority notifications go to the front
                        var tempQueue = new Queue<Notification>();
                        tempQueue.Enqueue(notification);
                        
                        while (notificationQueue.Count > 0)
                        {
                            tempQueue.Enqueue(notificationQueue.Dequeue());
                        }
                        
                        notificationQueue = tempQueue;
                    }
                    else
                    {
                        notificationQueue.Enqueue(notification);
                    }

                    LoggingSystem.Debug("Notification", $"Queued notification: [{notification.type}] {notification.title} - {notification.message}");
                }
                catch (System.Exception queueEx)
                {
                    LoggingSystem.Error("Notification", $"Exception queuing notification: {queueEx.Message}");
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Error("Notification", $"Exception in QueueNotification: {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Display the next notification from the queue
        /// v2.6.6: Enhanced with comprehensive error handling and structured logging
        /// </summary>
        /// <remarks>
        /// CRITICAL: Dequeues and displays notifications with color formatting.
        /// Failures here would prevent notifications from being shown to players.
        /// Includes nested error handling for event invocation and color conversion.
        /// </remarks>
        private void ShowNextNotification()
        {
            try
            {
                if (notificationQueue == null || notificationQueue.Count == 0)
                    return;

                var notification = notificationQueue.Dequeue();
                
                if (notification == null)
                {
                    LoggingSystem.Error("Notification", "Dequeued notification was null");
                    return;
                }

                // Initialize active notifications list if needed
                if (activeNotifications == null)
                {
                    LoggingSystem.Warning("Notification", "Active notifications list was null, initializing");
                    activeNotifications = new List<Notification>();
                }

                activeNotifications.Add(notification);

                // Log notification
                LoggingSystem.Info("Notification", $"Showing notification: [{notification.type}] {notification.title} - {notification.message}");

                // Invoke event with protection
                try
                {
                    OnNotificationShown?.Invoke(notification);
                }
                catch (System.Exception eventEx)
                {
                    LoggingSystem.Error("Notification", $"Exception in OnNotificationShown event handler: {eventEx.Message}");
                }

                // Display formatted notification
                try
                {
                    // In a real implementation, this would create a UI element
                    // For now, we just use Unity's Debug.Log with formatting
                    string colorHex = ColorUtility.ToHtmlStringRGB(notification.customColor);
                    string formattedMessage = $"<color=#{colorHex}>[{notification.type}] {notification.title}</color>: {notification.message}";
                    UnityEngine.Debug.Log(formattedMessage);
                }
                catch (System.Exception colorEx)
                {
                    LoggingSystem.Error("Notification", $"Exception formatting notification color: {colorEx.Message}");
                    // Fallback to unformatted message
                    UnityEngine.Debug.Log($"[{notification.type}] {notification.title}: {notification.message}");
                }

                // Start auto-dismiss coroutine
                try
                {
                    StartCoroutine(AutoDismissNotification(notification));
                }
                catch (System.Exception coroutineEx)
                {
                    LoggingSystem.Error("Notification", $"Exception starting auto-dismiss coroutine: {coroutineEx.Message}");
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Error("Notification", $"Exception in ShowNextNotification: {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        private IEnumerator AutoDismissNotification(Notification notification)
        {
            yield return new WaitForSeconds(notification.duration);
            DismissNotification(notification);
        }
        #endregion

        #region Notification Control
        /// <summary>
        /// Dismiss a specific notification
        /// v2.6.6: Enhanced with comprehensive error handling and structured logging
        /// </summary>
        /// <param name="notification">Notification to dismiss</param>
        /// <remarks>
        /// Removes notification from active list and invokes dismissal event.
        /// Includes validation and event handler protection.
        /// </remarks>
        public void DismissNotification(Notification notification)
        {
            try
            {
                if (notification == null)
                {
                    LoggingSystem.Warning("Notification", "Cannot dismiss null notification");
                    return;
                }

                if (activeNotifications == null)
                {
                    LoggingSystem.Warning("Notification", "Active notifications list is null");
                    return;
                }

                if (!activeNotifications.Contains(notification))
                    return;

                activeNotifications.Remove(notification);
                notification.isRead = true;

                LoggingSystem.Debug("Notification", $"Dismissed notification: {notification.id}");
                
                // Invoke event with protection
                try
                {
                    OnNotificationDismissed?.Invoke(notification);
                }
                catch (System.Exception eventEx)
                {
                    LoggingSystem.Error("Notification", $"Exception in OnNotificationDismissed event handler: {eventEx.Message}");
                }
            }
            catch (System.Exception ex)
            {
                LoggingSystem.Error("Notification", $"Exception in DismissNotification: {ex.Message}\nStack: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Dismiss all active notifications
        /// </summary>
        public static void DismissAll()
        {
            var notificationsCopy = new List<Notification>(Instance.activeNotifications);
            foreach (var notification in notificationsCopy)
            {
                Instance.DismissNotification(notification);
            }
        }

        /// <summary>
        /// Clear the notification queue
        /// </summary>
        public static void ClearQueue()
        {
            Instance.notificationQueue.Clear();
            LoggingSystem.Info("Notification", "Cleared notification queue");
        }
        #endregion

        #region Statistics & History
        /// <summary>
        /// Get notification statistics by type
        /// </summary>
        public static Dictionary<NotificationType, int> GetStatistics()
        {
            return new Dictionary<NotificationType, int>(Instance.notificationCounts);
        }

        /// <summary>
        /// Get notification history
        /// </summary>
        public static List<Notification> GetHistory(int count = 20)
        {
            int startIndex = Mathf.Max(0, Instance.notificationHistory.Count - count);
            int actualCount = Instance.notificationHistory.Count - startIndex;
            return Instance.notificationHistory.GetRange(startIndex, actualCount);
        }

        /// <summary>
        /// Get unread notification count
        /// </summary>
        public static int GetUnreadCount()
        {
            return Instance.notificationHistory.FindAll(n => !n.isRead).Count;
        }

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        public static void MarkAllAsRead()
        {
            foreach (var notification in Instance.notificationHistory)
            {
                notification.isRead = true;
            }
        }
        #endregion

        #region Helper Methods
        private Color GetColorForType(NotificationType type)
        {
            switch (type)
            {
                case NotificationType.Info:
                    return infoColor;
                case NotificationType.Success:
                    return successColor;
                case NotificationType.Warning:
                    return warningColor;
                case NotificationType.Error:
                    return errorColor;
                case NotificationType.Achievement:
                    return achievementColor;
                case NotificationType.Combat:
                    return combatColor;
                case NotificationType.Quest:
                    return questColor;
                case NotificationType.Loot:
                    return lootColor;
                case NotificationType.Level:
                    return levelColor;
                default:
                    return Color.white;
            }
        }
        #endregion

        #region Configuration
        /// <summary>
        /// Configure notification display settings
        /// </summary>
        public static void Configure(int maxVisible, float defaultDuration, bool playSound)
        {
            Instance.maxVisibleNotifications = maxVisible;
            Instance.defaultDuration = defaultDuration;
            Instance.playSound = playSound;
            
            LoggingSystem.Info("Notification", $"Updated notification settings: maxVisible={maxVisible}, duration={defaultDuration}, sound={playSound}");
        }
        #endregion
    }
}
