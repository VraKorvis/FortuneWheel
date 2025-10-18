using System;
using UnityEngine;

namespace Services.Logger
{
    public class UnityConsoleLogger : ILoggerService
    {
        public void Log(string message, LogLevel level = LogLevel.Info)
        {
            switch (level)
            {
                case LogLevel.Info:
                    Debug.Log($"[INFO] {message}");
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning($"[WARN] {message}");
                    break;
                case LogLevel.Error:
                    Debug.LogError($"[ERROR] {message}");
                    break;
            }
        }

        public void LogInfo(string message) => Log(message);
        public void LogWarning(string message) => Log(message, LogLevel.Warning);
        public void LogError(string message) => Log(message, LogLevel.Error);

        public void LogException(Exception ex)
        {
            Debug.LogError($"[EXCEPTION] {ex.Message}\n{ex.StackTrace}");
        }
    }
}