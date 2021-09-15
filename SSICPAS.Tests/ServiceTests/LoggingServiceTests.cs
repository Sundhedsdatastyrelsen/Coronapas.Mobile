using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SSICPAS.Core.Logging;
using SSICPAS.Core.WebServices;
using SSICPAS.Services;
using SSICPAS.Tests.TestMocks;

namespace SSICPAS.Tests.ServiceTests
{
    public class LoggingServiceTests
    {
        Mock<LoggingService> _loggingServiceMock;
        LoggingService _loggingService => _loggingServiceMock.Object;

        IDictionary<string, string> _loggedDict;
        LogSeverity _loggedSeverity;
        string _loggedMessage;

        string _buildNumber = "14";
        string _buildVersion = "1.0.0";
        string _apiVersion = "v2";

        [SetUp]
        public void Setup()
        {
            MockSettingsService settingsService = new MockSettingsService();
            settingsService.LogErrors = true;
            settingsService.Build = _buildNumber;
            settingsService.Version = _buildVersion;
            settingsService.ApiV = _apiVersion;

            MockRandomService randomService = new MockRandomService();

            _loggingServiceMock = new Mock<LoggingService>(settingsService, randomService);
            _loggingServiceMock.CallBase = true;
            _loggingServiceMock.Setup(x => x.LogMessageToSentry(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<LogSeverity>()))
                .Callback((string message, IDictionary<string, string> dict, LogSeverity severity) =>
                {
                    _loggedDict = dict;
                    _loggedSeverity = severity;
                    _loggedMessage = message;
                }).Verifiable();
        }


        private void MockThrottleLogMessageToSentryCall()
        {
            _loggingServiceMock.Setup(x => x.ThrottleLogMessageToSentry(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<LogSeverity>()))
                .Callback((string message, IDictionary<string, string> dict, LogSeverity severity) =>
                {
                    _loggedDict = dict;
                    _loggedSeverity = severity;
                    _loggedMessage = message;
                }).Verifiable();
        }

        void VerifyThrottleLogMessageWasCalled(Times times)
        {
            _loggingServiceMock.Verify(x => x.ThrottleLogMessageToSentry(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<LogSeverity>()), times);
        }

        void VerifyLogMessageToSentryWasCalled(Times times)
        {
            _loggingServiceMock.Verify(x => x.LogMessageToSentry(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<LogSeverity>()), times);
        }

        [Test]
        public void LoggingService_LogMessage()
        {
            MockThrottleLogMessageToSentryCall();             

            string logMessage = "This is an error";
            LogSeverity logSeverity = LogSeverity.ERROR;
            _loggingService.LogMessage(logSeverity, logMessage);

            VerifyThrottleLogMessageWasCalled(Times.Once());
            Assert.AreEqual(logSeverity, _loggedSeverity);
            Assert.AreEqual(logMessage, _loggedMessage);
            Assert.AreEqual("False", _loggedDict["App crashed"]);
            Assert.AreEqual("ERROR", _loggedDict["Severity"]);
            Assert.AreEqual(_buildVersion, _loggedDict["BuildVersion"]);
            Assert.AreEqual(_buildNumber, _loggedDict["BuildNumber"]);
            Assert.AreEqual(_apiVersion, _loggedDict["ApiVersion"]);
            Assert.IsFalse(_loggedDict.ContainsKey("AdditionalInfo"));

            //When logging something with sensitive data
            logMessage = "This is an error with CPR number 120488-7890";
            string addtionalInfo = "This is more info with phone number 34 56 78 90";
            logSeverity = LogSeverity.WARNING;
            _loggingService.LogMessage(logSeverity, logMessage, addtionalInfo);

            //Then the data is removed before logging it
            VerifyThrottleLogMessageWasCalled(Times.Exactly(2));
            Assert.AreEqual(logSeverity, _loggedSeverity);
            Assert.AreEqual("This is an error with CPR number xxxxxx-xxxx", _loggedMessage);
            Assert.AreEqual("This is more info with phone number xx-xx-xx-xx", _loggedDict["AdditionalInfo"]);
            Assert.AreEqual("False", _loggedDict["App crashed"]);
            Assert.AreEqual("WARNING", _loggedDict["Severity"]);
        }

