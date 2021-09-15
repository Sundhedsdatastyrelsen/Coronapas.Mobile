using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Fingerprint;
using SSICPAS.Core.Data;
using SSICPAS.Data;
using SSICPAS.Enums;
using SSICPAS.Models;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.ViewModels.Base;
using SSICPAS.Views.Elements;
using SSICPAS.Views.Menu;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.Menu
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private IUserService _userService;
        private readonly ISecureStorageService<PinCodeBiometricsModel> _pinCodeService;
        private readonly IDialogService _dialogService;
        private readonly ITextService _textService;
        private readonly IPreferencesService _preferencesService;
        
        public virtual bool FromLandingPage => false;
        private PinCodeBiometricsModel _biometricsModelInStorage;
        private bool _initialBiometricSwitchValue;

        public string SwitchSoundOnAndOffText => SoundSettingEnabled ? "SETTING_SWITCH_CHANGE_ON".Translate() : "SETTING_SWITCH_CHANGE_OFF".Translate();
        public string SwitchVibrationOnAndOffText => VibrationSettingEnabled ? "SETTING_SWITCH_VIBRATION_CHANGE_ON".Translate() : "SETTING_SWITCH_VIBRATION_CHANGE_OFF".Translate();
        public string BiometricSwitchOnAndOffText => BiometricsEnabled ? "SETTING_SWITCH_BIOMETRICS_CHANGE_ON".Translate() : "SETTING_SWITCH_BIOMETRICS_CHANGE_OFF".Translate();
        private bool _danishSelected;
        public bool DanishSelected
        {
            get
            {
                return _danishSelected;
            }
            set
            {
                if (_danishSelected == value)
                {
                    return;
                }
                _danishSelected = value;
                RaisePropertyChanged(() => DanishSelected);
                DisplayAppRestartDialog(value);
            }
        }

        private string _sectionTitleSecurity = "SETTINGS_SECTION_2_TITLE".Translate();
        public string SectionTitleSecurity
        {
            get
            {
                return _sectionTitleSecurity;
            }
            set
            {
                _sectionTitleSecurity = value;
                OnPropertyChanged(nameof(SectionTitleSecurity));
            }
        }

        private string _sectionTitleScanner = "QRSCANNER_SETTINGS_SECTION_TITLE".Translate();
        public string SectionTitleScanner
        {
            get
            {
                return _sectionTitleScanner;
            }
            set
            {
                _sectionTitleScanner = value;
                OnPropertyChanged((nameof(SectionTitleScanner)));
            }
        }

        private string _settingSound = "SETTINGS_SOUND".Translate();
        public string SettingsSound
        {
            get
            {
                return _settingSound;
            }
            set
            {
                _settingSound = value;
                OnPropertyChanged(nameof(SettingsSound));
            }
        }

        private string _settingsVibration = "SETTINGS_VIBRATION".Translate();
        public string SettingsVibration
        {
            get
            {
                return _settingsVibration;
            }
            set
            {
                _settingsVibration = value;
                OnPropertyChanged(nameof(SettingsVibration));
            }
        }

        public bool VibrationSettingEnabled =>
            _preferencesService.GetUserPreferenceAsBoolean(PreferencesKeys.SCANNER_VIBRATION_SETTING);

        public bool SoundSettingEnabled =>
            _preferencesService.GetUserPreferenceAsBoolean(PreferencesKeys.SCANNER_SOUND_SETTING);

        public bool BiometricsEnabled => _biometricsModelInStorage?.HasBiometrics ?? false;

        private bool _biometricsAvailable => CrossFingerprint.Current.IsAvailableAsync(false).GetAwaiter().GetResult();
        
        public bool IsVisibleBiometricsSetting
        {
            get
            {
                if (!_biometricsAvailable) {
                    return false;
                }
                return !FromLandingPage;
            }
        }
        
        public ICommand LogOut => new Command(async () => await ExecuteOnceAsync(async () => await _userService.UserLogoutAsync()));

        public ICommand ChangePinCode => new Command(async () => await ExecuteOnceAsync(async () =>
        {
            await _navigationService.PushPage(
                new ChangePinCodeView(ChangePinCodeViewModel.CreateChangePinCodeViewModel()));
        }));
        
        public SettingsPageViewModel(IUserService userService,
            ISecureStorageService<PinCodeBiometricsModel> pinCodeService,
            IDialogService dialogService,
            ITextService textService,
            IPreferencesService preferencesService)
        {
            _userService = userService;
            _pinCodeService = pinCodeService;
            _dialogService = dialogService;
            _textService = textService;
            _preferencesService = preferencesService;
            
            _biometricsModelInStorage = _pinCodeService.GetSecureStorageAsync(SecureStorageKeys.PIN_LOCATION).GetAwaiter().GetResult();
            _initialBiometricSwitchValue = BiometricsEnabled;
            
            LanguageSelection selectedLanguage = LocaleService.Current.GetLanguage();
            DanishSelected = selectedLanguage == LanguageSelection.Danish;
        }

        private async void DisplayAppRestartDialog(bool danishSelected)
        {
            LanguageSelection selectedLanguage = LocaleService.Current.GetLanguage();

            if (selectedLanguage == LanguageSelection.Danish && danishSelected || selectedLanguage == LanguageSelection.English && !danishSelected)
            {
                return;
            }

            string title = $"{"SETTINGS_CHOOSE_LANGUAGE_DIALOG_TITLE_DANISH".Translate()}/\n"
                + "SETTINGS_CHOOSE_LANGUAGE_DIALOG_TITLE_ENGLISH".Translate();
            string content = $"{"SETTINGS_CHOOSE_LANGUAGE_DIALOG_CONTENT_DANISH".Translate()}\n"
                + "SETTINGS_CHOOSE_LANGUAGE_DIALOG_CONTENT_ENGLISH".Translate();
            string accept = "SETTINGS_RESTART_APP_BUTTON".Translate();
            string cancel = "CANCEL_BUTTON_CHOOSE_LANGUAGE".Translate();
            bool result = await _dialogService.ShowAlertAsync(title, content, true, true, StackOrientation.Horizontal, accept, cancel);
            
            if (result)
            {
                if (danishSelected)
                    _textService.SetLocale(LanguageSelection.Danish.ToISOCode());
                else
                    _textService.SetLocale(LanguageSelection.English.ToISOCode());

                if (this.FromLandingPage)
                {
                    await _navigationService.OpenLandingPage();
                }
                else
                {
                    await _navigationService.OpenTabbar();
                }
                
                MessagingCenter.Send<object>(this, MessagingCenterKeys.LANGUAGE_CHANGED);
            }
            else
            {
                DanishSelected = !danishSelected;
            }
        }
        
        public void VibrationSettingChanged(CustomConsentSwitch.CustomConsentSwitchEventArgs e)
        {
            _preferencesService.SetUserPreference(PreferencesKeys.SCANNER_VIBRATION_SETTING, e.Selected);
            OnPropertyChanged(nameof(VibrationSettingEnabled));
            OnPropertyChanged(nameof(SwitchVibrationOnAndOffText));
        }
        
        public void SoundSettingChanged(CustomConsentSwitch.CustomConsentSwitchEventArgs e)
        {
            _preferencesService.SetUserPreference(PreferencesKeys.SCANNER_SOUND_SETTING, e.Selected);
            OnPropertyChanged(nameof(SoundSettingEnabled));
            OnPropertyChanged(nameof(SwitchSoundOnAndOffText));
        }

        public void BiometricSettingChanged(CustomConsentSwitch.CustomConsentSwitchEventArgs e)
        {
            if (_initialBiometricSwitchValue == e.Selected) return;
            
            if (!_biometricsAvailable)
            {
                _dialogService.ShowAlertAsync("ERROR".Translate(), "BIOMETRIC_NOT_REGISTERED".Translate(), true, true, StackOrientation.Horizontal, "BIOMETRIC_DISMISS".Translate(), null);
                OnPropertyChanged(nameof(BiometricsEnabled));
                OnPropertyChanged(nameof(BiometricSwitchOnAndOffText));
                return;
            }
            
            if (_navigationService.FindCurrentPage() is SettingsPage)
            {
                _navigationService.PushPage(new ChangePinCodeView(ToggleBiometricsViewModel.CreateNewToggleBiometricsViewModel()));
            }
        }

        public override Task ExecuteOnReturn(object data)
        {
            _biometricsModelInStorage = _pinCodeService.GetSecureStorageAsync(SecureStorageKeys.PIN_LOCATION).GetAwaiter().GetResult();
            OnPropertyChanged(nameof(BiometricsEnabled));
            _initialBiometricSwitchValue = BiometricsEnabled;
            return base.ExecuteOnReturn(data);
        }
    }
}