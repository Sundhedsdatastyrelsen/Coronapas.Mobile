using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SSICPAS.Services.Interfaces;
using SSICPAS.ViewModels.Base;
using Xamarin.Forms;
using Xamarin.Essentials;
using static SSICPAS.Views.Elements.PinCodeView;
using SSICPAS.Services;
using static SSICPAS.ViewModels.Custom.CustomPincodeBulletsViewModel;

namespace SSICPAS.ViewModels.Onboarding
{
    public abstract class BasePinCodeViewModel : BaseViewModel
    {
        private readonly IDeviceFeedbackService _deviceFeedbackService;
        
        public PinCodeStateEnum FirstCode { get; set; }
        public PinCodeStateEnum SecondCode { get; set; }
        public PinCodeStateEnum ThirdCode { get; set; }
        public PinCodeStateEnum FourthCode { get; set; }

        public static string PinMainText => "PINCODE".Translate();

        public static string PinSubText => "PINCODE_SUB_TEXT".Translate();

        public static string PinSubTextExisted => "PINCODE_EXISTED".Translate();

        public static string PinSubTextIncorrect => "PINCODE_SUB_INCORRECT".Translate();

        public static string PinSubTextVerify => "PINCODE_SUB_VERIFY".Translate();

        public static string PinSubText4Numbers => "PINCODE_SUB_4_NUMBERS".Translate();

        public static string PinSubTextDisabled => "PINCODE_DISABLE".Translate();

        public static string PinSubTextSuccess => "PINCODE_SUB_TEXT_SUCCESS".Translate();

        public static string PinSubText1 => "PINCODE_SUBTEXT_1".Translate();

        public static string PinSubText2 => "PINCODE_SUBTEXT_2".Translate();

        public static string PinSubText3 => "PINCODE_SUBTEXT_3".Translate();

        public static string Error => "ERROR".Translate();

        public static string BioNotRegistered => "BIOMETRIC_NOT_REGISTERED".Translate();

        public static string BioDismiss => "BIOMETRIC_DISMISS".Translate();

        public static string BioConfirmIdentity => "CONFIRM_IDENTITY".Translate();

        public static string ErrorString => "ERROR_TEXT".Translate();

        public static string PinCodeConfirm => "PINCODE_CONFIRM".Translate();

        public static string BioAndroidSubTitle => "BIOMETRIC_SUBTITLE_ANDROID".Translate();

        public static string ChangePinHeader => "CHANGE_PIN_HEADER".Translate();

        public string HelpText => "HELP".Translate();
        public string BackText => "BACK".Translate();

        public static string PinErrorConsecutive => "PINCODE_ERROR_CONSECUTIVE".Translate();

        public static string PinErrorSequence => "PINCODE_ERROR_SEQUENCE".Translate();

        public static string PinNotMatch => "PINCODE_NOT_MATCH".Translate();

        protected string PinCode = "";

        public string SubText1 { get; set; } = PinSubText1;

        public string SubText2 { get; set; } = PinSubText2;

        public string SubText3 { get; set; } = PinSubText3;

        private bool _visibleSubtext1 = true;

        private bool _visibleSubtext2 = true;

        private bool _visibleSubtext3 = true;

        private bool _visibleSubtext4;

        public bool VisibleSubtext1
        {
            get
            {
                return _visibleSubtext1;
            }
            set
            {
                _visibleSubtext1 = value;
                OnPropertyChanged(nameof(VisibleSubtext1));
            }
        }

        public bool VisibleSubtext2
        {
            get
            {
                return _visibleSubtext2;
            }
            set
            {
                _visibleSubtext2 = value;
                OnPropertyChanged(nameof(VisibleSubtext2));
            }
        }

        public bool VisibleSubtext3
        {
            get
            {
                return _visibleSubtext3;
            }
            set
            {
                _visibleSubtext3 = value;
                OnPropertyChanged(nameof(VisibleSubtext3));
            }
        }

        public bool VisibleSubtext4
        {
            get
            {
                return _visibleSubtext4;
            }
            set
            {
                _visibleSubtext4 = value;
                OnPropertyChanged(nameof(VisibleSubtext4));
            }
        }

        public bool VisibleBackButton { get; set; } = true;

        private string _errorText;

        public string ErrorText
        {
            get
            {
                return _errorText;
            }
            set
            {
                _errorText = value;
                OnPropertyChanged(nameof(ErrorText));
            }
        }

        private bool _visibleErrorText = false;

        private bool _visibleFingerPrint = false;

        public bool VisibleFingerPrint
        {
            get
            {
                return _visibleFingerPrint;
            }
            set
            {
                _visibleFingerPrint = value;
                OnPropertyChanged(nameof(VisibleFingerPrint));
            }
        }

        public bool VisibleErrorText
        {
            get
            {
                return _visibleErrorText;
            }
            set
            {
                _visibleErrorText = value;
                OnPropertyChanged(nameof(VisibleErrorText));
            }
        }
        
        public ICommand HelpButton => new Command(async () => await ExecuteOnceAsync(OnHelpButtonClicked));

        public ICommand DeletePinCommand => new Command(async () => DeleteButtonClicked());

        public ICommand OnPinButtonCommand => new Command<string>(async (x) => await ExecuteOnceAsync(async () =>
        {
            PinButtonClicked(x);
        }));

        
        public BasePinCodeViewModel(IDeviceFeedbackService deviceFeedbackService)
        {
            _deviceFeedbackService = deviceFeedbackService;

            UpdateBullets();
            VisibleSubtext4 = false;
        }

        public virtual void PinButtonClicked(string character)
        {

            PerformHapticFeedback();

            AddToCurrentPinCode(character);

            UpdateBullets();
        }

        protected bool CurrentPinCompleted()
        {
            return PinCode.Length == 4;
        }

        protected void DeleteButtonClicked()
        {
            if (CurrentPinNotEmpty())
            {
                DeleteCurrentPin();
            }
            UpdateBullets();
        }

        protected bool CurrentPinNotEmpty()
        {
            return PinCode.Length > 0;
        }

        protected void DeleteCurrentPin()
        {
            PinCode = PinCode.Remove(PinCode.Length - 1);
        }

        protected void AddToCurrentPinCode(string number)
        {
            if (PinCode.Length < 4)
            {
                PinCode += number;
            }
        }
        
        public Action<PinCodeViewStatusEnum> UpdatePincodeBullet;
        protected void UpdateBullets(bool isError = false)
        {
            if (isError)
            {
                UpdatePincodeBullet?.Invoke(PinCodeViewStatusEnum.Error);
            }
            else
            {
                switch (PinCode.Length)
                {
                    case 0:
                        UpdatePincodeBullet?.Invoke(PinCodeViewStatusEnum.NoPin);
                        break;
                    case 1:
                        UpdatePincodeBullet?.Invoke(PinCodeViewStatusEnum.FirstPin);
                        break;
                    case 2:
                        UpdatePincodeBullet?.Invoke(PinCodeViewStatusEnum.SecondPin);
                        break;
                    case 3:
                        UpdatePincodeBullet?.Invoke(PinCodeViewStatusEnum.ThirdPin);
                        break;
                    case 4:
                        UpdatePincodeBullet?.Invoke(PinCodeViewStatusEnum.FourthPin);
                        break;
                }
            }
        }

        public virtual Task OnHelpButtonClicked()
        {
            return Task.CompletedTask;
        }

        public void PerformVibration()
        {
            _deviceFeedbackService.Vibrate(1000);
        }

        public void PerformHapticFeedback()
        {
            _deviceFeedbackService.PerformHapticFeedback(HapticFeedbackType.Click);
        }
    }
}
