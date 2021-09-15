using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Core.WebServices;
using SSICPAS.Core.Auth;
using SSICPAS.Models.Validation;
using SSICPAS.Models.Validation.PinCodeRules;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.Views.Onboarding;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.Onboarding
{
    public class RegisterPinCodeViewModel : BasePinCodeViewModel
    {
        public string SubText { get; set; } = PinSubText;
        public string MainText { get; set; } = PinMainText;
        public string SubTextError { get; set; } = ErrorString;

        public override ICommand BackCommand => new Command(async () => await ExecuteOnceAsync(GoBack));

        public static RegisterPinCodeViewModel CreateRegisterPinCodeViewModel()
        {
            return new RegisterPinCodeViewModel(
                IoCContainer.Resolve<IDeviceFeedbackService>()
            );
        }

        public RegisterPinCodeViewModel(IDeviceFeedbackService deviceFeedbackService) : base(deviceFeedbackService)
        {
        }

        private async Task GoBack()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var response = await IoCContainer.Resolve<IDialogService>().ShowAlertAsync(
                "LOGOUT_TITLE".Translate(),
                "LOGOUT_QUESTION".Translate(),
                true, 
                true, 
                StackOrientation.Horizontal,
                "OK_BUTTON".Translate(),
                "CANCEL_BUTTON".Translate());

                if (response)
                {
                    // Clear auth token that's just been added - "logout" user
                    await IoCContainer.Resolve<ISecureStorageService<AuthData>>().Clear(CoreSecureStorageKeys.AUTH_TOKEN);
                    IoCContainer.Resolve<IRestClient>().ClearAccessTokenHeader();
                    await _navigationService.OpenLandingPage();
                }
            });
        }
        
        public override async void PinButtonClicked(string character)
        {
            base.PinButtonClicked(character);

            if (CurrentPinCompleted())
            {
                ValidatableObject<string> _pinCodeValidator = new ValidatableObject<string>();
                _pinCodeValidator.ValidationRules.Add(new IsValidPinCodeRule<string>());
                _pinCodeValidator.Value = PinCode;
                if (_pinCodeValidator.Validate())
                {
                    RegisterVerifyPinCodeViewModel.tempPinCode = PinCode;
                    await _navigationService.PushPage(
                                              new RegisterPinCodeView(RegisterVerifyPinCodeViewModel.CreateRegisterVerifyPinCodeViewModel()));
                }
                else
                {
                    PinCodeValidationFailed(_pinCodeValidator);
                }
            }
        }

        public void ResetView()
        {
            PinCode = "";
            UpdateBullets();
            ErrorText = string.Empty;
            OnPropertyChanged(nameof(ErrorText));
        }

        private void PinCodeValidationFailed(ValidatableObject<string> _pinCodeValidator)
        {
            VisibleErrorText = true;
            if (_pinCodeValidator.Errors.First() == IsValidPinCodeRule<string>.ValidationConsecutive)
            {
                ErrorText = PinErrorConsecutive;
            }
            else if (_pinCodeValidator.Errors.First() == IsValidPinCodeRule<string>.ValidationSequence)
            {
                ErrorText = PinErrorSequence;
            }
            OnPropertyChanged(nameof(ErrorText));
            OnPropertyChanged(nameof(VisibleErrorText));
            PinCode = string.Empty;
            UpdateBullets(true);
            PerformVibration();
        }
    }
}