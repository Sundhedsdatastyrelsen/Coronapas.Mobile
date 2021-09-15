using System.Threading.Tasks;
using SSICPAS.Configuration;
using SSICPAS.Core.Auth;
using SSICPAS.Core.Data;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.ViewModels.Onboarding;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.Menu
{
    public class TermsPageViewModel : AcceptDataViewModel
    {
        private IDialogService _dialogService;
        private IUserService _userService;

        private static string _buttonText => "CONSENT_BUTTON_TEXT".Translate();
        private static string _dialogTitle => "CONSENT_DIALOG_TITLE".Translate();
        private static string _dialogContent => "CONSENT_DIALOG_CONTENT".Translate();
        private static string _dialogCancel => "CONSENT_DIALOG_CANCEL".Translate();
        private static string _dialogAccept => "CONSENT_DIALOG_ACCEPT".Translate();
        private bool _lottie = false;

        public bool VisibleLottie
        {
            get
            {
                return _lottie;
            }
            set
            {
                _lottie = value;
                OnPropertyChanged(nameof(VisibleLottie));
            }
        }

        public string ConsentButtonText => _buttonText;

        public Command ConsentButtonCommand => new Command(async () => await ExecuteOnceAsync(ShowConsentDialog));

        public TermsPageViewModel(IDialogService dialogService, 
            ISecureStorageService<AuthData> accessTokenStorageService, 
            IUserService userService,
            IDateTimeService dateTimeService): base(dialogService, accessTokenStorageService, dateTimeService)
        {
            _dialogService = dialogService;
            _userService = userService;
        }

        public async Task ShowConsentDialog()
        {
            bool result = await _dialogService.ShowAlertAsync(_dialogTitle, _dialogContent, true, true, StackOrientation.Horizontal, _dialogAccept, _dialogCancel);
            if (result)
            {
                await _userService.UserLogoutAsync(false);
                await _navigationService.OpenLandingPage();
            }
        }
        
        public bool IsVoiceOverEnabled => IoCContainer.Resolve<IVoiceOverManager>().IsVoiceOverEnabled;
    }
}