using System.Threading.Tasks;
using SSICPAS.Core.Data;
using SSICPAS.Data;
using SSICPAS.Models;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;

namespace SSICPAS.ViewModels.Menu
{
    public class VerifyNewPinCodeViewModel: ChangePinCodeViewModel
    {
        private IPopupService _popupService;
        private string _tempPinCode;
        private PinCodeBiometricsModel pinCodeModel;

        public static string tempPinCode { get; set; }
        public static string MainTitle => "VERIFY_PIN_MAIN_TITLE".Translate();
        public static string ToastSuccess => "CHANGE_PIN_SUCCESS".Translate();

        public VerifyNewPinCodeViewModel(ISecureStorageService<PinCodeBiometricsModel> pinCodeService,
            INavigationTaskManager navigationTaskManager,
            IDeviceFeedbackService deviceFeedbackService,
            IUserService userService, 
            IPopupService popupService): base(navigationTaskManager, pinCodeService,deviceFeedbackService, userService)
        {
            _popupService = popupService;
            if (!string.IsNullOrEmpty(tempPinCode))
            {
                _tempPinCode = tempPinCode;
                tempPinCode = null;
            }
            InitText();
        }
        
        public override async void PinButtonClicked(string character)
        {
            PerformHapticFeedback();
            AddToCurrentPinCode(character);

            if (CurrentPinCompleted())
            {
                await SetupPinCode();
            }

            UpdateBullets();
        }

        private void InitText()
        {
            MainText = MainTitle;
            HeaderText = ChangePinHeader;
            VisibleSubText1 = true;
            VisibleSubText2 = true;
            VisibleSubText3 = true;
            VisibleSubText4 = false;
            SubText1 = PinSubText1;
            SubText2 = PinSubText2;
            SubText3 = PinSubText3;
        }

        private async Task SetupPinCode()
        {
            if (PinCode == _tempPinCode)
            {
                pinCodeModel = new PinCodeBiometricsModel
                {
                    PinCode = PinCode,
                    Attempts = 0,
                    HasBiometrics = _pinCodeService.GetSecureStorageAsync(SecureStorageKeys.PIN_LOCATION).GetAwaiter().GetResult().HasBiometrics
                };

                await _pinCodeService.SetSecureStorageAsync(SecureStorageKeys.PIN_LOCATION, pinCodeModel);
                await GoToSettingsPage();
            } 
            else
            {
                PinCodeIncorrect();
            }
        }

        private void PinCodeIncorrect()
        {
            PinCode = "";
            IsVisibleError = true;
            VerifyErrorText = PinNotMatch;
            VisibleErrorText = true;
            OnPropertyChanged(nameof(VerifyErrorText));
            OnPropertyChanged(nameof(IsVisibleError));
            UpdateBullets(true);
            PerformVibration();
        }

        private async Task GoToSettingsPage()
        {
            await _navigationService.PopPage(false);
            await _navigationService.PopPage(false);
            await _navigationService.PopPage(true);
            _popupService.ShowSuccessToast(ToastSuccess);
        }
    }
}
