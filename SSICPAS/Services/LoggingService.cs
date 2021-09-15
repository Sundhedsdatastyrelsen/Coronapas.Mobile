using System;
using System.Collections.Generic;
using System.Linq;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.WebServices;
using SSICPAS.Core.Services.RandomService;
using Sentry;
using System.Diagnostics;

namespace SSICPAS.Services
{
    public class LoggingService : ILoggingService
    {
        ISettingsService _settingsService;
        IRandomService _randomService;
        bool _shouldLogErrors;

        public LoggingService(ISettingsService settingsService, IRandomService randomService = null)
        {
            _settingsService = settingsService;
            _shouldLogErrors = settingsService.ShouldLogErrors;
            _randomService = (randomService == null) ? new RandomService() : randomService;
        }

        public void LogApiError<T>(LogSeverity severity, ApiResponse<T> apiResponse, string message = null)
        {
            var dict = GetLogInfo(severity);
            AddExceptionInfo(dict, apiResponse.Exception);
            dict.Add("App crashed", false.ToString());

            dict.Add("API", apiResponse.Endpoint);

            var errorCode = apiResponse.StatusCode > 0 ? apiResponse.StatusCode.ToString() : "";
            dict.Add("ApiErrorCode", errorCode);

            var errorMessage = (new string[] { "200", "201" }).Contains(errorCode) ? "" : apiResponse.ResponseText;
            dict.Add("ApiErrorMessage", Anonymizer.RedactText(errorMessage));

            string logMessage = $"API {apiResponse.Endpoint} failed";
            if (apiResponse.StatusCode != 0)
            {
                logMessage += $" with code { apiResponse.StatusCode}";
            }
            if (!string.IsNullOrWhiteSpace(message))
            {
                logMessage += $" ({Anonymizer.RedactText(message)})";
            }

            if (_shouldLogErrors)
            {
                ThrottleLogMessageToSentry(logMessage, dict, severity);
            }

            PrettyPrintDictToConsole(dict, severity, logMessage);
        }

        public void LogException(LogSeverity severity, Exception e, string additionalInfo = null, bool crashed = false)
        {
            var dict = GetLogInfo(severity);
            AddExceptionInfo(dict, e);

            dict.Add("App crashed", crashed.ToString());

            if (!string.IsNullOrWhiteSpace(additionalInfo))
                dict.Add("AdditionalInfo", Anonymizer.RedactText(additionalInfo));

            string message = $"[{e.GetType().Name}]: {Anonymizer.RedactText(e.Message)}";

            if (severity is LogSeverity.FATAL)
            {
                LogMessageToSentry(message, dict, severity);
                PrettyPrintDictToConsole(dict, severity, message);
                return;
            }

            if (_shouldLogErrors)
            {
                ThrottleLogMessageToSentry(message, dict, severity);
            }

            PrettyPrintDictToConsole(dict, severity, message);
        }

        public void LogMessage(LogSeverity severity, string message, string additionalInfo = null)
        {
            var dict = GetLogInfo(severity);
            dict.Add("App crashed", false.ToString());

            if (!string.IsNullOrWhiteSpace(additionalInfo))
                dict.Add("AdditionalInfo", Anonymizer.RedactText(additionalInfo));

            string logMessage = Anonymizer.RedactText(message);

            if (_shouldLogErrors)
            {
                ThrottleLogMessageToSentry(logMessage, dict, severity);
            }

            PrettyPrintDictToConsole(dict, severity, logMessage);
        }

        public virtual void ThrottleLogMessageToSentry(string message, IDictionary<string, string> dict, LogSeverity severity)
        {
            // Assign business value by assigning a message group
            MessageGroup messageGroup = AssignMessageGroupOnSeverity(severity);

            // Apply throttling factor configured in setting based on MessageGroup
            bool shouldSendLogMessage = ShouldSendLogMessageAfterThrottle(messageGroup);

            // Abort if the message was not selected for sending to Sentry
            if (!shouldSendLogMessage)
            {
                return;
            }

            LogMessageToSentry(message, dict, severity);
        }

