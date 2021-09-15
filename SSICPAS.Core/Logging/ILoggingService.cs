using System;
using System.Collections.Generic;
using SSICPAS.Core.WebServices;

namespace SSICPAS.Core.Logging
{
    public interface ILoggingService
    {
        void LogException(LogSeverity severity, Exception e, string additionalInfo = null, bool crashed = false);
        void LogApiError<T>(LogSeverity severity, ApiResponse<T> apiResponse, string additionalInfo = null);
        void LogMessage(LogSeverity severity, string message, string additionalInfo = null);
        MessageGroup AssignMessageGroupOnSeverity(LogSeverity severity);
        bool ShouldSendLogMessageAfterThrottle(MessageGroup messageGroup);
        void ThrottleLogMessageToSentry(string message, IDictionary<string, string> dict, LogSeverity severity);
        void LogMessageToSentry(string message, IDictionary<string, string> dict, LogSeverity severity);
    }
}
