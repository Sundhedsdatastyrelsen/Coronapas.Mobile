using SSICPAS.Data;
using SSICPAS.ViewModels.QrScannerViewModels;
using Xamarin.Forms;

namespace SSICPAS.Views.ScannerPages
{
    public partial class ScannerErrorPage : ContentPage, IScanResultView
    {
        public ScannerErrorPage()
        {
            InitializeComponent();
            BindingContext = new ScannerErrorViewModel();

            MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN, ((ScannerErrorViewModel)BindingContext).OnScreenshotTaken);
            MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN_TIMER_ELAPSED, ((ScannerErrorViewModel)BindingContext).OnScreenshotTimerElapsed);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            ((ScannerErrorViewModel) BindingContext).Timer.Enabled = false;
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN);
            MessagingCenter.Unsubscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN_TIMER_ELAPSED);
        }

    }
}
