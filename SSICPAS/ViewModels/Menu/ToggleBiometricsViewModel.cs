using System.Threading.Tasks;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Data;
using SSICPAS.Models;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Navigation;

namespace SSICPAS.ViewModels.Menu
{
    public class ToggleBiometricsViewModel : ChangePinCodeViewModel
    {
        private PinCodeBiometricsModel _pinCodeModel;
        private int _currentAttempts;
        private static int _maxAttempts = 5;
        private new ISecureStorageService<PinCodeBiometricsModel> _pinCodeService;
        private new INavigationService _navigationService;
        private IPopupService _popupService;

        private static string ToastTurnOffSuccess => "TURN_OFF_BIO_SUCCESS".Translate();
        private static string ToastTurnOnSuccess => "TURN_ON_BIO_SUCCESS".Translate();
        
        public static ToggleBiometricsViewModel CreateNewToggleBiometricsViewModel()
        {
            return new ToggleBiometricsViewModel(
                IoCContainer.Resolve<INavigationTaskManager>(),
                IoCContainer.Resolve<ISecureStorageService<PinCodeBiometricsModel>>(),
                IoCContainer.Resolve<IDeviceFeedbackService>(),
                IoCContainer.Resolve<INavigationService>(),
                IoCContainer.Resolve<IUserService>(),
                IoCContainer.Resolve<IPopupService>());
        }

        public ToggleBiometricsViewModel(INavigationTaskManager navigationTaskManager,
            ISecureStorageService<PinCodeBiometricsModel> pinCodeService,
            IDeviceFeedbackService deviceFeedbackService,
            INavigationService navigationService,
            IUserService userService,
            IPopupService popupService) :
            base(navigationTaskManager, pinCodeService, deviceFeedbackService, userService)
        {
            _pinCodeService = pinCodeService;
            _navigationService = navigationService;
            _popupService = popupService;
            InitText();
            SetupData();
        }

        private void InitText()
        {
            HeaderText = string.Empty;
            MainText = "BIOMETRIC_ENTER_PIN".Translate();
        }

        private async void SetupData()
        {
            _pinCodeModel = await _pinCodeService.GetSecureStorageAsync(SecureStorageKeys.PIN_LOCATION);
            _currentAttempts = _pinCodeModel.Attempts;
        }

        public override void PinButtonClicked(string character)
        {
            PerformHapticFeedback();
            AddToCurrentPinCode(character);
            UpdateBullets();

            if (CurrentPinCompleted())
            {
                VerifyPinCode();
            }
        }

        protected override async void VerifyPinCode()
        {
            string storagePin = _pinCodeModel.PinCode;
            IncreaseAttempts();
            
            if (PinCode == storagePin)
            {
                await VerifyPinCodeSucceed();
            } 
            else
            {
                if (_currentAttempts == _maxAttempts)
                {
                    ResetStorage();
                }
                else
                {
                    VerifyPinCodeFailed();
                }
            }
        }

        private void VerifyPinCodeFailed()
        {
            PinCode = "";
            IsVisibleError = true;
            OnPropertyChanged(nameof(IsVisibleError));
            UpdateBullets(true);
            PerformVibration();
        }

        private async Task VerifyPinCodeSucceed()
        {
            bool hasBiometrics = _pinCodeModel.HasBiometrics;
            if (!hasBiometrics)
            {
                await VerifyBiometrics();
            }
            else
            {
                _pinCodeModel.HasBiometrics = false;
                await _pinCodeService.SetSecureStorageAsync(SecureStorageKeys.PIN_LOCATION, _pinCodeModel);
                await BackToSettings();
            }
        }

        private async Task VerifyBiometrics()
        {
            AuthenticationRequestConfiguration config = new AuthenticationRequestConfiguration(BioConfirmIdentity, BioAndroidSubTitle);
            var authResult = await CrossFingerprint.Current.AuthenticateAsync(config);
            
            if (authResult.Authenticated)
            {
                _pinCodeModel.Attempts = 0;
                _pinCodeModel.HasBiometrics = true;
                await _pinCodeService.SetSecureStorageAsync(SecureStorageKeys.PIN_LOCATION, _pinCodeModel);
                await _navigationService.PopPage(true);
                _popupService.ShowSuccessToast(ToastTurnOnSuccess);
            }
            else
            {
                await BackToSettings();
            }
        }

        private async void IncreaseAttempts()
        {
            _pinCodeModel.Attempts++;
            _currentAttempts = _pinCodeModel.Attempts;
            await _pinCodeService.SetSecureStorageAsync(SecureStorageKeys.PIN_LOCATION, _pinCodeModel);
        }

        private async void ResetStorage()
        {
            await _pinCodeService.Clear(SecureStorageKeys.PIN_LOCATION);
            await _navigationService.GoToErrorPage(Errors.LockError);
        }

        private async Task BackToSettings()
        {
            await _navigationService.PopPage(true);
            _popupService.ShowSuccessToast(ToastTurnOffSuccess);
        }
    }
}