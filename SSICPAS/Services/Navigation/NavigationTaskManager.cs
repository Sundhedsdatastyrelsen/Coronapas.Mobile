using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.WebServices;
using SSICPAS.Data;
using SSICPAS.Enums;
using SSICPAS.Services.Interfaces;
using SSICPAS.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSICPAS.Services.Navigation
{
    public class NavigationTaskManager : INavigationTaskManager
    {
        public static int SUCCESS_SHOWN_MILLISECONDS = 1000;
        private INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IPlatformSettingsService _platformSettingsService;
        private readonly ILoggingService _loggingService;
        private volatile bool _isInternetDialogInProgress;

        public NavigationTaskManager(INavigationService navigationService, IDialogService dialogService, IPlatformSettingsService platformSettingsService, ILoggingService loggingService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _platformSettingsService = platformSettingsService;
            _loggingService = loggingService;
        }

        /// <param name="response">ApiResponse object.</param>
        /// <param name="silently">Set to true if the user should not be notified of error, e.g. fetching texts.</param>
        /// <returns></returns>
        public Task HandlerErrors(ApiResponse response, bool silently = false)
        {
            if (response.IsSuccessfull)
                return Task.CompletedTask;

            Device.InvokeOnMainThreadAsync(async () =>
            { 
                switch (response.ErrorType)
                {
                    case ServiceErrorType.NoInternetConnection:
                    case ServiceErrorType.BadInternetConnection:
                        // Helps to handle these two errors silently on TextService and RATList endpoints
                        if (silently) return;

                        double delayBetweenAttempts = IoCContainer.Resolve<ISettingsService>().SuspendedFetchingDelaySeconds;
                        DateTime resetLastFetchTimestamp = IoCContainer.Resolve<IDateTimeService>().Now.AddSeconds(0 - delayBetweenAttempts);

                        // Set timer to <delayBetweenAttempts> min back so next call after BadInternet/NoInternet will go to backend
                        IoCContainer.Resolve<IPreferencesService>()
                            .SetUserPreference(
                                PreferencesKeys.LATEST_PASSPORT_CALL_TO_BACKEND_TIMESTAMP, resetLastFetchTimestamp);

                        string title = "INTERNET_CONNECTIVITY_ISSUE_DIALOG_TITLE".Translate();
                        string message = "INTERNET_CONNECTIVITY_ISSUE_DIALOG_MESSAGE".Translate();
                        string okButtonText = "INTERNET_CONNECTIVITY_ISSUE_DIALOG_OK_BUTTON".Translate();
                        string cancelButtonText = "INTERNET_CONNECTIVITY_ISSUE_DIALOG_CANCEL_BUTTON".Translate();

                        if (!_isInternetDialogInProgress)
                        {
                            try
                            {
                                _isInternetDialogInProgress = true;

                                if (!await _dialogService.ShowStyleAlertAsync(title, message, false, false, StackOrientation.Vertical, okButtonText, cancelButtonText, DialogStyle.InternetConnectivityIssue))
                                {
                                    try
                                    {
                                        await _platformSettingsService.OpenWirelessSettings();
                                    }
                                    catch (Exception e)
                                    {
                                        _loggingService.LogException(LogSeverity.WARNING, e, "The native platform wireless settings page could not be accessed from the internet connectivity issue dialog.");
                                    }
                                }
                            }
                            finally
                            {
                                _isInternetDialogInProgress = false;
                            }
                        }
                        break;
                    case ServiceErrorType.Maintenance:
                        if (!silently)
                            await _navigationService.GoToErrorPage(Errors.MaintenanceError);
                        break;
                    case ServiceErrorType.InQueue:
                        if (!silently)
                            await _navigationService.GoToErrorPage(Errors.InQueueError);
                        break;
                    case ServiceErrorType.LockNemID:
                        await _navigationService.GoToErrorPage(Errors.LockError);
                        break;
                    case ServiceErrorType.UserSessionExpired:
                        await _navigationService.GoToErrorPage(Errors.SessionExpiredError);
                        break;
                    case ServiceErrorType.RefreshTokenRenewalFailed:
                        await _navigationService.GoToErrorPage(Errors.SessionRenewalFailedError);
                        break;
                    case ServiceErrorType.Gone:
                        await _navigationService.GoToErrorPage(Errors.ForceUpdateRequiredError);
                        break;
                    default:
                        if (!silently)
                            await _navigationService.GoToErrorPage(Errors.UnknownError);
                        break;
                }
            });
            return Task.CompletedTask;
        }

        public async Task ShowSuccessPage(string message)
        {
            await _navigationService.PushPage(new SuccessPage(), false, PageNavigationStyle.PushInNavigation, message);
            await Task.Delay(SUCCESS_SHOWN_MILLISECONDS);
        }
    }
}
