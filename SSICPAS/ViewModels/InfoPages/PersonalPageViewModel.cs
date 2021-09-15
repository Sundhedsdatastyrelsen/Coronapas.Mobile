using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;
using SSICPAS.Data;
using SSICPAS.Enums;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Translator;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Base;
using SSICPAS.ViewModels.Certificates;
using SSICPAS.ViewModels.Menu;
using SSICPAS.Views.InfoPages;
using SSICPAS.Views.Menu;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.InfoPages
{
    public class PersonalPageViewModel : BaseViewModel
    {
        FamilyPassportItemsViewModel _itemsViewModel = new FamilyPassportItemsViewModel();
        public FamilyPassportItemsViewModel ItemsViewModel
        {
            get => _itemsViewModel;
            set
            {
                _itemsViewModel = value;
                OnPropertyChanged(nameof(ItemsViewModel));
            }
        }

        public InfoVaccineTextViewModel InfoVaccineTextViewModel { get; set; }

        public ICommand ShowVaccineInfoCommand => new Command(async () => await ExecuteOnceAsync(ShowVaccineInfo));
        public ICommand ShowTestInfoCommand => new Command(async () => await ExecuteOnceAsync(ShowTestInfo));
        public ICommand ShowImmunityInfoCommand => new Command(async () => await ExecuteOnceAsync(ShowImmunityInfo));

        public string CheckBoxAccessibilityText => IsCheckBoxChecked ? "MY_PAGE_CHECKBOX".Translate() + " " + "MY_PAGE_CHECKBOX_CHECKED".Translate() : "MY_PAGE_CHECKBOX".Translate() + " " + "MY_PAGE_CHECKBOX_UNCHECKED".Translate();

        private LanguageSelection _selectedLanguage => LocaleService.Current.GetLanguage();
        private bool EnglishSelected => _selectedLanguage == LanguageSelection.English;

        public bool IsCheckBoxChecked
        {
            get
            {
                return Preferences.Get(nameof(IsCheckBoxChecked), false);
            }
            set
            {
                Preferences.Set(nameof(IsCheckBoxChecked), value);
                OnPropertyChanged(nameof(IsCheckBoxChecked));
                OnPropertyChanged(nameof(CheckBoxAccessibilityText));
            }
        }

        private bool _showPersonalPage = false;
        public bool ShowPersonalPage
        {
            get
            {
                return _showPersonalPage;
            }
            set
            {
                _showPersonalPage = value;
                OnPropertyChanged(nameof(ShowPersonalPage));
            }
        }

        private bool _showPersonalPageLocked = false;
        public bool ShowPersonalPageLocked
        {
            get
            {
                return _showPersonalPageLocked;
            }
            set
            {
                _showPersonalPageLocked = value;
                OnPropertyChanged(nameof(ShowPersonalPageLocked));
            }
        }

        private bool _showNoPersonalPage = true;
        public bool ShowNoPersonalPage
        {
            get
            {
                return _showNoPersonalPage;
            }
            set
            {
                _showNoPersonalPage = value;
                OnPropertyChanged(nameof(ShowNoPersonalPage));
            }
        }

        private bool _isDKInfoAvailable { get; set; }
        public bool IsDKInfoAvailable
        {
            get => _isDKInfoAvailable;
            set
            {
                _isDKInfoAvailable = value;
                OnPropertyChanged(nameof(IsDKInfoAvailable));
            }
        }

        private string _vaccineHeaderValue { get; set; }
        public string VaccineHeaderValue
        {
            get => _vaccineHeaderValue;
            set
            {
                _vaccineHeaderValue = value;
                OnPropertyChanged(nameof(VaccineHeaderValue));
            }
        }
        private string _vaccineValidTo { get; set; }
        public string VaccineValidTo
        {
            get => _vaccineValidTo;
            set
            {
                _vaccineValidTo = value;
                OnPropertyChanged(nameof(VaccineValidTo));
            }
        }
        private string _negativeTestHeaderValue { get; set; }
        public string NegativeTestHeaderValue
        {
            get => _negativeTestHeaderValue;
            set
            {
                _negativeTestHeaderValue = value;
                OnPropertyChanged(nameof(NegativeTestHeaderValue));
            }
        }
        private string _negativeTestDateValue { get; set; }
        public string NegativeTestDateValue
        {
            get => _negativeTestDateValue;
            set
            {
                _negativeTestDateValue = value;
                OnPropertyChanged(nameof(NegativeTestDateValue));
            }
        }
        private string _recoveryHeaderValue { get; set; }
        public string RecoveryHeaderValue
        {
            get => _recoveryHeaderValue;
            set
            {
                _recoveryHeaderValue = value;
                OnPropertyChanged(nameof(RecoveryHeaderValue));
            }
        }
        private string _recoveryValidToValue { get; set; }
        public string RecoveryValidToValue
        {
            get => _recoveryValidToValue;
            set
            {
                _recoveryValidToValue = value;
                OnPropertyChanged(nameof(RecoveryValidToValue));
            }
        }
        private bool _myPageOnboard = Preferences.Get(nameof(IsCheckBoxChecked), false);

        private readonly IPassportDataManager _passportDataManager;

        public bool IsVaccineAvailable => ItemsViewModel.FamilyMembersGetParent.PassportData.IsVaccineAvailable;
        public bool IsTestAvailable => ItemsViewModel.FamilyMembersGetParent.PassportData.IsTestAvailable;
        public bool IsRecoveryAvailable => ItemsViewModel.FamilyMembersGetParent.PassportData.IsRecoveryAvailable;

        public PersonalPageViewModel(IPassportDataManager passportDataManager)
        {
            _passportDataManager = passportDataManager;

            MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.PASSPORT_UPDATED, PerformOnPassportUpdate);
        }

        private void PerformOnPassportUpdate(object sender)
        {
            _ = FetchPassport();
        }

        public void SeeInformationButtonClicked()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _myPageOnboard = true;
                UpdateView();
            });
        }

        public void CheckBoxChanged(bool newValue)
        {
            IsCheckBoxChecked = newValue;
        }

        public void ShowPage()
        {
            if (_myPageOnboard)
            {
                ShowPersonalPageLocked = false;
                ShowNoPersonalPage = false;
                ShowPersonalPage = true;
            }
            else
            {
                ShowPersonalPageLocked = true;
                ShowPersonalPage = false;
                ShowNoPersonalPage = false;
            }
        }

        public void UpdateView()
        {
            IsDKInfoAvailable =
            (ItemsViewModel?.FamilyMembersGetParent?.PassportData?.IsTestAvailable ?? false) ||
            (ItemsViewModel?.FamilyMembersGetParent?.PassportData?.IsVaccineAvailable ?? false) ||
            (ItemsViewModel?.FamilyMembersGetParent?.PassportData?.IsRecoveryAvailable ?? false);

            if (IsDKInfoAvailable)
            {
                ShowPage();
                
            } else
            {
                ShowNoPersonalPage = true;
                ShowPersonalPageLocked = false;
                ShowPersonalPage = false;
            }
        }

        public void FetchData()
        {
            if (ItemsViewModel == null || ItemsViewModel?.FamilyMembersGetParent == null) return;

            OnPropertyChanged(nameof(IsVaccineAvailable));
            OnPropertyChanged(nameof(IsTestAvailable));
            OnPropertyChanged(nameof(IsRecoveryAvailable));

            VaccineHeaderValue = ItemsViewModel?.FamilyMembersGetParent.PassportData.MarketingAuthorizationHolder;
            VaccineValidTo = "MY_PAGE_FROM_TEXT".Translate() + " " + ItemsViewModel?.FamilyMembersGetParent.PassportData?.CertificateValidFrom?.LocaleFormatDate() ?? "-";

            NegativeTestHeaderValue = DCCValueSetTranslator.ToLocale(ItemsViewModel?.FamilyMembersGetParent.PassportData?.TypeOfTest, DCCValueSetEnum.TypeOfTest, EnglishSelected);
            NegativeTestDateValue = "MY_PAGE_TO_TEXT".Translate() + " " + (ItemsViewModel?.FamilyMembersGetParent.PassportData?.CertificateValidTo?.LocaleFormatDate() ?? "-") + " " + "MY_PAGE_TIME".Translate() + (ItemsViewModel?.FamilyMembersGetParent.PassportData?.CertificateValidTo?.LocaleFormatTime() ?? "-");

            RecoveryHeaderValue = DCCValueSetTranslator.ToLocale(ItemsViewModel?.FamilyMembersGetParent.PassportData?.RecoveryDisease, DCCValueSetEnum.Disease, EnglishSelected);
            RecoveryValidToValue = "MY_PAGE_TO_TEXT".Translate() + " " + ItemsViewModel?.FamilyMembersGetParent.PassportData?.CertificateValidTo?.LocaleFormatDate() ?? "-";
        }

        public async Task FetchPassport()
        {
            Debug.Print($"{nameof(PersonalPageViewModel)}.{nameof(FetchPassport)} is called");

            FamilyPassportItemsViewModel response = await _passportDataManager.FetchPassport();

            Device.BeginInvokeOnMainThread(() =>
            {
                ItemsViewModel = response;
                FetchData();
                UpdateView();
            });
        }

        public ICommand OpenMenuPage => new Command(async () =>
        {
            await ExecuteOnceAsync(async () =>
            {
                await _navigationService.PushPage(new MenuPage(new MenuPageViewModel()));
            });
        });

        private async Task ShowVaccineInfo()
        {
            await _navigationService.PushPage(new PersonalPageGenericInfoModal(ItemsViewModel.FamilyMembersGetParent, EuPassportType.VACCINE), true, PageNavigationStyle.PushModallySheetPageIOS);
        }

        private async Task ShowTestInfo()
        {
            await _navigationService.PushPage(new PersonalPageGenericInfoModal(ItemsViewModel.FamilyMembersGetParent, EuPassportType.TEST), true, PageNavigationStyle.PushModallySheetPageIOS);
        }

        private async Task ShowImmunityInfo()
        {
            await _navigationService.PushPage(new PersonalPageGenericInfoModal(ItemsViewModel.FamilyMembersGetParent, EuPassportType.RECOVERY), true, PageNavigationStyle.PushModallySheetPageIOS);
        }

        ~PersonalPageViewModel()
        {
            MessagingCenter.Unsubscribe<object>(this, MessagingCenterKeys.PASSPORT_UPDATED);
        }
    }
}