        [Test]
        public void LoggingService_LogException()
        {
            MockThrottleLogMessageToSentryCall();

            string additionalMessage = "My contextual description";
            string eMessage = "An exception was thrown and logged";
            string eStackTrace = "My test stacktrace";
            TestException e = new TestException(eMessage);
            e.StackTraceString = eStackTrace;

            _loggingService.LogException(LogSeverity.ERROR, e, additionalMessage);

            VerifyThrottleLogMessageWasCalled(Times.Once());
            Assert.AreEqual(LogSeverity.ERROR, _loggedSeverity);
            Assert.AreEqual($"[TestException]: {eMessage}", _loggedMessage);
            Assert.AreEqual("False", _loggedDict["App crashed"]);
            Assert.AreEqual("ERROR", _loggedDict["Severity"]);
            Assert.AreEqual(_buildVersion, _loggedDict["BuildVersion"]);
            Assert.AreEqual(_buildNumber, _loggedDict["BuildNumber"]);
            Assert.AreEqual(_apiVersion, _loggedDict["ApiVersion"]);
            Assert.AreEqual(additionalMessage, _loggedDict["AdditionalInfo"]);
            Assert.AreEqual("TestException", _loggedDict["ExceptionType"]);
            Assert.AreEqual(eMessage, _loggedDict["ExceptionMessage"]);
            Assert.AreEqual(eStackTrace, _loggedDict["ExceptionStackTrace"]);
            Assert.IsFalse(_loggedDict.ContainsKey("InnerExceptionType"));
            Assert.IsFalse(_loggedDict.ContainsKey("InnerExceptionMessage"));
            Assert.IsFalse(_loggedDict.ContainsKey("InnerExceptionStackTrace"));

            //Innerexception + redacted content
            string innerMessage = "The exception has an inner exception. Contact Simon on test@gmail.dk if you get this exception";
            string innerStackTrace = "My inner e stacktrace contains a mac adress 00:22:64:a6:c4:f0. That is not good.";
            TestInnerException innerE = new TestInnerException(innerMessage);
            innerE.StackTraceString = innerStackTrace;

            eMessage = "An exception was thrown and logged with IMEI 490154203237518";
            eStackTrace = "My test <IMG \"\"\"><SCRIPT>alert(\"XSS\")</SCRIPT>\"\\> stacktrace";
            e = new TestException(eMessage, innerE);
            e.StackTraceString = eStackTrace;

            additionalMessage = "User with CPR 2309783405 used the app wrong.";
            _loggingService.LogException(LogSeverity.INFO, e, additionalMessage, true);

            VerifyThrottleLogMessageWasCalled(Times.Exactly(2));
            Assert.AreEqual(LogSeverity.INFO, _loggedSeverity);
            Assert.AreEqual($"[TestException]: An exception was thrown and logged with IMEI xxxxxxxxxxxxxxx", _loggedMessage);
            Assert.AreEqual("True", _loggedDict["App crashed"]);
            Assert.AreEqual("INFO", _loggedDict["Severity"]);
            Assert.AreEqual("User with CPR xxxxxxxxxx used the app wrong.", _loggedDict["AdditionalInfo"]);
            Assert.AreEqual("TestException", _loggedDict["ExceptionType"]);
            Assert.AreEqual("An exception was thrown and logged with IMEI xxxxxxxxxxxxxxx", _loggedDict["ExceptionMessage"]);
            Assert.AreEqual("My test &lt;img&gt;&amp;quot;\\&amp;gt; stacktrace", _loggedDict["ExceptionStackTrace"]);
            Assert.AreEqual("TestInnerException", _loggedDict["InnerExceptionType"]);
            Assert.AreEqual("The exception has an inner exception. Contact Simon on ****@*****.dk if you get this exception", _loggedDict["InnerExceptionMessage"]);
            Assert.AreEqual("My inner e stacktrace contains a mac adress xx:xx:xx:xx. That is not good.", _loggedDict["InnerExceptionStackTrace"]);

            // Log the same error but without crash
            _loggingService.LogException(LogSeverity.INFO, e, additionalMessage, false);

            VerifyThrottleLogMessageWasCalled(Times.Exactly(3));
            Assert.AreEqual(LogSeverity.INFO, _loggedSeverity); // INFO because the app has crashed
            Assert.AreEqual($"[TestException]: An exception was thrown and logged with IMEI xxxxxxxxxxxxxxx", _loggedMessage);
            Assert.AreEqual("False", _loggedDict["App crashed"]);
            Assert.AreEqual("INFO", _loggedDict["Severity"]);
            Assert.AreEqual("User with CPR xxxxxxxxxx used the app wrong.", _loggedDict["AdditionalInfo"]);
            Assert.AreEqual("TestException", _loggedDict["ExceptionType"]);
            Assert.AreEqual("An exception was thrown and logged with IMEI xxxxxxxxxxxxxxx", _loggedDict["ExceptionMessage"]);
            Assert.AreEqual("My test &lt;img&gt;&amp;quot;\\&amp;gt; stacktrace", _loggedDict["ExceptionStackTrace"]);
            Assert.AreEqual("TestInnerException", _loggedDict["InnerExceptionType"]);
            Assert.AreEqual("The exception has an inner exception. Contact Simon on ****@*****.dk if you get this exception", _loggedDict["InnerExceptionMessage"]);
            Assert.AreEqual("My inner e stacktrace contains a mac adress xx:xx:xx:xx. That is not good.", _loggedDict["InnerExceptionStackTrace"]);
        }

