using System.Threading.Tasks;
using System.Windows.Input;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Data;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.ViewModels.Base;
using SSICPAS.Views;
using SSICPAS.Views.Menu;
using SSICPAS.Views.ScannerPages;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.QrScannerViewModels
{
    public class OnboardingInfoScannerViewModel : BaseViewModel
    {
        private IScannerFactory _scannerFactoryService;
        private readonly IPreferencesService _preferencesService = IoCContainer.Resolve<IPreferencesService>();

        public string BackButton { get; set; }
        public string NextButton { get; set; }
        public string HelpButton { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        
        public new ICommand HelpButtonCommand => new Command(async () => await ExecuteOnceAsync(async () => await _navigationService.PushPage(new HelpPage())));

        public ICommand NextCommand => new Command(async () => await ExecuteOnceAsync(GoToScannerPage));

        public static OnboardingInfoScannerViewModel CreateOnboardingInfoScannerViewModel()
        {
            return new OnboardingInfoScannerViewModel(
                IoCContainer.Resolve<IScannerFactory>(), IoCContainer.Resolve<IPreferencesService>()
            );
        }

        public OnboardingInfoScannerViewModel(IScannerFactory scannerFactoryService, IPreferencesService preferencesService)
        {
            InitText();
            _scannerFactoryService = scannerFactoryService;
            _preferencesService = preferencesService;
        }

        private void InitText()
        {
            BackButton = "BACK".Translate();
            NextButton = "NEXT".Translate();
            HelpButton = "HELP".Translate();

            Title = "ONBOARDING_SCANNER_TITLE".Translate();
            Content = "ONBOARDING_SCANNER_CONTENT".Translate();
        }
        
        private async Task GoToScannerPage()
        {
            _preferencesService.SetUserPreference(PreferencesKeys.ONBOARDING_SCANNER_COMPLETED, true);
            
            if (_scannerFactoryService.GetAvailableScanner() == null)
            {
                await _navigationService.PushPage(new QRScannerPage());
            }
            else
            {
                await _navigationService.PushPage(new ImagerScannerPage());
            }
        }

    }

}