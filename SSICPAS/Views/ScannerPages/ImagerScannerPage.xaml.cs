using SSICPAS.Configuration;
using SSICPAS.Data;
using SSICPAS.Enums;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.QrScannerViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.ScannerPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImagerScannerPage : ContentPage, IExtraOrientationSupport
    {
        private bool _inTabbar = false;

        public ImagerScannerPage()
        {
            InitializeComponent();
            BindingContext = ImagerScannerViewModel.CreateImagerScannerViewModel();
        }

        protected override void OnAppearing()
        {
            ((ImagerScannerViewModel)BindingContext).EnableScanner();
            IoCContainer.Resolve<INavigationService>().SetStatusBar(SSICPASColor.NavigationHeaderBackgroundColor.Color(), Color.Black);

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        public void SetFromTabbar()
        {
            _inTabbar = true;
            ((ImagerScannerViewModel)BindingContext).InTabbar = _inTabbar;
        }
        protected override bool OnBackButtonPressed()
        {
            ((QRScannerViewModel)BindingContext).BackCommand.Execute(null);
            return true;
        }

        public SupportedOrientation SupportedOrientation => SupportedOrientation.SensorPortrait;
    }
}