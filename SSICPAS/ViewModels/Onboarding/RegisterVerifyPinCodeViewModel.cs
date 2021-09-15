using System.Threading.Tasks;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SSICPAS.Core.Data;
using SSICPAS.Models;
using Xamarin.Forms;
using SSICPAS.Services;
using System.Threading;
using SSICPAS.Data;
using SSICPAS.Services.Interfaces;
using SSICPAS.Configuration;

namespace SSICPAS.ViewModels.Onboarding
{
    public class RegisterVerifyPinCodeViewModel : BasePinCodeViewModel
    {
        private readonly ISecureStorageService<PinCodeBiometricsModel> _pinCodeService;
        private readonly INavigationTaskManager _navigationTaskManager;
        private PinCodeBiometricsModel pinCodeModel;
        public static string tempPinCode { get; set; }
        private string _tempPinCode;

        public string MainText { get; set; } = PinCodeConfirm;

        public static RegisterVerifyPinCodeViewModel CreateRegisterVerifyPinCodeViewModel()
        {
            return new RegisterVerifyPinCodeViewModel(
                IoCContainer.Resolve<ISecureStorageService<PinCodeBiometricsModel>>(),
                IoCContainer.Resolve<INavigationTaskManager>(),
                IoCContainer.Resolve<IDeviceFeedbackService>()
            );
        }

        public RegisterVerifyPinCodeViewModel(ISecureStorageService<PinCodeBiometricsModel> pinCodeService,
            INavigationTaskManager navigationTaskManager,
            IDeviceFeedbackService deviceFeedbackService) : base(deviceFeedbackService)
        {
            _pinCodeService = pinCodeService;
            _navigationTaskManager = navigationTaskManager;

            if (!string.IsNullOrEmpty(tempPinCode))
            {
                _tempPinCode = tempPinCode;
                tempPinCode = null;
            }
            else
            {
                _navigationService.PopPage();
            }
        }

        private async Task SaveAndContinue()
        {
            bool isFingerprintAvailable = await CrossFingerprint.Current.IsAvailableAsync(false);
            FingerprintAuthenticationResult authResult = null;

            if (isFingerprintAvailable)
            {
                AuthenticationRequestConfiguration config = new AuthenticationRequestConfiguration(BioConfirmIdentity, BioAndroidSubTitle);
                authResult = await CrossFingerprint.Current.AuthenticateAsync(config);
                if (Device.RuntimePlatform == Device.iOS)
                {
                    Thread.Sleep(800); //Sleep so the faceId animation doesn't overlap with the success animation.
                }
            }

            pinCodeModel.HasBiometrics = authResult?.Authenticated ?? false;
            await _pinCodeService.SetSecureStorageAsync(SecureStorageKeys.PIN_LOCATION, pinCodeModel);
            await GoToNextPage();
        }

        async Task GoToNextPage()
        {
            await _navigationTaskManager.ShowSuccessPage("LOADING_PAGE_RETRIEVING_SUCCESS".Translate());
            await _navigationService.OpenTabbar();
        }

        private async Task SetUpPinCode()
        {
            if (CurrentPinCompleted())
            {
                if (PinCode == _tempPinCode)
                {
                    pinCodeModel = new PinCodeBiometricsModel
                    {
                        PinCode = PinCode,
                        Attempts = 0
                    };

                    await _pinCodeService.SetSecureStorageAsync(SecureStorageKeys.PIN_LOCATION, pinCodeModel);
                    await SaveAndContinue();

                }
                else
                {
                    PinCodeIncorrect();
                    return;
                }
            }
        }

        public override async void PinButtonClicked(string character)
        {

            base.PinButtonClicked(character);

            if (CurrentPinCompleted())
            {
                await SetUpPinCode();
            }
        }

        private void PinCodeIncorrect()
        {
            PinCode = "";
            ErrorText = PinNotMatch;
            VisibleErrorText = true;
            OnPropertyChanged(nameof(ErrorText));
            OnPropertyChanged(nameof(VisibleErrorText));
            UpdateBullets(true);
            PerformVibration();
        }
    }
}