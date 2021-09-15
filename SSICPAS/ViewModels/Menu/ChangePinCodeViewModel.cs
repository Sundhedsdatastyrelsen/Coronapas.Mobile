using System.Threading.Tasks;
using System.Windows.Input;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Data;
using SSICPAS.Models;
using SSICPAS.Services;
using SSICPAS.Services.Navigation;
using SSICPAS.Services.Interfaces;
using SSICPAS.ViewModels.Onboarding;
using SSICPAS.Views.Menu;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.Menu
{
    public class ChangePinCodeViewModel: BasePinCodeViewModel
    {
        private readonly INavigationTaskManager _navigationTaskManager;
        private PinCodeBiometricsModel pinCodeModel;
        protected ISecureStorageService<PinCodeBiometricsModel> _pinCodeService;
        private IUserService _userService;
        private int _currentAttemps;
        private static int _maxAttemps = 5;
        
        public static string ChangePinTitle => "CHANGE_PIN_MAIN_TITLE".Translate();
        public static string ForgottenCode => "PINCODE_LOGIN_FORGET".Translate();
        public static string VerifyErrorSubText => "PINCODE_LOGIN_ERROR_TEXT".Translate();

        private string _mainText;
        private string _headerText;

        private bool _isVisibleError = false;

        public bool VisibleSubText1 { get; set; }
        public bool VisibleSubText2 { get; set; }
        public bool VisibleSubText3 { get; set; }
        public bool VisibleSubText4 { get; set; }

        public string SubText4 { get; set; }
        public string MainText
        {
            get
            {
                return _mainText;
            }
            set
            {
                _mainText = value;
                OnPropertyChanged(nameof(MainText));
            }
        }
        
        public string HeaderText
        {
            get
            {
                return _headerText;
            }
            set
            {
                _headerText = value;
                OnPropertyChanged(nameof(HeaderText));
            }
        }

        public bool IsVisibleError
        {
            get
            {
                return _isVisibleError;
            }
            set
            {
                _isVisibleError = value;
                OnPropertyChanged(nameof(IsVisibleError));
            }
        }

        public string VerifyErrorText { get; set; }

        public static ChangePinCodeViewModel CreateChangePinCodeViewModel()
        {
            return new ChangePinCodeViewModel(
                IoCContainer.Resolve<INavigationTaskManager>(),
                IoCContainer.Resolve<ISecureStorageService<PinCodeBiometricsModel>>(),
                IoCContainer.Resolve<IDeviceFeedbackService>(),
                IoCContainer.Resolve<IUserService>()
            );
        }

        public ICommand ResetPassword => new Command(() => ForgetPassword());
        
        public ChangePinCodeViewModel(INavigationTaskManager navigationTaskManager, 
            ISecureStorageService<PinCodeBiometricsModel> pinCodeService, 
            IDeviceFeedbackService deviceFeedbackService,
            IUserService userService) : base(deviceFeedbackService)
        {
            _navigationTaskManager = navigationTaskManager;
            _pinCodeService = pinCodeService;
            _userService = userService;
            InitText();
            SetupData();
        }

        private void InitText()
        {
            _mainText = ChangePinTitle;
            _headerText = ChangePinHeader;
            SubText4 = ForgottenCode;
            VerifyErrorText = VerifyErrorSubText;
            VisibleSubText1 = false;
            VisibleSubText2 = false;
            VisibleSubText3 = false;
            VisibleSubText4 = true;
        }

        private async void SetupData()
        {
            pinCodeModel = await _pinCodeService.GetSecureStorageAsync(SecureStorageKeys.PIN_LOCATION);
            _currentAttemps = pinCodeModel.Attempts;
        }

        public override void PinButtonClicked(string character)
        {

            base.PinButtonClicked(character);

            if (CurrentPinCompleted())
            {
                VerifyPinCode();
            }
        }

        protected virtual async void VerifyPinCode()
        {
            string storagePin = pinCodeModel.PinCode;
            IncreaseAttempts();
            if (PinCode == storagePin) {
                await VerifyPinCodeSucceeded();
            } else
            {
                if (_currentAttemps == _maxAttemps)
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
            _isVisibleError = true;
            OnPropertyChanged(nameof(IsVisibleError));
            UpdateBullets(true);
            PerformVibration();
        }

        private async Task VerifyPinCodeSucceeded()
        {
            pinCodeModel.Attempts = 0;
            await _pinCodeService.SetSecureStorageAsync(SecureStorageKeys.PIN_LOCATION, pinCodeModel);
            await _navigationService.PushPage(new ChangePinCodeView(NewPinCodeViewModel.CreateNewPinCodeViewModel()));
        }

        private async void IncreaseAttempts()
        {
            pinCodeModel.Attempts++;
            _currentAttemps = pinCodeModel.Attempts;
            await _pinCodeService.SetSecureStorageAsync(SecureStorageKeys.PIN_LOCATION, pinCodeModel);
        }

        private async void ResetStorage()
        {
            await _pinCodeService.Clear(SecureStorageKeys.PIN_LOCATION);
            await _navigationService.GoToErrorPage(Errors.LockError);
        }

        public void ResetView()
        {
            PinCode = "";
            UpdateBullets();
            ErrorText = string.Empty;
            OnPropertyChanged(nameof(ErrorText));
        }
        
        private async void ForgetPassword()
        {
            await _userService.UserLogoutAsync();
        }
    }
}
