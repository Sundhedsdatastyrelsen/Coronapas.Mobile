using SSICPAS.Configuration;
using SSICPAS.Data;
using SSICPAS.Services.Interfaces;
using System;
using System.Threading.Tasks;
using SSICPAS.Core.Services.Interface;
using System.Globalization;
using SSICPAS.Core.Data;

#nullable enable

namespace SSICPAS.Services
{
    class ScreenshotDetectionService : IScreenshotDetectionService
    {
        private const int screenLockTimeout = 300;

        private readonly IDialogService _dialogService;
        private readonly IDateTimeService _dateTimeService;
        private readonly ISecureStorageService<string> _secureStorageService;

        public ScreenshotDetectionService(
            IDialogService dialogService,
            IDateTimeService dateTimeService,
            ISecureStorageService<string> secureStorageService)
        {
            _dialogService = dialogService;
            _dateTimeService = dateTimeService;
            _secureStorageService = secureStorageService;
        }

        public ScreenshotDetectionService()
            : this( IoCContainer.Resolve<IDialogService>(),
                    IoCContainer.Resolve<IDateTimeService>(),
                    IoCContainer.Resolve <ISecureStorageService<string>>())
        {

        }

        public async Task ShowPassportPageScreenshotProtectionDialog(int lockForSeconds = 0, bool setTimestamp = true)
        {
            if (setTimestamp)
            {
                await SetPassportPageScreenshotTimestamp(_dateTimeService.Now.ToString(CultureInfo.InvariantCulture));
            }

            bool lockScreen = lockForSeconds > 0 ? true : await GetPassportPageShouldLockScreen();

            if (lockScreen && lockForSeconds == 0)
            {
                lockForSeconds = screenLockTimeout;
            }

            string title = "SCREENSHOT_DETECTED_DIALOG_HEADER".Translate();
            string body = !lockScreen ? "SCREENSHOT_DETECTED_PASSPORT_PAGE_FIRST_TIME".Translate()
                                    : "SCREENSHOT_DETECTED_PASSPORT_PAGE_SECOND_TIME".Translate();
            string buttonText = "SCREENSHOT_DETECTED_DISMISS_BUTTON".Translate();

            await _dialogService.ShowTimerAlertWithoutTouchOutsideAsync(title, body, buttonText, Enums.DialogStyle.Info, lockForSeconds);
        }

        public async Task ShowResultPageScreenshotProtectionDialog(int lockForSeconds = 0, bool setTimestamp = true)
        {
            if (setTimestamp)
            {
                await SetResultPageScreenshotTimestamp(_dateTimeService.Now.ToString(CultureInfo.InvariantCulture));
            }

            bool lockScreen = lockForSeconds > 0 ? true : await GetResultPageShouldLockScreen();

            if (lockScreen && lockForSeconds == 0)
            {
                lockForSeconds = screenLockTimeout;
            }

            string title = "SCREENSHOT_DETECTED_DIALOG_HEADER".Translate();
            string body = !lockScreen ? "SCREENSHOT_DETECTED_RESULT_PAGE_FIRST_TIME".Translate()
                                    : "SCREENSHOT_DETECTED_RESULT_PAGE_SECOND_TIME".Translate();
            string buttonText = "SCREENSHOT_DETECTED_DISMISS_BUTTON".Translate();

            await _dialogService.ShowTimerAlertWithoutTouchOutsideAsync(title, body, buttonText, Enums.DialogStyle.Info, lockForSeconds);
        }

        public async Task ShowScannerPageScreenshotProtectionDialog(int lockForSeconds = 0, bool setTimeStamp = true)
        {
            if (setTimeStamp)
            {
                await SetScannerPageScreenshotTimestamp(_dateTimeService.Now.ToString(CultureInfo.InvariantCulture));
            }

            bool lockScreen = lockForSeconds > 0 ? true : await GetScannerPageShouldLockScreen();

            if (lockScreen && lockForSeconds == 0)
            {
                lockForSeconds = screenLockTimeout;
            }

            string title = "SCREENSHOT_DETECTED_DIALOG_HEADER".Translate();
            string body = !lockScreen ? "SCREENSHOT_DETECTED_SCANNER_PAGE_FIRST_TIME".Translate()
                                    : "SCREENSHOT_DETECTED_SCANNER_PAGE_SECOND_TIME".Translate();
            string buttonText = "SCREENSHOT_DETECTED_DISMISS_BUTTON".Translate();

            await _dialogService.ShowTimerAlertWithoutTouchOutsideAsync(title, body, buttonText, Enums.DialogStyle.Info, lockForSeconds);
        }

        public async Task StartupShowScreenshotProtectionDialog(bool scannerOnly = false)
        {
            if (await StartupShowPassportPageScreenshotProtectionDialog())
            {
                return;
            }
            else if (scannerOnly && await StartupShowResultPageScreenshotProtectionDialog())
            {
                return;
            }
            else if (scannerOnly)
            {
                await StartupShowScannerPageScreenshotProtectionDialog();
            }
        }

        private async Task<string?> GetPassportPageScreenshotTimestamp()
        {
            return await GetScreenshotTimestamp(SecureStorageKeys.PASSPORT_SCREENSHOT_TIMESTAMP);
        }

