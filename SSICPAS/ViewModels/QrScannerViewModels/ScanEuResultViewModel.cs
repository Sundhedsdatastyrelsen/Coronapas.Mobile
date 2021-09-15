using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using SSICPAS.Configuration;
using SSICPAS.Core.Services.Interface;
using DCCVersion_1_0_x = SSICPAS.Core.Services.Model.EuDCCModel._1._0._x;
using DCCVersion_1_3_0 = SSICPAS.Core.Services.Model.EuDCCModel._1._3._0;
using SSICPAS.Enums;
using SSICPAS.Models;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Base;
using SSICPAS.ViewModels.Certificates;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.QrScannerViewModels
{
    public class ScanEuResultViewModel : BaseViewModel, IScreenshotDetectorOnResultPage
    {
        public SinglePassportViewModel PassportViewModel { get; set; } = new SinglePassportViewModel();
        public EuPassportType EuPassportType { get; set; }
        
        private const double TimerInterval = 1000;
        private static readonly string ClosesInText = "POPUP_CLOSES_IN".Translate();
        private static readonly string SecondsText = "POPUP_CLOSES_IN_2".Translate();
        private string _fullName;
        private string _dateOfBirth;
        protected double MsRemaining { get; set; } = _settingsService.ScannerEUShownDurationMs;

        public readonly Timer Timer = new Timer();

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                _dateOfBirth = value;
                OnPropertyChanged(nameof(DateOfBirth));
            }
        }

        public string RepeatedText => string.Concat(Enumerable.Repeat($"{"SCANNER_EU_BANNER_TEXT".Translate()}         ", 10));

        public string SecondsRemainingText => $"{ClosesInText} {Math.Truncate(MsRemaining / 1000)} {SecondsText}";
        
        public ICommand ScanAgainCommand => new Command(async () =>
            await ExecuteOnceAsync(async () => await Task.Run(ClosePage)));
        public bool IsTest => EuPassportType == EuPassportType.TEST;
        public bool IsVaccine => EuPassportType == EuPassportType.VACCINE;
        public bool IsRecovery => EuPassportType == EuPassportType.RECOVERY;
        
        public ScanEuResultViewModel(ITokenPayload payload, EuPassportType passportType)
        {
            EuPassportType = passportType;
            Timer.Interval = TimerInterval;
            Timer.Elapsed += TimerOnElapsed;
            Timer.Enabled = true;
            try
            {
                if (payload is DCCVersion_1_0_x.DCCPayload cwt1_0_x)
                {
                    FullName = cwt1_0_x.DCCPayloadData.DCC.PersonName.FullNameTransliteratedReversedWithComma;
                    DateOfBirth = cwt1_0_x.DCCPayloadData.DCC.DateOfBirth.ToLocaleDateFormat(true);
                    PassportViewModel.PassportData = new PassportData(string.Empty, cwt1_0_x, string.Empty);
                    OnPropertyChanged(nameof(PassportViewModel));
                }
                else if (payload is DCCVersion_1_3_0.DCCPayload cwt1_3_0)
                {
                    FullName = cwt1_3_0.DCCPayloadData.DCC.PersonName.FullNameTransliteratedReversedWithComma;
                    DateOfBirth = cwt1_3_0.DCCPayloadData.DCC.DateOfBirth;
                    PassportViewModel.PassportData = new PassportData(string.Empty, cwt1_3_0, string.Empty);
                    OnPropertyChanged(nameof(PassportViewModel));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to initialise TestResult scan: {e}");
            }
        }

        protected virtual void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (MsRemaining <= 0 && Timer.Enabled)
            {
                ClosePage();
            }
            else
            {
                MsRemaining -= Timer.Interval;
                OnPropertyChanged(nameof(SecondsRemainingText));
            }
        }
        
        private void ClosePage()
        {
            Timer.Enabled = false;
            Device.BeginInvokeOnMainThread(async () => { await _navigationService.PopPage(); });
        }

        public void OnScreenshotTaken(object sender)
        {
            Debug.Print($"{nameof(ScanEuResultViewModel)}.{nameof(OnScreenshotTaken)} is called");

            Timer.Enabled = false;
            Timer.Elapsed -= TimerOnElapsed;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await IoCContainer.Resolve<IScreenshotDetectionService>().ShowResultPageScreenshotProtectionDialog();
            });
        }

        public void OnScreenshotTimerElapsed(object sender)
        {
            Debug.Print($"{nameof(ScanEuResultViewModel)}.{nameof(OnScreenshotTimerElapsed)} is called");

            MsRemaining = 0;
            Timer.Enabled = true;
            Timer.Elapsed += TimerOnElapsed;
        }
    }
}

