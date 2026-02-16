using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

namespace ACOTAR
{
    /// <summary>
    /// Performance monitoring system for tracking FPS, memory usage, and execution time.
    /// Provides real-time metrics and performance profiling capabilities.
    /// </summary>
    public class PerformanceMonitor : MonoBehaviour
    {
        #region Singleton
        private static PerformanceMonitor _instance;
        public static PerformanceMonitor Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("PerformanceMonitor");
                    _instance = go.AddComponent<PerformanceMonitor>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        #endregion

        #region Configuration
        [Header("Performance Monitoring")]
        [SerializeField] private bool enableMonitoring = true;
        [SerializeField] private bool showDebugOverlay = false;
        [SerializeField] private float updateInterval = 0.5f;
        [SerializeField] private int fpsHistorySize = 60;
        
        [Header("Thresholds")]
        [SerializeField] private int targetFPS = 60;
        [SerializeField] private int lowFPSThreshold = 30;
        [SerializeField] private long highMemoryThreshold = 500 * 1024 * 1024; // 500 MB
        [SerializeField] private float longFrameThreshold = 50f; // ms
        #endregion

        #region Performance Metrics
        [Serializable]
        public class PerformanceMetrics
        {
            // FPS Metrics
            public float currentFPS;
            public float averageFPS;
            public float minFPS;
            public float maxFPS;
            
            // Frame Time Metrics (in milliseconds)
            public float currentFrameTime;
            public float averageFrameTime;
            public float maxFrameTime;
            
            // Memory Metrics (in MB)
            public float totalAllocatedMemory;
            public float totalReservedMemory;
            public float totalUnusedReservedMemory;
            public float monoHeapSize;
            public float monoUsedSize;
            
            // Performance Statistics
            public int totalFrames;
            public int lowFPSFrameCount;
            public int longFrameCount;
            public float uptime;
            
            public override string ToString()
            {
                return $"FPS: {currentFPS:F1} (Avg: {averageFPS:F1}, Min: {minFPS:F1}, Max: {maxFPS:F1})\n" +
                       $"Frame Time: {currentFrameTime:F2}ms (Avg: {averageFrameTime:F2}ms, Max: {maxFrameTime:F2}ms)\n" +
                       $"Memory: {totalAllocatedMemory:F1}MB / {totalReservedMemory:F1}MB\n" +
                       $"Mono Heap: {monoUsedSize:F1}MB / {monoHeapSize:F1}MB\n" +
                       $"Uptime: {uptime:F1}s | Frames: {totalFrames} | Low FPS: {lowFPSFrameCount}";
            }
        }
        #endregion

        #region Private Fields
        private PerformanceMetrics metrics = new PerformanceMetrics();
        private Queue<float> fpsHistory = new Queue<float>();
        private float deltaTime = 0f;
        private float updateTimer = 0f;
        private float startTime = 0f;
        private Dictionary<string, Stopwatch> activeTimers = new Dictionary<string, Stopwatch>();
        private Dictionary<string, PerformanceProfile> profiles = new Dictionary<string, PerformanceProfile>();
        private GUIStyle debugStyle;
        #endregion

        #region Performance Profile
        private class PerformanceProfile
        {
            public string name;
            public long totalExecutions;
            public long totalTimeMs;
            public long minTimeMs;
            public long maxTimeMs;
            public float averageTimeMs => totalExecutions > 0 ? (float)totalTimeMs / totalExecutions : 0f;

            public PerformanceProfile(string name)
            {
                this.name = name;
                this.minTimeMs = long.MaxValue;
                this.maxTimeMs = 0;
            }

            public void RecordExecution(long timeMs)
            {
                totalExecutions++;
                totalTimeMs += timeMs;
                if (timeMs < minTimeMs) minTimeMs = timeMs;
                if (timeMs > maxTimeMs) maxTimeMs = timeMs;
            }
        }
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
            if (!enableMonitoring)
                return;

            UpdatePerformanceMetrics();
        }