        public virtual void LogMessageToSentry(string message, IDictionary<string, string> dict, LogSeverity severity)
        {
            SentrySdk.ConfigureScope(scope =>
            {
                scope.Contexts["APP INFO"] = dict;
            });
            SentrySdk.CaptureMessage(message, LogSeverityToSentryLevel(severity));
            Debug.Print($"Captured in Sentry: [{severity}] {message}");
        }

        private SentryLevel LogSeverityToSentryLevel(LogSeverity severity)
        {
            switch (severity)
            {
                case LogSeverity.INFO:
                    return SentryLevel.Info;
                case LogSeverity.WARNING:
                case LogSeverity.SECURITY_WARNING:
                    return SentryLevel.Warning;
                case LogSeverity.ERROR:
                case LogSeverity.SECURITY_ERROR:
                    return SentryLevel.Error;
                case LogSeverity.FATAL:
                    return SentryLevel.Fatal;
                default:
                    return SentryLevel.Info;
            }
        }

        void PrettyPrintDictToConsole(IDictionary<string, string> logObj, LogSeverity severity, string message)
        {
            try
            {
                Debug.Print($"Logged {severity.ToString()} with message '{message}'");
                Debug.Print("{");
                foreach (KeyValuePair<string, string> kvp in logObj)
                {
                    Debug.Print("   {0}: {1}", kvp.Key, kvp.Value);
                }
                Debug.Print("}");

            }
            catch (Exception e)
            {
                Debug.Print("Tried to log, but failed to print log to console");
                Debug.Print(e.ToString());
            }
        }

        public MessageGroup AssignMessageGroupOnSeverity(LogSeverity severity)
        {
            switch (severity)
            {
                case LogSeverity.INFO:
                    return MessageGroup.BUSINESS;

                case LogSeverity.SECURITY_ERROR:
                    return MessageGroup.SECURITY;

                case LogSeverity.ERROR:
                    return MessageGroup.BLOCKING;

                case LogSeverity.WARNING:
                case LogSeverity.SECURITY_WARNING:
                default:
                    return MessageGroup.TRANSIENT;
            }
        }

        public bool ShouldSendLogMessageAfterThrottle(MessageGroup messageGroup)
        {
            double chance = _randomService.GenerateRandomDouble();
            switch (messageGroup)
            {
                case MessageGroup.SECURITY:
                    return chance < _settingsService.SecurityGroupLogThrottleFactor;
                case MessageGroup.BLOCKING:
                    return chance < _settingsService.BlockingGroupLogThrottleFactor;
                case MessageGroup.BUSINESS:
                    return chance < _settingsService.BusinessGroupLogThrottleFactor;
                case MessageGroup.TRANSIENT:
                default:
                    return chance < _settingsService.TransientGroupLogThrottleFactor;
            }
        }

        private IDictionary<string, string> GetLogInfo(LogSeverity sevrity)
        {
            return new Dictionary<string, string>
            {
                //Mandatory information 
                { "Severity", sevrity.ToString() },
                { "BuildVersion", _settingsService.VersionString },
                { "BuildNumber", _settingsService.BuildString },
                { "ApiVersion", _settingsService.ApiVersion }
            };
        }

        private void AddExceptionInfo(IDictionary<string, string> dict, Exception e)
        {
            if (e != null)
            {
                dict.Add("ExceptionType", e.GetType().Name);
                dict.Add("ExceptionMessage", Anonymizer.RedactText(e.Message));
                dict.Add("ExceptionStackTrace", Anonymizer.RedactText(e.StackTrace));
            }
            
            Exception innerE = e?.InnerException;
            if (innerE != null)
            {
                dict.Add("InnerExceptionType", innerE.GetType().Name);
                dict.Add("InnerExceptionMessage", Anonymizer.RedactText(innerE.Message));
                dict.Add("InnerExceptionStackTrace", Anonymizer.RedactText(innerE.StackTrace));
            }
        }
    }
}
