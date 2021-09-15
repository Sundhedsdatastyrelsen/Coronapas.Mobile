using SSICPAS.Configuration;
using SSICPAS.Core.CustomExceptions;
using SSICPAS.Core.Logging;
using SSICPAS.Models.Exceptions;
using SSICPAS.Services.Interfaces;
using System;
using UIKit;

namespace SSICPAS.iOS
{
    public class Application
    {
        static void Main(string[] args)
        {
            try
            {
                UIApplication.Main(args, null, "AppDelegate");
            }
            catch (Exception e)
            {
                var loggingService = IoCContainer.Resolve<ILoggingService>();

                string message;
                LogSeverity logLevel;

                if (e is MissingSettingException)
                {
                    message = $"{nameof(AppDelegate)}.{nameof(Main)}: {e.Message}";
                    logLevel = LogSeverity.FATAL;
                }
                else
                {
                    message = "Caught in AppDelegate";
                    logLevel = LogSeverity.ERROR;
                }

                loggingService.LogException(logLevel, e, message, true);

                if (e is FailedOperationSecureStorageException)
                {
                    _ = IoCContainer.Resolve<IUserService>().UserLogoutAsync(false).Wait(TimeSpan.FromSeconds(5));
                }

                throw;
            }
        }
    }
}
