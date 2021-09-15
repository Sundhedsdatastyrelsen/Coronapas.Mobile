using SSICPAS.Configuration;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.QrScannerViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.ScannerPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OnboardingInfoViewScanner : ContentPage
    {
        public OnboardingInfoViewScanner()
        {
            InitializeComponent();
            BindingContext = OnboardingInfoScannerViewModel.CreateOnboardingInfoScannerViewModel();
        }

        protected override void OnAppearing()
        {
            IoCContainer.Resolve<INavigationService>().SetStatusBar(SSICPASColor.DefaultBackgroundColor.Color(), Color.Black);
            base.OnAppearing();
        }
    }
}