        [Test]
        public void LoggingService_LogApiError()
        {
            MockThrottleLogMessageToSentryCall();

            string message = "My contextual description";
            string reasonPhraseFromService = "Something failed";
            string apiMethod = "GetPassports";
            ApiResponse<bool> apiResponse = new ApiResponse<bool>($"https://myendpoint.dk/api/v1/{apiMethod}")
            {
                StatusCode = 500,
                ResponseText = reasonPhraseFromService
            };
            _loggingService.LogApiError(LogSeverity.WARNING, apiResponse, message);

            VerifyThrottleLogMessageWasCalled(Times.Once());
            Assert.AreEqual(LogSeverity.WARNING, _loggedSeverity);
            Assert.AreEqual($"API GetPassports failed with code 500 (My contextual description)", _loggedMessage);
            Assert.AreEqual(apiMethod, _loggedDict["API"]);
            Assert.AreEqual("500", _loggedDict["ApiErrorCode"]);
            Assert.AreEqual(reasonPhraseFromService, _loggedDict["ApiErrorMessage"]);

            Assert.AreEqual("False", _loggedDict["App crashed"]);
            Assert.AreEqual("WARNING", _loggedDict["Severity"]);
            Assert.AreEqual(_buildVersion, _loggedDict["BuildVersion"]);
            Assert.AreEqual(_buildNumber, _loggedDict["BuildNumber"]);
            Assert.AreEqual(_apiVersion, _loggedDict["ApiVersion"]);
            Assert.IsFalse(_loggedDict.ContainsKey("AdditionalInfo"));
            Assert.IsFalse(_loggedDict.ContainsKey("ExceptionType"));
            Assert.IsFalse(_loggedDict.ContainsKey("ExceptionMessage"));
            Assert.IsFalse(_loggedDict.ContainsKey("ExceptionStackTrace"));
            Assert.IsFalse(_loggedDict.ContainsKey("InnerExceptionType"));
            Assert.IsFalse(_loggedDict.ContainsKey("InnerExceptionMessage"));
            Assert.IsFalse(_loggedDict.ContainsKey("InnerExceptionStackTrace"));


            string eMessage = "My yes.yes@netcompany.dk description";
            string script =
                "javascript:/*--></title></style></textarea></script></xmp><svg/onload='+/\"/+/onmouseover=1/+/[*/[]/+alert(1)//'>";
            TestException e = new TestException(eMessage);
            e.StackTraceString = script;

            reasonPhraseFromService = "Something failed hard. Probably because of this MAC adress: 00-22-64-a6-c4-f0";
            apiMethod = "GetSomething";
            apiResponse = new ApiResponse<bool>($"https://myendpoint.dk/api/v1/{apiMethod}")
            {
                StatusCode = 404,
                ResponseText = reasonPhraseFromService,
                Exception = e
            };
            _loggingService.LogApiError(LogSeverity.ERROR, apiResponse);

            VerifyThrottleLogMessageWasCalled(Times.Exactly(2));
            Assert.AreEqual(LogSeverity.ERROR, _loggedSeverity);
            Assert.AreEqual($"API GetSomething failed with code 404", _loggedMessage);
            Assert.AreEqual(apiMethod, _loggedDict["API"]);
            Assert.AreEqual("404", _loggedDict["ApiErrorCode"]);
            Assert.AreEqual("Something failed hard. Probably because of this MAC adress: xx:xx:xx:xx", _loggedDict["ApiErrorMessage"]);

            Assert.AreEqual("False", _loggedDict["App crashed"]);
            Assert.AreEqual("ERROR", _loggedDict["Severity"]);
            Assert.AreEqual("TestException", _loggedDict["ExceptionType"]);
            Assert.AreEqual("My *******@**********.dk description", _loggedDict["ExceptionMessage"]);
            Assert.AreEqual("javascript:/*--&amp;gt;", _loggedDict["ExceptionStackTrace"]);
        }

