using System.Threading.Tasks;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Data;
using SSICPAS.Models;
using SSICPAS.Services.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSICPAS.Services
{
    public class UserService : IUserService
    {
        private readonly IPreferencesService _preferences;
        private readonly INavigationService _navigationService;
        private readonly ISecureStorageService<PinCodeBiometricsModel> _pinCodeService;
        private readonly IDialogService _dialogService;
        private readonly ITextService _textService;
        private readonly IRatListService _ratListService;
        private static bool firstRunSafeCheck = true;

        public UserService(
            IPreferencesService preferences,
            INavigationService navigationService,
            ISecureStorageService<PinCodeBiometricsModel> pinCodeService,
            IDialogService dialogService,
            ITextService textService,
            IRatListService ratListService)
        {
            _preferences = preferences;
            _navigationService = navigationService;
            _pinCodeService = pinCodeService;
            _dialogService = dialogService;
            _textService = textService;
            _ratListService = ratListService;
        }

        public async Task CleanupAfterFreshInstallAsync()
        {
            VersionTracking.Track();
            if (VersionTracking.IsFirstLaunchEver && firstRunSafeCheck)
            {
                firstRunSafeCheck = false;
                await ClearAppData(false);
                _preferences.SetUserPreference(PreferencesKeys.LANGUAGE_SETTING, "dk");
            }
        }

        public async Task InitializeAsync()
        {
            PinCodeBiometricsModel PinCodeModel = await _pinCodeService.GetSecureStorageAsync(SecureStorageKeys.PIN_LOCATION);
            // In some devices, there might be some cases that PinCode is null but not the object
            if (PinCodeModel == null || PinCodeModel.PinCode == null)
            {
                await _navigationService.OpenLandingPage();
            }
            else
            {
                await _navigationService.OpenVerifyPinCodePageAsync();
            }

            Task.WhenAll(_textService.LoadRemoteLocales(), _ratListService.LoadRemoteFiles());
        }

        public async Task UserLogoutAsync(bool showWarning = true, bool shouldNavigateToLanding = true)
        {
            bool shouldLogout;

            if (showWarning)
            {
                var title = "LOGOUT_TITLE".Translate();
                var description = $"{"LOGOUT_QUESTION".Translate()}\n{"LOGOUT_DESCRIPTION".Translate()}";
                var accept = "OK_BUTTON".Translate();
                var cancel = "CANCEL_BUTTON".Translate();
                shouldLogout = await _dialogService.ShowAlertAsync(title, description, true, true, StackOrientation.Horizontal, accept, cancel);
            }
            else
            {
                shouldLogout = true;
            }
            
            if (shouldLogout)
            {
                await ClearAppData(shouldNavigateToLanding);
            }
        }

        private async Task ClearAppData(bool shouldNavigateToLanding)
        {
            IoCContainer.ResetSingletons();
            if (shouldNavigateToLanding)
            {
                await _navigationService.OpenLandingPage();
            }
            _preferences.ClearAllUserPreferences();
            SetDefaultUserPreferences();
            SecureStorage.RemoveAll();
        }

        private void SetDefaultUserPreferences()
        {
            _preferences.SetUserPreference(PreferencesKeys.SCANNER_VIBRATION_SETTING, true);
            _preferences.SetUserPreference(PreferencesKeys.SCANNER_SOUND_SETTING, true);
            _preferences.SetUserPreference(PreferencesKeys.MIGRATION_COUNT, MigrationService.CurrentMigrationVersion);
        }
    }
}