using System;

namespace Services.Logger
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }
    
    public interface ILoggerService
    {
        void Log(string message, LogLevel level = LogLevel.Info);
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);

        void LogException(Exception ex); 
        
    }
}