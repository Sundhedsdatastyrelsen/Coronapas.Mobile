using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using SSICPAS.Core.Services.Model.DK;
using SSICPAS.Services;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Base;
using Xamarin.Forms;
using SSICPAS.Configuration;
using SSICPAS.Services.Interfaces;
using SSICPAS.Data;
using System.Diagnostics;

namespace SSICPAS.ViewModels.QrScannerViewModels
{
    public class ScanSuccessResultPopupViewModel : BaseViewModel, IScreenshotDetectorOnResultPage
    {
        private const double TimerInterval = 1000;

        private string _successBannerText => "VALID_RESULT_BANNER_TEXT".Translate();
        private string _closesInText => "POPUP_CLOSES_IN".Translate();
        private string _secondsText => "POPUP_CLOSES_IN_2".Translate();

        private double _msRemaining = _settingsService.ScannerSuccessShownDurationMs;

        public Timer Timer = new Timer();

        public string ValidText => "VALID_RESULT_TEXT".Translate();
        public string FullName { get; set; }
        public string DateOfBirth { get; set; }
        public string SuccessBannerText => string.Concat(Enumerable.Repeat($"{_successBannerText}      ", 10));
        public string SecondsRemainingText => $"{_closesInText} {Math.Truncate(_msRemaining / 1000)} {_secondsText}";

        public ICommand ClosePopupCommand => new Command(CloseResultPopup);

        public ScanSuccessResultPopupViewModel()
        {
            Timer.Interval = TimerInterval;
            Timer.Elapsed += TimerOnElapsed;
            Timer.Enabled = true;
            MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN, OnScreenshotTaken);
            MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN_TIMER_ELAPSED, OnScreenshotTimerElapsed);
        }

        ~ScanSuccessResultPopupViewModel()
        {
            MessagingCenter.Unsubscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN);
            MessagingCenter.Unsubscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN_TIMER_ELAPSED);
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (_msRemaining <= 0 && Timer.Enabled)
            {
                CloseResultPopup();
            }
            else
            {
                _msRemaining -= Timer.Interval;
                OnPropertyChanged(nameof(SecondsRemainingText));
            }
        }

        private void CloseResultPopup()
        {
            Timer.Enabled = false;
            PopupNavigation.Instance.PopAllAsync();
        }

        public override Task InitializeAsync(object navigationData)
        {
            if (navigationData is DK2Payload dk2)
            {
                FullName = dk2.LegalName;
                DateOfBirth = dk2.DateOfBirth.ToLocaleDateFormat();

                OnPropertyChanged(nameof(FullName));
                OnPropertyChanged(nameof(DateOfBirth));
            }

            return base.InitializeAsync(navigationData);
        }

        public void OnScreenshotTaken(object sender)
        {
            Debug.Print($"{nameof(ScanSuccessResultPopupViewModel)}.{nameof(OnScreenshotTaken)} is called");

            Timer.Enabled = false;
            Timer.Elapsed -= TimerOnElapsed;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await IoCContainer.Resolve<IScreenshotDetectionService>().ShowResultPageScreenshotProtectionDialog();
            });
        }

        public void OnScreenshotTimerElapsed(object sender)
        {
            Debug.Print($"{nameof(ScanSuccessResultPopupViewModel)}.{nameof(OnScreenshotTimerElapsed)} is called");

            _msRemaining = 0;
            Timer.Enabled = true;
            Timer.Elapsed += TimerOnElapsed;
        }
    }
}