        private async Task SetPassportPageScreenshotTimestamp(string timeStamp)
        {
            if (!string.IsNullOrEmpty(await GetScreenshotTimestamp(SecureStorageKeys.PASSPORT_SCREENSHOT_TIMESTAMP)))
            {
                await SetShouldLockScreen(SecureStorageKeys.PASSPORT_SCREENSHOT_LOCK);
            }

            await SetScreenshotTimestamp(SecureStorageKeys.PASSPORT_SCREENSHOT_TIMESTAMP, timeStamp);
        }
        
        private async Task<bool> GetPassportPageShouldLockScreen()
        {
            return await GetShouldLockScreen(SecureStorageKeys.PASSPORT_SCREENSHOT_LOCK);
        }

        private async Task<string?> GetResultPageScreenshotTimestamp()
        {
            return await GetScreenshotTimestamp(SecureStorageKeys.RESULT_SCREENSHOT_TIMESTAMP);
        }

        private async Task SetResultPageScreenshotTimestamp(string timeStamp)
        {
            if (!string.IsNullOrEmpty(await GetScreenshotTimestamp(SecureStorageKeys.RESULT_SCREENSHOT_TIMESTAMP)))
            {
                await SetShouldLockScreen(SecureStorageKeys.RESULT_SCREENSHOT_LOCK);
            }

            await SetScreenshotTimestamp(SecureStorageKeys.RESULT_SCREENSHOT_TIMESTAMP, timeStamp);
        }

        private async Task<bool> GetResultPageShouldLockScreen()
        {
            return await GetShouldLockScreen(SecureStorageKeys.RESULT_SCREENSHOT_LOCK);
        }

        private async Task<string?> GetScannerPageScreenshotTimestamp()
        {
            return await GetScreenshotTimestamp(SecureStorageKeys.SCANNER_SCREENSHOT_TIMESTAMP);
        }

        private async Task SetScannerPageScreenshotTimestamp(string timeStamp)
        {
            if (!string.IsNullOrEmpty(await GetScreenshotTimestamp(SecureStorageKeys.SCANNER_SCREENSHOT_TIMESTAMP)))
            {
                await SetShouldLockScreen(SecureStorageKeys.SCANNER_SCREENSHOT_LOCK);
            }

            await SetScreenshotTimestamp(SecureStorageKeys.SCANNER_SCREENSHOT_TIMESTAMP, timeStamp);
        }

        private async Task<bool> GetScannerPageShouldLockScreen()
        {
            return await GetShouldLockScreen(SecureStorageKeys.SCANNER_SCREENSHOT_LOCK);
        }

        private async Task<string?> GetScreenshotTimestamp(string secureStorageKey)
        {
            return await _secureStorageService.GetSecureStorageAsync(secureStorageKey);
        }

        private async Task<bool> SetScreenshotTimestamp(string secureStorageKey, string value)
        {   
            await _secureStorageService.SetSecureStorageAsync(secureStorageKey, value);
            return true;
        }

        private async Task<bool> GetShouldLockScreen(string secureStorageKey)
        {
            string shouldLockString = await _secureStorageService.GetSecureStorageAsync(secureStorageKey);

            if (!string.IsNullOrEmpty(shouldLockString))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> SetShouldLockScreen(string secureStorageKey)
        {
            await _secureStorageService.SetSecureStorageAsync(secureStorageKey, Boolean.TrueString);
            return true;
        }

        private async Task<bool> StartupShowPassportPageScreenshotProtectionDialog()
        {
            DateTime currentDateTime = _dateTimeService.Now;
            DateTime screenshotDateTime;

            if (DateTime.TryParse(await GetPassportPageScreenshotTimestamp(), CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal, out screenshotDateTime))
            {
                int secondsElapsed = (int)(currentDateTime - screenshotDateTime).TotalSeconds;

                if (await GetPassportPageShouldLockScreen() && secondsElapsed <= screenLockTimeout)
                {
                    int lockForSeconds = screenLockTimeout - secondsElapsed;
                    await ShowPassportPageScreenshotProtectionDialog(lockForSeconds, false);
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> StartupShowResultPageScreenshotProtectionDialog()
        {
            DateTime currentDateTime = _dateTimeService.Now;
            DateTime screenshotDateTime;

            if (DateTime.TryParse(await GetResultPageScreenshotTimestamp(), CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal, out screenshotDateTime))
            {
                int secondsElapsed = (int)(currentDateTime - screenshotDateTime).TotalSeconds;

                if (await GetResultPageShouldLockScreen() && secondsElapsed <= screenLockTimeout)
                {
                    int lockForSeconds = screenLockTimeout - secondsElapsed;
                    await ShowResultPageScreenshotProtectionDialog(lockForSeconds, false);
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> StartupShowScannerPageScreenshotProtectionDialog()
        {
            DateTime currentDateTime = _dateTimeService.Now;
            DateTime screenshotDateTime;

            if (DateTime.TryParse(await GetScannerPageScreenshotTimestamp(), CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal, out screenshotDateTime))
            {
                int secondsElapsed = (int)(currentDateTime - screenshotDateTime).TotalSeconds;

                if (await GetScannerPageShouldLockScreen() && secondsElapsed <= screenLockTimeout)
                {
                    int lockForSeconds = screenLockTimeout - secondsElapsed;
                    await ShowScannerPageScreenshotProtectionDialog(lockForSeconds, false);
                    return true;
                }
            }

            return false;
        }
    }
}
