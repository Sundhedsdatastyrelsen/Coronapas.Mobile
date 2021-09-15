using SSICPAS.Core.Services.Interface;
using SSICPAS.Data;
using SSICPAS.Enums;
using SSICPAS.ViewModels.QrScannerViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.ScannerPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanEuResultView : ContentPage, IScanResultView
    {
        public ScanEuResultView(ITokenPayload payload, EuPassportType passportType)
        {
            InitializeComponent();
            BindingContext = new ScanEuResultViewModel(payload, passportType);

            MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN, ((ScanEuResultViewModel)BindingContext).OnScreenshotTaken);
            MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN_TIMER_ELAPSED, ((ScanEuResultViewModel)BindingContext).OnScreenshotTimerElapsed);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            ((ScanEuResultViewModel) BindingContext).Timer.Enabled = false;
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN);
            MessagingCenter.Unsubscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN_TIMER_ELAPSED);
        }
    }
}