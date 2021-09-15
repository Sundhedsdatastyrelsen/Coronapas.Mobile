using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using SSICPAS.Configuration;
using SSICPAS.Core.Services.Enum;
using SSICPAS.Core.Services.Model;
using SSICPAS.Core.Services.Model.DK;
using SSICPAS.Core.Services.Model.EuDCCModel._1._0._x;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Base;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.QrScannerViewModels
{
    public class ScannerErrorViewModel : BaseViewModel, IScreenshotDetectorOnResultPage
    {
        private const double TimerInterval = 1000;
        private string ClosesInText => "POPUP_CLOSES_IN".Translate();
        private string SecondsText => "POPUP_CLOSES_IN_2".Translate();

        public readonly Timer Timer = new Timer();

        public string PageTitle => ShowInvalidPage ? "SCANNER_ERROR_INVALID_TITLE".Translate() : "SCANNER_ERROR_EXPIRED_TITLE".Translate();
        public string PageBannerTitle => ShowInvalidPage ? "SCANNER_ERROR_INVALID_BANNER".Translate() : "SCANNER_ERROR_EXPIRED_BANNER".Translate();
        public bool ShowInvalidPage => TokenValidateResultModel.ValidationResult == TokenValidateResult.Invalid;
        public TokenValidateResultModel TokenValidateResultModel { get; set; } = new TokenValidateResultModel();
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public double MsRemaining { get; set; } = _settingsService.ScannerInvalidShownDurationMs;
        public string SecondsRemainingText => $"{ClosesInText} {Math.Truncate(MsRemaining / 1000)} {SecondsText}";
        public string RepeatedText => string.Concat(Enumerable.Repeat(PageBannerTitle.PadLeft(20), 10));
        public ICommand ScanAgainCommand => new Command(async () =>
            await ExecuteOnceAsync(async () => await Task.Run(CloseScanningErrorPage)));

        public ScannerErrorViewModel()
        {
            Timer.Interval = TimerInterval;
            Timer.Elapsed += TimerOnElapsed;
            Timer.Enabled = true;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (MsRemaining <= 0 && Timer.Enabled)
            {
                CloseScanningErrorPage();
            }
            else
            {
                MsRemaining -= Timer.Interval;
                OnPropertyChanged(nameof(SecondsRemainingText));
            }
        }

        private void CloseScanningErrorPage()
        {
            Timer.Enabled = false;
            Device.BeginInvokeOnMainThread(async () => { await _navigationService.PopPage(); });
        }

        public override Task InitializeAsync(object navigationData)
        {
            if (!(navigationData is TokenValidateResultModel tokenValidateResultModel))
                return base.InitializeAsync(navigationData);

            TokenValidateResultModel = tokenValidateResultModel;

            switch (tokenValidateResultModel.DecodedModel)
            {
                case DCCPayload cwt:
                    DateOfBirth = cwt.DCCPayloadData.DCC.DateOfBirth.ToLocaleDateFormat(true);
                    Name = cwt.DCCPayloadData.DCC.PersonName.FullNameTransliteratedReversedWithComma;
                    break;
                case DK2Payload dk2:
                    DateOfBirth = dk2.DateOfBirth.ToLocaleDateFormat();
                    Name = dk2.LegalName;
                    break;
            }

            OnPropertyChanged(nameof(TokenValidateResultModel));
            OnPropertyChanged(nameof(ShowInvalidPage));
            OnPropertyChanged(nameof(PageTitle));
            OnPropertyChanged(nameof(PageBannerTitle));
            OnPropertyChanged(nameof(RepeatedText));
            OnPropertyChanged(nameof(DateOfBirth));
            OnPropertyChanged(nameof(Name));

            return base.InitializeAsync(navigationData);
        }

        public void OnScreenshotTaken(object sender)
        {
            Debug.Print($"{nameof(ScannerErrorViewModel)}.{nameof(OnScreenshotTaken)} is called");

            Timer.Enabled = false;
            Timer.Elapsed -= TimerOnElapsed;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await IoCContainer.Resolve<IScreenshotDetectionService>().ShowResultPageScreenshotProtectionDialog();
            });
        }

        public void OnScreenshotTimerElapsed(object sender)
        {
            Debug.Print($"{nameof(ScannerErrorViewModel)}.{nameof(OnScreenshotTimerElapsed)} is called");

            MsRemaining = 0;
            Timer.Enabled = true;
            Timer.Elapsed += TimerOnElapsed;
        }
    }
}