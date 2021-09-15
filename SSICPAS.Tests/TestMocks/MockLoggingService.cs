using System;
using System.Collections.Generic;
using SSICPAS.Core.Logging;
using SSICPAS.Core.WebServices;

namespace SSICPAS.Tests.TestMocks
{
    public class MockLoggingService: ILoggingService
    {
        public void LogException(LogSeverity severity, Exception e, string additionalInfo = null, bool crashed = false)
        {
        }

        public void LogApiError<T>(LogSeverity severity, ApiResponse<T> apiResponse, string additionalInfo = null)
        {
        }

        public void LogMessage(LogSeverity severity, string message, string additionalInfo = null)
        {
        }

        public MessageGroup AssignMessageGroupOnSeverity(LogSeverity severity)
        {
            return MessageGroup.TRANSIENT;
        }

        public bool ShouldSendLogMessageAfterThrottle(MessageGroup messageGroup)
        {
            return true;
        }

        public void ThrottleLogMessageToSentry(string message, IDictionary<string, string> dict, LogSeverity severity)
        {
        }

        public void LogMessageToSentry(string message, IDictionary<string, string> dict, LogSeverity severity)
        {
        }
    }
}