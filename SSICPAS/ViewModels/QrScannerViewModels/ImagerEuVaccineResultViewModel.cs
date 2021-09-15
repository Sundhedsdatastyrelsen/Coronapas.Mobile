using System.Timers;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Enums;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.QrScannerViewModels
{
    public class ImagerEuVaccineResultViewModel : ScanEuResultViewModel
    {
        private const double TimerInterval = 1000;
        
        public ImagerEuVaccineResultViewModel(ITokenPayload payload, EuPassportType passportType) : base(payload, passportType)
        {
        }
        
        public void OnAttachTimer()
        {
            MsRemaining = _settingsService.ScannerEUShownDurationMs;
            Timer.Interval = TimerInterval;
            Timer.Elapsed += TimerOnElapsed;
            Timer.Enabled = true;
        }

        public void OnDetachTimer()
        {
            Timer.Elapsed -= TimerOnElapsed;
            Timer.Enabled = false;
        }

        protected override void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (MsRemaining <= 0 && Timer.Enabled)
            {
                OnDetachTimer();
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
            Device.BeginInvokeOnMainThread(async () => await _navigationService.PopPage()); 
        }
    }
}