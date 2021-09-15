using System.Linq;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Models;
using SSICPAS.Models.Validation;
using SSICPAS.Models.Validation.PinCodeRules;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.Views.Menu;

namespace SSICPAS.ViewModels.Menu
{
    public class NewPinCodeViewModel: ChangePinCodeViewModel
    {
        public static string MainTitle => "NEW_PIN_MAIN_TITLE".Translate();

        public static NewPinCodeViewModel CreateNewPinCodeViewModel()
        {
            return new NewPinCodeViewModel(
                IoCContainer.Resolve<ISecureStorageService<PinCodeBiometricsModel>>(),
                IoCContainer.Resolve<INavigationTaskManager>(),
                IoCContainer.Resolve<IDeviceFeedbackService>(),
                IoCContainer.Resolve<IUserService>()
            );
        }

        public NewPinCodeViewModel(ISecureStorageService<PinCodeBiometricsModel> pinCodeService,
            INavigationTaskManager navigationTaskManager,
            IDeviceFeedbackService deviceFeedbackService,
            IUserService userService): base(navigationTaskManager, pinCodeService, deviceFeedbackService, userService)
        {
            InitText();
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

        public override async void PinButtonClicked(string character)
        {
            PerformHapticFeedback();

            AddToCurrentPinCode(character);

            if (CurrentPinCompleted())
            {
                ValidatableObject<string> _pinCodeValidator = new ValidatableObject<string>();
                _pinCodeValidator.ValidationRules.Add(new IsValidPinCodeRule<string>());
                _pinCodeValidator.Value = PinCode;
                if (_pinCodeValidator.Validate())
                {
                    VerifyNewPinCodeViewModel.tempPinCode = PinCode;
                    await _navigationService.PushPage(
                                              new ChangePinCodeView(IoCContainer.Resolve<VerifyNewPinCodeViewModel>()));
                }
                else
                {
                    PinCodeValidationFailed(_pinCodeValidator);
                }
            }
            
            UpdateBullets();
        }

        private void PinCodeValidationFailed(ValidatableObject<string> _pinCodeValidator)
        {
            IsVisibleError = true;
            if (_pinCodeValidator.Errors.First() == IsValidPinCodeRule<string>.ValidationConsecutive)
            {
                VerifyErrorText = PinErrorConsecutive;
            }
            else if (_pinCodeValidator.Errors.First() == IsValidPinCodeRule<string>.ValidationSequence)
            {
                VerifyErrorText = PinErrorSequence;
            }
            OnPropertyChanged(nameof(IsVisibleError));
            OnPropertyChanged(nameof(VerifyErrorText));
            PinCode = string.Empty;
            UpdateBullets(true);
            PerformVibration();
        }
    }
}
