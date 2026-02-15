using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ACOTAR
{
    /// <summary>
    /// Centralized logging system for the ACOTAR RPG.
    /// Provides structured logging with different log levels, file output, and error tracking.
    /// </summary>
    public class LoggingSystem : MonoBehaviour
    {
        #region Singleton
        private static LoggingSystem _instance;
        public static LoggingSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("LoggingSystem");
                    _instance = go.AddComponent<LoggingSystem>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        #endregion

        #region Configuration
        [Header("Logging Configuration")]
        [SerializeField] private bool enableFileLogging = true;
        [SerializeField] private bool enableConsoleLogging = true;
        [SerializeField] private LogLevel minimumLogLevel = LogLevel.Info;
        [SerializeField] private int maxLogFileSize = 10 * 1024 * 1024; // 10 MB
        [SerializeField] private int maxLogFiles = 5;
        #endregion

        #region Private Fields
        private string logDirectory;
        private string currentLogFile;
        private Queue<LogEntry> logBuffer = new Queue<LogEntry>();
        private const int BUFFER_SIZE = 100;
        private Dictionary<string, int> errorCounts = new Dictionary<string, int>();
        #endregion

        #region Log Levels
        public enum LogLevel
        {
            Trace = 0,
            Debug = 1,
            Info = 2,
            Warning = 3,
            Error = 4,
            Critical = 5
        }
        #endregion

        #region Log Entry
        private class LogEntry
        {
            public DateTime Timestamp { get; set; }
            public LogLevel Level { get; set; }
            public string Category { get; set; }
            public string Message { get; set; }
            public string StackTrace { get; set; }

            public override string ToString()
            {
                string levelStr = Level.ToString().ToUpper().PadRight(8);
                string categoryStr = !string.IsNullOrEmpty(Category) ? $"[{Category}] " : "";
                string result = $"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] {levelStr} {categoryStr}{Message}";
                
                if (!string.IsNullOrEmpty(StackTrace))
                {
                    result += $"\n{StackTrace}";
                }
                
                return result;
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

            InitializeLogging();
        }

        private void OnDestroy()
        {
            FlushLogs();
        }

        private void OnApplicationQuit()
        {
            FlushLogs();
        }
        #endregion

        #region Initialization
        private void InitializeLogging()
        {
            logDirectory = Path.Combine(Application.persistentDataPath, "Logs");
            
            try
            {
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Create new log file with timestamp
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                currentLogFile = Path.Combine(logDirectory, $"acotar_log_{timestamp}.txt");

                // Clean up old log files if needed
                CleanupOldLogs();

                // Write header to log file
                if (enableFileLogging)
                {
                    File.WriteAllText(currentLogFile, 
                        $"=== ACOTAR RPG Log File ===\n" +
                        $"Started: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                        $"Unity Version: {Application.unityVersion}\n" +
                        $"Platform: {Application.platform}\n" +
                        $"Log Level: {minimumLogLevel}\n" +
                        $"================================\n\n");
                }

                Info("Logging", "LoggingSystem initialized successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize LoggingSystem: {ex.Message}");
                enableFileLogging = false; // Disable file logging if initialization fails
            }
        }

        private void CleanupOldLogs()
        {
            try
            {
                var logFiles = Directory.GetFiles(logDirectory, "acotar_log_*.txt");
                
                // Sort by creation time, oldest first
                Array.Sort(logFiles, (a, b) => File.GetCreationTime(a).CompareTo(File.GetCreationTime(b)));

                // Delete oldest files if we exceed maxLogFiles
                int filesToDelete = logFiles.Length - maxLogFiles;
                for (int i = 0; i < filesToDelete; i++)
                {
                    File.Delete(logFiles[i]);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to cleanup old logs: {ex.Message}");
            }
        }
        #endregion

        #region Public Logging Methods
        /// <summary>
        /// Log a trace message (very detailed, for debugging only)
        /// </summary>
        public static void Trace(string category, string message)
        {
            Instance.Log(LogLevel.Trace, category, message);
        }

        /// <summary>
        /// Log a debug message
        /// </summary>
        public static void Debug(string category, string message)
        {
            Instance.Log(LogLevel.Debug, category, message);
        }

        /// <summary>
        /// Log an informational message
        /// </summary>
        public static void Info(string category, string message)
        {
            Instance.Log(LogLevel.Info, category, message);
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        public static void Warning(string category, string message)
        {
            Instance.Log(LogLevel.Warning, category, message);
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        public static void Error(string category, string message, Exception exception = null)
        {
            string fullMessage = exception != null ? $"{message}: {exception.Message}" : message;
            string stackTrace = exception?.StackTrace;
            Instance.Log(LogLevel.Error, category, fullMessage, stackTrace);
        }

        /// <summary>
        /// Log a critical error message
        /// </summary>
        public static void Critical(string category, string message, Exception exception = null)
        {
            string fullMessage = exception != null ? $"{message}: {exception.Message}" : message;
            string stackTrace = exception?.StackTrace;
            Instance.Log(LogLevel.Critical, category, fullMessage, stackTrace);
        }
        #endregion

        #region Core Logging Logic
        private void Log(LogLevel level, string category, string message, string stackTrace = null)
        {
            // Check if this log level should be recorded
            if (level < minimumLogLevel)
                return;

            // Create log entry
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = level,
                Category = category,
                Message = message,
                StackTrace = stackTrace
            };

            // Add to buffer
            logBuffer.Enqueue(entry);
            if (logBuffer.Count > BUFFER_SIZE)
            {
                FlushLogs();
            }

            // Track error counts
            if (level >= LogLevel.Error)
            {
                string key = $"{category}:{message}";
                if (errorCounts.ContainsKey(key))
                    errorCounts[key]++;
                else
                    errorCounts[key] = 1;
            }

            // Write to console
            if (enableConsoleLogging)
            {
                WriteToConsole(entry);
            }

            // Write to file immediately for errors and critical
            if (level >= LogLevel.Error)
            {
                FlushLogs();
            }
        }

        private void WriteToConsole(LogEntry entry)
        {
            string logMessage = entry.ToString();

            switch (entry.Level)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                case LogLevel.Info:
                    UnityEngine.Debug.Log(logMessage);
                    break;
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(logMessage);
                    break;
                case LogLevel.Error:
                case LogLevel.Critical:
                    UnityEngine.Debug.LogError(logMessage);
                    break;
            }
        }

        private void FlushLogs()
        {
            if (!enableFileLogging || logBuffer.Count == 0)
                return;

            try
            {
                using (StreamWriter writer = File.AppendText(currentLogFile))
                {
                    while (logBuffer.Count > 0)
                    {
                        var entry = logBuffer.Dequeue();
                        writer.WriteLine(entry.ToString());
                    }
                }

                // Check if log file is too large
                FileInfo fileInfo = new FileInfo(currentLogFile);
                if (fileInfo.Length > maxLogFileSize)
                {
                    RotateLogFile();
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Failed to flush logs: {ex.Message}");
            }
        }

        private void RotateLogFile()
        {
            try
            {
                FlushLogs(); // Flush any remaining logs
                
                // Create new log file
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                currentLogFile = Path.Combine(logDirectory, $"acotar_log_{timestamp}.txt");
                
                Info("Logging", "Log file rotated due to size limit");
                
                CleanupOldLogs();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Failed to rotate log file: {ex.Message}");
            }
        }
        #endregion

        #region Statistics & Reporting
        /// <summary>
        /// Get error statistics
        /// </summary>
        public Dictionary<string, int> GetErrorStatistics()
        {
            return new Dictionary<string, int>(errorCounts);
        }

        /// <summary>
        /// Clear error statistics
        /// </summary>
        public void ClearErrorStatistics()
        {
            errorCounts.Clear();
        }

        /// <summary>
        /// Export logs to a specific file
        /// </summary>
        public string ExportLogs()
        {
            try
            {
                FlushLogs();
                string exportPath = Path.Combine(logDirectory, $"export_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                File.Copy(currentLogFile, exportPath, true);
                return exportPath;
            }
            catch (Exception ex)
            {
                Error("Logging", "Failed to export logs", ex);
                return null;
            }
        }

        /// <summary>
        /// Get the current log file path
        /// </summary>
        public string GetCurrentLogPath()
        {
            return currentLogFile;
        }
        #endregion
    }
}
