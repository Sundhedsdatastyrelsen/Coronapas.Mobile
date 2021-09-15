using System.Linq;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SkiaSharp;
using SSICPAS.Configuration;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Enums;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Base;
using SSICPAS.Views.Certificates;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Globalization;

namespace SSICPAS.ViewModels.Certificates
{
    public class PassportInfoViewModel : BaseViewModel
    {
        
        public DateTime CurrentLocalTime
        {
            get => _currentLocalTime;
            set
            {
                _currentLocalTime = value;
                OnPropertyChanged(nameof(CurrentLocalTime));
            }
        }

        public string ValidPassportText
        {
            get
            {
                return _validPassportText;
            }
            set
            {
                _validPassportText = value;
                OnPropertyChanged(nameof(ValidPassportText));
            }
        }
        
        public bool IsInfoIconVisible
        {
            get
            {
                return _isInfoIconVisible;
            }
            set
            {
                _isInfoIconVisible = value;
                OnPropertyChanged(nameof(IsInfoIconVisible));
            }
        }

        public bool IsQrCodeValid
        {
            get
            {
                return _isQrCodeValid;
            }
            set
            {
                _isQrCodeValid = value;
                OnPropertyChanged(nameof(IsQrCodeValid));
            }
        }

        public Color TextColor
        {
            get
            {
                return _textColor;
            }
            set
            {
                _textColor = value;
                OnPropertyChanged(nameof(TextColor));
            }
        }

        public bool IsBadgeAnimationPlaying
        {
            get => _isBadgeAnimationPlaying;
            set
            {
                _isBadgeAnimationPlaying = value;
                OnPropertyChanged(nameof(IsBadgeAnimationPlaying));
            }
        }

        public float BadgeAnimationProgress
        {
            get => _badgeAnimationProgress;
            set
            {
                _badgeAnimationProgress = value;
                OnPropertyChanged(nameof(BadgeAnimationProgress));
            }
        }
        
        private const int ValidEuPassportAnimationDurationInMs = 3500;

        private bool _isInfoIconVisible;
        private string _validPassportText;
        private Color _textColor;
        private bool _isBadgeAnimationPlaying;
        private float _badgeAnimationProgress;
        private volatile bool _isAnimationInProgress;
        private bool _isQrCodeValid;
        private DateTime _currentLocalTime;
        public string QrCodeString => PassportViewModel?.QRToken;
        public string Birthdate => DateUtils.ParseDateOfBirth(
            PassportItemsViewModel,
            time => time.FormatEuDate());
        public string FullName =>
            PassportItemsViewModel?.SelectedPassport?.FullName ??
            PassportItemsViewModel?.SelectedMandatoryInfo?.FullName;
        
        public bool IsLoadingPassport => false;
        public bool IsTestSelected => EuPassportType == EuPassportType.TEST;
        public bool IsVaccineSelected => EuPassportType == EuPassportType.VACCINE;
        public bool IsRecoverySelected => EuPassportType == EuPassportType.RECOVERY;

        public SinglePassportViewModel PassportViewModel => SelectedPassport != null ? SelectedPassport : SelectEuPassport(EuPassportType);
        public SKColor[] BottomGradientList => SSICPASColorGradient.MainPageStatusGradientBlue.Gradient();
        public SKColor[] TopGradientList => SSICPASColorGradient.MainPageCrownGradient.Gradient();

        public FamilyPassportItemsViewModel PassportItemsViewModel { get; set; }
        public SinglePassportViewModel SelectedPassport { get; set; } = null;
        public EuPassportType EuPassportType { get; set; }

        private readonly IGyroscopeService _gyroscopeService;
        private readonly IPassportDataManager _passportDataManager;
        private readonly IDateTimeService _dateTimeService;

        public static PassportInfoViewModel CreatePassportInfoViewModel()
        {
            return new PassportInfoViewModel(
                IoCContainer.Resolve<IGyroscopeService>(),
                IoCContainer.Resolve<IPassportDataManager>(),
                IoCContainer.Resolve<IDateTimeService>()
            );
        }
        
