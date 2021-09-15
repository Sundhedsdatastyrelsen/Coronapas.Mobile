using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Data;
using SSICPAS.Enums;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.ViewModels.Base;
using SSICPAS.Views.Onboarding;
using SSICPAS.Views.ScannerPages;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SSICPAS.ViewModels
{
    public class LandingViewModel : BaseViewModel
    {
        public string RetrieveCertificateText => "LANDING_PAGE_COVID_STATUS".Translate();
        public string OpenScannerText => "LANDING_PAGE_QR_CODE".Translate();
        public string LandingPageTitle => "LANDING_PAGE_TITLE".Translate();
        public string LanguageChangeButtonText => "LANDING_PAGE_LANGUAGE_CHANGE_BUTTON_TEXT".Translate();
        public string HelpButton => "HELP".Translate();

        public ICommand RetrieveCertificateCommand => new Command(async () => await ExecuteOnceAsync(GoToNextPageAsync));
        public ICommand ChangeLanguageCommand => new Command(async () => await ExecuteOnceAsync(DisplayChangeLanguageDialog));
        public ICommand OpenScannerCommand => new Command(async () => await ExecuteOnceAsync(GoToScannerPage));

        private readonly IPreferencesService _preferencesService = IoCContainer.Resolve<IPreferencesService>();
        private IDialogService _dialogService = IoCContainer.Resolve<IDialogService>();
        private ITextService _textService = IoCContainer.Resolve<ITextService>();
        private IScannerFactory _scannerFactoryService = IoCContainer.Resolve<IScannerFactory>();

        private async Task GoToScannerPage()
        {
            bool onboardingScannerCompleted = _preferencesService.GetUserPreferenceAsBoolean(PreferencesKeys.ONBOARDING_SCANNER_COMPLETED);

            if (!onboardingScannerCompleted)
            {
                await _navigationService.PushPage(new OnboardingInfoViewScanner());

            }
            else
            {
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

        private async Task GoToNextPageAsync()
        {
            await _navigationService.PushPage(new OnboardingBasePage());
        }

        private async Task DisplayChangeLanguageDialog()
        {
            string title = $"{"SETTINGS_CHOOSE_LANGUAGE_DIALOG_TITLE_DANISH".Translate()}/\n"
                + "SETTINGS_CHOOSE_LANGUAGE_DIALOG_TITLE_ENGLISH".Translate();
            string content = $"{"SETTINGS_CHOOSE_LANGUAGE_DIALOG_CONTENT_DANISH".Translate()}\n"
                + "SETTINGS_CHOOSE_LANGUAGE_DIALOG_CONTENT_ENGLISH".Translate();
            string accept = "SETTINGS_RESTART_APP_BUTTON".Translate();
            string cancel = "CANCEL_BUTTON_CHOOSE_LANGUAGE".Translate();
            LanguageSelection selectedLanguage = LocaleService.Current.GetLanguage();
            bool result = await _dialogService.ShowAlertAsync(title, content, true, true, StackOrientation.Horizontal, accept, cancel);
            if (result)
            {
                if (selectedLanguage == LanguageSelection.Danish)
                {
                    selectedLanguage = LanguageSelection.English;
                }
                else
                {
                    selectedLanguage = LanguageSelection.Danish;
                }
                _textService.SetLocale(selectedLanguage.ToISOCode());
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            OnPropertyChanged(nameof(RetrieveCertificateText));
            OnPropertyChanged(nameof(OpenScannerText));
            OnPropertyChanged(nameof(LandingPageTitle));
            OnPropertyChanged(nameof(LanguageChangeButtonText));
        }
    }
}