        private void OnGUI()
        {
            if (!showDebugOverlay || !enableMonitoring)
                return;

            if (debugStyle == null)
            {
                debugStyle = new GUIStyle(GUI.skin.box);
                debugStyle.alignment = TextAnchor.UpperLeft;
                debugStyle.fontSize = 12;
                debugStyle.normal.textColor = Color.white;
                debugStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.8f));
            }

            GUI.Box(new Rect(10, 10, 400, 150), metrics.ToString(), debugStyle);
        }
        #endregion

        #region Initialization
        private void Initialize()
        {
            startTime = Time.realtimeSinceStartup;
            metrics.minFPS = float.MaxValue;
            metrics.maxFPS = 0f;
            
            LoggingSystem.Info("Performance", "PerformanceMonitor initialized");
        }

        private Texture2D MakeTex(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = color;

            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
        #endregion

        #region Performance Metrics Update
        private void UpdatePerformanceMetrics()
        {
            // Calculate delta time
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            updateTimer += Time.unscaledDeltaTime;

            // Update frame count
            metrics.totalFrames++;
            metrics.uptime = Time.realtimeSinceStartup - startTime;

            if (updateTimer >= updateInterval)
            {
                updateTimer = 0f;

                // FPS Calculations
                metrics.currentFPS = 1f / deltaTime;
                metrics.currentFrameTime = deltaTime * 1000f;

                // Track FPS history
                fpsHistory.Enqueue(metrics.currentFPS);
                if (fpsHistory.Count > fpsHistorySize)
                    fpsHistory.Dequeue();

                // Calculate average FPS
                float totalFPS = 0f;
                foreach (float fps in fpsHistory)
                    totalFPS += fps;
                metrics.averageFPS = totalFPS / fpsHistory.Count;

                // Update min/max FPS
                if (metrics.currentFPS < metrics.minFPS)
                    metrics.minFPS = metrics.currentFPS;
                if (metrics.currentFPS > metrics.maxFPS)
                    metrics.maxFPS = metrics.currentFPS;

                // Track low FPS frames
                if (metrics.currentFPS < lowFPSThreshold)
                    metrics.lowFPSFrameCount++;

                // Update frame time metrics
                if (metrics.currentFrameTime > metrics.maxFrameTime)
                    metrics.maxFrameTime = metrics.currentFrameTime;

                // Calculate average frame time
                float totalFrameTime = 0f;
                foreach (float fps in fpsHistory)
                    totalFrameTime += 1000f / fps;
                metrics.averageFrameTime = totalFrameTime / fpsHistory.Count;

                // Track long frames
                if (metrics.currentFrameTime > longFrameThreshold)
                {
                    metrics.longFrameCount++;
                    LoggingSystem.Warning("Performance", $"Long frame detected: {metrics.currentFrameTime:F2}ms");
                }

                // Memory Metrics
                UpdateMemoryMetrics();

                // Check for performance issues
                CheckPerformanceThresholds();
            }
        }

        private void UpdateMemoryMetrics()
        {
            // Unity memory stats (in MB)
            metrics.totalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);
            metrics.totalReservedMemory = Profiler.GetTotalReservedMemoryLong() / (1024f * 1024f);
            metrics.totalUnusedReservedMemory = Profiler.GetTotalUnusedReservedMemoryLong() / (1024f * 1024f);
            
            // Mono memory stats (in MB)
            metrics.monoHeapSize = Profiler.GetMonoHeapSizeLong() / (1024f * 1024f);
            metrics.monoUsedSize = Profiler.GetMonoUsedSizeLong() / (1024f * 1024f);
        }

        private void CheckPerformanceThresholds()
        {
            // Check FPS threshold
            if (metrics.currentFPS < lowFPSThreshold)
            {
                LoggingSystem.Warning("Performance", $"Low FPS detected: {metrics.currentFPS:F1} (threshold: {lowFPSThreshold})");
            }

            // Check memory threshold
            long totalMemoryBytes = Profiler.GetTotalAllocatedMemoryLong();
            if (totalMemoryBytes > highMemoryThreshold)
            {
                LoggingSystem.Warning("Performance", $"High memory usage: {metrics.totalAllocatedMemory:F1}MB (threshold: {highMemoryThreshold / (1024f * 1024f)}MB)");
            }
        }
        #endregion

        #region Public API
        /// <summary>
        /// Get current performance metrics
        /// </summary>
        public static PerformanceMetrics GetMetrics()
        {
            return Instance.metrics;
        }

        /// <summary>
        /// Start timing a code section
        /// </summary>
        public static void StartTimer(string name)
        {
            if (!Instance.enableMonitoring)
                return;

            if (Instance.activeTimers.ContainsKey(name))
            {
                LoggingSystem.Warning("Performance", $"Timer '{name}' is already running");
                return;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Instance.activeTimers[name] = stopwatch;
        }

        /// <summary>
        /// Stop timing a code section and record the result
        /// </summary>
        public static void StopTimer(string name)
        {
            if (!Instance.enableMonitoring)
                return;

            if (!Instance.activeTimers.ContainsKey(name))
            {
                LoggingSystem.Warning("Performance", $"Timer '{name}' was not started");
                return;
            }

            var stopwatch = Instance.activeTimers[name];
            stopwatch.Stop();
            Instance.activeTimers.Remove(name);

            // Record the profile
            long elapsedMs = stopwatch.ElapsedMilliseconds;
            Instance.RecordProfile(name, elapsedMs);

            // Log if it took too long
            if (elapsedMs > 10)
            {
                LoggingSystem.Debug("Performance", $"Timer '{name}' took {elapsedMs}ms");
            }
        }

        /// <summary>
        /// Measure execution time of an action
        /// </summary>
        public static void Measure(string name, System.Action action)
        {
            StartTimer(name);
            try
            {
                action?.Invoke();
            }
            finally
            {
                StopTimer(name);
            }
        }

        /// <summary>
        /// Reset all performance statistics
        /// </summary>
        public static void ResetStatistics()
        {
            Instance.metrics = new PerformanceMetrics();
            Instance.metrics.minFPS = float.MaxValue;
            Instance.fpsHistory.Clear();
            Instance.profiles.Clear();
            Instance.startTime = Time.realtimeSinceStartup;
            
            LoggingSystem.Info("Performance", "Performance statistics reset");
        }

        /// <summary>
        /// Get performance profile for a named section
        /// </summary>
        public static string GetProfile(string name)
        {
            if (!Instance.profiles.ContainsKey(name))
                return $"No profile data for '{name}'";

            var profile = Instance.profiles[name];
            return $"Profile: {profile.name}\n" +
                   $"  Executions: {profile.totalExecutions}\n" +
                   $"  Average: {profile.averageTimeMs:F2}ms\n" +
                   $"  Min: {profile.minTimeMs}ms\n" +
                   $"  Max: {profile.maxTimeMs}ms\n" +
                   $"  Total: {profile.totalTimeMs}ms";
        }

        /// <summary>
        /// Get all performance profiles
        /// </summary>
        public static string GetAllProfiles()
        {
            if (Instance.profiles.Count == 0)
                return "No performance profiles recorded";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== Performance Profiles ===");
            
            foreach (var profile in Instance.profiles.Values)
            {
                sb.AppendLine($"\n{profile.name}:");
                sb.AppendLine($"  Executions: {profile.totalExecutions}");
                sb.AppendLine($"  Average: {profile.averageTimeMs:F2}ms");
                sb.AppendLine($"  Min: {profile.minTimeMs}ms | Max: {profile.maxTimeMs}ms");
            }
            
            return sb.ToString();
        }

        /// <summary>
        /// Enable or disable the debug overlay
        /// </summary>
        public static void SetDebugOverlay(bool enabled)
        {
            Instance.showDebugOverlay = enabled;
            LoggingSystem.Info("Performance", $"Debug overlay {(enabled ? "enabled" : "disabled")}");
        }

        /// <summary>
        /// Export performance report
        /// </summary>
        public static string ExportReport()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== ACOTAR RPG Performance Report ===");
            sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();
            sb.AppendLine("=== Performance Metrics ===");
            sb.AppendLine(Instance.metrics.ToString());
            sb.AppendLine();
            sb.AppendLine(GetAllProfiles());
            
            return sb.ToString();
        }
        #endregion

        #region Private Methods
        private void RecordProfile(string name, long timeMs)
        {
            if (!profiles.ContainsKey(name))
            {
                profiles[name] = new PerformanceProfile(name);
            }

            profiles[name].RecordExecution(timeMs);
        }
        #endregion

        #region Configuration
        /// <summary>
        /// Configure performance monitoring
        /// </summary>
        public static void Configure(bool enable, int targetFPS, float updateInterval)
        {
            Instance.enableMonitoring = enable;
            Instance.targetFPS = targetFPS;
            Instance.updateInterval = updateInterval;
            
            LoggingSystem.Info("Performance", $"Performance monitoring configured: enabled={enable}, targetFPS={targetFPS}, updateInterval={updateInterval}");
        }
        #endregion
    }
}