        public PassportInfoViewModel(IGyroscopeService gyroscopeService, IPassportDataManager passportDataManager, IDateTimeService dateTimeService)
        {
            _gyroscopeService = gyroscopeService;
            _gyroscopeService.SubscribeGyroscopeReadingUpdatedEvent(
                OrientationSensor_ReadingChanged,
                () =>
                {
                    _isAnimationInProgress = true;
                    IsBadgeAnimationPlaying = true;
                });
            _passportDataManager = passportDataManager;
            _dateTimeService = dateTimeService;

            CurrentLocalTime = _dateTimeService.Now.ToLocalTime();
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                CurrentLocalTime = _dateTimeService.Now.ToLocalTime();
                return true;
            });
            BadgeAnimationProgress = 0f; // reset cached progress
        }

        private void OrientationSensor_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            if (e.Reading.AngularVelocity.Length() > 2.0)
            {
                if (!_isAnimationInProgress)
                {
                    _isAnimationInProgress = true;
                    IsBadgeAnimationPlaying = true;

                    Device.StartTimer(TimeSpan.FromMilliseconds(ValidEuPassportAnimationDurationInMs), () =>
                    {
                        IsBadgeAnimationPlaying = false;
                        BadgeAnimationProgress = 0f;
                        _isAnimationInProgress = false;
                        return false;
                    });
                }
            }
        }

        public bool ShouldUseDanishForAccessibility => CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "da";

        public void UpdateView()
        {
            ShowHideInfoIcon();

            OnPropertyChanged(nameof(IsTestSelected));
            OnPropertyChanged(nameof(IsRecoverySelected));
            OnPropertyChanged(nameof(IsVaccineSelected));
            OnPropertyChanged(nameof(PassportViewModel));
            OnPropertyChanged(nameof(QrCodeString));
            OnPropertyChanged(nameof(IsQrCodeValid));
            OnPropertyChanged(nameof(ValidPassportText));
            OnPropertyChanged(nameof(IsLoadingPassport));
            OnPropertyChanged(nameof(Birthdate));
            OnPropertyChanged(nameof(FullName));
            OnPropertyChanged(nameof(IsInfoIconVisible));
            OnPropertyChanged(nameof(ShouldUseDanishForAccessibility));

            if (PassportItemsViewModel != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsQrCodeValid = PassportViewModel?.IsValid ?? false;
                    ValidPassportText = "PASSPORT_PAGE_VALID_BORDER_CONTROL_PASSPORT_TEXT".Translate();
                    TextColor = Color.White;
                });
            }
        }

        public SinglePassportViewModel SelectEuPassport(EuPassportType euPassportType)
        {
            if (euPassportType == EuPassportType.RECOVERY)
            {
                return PassportItemsViewModel.SelectedFamilyMemberPassport.EuRecoveryPassports.FirstOrDefault();
            }
            if (euPassportType == EuPassportType.VACCINE)
            {
                return PassportItemsViewModel.SelectedFamilyMemberPassport.EuVaccinePassports.FirstOrDefault();
            }
            if (euPassportType == EuPassportType.TEST)
            {
                return PassportItemsViewModel.SelectedFamilyMemberPassport.EuTestPassports.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public void ShowHideInfoIcon()
        {

            if (PassportItemsViewModel?.IsMoreThanOneEuPassportAvailable ?? false)
            {
                IsInfoIconVisible = false;
            }
            else
            {
                IsInfoIconVisible = true;
            }
        }

        public ICommand OnEuInfoButtonClicked => new Command(async () => await ExecuteOnceAsync(OpenEuInfoPage));
        
        public async Task OpenEuInfoPage()
        {
            await _navigationService.PushPage(new InfoBorderControlPage());
        }
        
        public void StartGyroService()
        {
            _gyroscopeService.TurnOnOrientation();
        }
        public void StopGyroService()
        {
            _gyroscopeService.TurnOffOrientation();
        }
    }
}