        [TestCase(LogSeverity.INFO, MessageGroup.BUSINESS)]
        [TestCase(LogSeverity.ERROR, MessageGroup.BLOCKING)]
        [TestCase(LogSeverity.WARNING, MessageGroup.TRANSIENT)]
        [TestCase(LogSeverity.SECURITY_ERROR, MessageGroup.SECURITY)]
        [TestCase(LogSeverity.SECURITY_WARNING, MessageGroup.TRANSIENT)]
        public void AssignMessageGroupOnSeverityTest(LogSeverity input, MessageGroup output)
        {
            Assert.AreEqual(_loggingService.AssignMessageGroupOnSeverity(input), output);
        }

        /// <summary>
        /// MockedRandomService returns 0.5 always, while in real setting this is randomly generated.
        /// Based on the throttle factor settings, the log message will be send or not.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="shouldSend"></param>
        [TestCase(MessageGroup.BUSINESS, false)]
        [TestCase(MessageGroup.TRANSIENT, false)]
        [TestCase(MessageGroup.BLOCKING, true)]
        [TestCase(MessageGroup.SECURITY, true)]
        public void ShouldSendLogMessageAfterThrottleTest(MessageGroup input, bool shouldSend)
        {
            Assert.AreEqual(_loggingService.ShouldSendLogMessageAfterThrottle(input), shouldSend);
        }

        [TestCase(LogSeverity.SECURITY_ERROR, 1)]
        [TestCase(LogSeverity.SECURITY_WARNING, 0)]
        [TestCase(LogSeverity.ERROR, 1)]
        [TestCase(LogSeverity.WARNING, 0)]
        [TestCase(LogSeverity.INFO, 0)]
        public void ThrottlingSentryLoggingIntegrationTests_LogApiError(LogSeverity severity, int timesLogSentToSentry)
        {
            string message = "My contextual description";
            string reasonPhraseFromService = "Something failed";
            string apiMethod = "GetPassports";
            ApiResponse<bool> apiResponse = new ApiResponse<bool>($"https://myendpoint.dk/api/v1/{apiMethod}")
            {
                StatusCode = 500,
                ResponseText = reasonPhraseFromService
            };

            _loggingService.LogApiError(severity, apiResponse, message);

            VerifyLogMessageToSentryWasCalled(Times.Exactly(timesLogSentToSentry));
        }

        [TestCase(LogSeverity.SECURITY_ERROR, 1, false)]
        [TestCase(LogSeverity.SECURITY_WARNING, 0, false)]
        [TestCase(LogSeverity.ERROR, 1, false)]
        [TestCase(LogSeverity.ERROR, 1, true)]
        [TestCase(LogSeverity.WARNING, 0, false)]
        [TestCase(LogSeverity.INFO, 0, false)]
        public void ThrottlingSentryLoggingIntegrationTests_LogException(
            LogSeverity severity,
            int timesLogSentToSentry,
            bool crashed)
        {
            string additionalMessage = "My contextual description";
            string eMessage = "An exception was thrown and logged";
            string eStackTrace = "My test stacktrace";
            TestException e = new TestException(eMessage);
            e.StackTraceString = eStackTrace;

            _loggingService.LogException(severity, e, additionalMessage, crashed);

            VerifyLogMessageToSentryWasCalled(Times.Exactly(timesLogSentToSentry));
        }

        [TestCase(LogSeverity.SECURITY_ERROR, 1)]
        [TestCase(LogSeverity.SECURITY_WARNING, 0)]
        [TestCase(LogSeverity.ERROR, 1)]
        [TestCase(LogSeverity.WARNING, 0)]
        [TestCase(LogSeverity.INFO, 0)]
        public void ThrottlingSentryLoggingIntegrationTests_LogMessage(LogSeverity severity, int timesLogSentToSentry)
        {
            string logMessage = "This is a log message";
            _loggingService.LogMessage(severity, logMessage);

            VerifyLogMessageToSentryWasCalled(Times.Exactly(timesLogSentToSentry));
        }

        [TestCase(LogSeverity.FATAL, 1)]
        public void VerifyFATALSeverityMessageIsAlwaysLogged(
            LogSeverity severity,
            int timesLogSentToSentry)
        {
            string additionalMessage = "My contextual description";
            string eMessage = "An exception was thrown and logged";
            string eStackTrace = "My test stacktrace";
            TestException e = new TestException(eMessage);
            e.StackTraceString = eStackTrace;

            _loggingService.LogException(severity, e, additionalMessage, true);

            VerifyThrottleLogMessageWasCalled(Times.Never());
            VerifyLogMessageToSentryWasCalled(Times.Exactly(timesLogSentToSentry));
        }
    }
}
