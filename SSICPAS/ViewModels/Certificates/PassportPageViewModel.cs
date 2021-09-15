using SkiaSharp;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Data;
using SSICPAS.Enums;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Base;
using SSICPAS.ViewModels.Menu;
using SSICPAS.Views.Certificates;
using SSICPAS.Views.Menu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.Certificates
{
    public class PassportPageViewModel : BaseViewModel
    {
        private static int _isRefreshInProgressGuard; // 0 = false, other values = true. (original C convention) 
        private const int ValidDkPassportAnimationDurationInMs = 3000;
        private volatile bool _isAnimationInProgress;

        public bool IsGyroNotEnabled => !GyroscopeService.IsGyroEnabled;

        public ICommand RefreshCommand => new Command(execute: async () =>
            await ParallelizationUtils.CreateInterlockedTask(async () =>
            {
                try
                {
                    IsRefreshing = true;
                    await Task.Delay(1000); // await to show spinner for at least 1 second
                    var timeSinceLastFetch = _dateTimeService.Now - _preferencesService.GetUserPreferenceAsDateTime(PreferencesKeys.LATEST_PASSPORT_CALL_TO_BACKEND_TIMESTAMP);
                    if (timeSinceLastFetch < new TimeSpan(0, 5, 0))
                        return;

                    await ThreadSafeExecuteOnceAsync(async () =>
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ShowManuelFetchSpinner = true;
                        });
                        ItemsViewModel = await _passportDataManager.FetchPassport();
                    });
                }
                finally
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        IsRefreshing = false;
                        ShowManuelFetchSpinner = false;
                        UpdateView();
                    });
                }
            }, ref _isRefreshInProgressGuard));

        private DateTime _currentLocalTime;
        public DateTime CurrentLocalTime
        {
            get => _currentLocalTime;
            set
            {
                _currentLocalTime = value;
                OnPropertyChanged(nameof(CurrentLocalTime));
            }
        }
                
        private readonly IDialogService _dialogService;
        private readonly IPassportDataManager _passportDataManager;
        private readonly IGyroscopeService _gyroscopeService;
        private readonly IPreferencesService _preferencesService;
        private readonly IScreenshotDetectionService _screenshotDetectionService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IConnectivityService _connectivityService;

        private PassportItemListViewModel _itemsListViewModel;
        private FamilyPassportItemsViewModel _itemsViewModel = new FamilyPassportItemsViewModel();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private string _badgeIconAnimationSource;
        public string BadgeIconAnimationSource
        {
            get => _badgeIconAnimationSource;
            set
            {
                _badgeIconAnimationSource = value;
                OnPropertyChanged(nameof(BadgeIconAnimationSource));
            }
        }

        private bool _isBadgeAnimationPlaying;
        public bool IsBadgeAnimationPlaying
        {
            get => _isBadgeAnimationPlaying;
            set
            {
                _isBadgeAnimationPlaying = value;
                OnPropertyChanged(nameof(IsBadgeAnimationPlaying));
            }
        }

        private float _badgeAnimationProgress;
        public float BadgeAnimationProgress
        {
            get => _badgeAnimationProgress;
            set
            {
                _badgeAnimationProgress = value;
                OnPropertyChanged(nameof(BadgeAnimationProgress));
            }
        }

        public string AwaitingPassportHeader => IsChildenPassportSelected
            ? AddEndingToChildensName(ItemsViewModel.SelectedFamilyMemberPassport.DkInfo.PassportData.FirstName) + " " + "AWAITING_PASSPORT_HEADER_CHILD_EU".Translate()
            : "AWAITING_PASSPORT_HEADER_EU".Translate();
        
        public ICommand TestInfoButton => new Command(async () => await ExecuteOnceAsync(GoToTestInfoPage));

        public ICommand RecoveryInfoButton => new Command(async () => await ExecuteOnceAsync(GoToRecoveryInfoPage));

        public ICommand ToggleInformationCommand => new Command<PassportType>(async x =>
            await ThreadSafeExecuteOnceAsync(async () => await PassportTypeChanged(x)));

        public bool IsRecoveryAvailable => ItemsViewModel.IsEuRecoveryPassportForSelectedMemberAvailable;
        public bool IsVaccineAvailable => ItemsViewModel.IsEuVaccinePassportForSelectedMemberAvailable;
        public bool IsTestAvailable => ItemsViewModel.IsEuTestPassportForSelectedMemberAvailable;
        public bool ShowPassportCanNotBeRetrieved => !IsPassportAvailable && !_connectivityService.HasInternetConnection();
        public bool ShowAwaitingPassport => _connectivityService.HasInternetConnection() &&
            (IsChildenPassportSelected ?
            !IsPassportAvailable && !IsFetchingPassport :
            !IsPassportAvailable && !HasExistingPassport && !IsFetchingPassport);
        public bool ShowAwaitingPassportHasPassportBefore => _connectivityService.HasInternetConnection() && 
            (IsChildenPassportSelected ?
            false :
            !IsPassportAvailable && HasExistingPassport && !IsFetchingPassport);
        public bool IsChildenPassportSelected => ItemsViewModel.SelectedFamilyMember > 0;
        public bool IsFetchingPassport => _passportDataManager.IsContinuouslyFetchingPassport && !ShowPassportCanNotBeRetrieved;
        public SKColor[] BottomGradientList => ItemsViewModel?.SelectedPassportType == PassportType.UNIVERSAL_EU
                ? SSICPASColorGradient.MainPageStatusGradientBlue.Gradient()
                : SSICPASColorGradient.MainPageStatusGradientGreen.Gradient();
        public SKColor[] TopGradientList => SSICPASColorGradient.MainPageStatusGradientBlue.Gradient();
        public SKColor[] TopGradient => SSICPASColorGradient.MainPageCrownGradient.Gradient();
        public SKColor[] ClockGradient => SSICPASColorGradient.ClockGradient.Gradient();

        public static PassportPageViewModel CreatePassportPageViewModel()
        {
            return new PassportPageViewModel(
                IoCContainer.Resolve<IPassportDataManager>(),
                IoCContainer.Resolve<IGyroscopeService>(),
                IoCContainer.Resolve<IPreferencesService>(),
                IoCContainer.Resolve<IDialogService>(),
                IoCContainer.Resolve<IScreenshotDetectionService>(),
                IoCContainer.Resolve<IDateTimeService>(),
                IoCContainer.Resolve<IConnectivityService>()
            );
        }
    
        public PassportPageViewModel(
            IPassportDataManager passportDataManager,
            IGyroscopeService gyroscopeService,
            IPreferencesService preferencesService,
            IDialogService dialogService,
            IScreenshotDetectionService screenshotDetectionService,
            IDateTimeService dateTimeService,
            IConnectivityService connectivityService)
        {
            _passportDataManager = passportDataManager;
            _gyroscopeService = gyroscopeService;
            _gyroscopeService.SubscribeGyroscopeReadingUpdatedEvent(
                OrientationSensor_ReadingChanged,
                () =>
                {
                    _isAnimationInProgress = true;
                    IsBadgeAnimationPlaying = true;
                });
            _preferencesService = preferencesService;
            _dialogService = dialogService;
            _screenshotDetectionService = screenshotDetectionService;
            _dateTimeService = dateTimeService;
            _connectivityService = connectivityService;
            _hasExistingPassport = _preferencesService.GetUserPreferenceAsBoolean(PreferencesKeys.EXIST_VALID_DK_PASSPORTS);
            _passportDataManager.StopContinuousFetching += StartOrStopContinuousFetching;
            _passportDataManager.StartContinuousFetching += StartOrStopContinuousFetching;

            CurrentLocalTime = _dateTimeService.Now.ToLocalTime();
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                CurrentLocalTime = _dateTimeService.Now.ToLocalTime();
                return true;
            });

            BadgeAnimationProgress = 0f; // reset cached progress
        }

        private async Task OpenPassportPicker()
        {
            List<PassportTypePickerViewModel> pickerViewModels = new List<PassportTypePickerViewModel>
            {
                new PassportTypePickerViewModel(PassportType.DK_LIMITED)
                {
                    IsSelected = ItemsViewModel?.SelectedPassportType != PassportType.UNIVERSAL_EU,
                    SelectedPassportType = PassportType.DK_LIMITED
                },
                new PassportTypePickerViewModel(PassportType.UNIVERSAL_EU)
                {
                    IsSelected = ItemsViewModel?.SelectedPassportType == PassportType.UNIVERSAL_EU,
                    SelectedPassportType = PassportType.UNIVERSAL_EU
                }
            };

            
            await _dialogService.ShowPicker(
                pickerViewModels,
                 obj =>
                {
                    SelectionControl pickedType = obj as SelectionControl;
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await PassportTypeChanged(pickedType.SelectedPassportType);
                    });
                },
                "PASSPORT_TYPE_PICKER_TEXT".Translate(),
                ItemsViewModel.FamilyMembersNames
                    .Select((name, i) => new SelectionControl
                    {
                        Text = name,
                        IsSelected = i == ItemsViewModel.SelectedFamilyMember,
                        NumberInFamilyList = i,
                        SelectedPassportType = ItemsViewModel.SelectedPassportType
                    }),
                async obj =>
                {
                    SelectionControl pickedType = obj as SelectionControl;
                    ItemsViewModel.SelectedFamilyMember = pickedType!.NumberInFamilyList;

                    await FamilyMemberChanged();
                },
                async () =>
                {
                    await ShowEuPassportAlert(this);
                });
        }

        private void OrientationSensor_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            if (e.Reading.AngularVelocity.Length() > 2.0)
            {
                if (!_isAnimationInProgress)
                {
                    _isAnimationInProgress = true;
                    IsBadgeAnimationPlaying = true;

                    Device.StartTimer(TimeSpan.FromMilliseconds(ValidDkPassportAnimationDurationInMs), () =>
                    {
                        IsBadgeAnimationPlaying = false;
                        BadgeAnimationProgress = 0f;
                        _isAnimationInProgress = false;
                        return false;
                    });
                }
            }
        }

        private async void StartOrStopContinuousFetching() => await FetchPassport();

        public async Task FetchPassport(bool force = false)
        {
            Debug.Print($"{nameof(PassportPageViewModel)}.{nameof(FetchPassport)} is called");

            await ThreadSafeExecuteOnceAsync(async () =>
            {
                Device.BeginInvokeOnMainThread(() => { IsLoadingPassport = true; });

                FamilyPassportItemsViewModel response = await _passportDataManager.FetchPassport(force);

                ItemsViewModel = response;

                UpdateView();
            });
        }

        public async Task PassportTypeChanged(PassportType passportType)
        {
            bool shouldChangeView = ItemsViewModel.SelectedPassportType != PassportType.UNIVERSAL_EU &&
                                    passportType == PassportType.UNIVERSAL_EU
                                    || ItemsViewModel.SelectedPassportType == PassportType.UNIVERSAL_EU &&
                                    passportType != PassportType.UNIVERSAL_EU;

            ItemsViewModel.SelectedPassportType = passportType;
            _passportDataManager.UpdateSelectedPassportPreference(passportType);
            if (shouldChangeView)
            {
                await PushPassportPageWithViewModel(this);
            }
            else
            {
                UpdateView();
            }
        }

        public async Task FamilyMemberChanged()
        {
            _passportDataManager.UpdateSelectedPassportPreference(PassportType.UNIVERSAL_EU);
            await FetchPassport();
            await PushPassportPageWithViewModel(this);
        }

        public void UpdateView()
        {
            if (ItemsViewModel == null) return;

            Device.BeginInvokeOnMainThread(() =>
            {
                OnPropertyChanged(nameof(IsVaccineAvailable));
                OnPropertyChanged(nameof(IsTestAvailable));
                OnPropertyChanged(nameof(IsRecoveryAvailable));

                IsLoadingPassport = true;
                IsLimitedDKCode =
                    ItemsViewModel?.SelectedPassportType == PassportType.DK_LIMITED;

                IsPassportAvailable =
                    ItemsViewModel?.SelectedPassportType == PassportType.UNIVERSAL_EU
                        ? ItemsViewModel?.IsAnyEuPassportForSelectedMemberAvailable ?? false 
                        : ItemsViewModel?.SelectedPassport != null;

                if (IsPassportAvailable)
                {
                    QrCodeString = ItemsViewModel?.SelectedPassport?.QRToken ?? string.Empty;
                    FullName = ItemsViewModel?.SelectedPassport?.FullName ?? string.Empty;
                    Birthdate = DateUtils.ParseDateOfBirth(
                        ItemsViewModel,
                        time =>
                            time.ToLocaleDateFormat(
                                ItemsViewModel?.SelectedPassportType == PassportType.UNIVERSAL_EU),
                        false);
                    
                    VaccineHeaderValue =
                        ItemsViewModel?.SelectedFamilyMemberPassport.EuVaccinePassports?.FirstOrDefault()?.PassportData.MarketingAuthorizationHolder;
                    NegativeTestHeaderValue =
                        ItemsViewModel?.SelectedFamilyMemberPassport.EuTestPassports?.FirstOrDefault()?.PassportData.TypeOfTest;
                    OnPropertyChanged(nameof(RecoveryHeaderValue));

                    IsQrCodeValid = ItemsViewModel?.IsPassportValid ?? false;
                    ValidPassportText = GetValidPassportText();
                    RaisePropertyChanged(() => BottomGradientList);

                    SetEuPassportView();
                }
                IsInternationalPage = ItemsViewModel?.SelectedPassportType == PassportType.UNIVERSAL_EU;
                IsLoadingPassport = false;
                RaisePropertyChanged(() => IsFetchingPassport);
                RaisePropertyChanged(() => ShowPassportCanNotBeRetrieved);
                RaisePropertyChanged(() => ShowAwaitingPassport);
                RaisePropertyChanged(() => ShowAwaitingPassportHasPassportBefore);
                RaisePropertyChanged(() => AwaitingPassportHeader);
                RaisePropertyChanged(() => IsChildenPassportSelected);
                RaisePropertyChanged(() => IsFetchingPassport);
                RaisePropertyChanged(() => IsMoreThanOneAvailable);
                RaisePropertyChanged(() => IsOnlyVaccineAvailable);
                RaisePropertyChanged(() => IsOnlyTestAvailable);
                RaisePropertyChanged(() => IsOnlyRecoveryAvailable);
                RaisePropertyChanged(() => IsRefreshing);
                RaisePropertyChanged(() => ShowManuelFetchSpinner);
            });
        }

        private void SetEuPassportView()
        {
            if (IsQrCodeValid)
            {
                IsInfoIconVisible = true;
                BadgeIconImageSource = SSICPASImage.ValidPassportIcon.Image();
            }
            else
            {
                IsInfoIconVisible = false;
                BadgeIconImageSource = SSICPASImage.ExclamationIconBlue.Image();
            }
            if (IsOnlyVaccineAvailable)
            {
                SingleEuPassportType = EuPassportType.VACCINE;
            }
            else if (IsOnlyRecoveryAvailable)
            {
                SingleEuPassportType = EuPassportType.RECOVERY;
            }
            else if (IsOnlyTestAvailable)
            {
                SingleEuPassportType = EuPassportType.TEST;
            }
        }

        private string GetValidPassportText()
        {
            switch (ItemsViewModel.SelectedPassportType)
            {
                case PassportType.UNIVERSAL_EU:
                    return "PASSPORT_PAGE_VALID_BORDER_CONTROL_PASSPORT_TEXT".Translate();
                default:
                    return "PASSPORT_PAGE_VALID_PASSPORT_TEXT".Translate();
            }
        }

        private string AddEndingToChildensName(string name)
        {
            CultureInfo culture = new CultureInfo("LANG_DATEUTIL".Translate());

            if (culture.Name == "da-DK")
            {
                if (name.ToLower().EndsWith("s") || name.ToLower().EndsWith("x") || name.ToLower().EndsWith("z"))
                    return name + "'";
                return name + "s";
            }
            return name + "'s";
        }
        
        public void StartGyroService() => _gyroscopeService.TurnOnOrientation();

        public void StopGyroService() => _gyroscopeService.TurnOffOrientation();

        public ICommand OpenMenuPage => new Command(async () =>
        {
            await ExecuteOnceAsync(async () =>
            {
                await _navigationService.PushPage(new MenuPage(new MenuPageViewModel()));
            });
        });

        public ICommand VaccinationInfoButton => new Command(async () => await ExecuteOnceAsync(GoToVaccinationInfoPage));

        private async Task GoToVaccinationInfoPage() => 
            await _navigationService.PushPage(new PassportInfoModalView(ItemsViewModel, EuPassportType.VACCINE), true, PageNavigationStyle.PushModallySheetPageIOS);

        private async Task GoToTestInfoPage() => 
            await _navigationService.PushPage(new PassportInfoModalView(ItemsViewModel, EuPassportType.TEST), true, PageNavigationStyle.PushModallySheetPageIOS);

        private async Task GoToRecoveryInfoPage() => 
            await _navigationService.PushPage(new PassportInfoModalView(ItemsViewModel, EuPassportType.RECOVERY), true, PageNavigationStyle.PushModallySheetPageIOS);

        public ICommand OpenPassportTypePicker => new Command(async () => await ExecuteOnceAsync(OpenPassportPicker));

        public ICommand OnQrInfoButtonClicked => new Command(async () => await ExecuteOnceAsync(OpenQrInfoPage));

        private async Task OpenQrInfoPage() => await _navigationService.PushPage(new PassportContent());

        public ICommand OnQREuInfoButtonClicked => new Command(async () => await ExecuteOnceAsync(OpenQrEuInfoPage));

        public async Task OpenQrEuInfoPage() => await _navigationService.PushPage(new InfoBorderControlPage());

        public static Page FetchCurrentPassportPage()
        {
            int savedPassportType = IoCContainer.Resolve<IPreferencesService>()
                .GetUserPreferenceAsInt(PreferencesKeys.PASSPORT_TYPE_SETTING);
            if (savedPassportType == -1)
            {
                return new PassportPageDkView();
            }

            return (PassportType)Enum.ToObject(typeof(PassportType), savedPassportType) == PassportType.UNIVERSAL_EU
                ? new PassportPageEuView()
                : (ContentPage)new PassportPageDkView();
        }

        public static async Task PushPassportPageWithViewModel(PassportPageViewModel viewModel)
        {
            if (viewModel.ItemsViewModel.SelectedPassportType == PassportType.UNIVERSAL_EU)
            {
                // For people without children we need to show a dialog after the picker is closed
                if (viewModel.ItemsViewModel.FamilyMembersNames.Count <= 1)
                {
                    await ShowEuPassportAlert(viewModel);
                }

                IoCContainer.Resolve<INavigationService>().ChangePageInTab(new PassportPageEuView(),
                    TabPageLocationEnum.PassportPage);
            }
            else
            {
                IoCContainer.Resolve<INavigationService>().ChangePageInTab(new PassportPageDkView(),
                    TabPageLocationEnum.PassportPage);
            }
        }

        private static async Task ShowEuPassportAlert(PassportPageViewModel viewModel)
        {
            string title = "PASSPORT_PAGE_EU_NOTIFICATON_HEADER".Translate();
            string description = "PASSPORT_PAGE_EU_NOTIFICATON_DESCRIPTION".Translate();
            string accept = "PASSPORT_PAGE_EU_NOTIFICATON_AACCEPT".Translate();
            await viewModel._dialogService.ShowAlertAsync(title, description, true, true, StackOrientation.Horizontal, accept, null);
        }

        ~PassportPageViewModel()
        {
            _passportDataManager.StopContinuousFetching -= StartOrStopContinuousFetching;
            _passportDataManager.StartContinuousFetching -= StartOrStopContinuousFetching;
        }

        public async void OnScreenshotTaken(object sender)
        {
            await _screenshotDetectionService.ShowPassportPageScreenshotProtectionDialog();
        }

        #region Bindable properties

        public FamilyPassportItemsViewModel ItemsViewModel
        {
            get => _itemsViewModel;
            set
            {
                _itemsViewModel = value;
                ItemsListViewModel = new PassportItemListViewModel(_itemsViewModel);
                OnPropertyChanged(nameof(ItemsViewModel));
            }
        }

        public PassportItemListViewModel ItemsListViewModel
        {
            get => _itemsListViewModel;
            set
            {
                _itemsListViewModel = value;
                OnPropertyChanged(nameof(ItemsListViewModel));
            }
        }

        private string _birthdate { get; set; }

        public string Birthdate
        {
            get => _birthdate;
            set
            {
                _birthdate = value;
                OnPropertyChanged(nameof(Birthdate));
            }
        }

        private string _fullName { get; set; }

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        private string _validPassportText { get; set; }

        public string ValidPassportText
        {
            get => _validPassportText;
            set
            {
                _validPassportText = value;
                OnPropertyChanged(nameof(ValidPassportText));
            }
        }

        private string _qrCodeString { get; set; }

        public string QrCodeString
        {
            get => _qrCodeString;
            set
            {
                _qrCodeString = value;
                OnPropertyChanged(nameof(QrCodeString));
            }
        }

        private bool _isQrCodeValid { get; set; } = true;

        public bool IsQrCodeValid
        {
            get => _isQrCodeValid;
            set
            {
                _isQrCodeValid = value;

                // If code is invalid (expired), we trigger fetch to server.
                if (!_isQrCodeValid)
                {
                    Task.Run(async () => await FetchPassport(true));
                }

                OnPropertyChanged(nameof(IsQrCodeValid));
            }
        }

        private bool _isInternationalCode { get; set; }

        public bool IsInternationalCode
        {
            get => _isInternationalCode;
            set
            {
                _isInternationalCode = value;
                OnPropertyChanged(nameof(IsInternationalCode));
            }
        }

        private bool _isLimitedDKCode { get; set; }

        public bool IsLimitedDKCode
        {
            get => _isLimitedDKCode;
            set
            {
                _isLimitedDKCode = value;
                OnPropertyChanged(nameof(IsLimitedDKCode));
            }
        }

        private bool _isPassportAvailable { get; set; }

        public bool IsPassportAvailable
        {
            get => _isPassportAvailable;
            set
            {
                _isPassportAvailable = value;
                MessagingCenter.Send<object>(this, MessagingCenterKeys.PASSPORT_UPDATED);
                OnPropertyChanged(nameof(IsPassportAvailable));
            }
        }

        private bool _isLoadingPassport;

        public bool IsLoadingPassport
        {
            get => _isLoadingPassport;
            set
            {
                _isLoadingPassport = value;
                OnPropertyChanged(nameof(IsLoadingPassport));
            }
        }

        private bool _hasExistingPassport;

        public bool HasExistingPassport
        {
            get => _hasExistingPassport;
            set
            {
                _hasExistingPassport = value;
                OnPropertyChanged(nameof(HasExistingPassport));
            }
        }

        private bool _isInfoIconVisible;

        public bool IsInfoIconVisible
        {
            get => _isInfoIconVisible;
            set
            {
                _isInfoIconVisible = value;
                OnPropertyChanged(nameof(IsInfoIconVisible));
            }
        }

        private ImageSource _badgeIconImageSource;

        public ImageSource BadgeIconImageSource
        {
            get => _badgeIconImageSource;
            set
            {
                _badgeIconImageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        public bool IsOnlyTestAvailable => IsTestAvailable && !IsVaccineAvailable && !IsRecoveryAvailable;
        public bool IsOnlyVaccineAvailable => !IsTestAvailable && IsVaccineAvailable && !IsRecoveryAvailable;
        public bool IsOnlyRecoveryAvailable => !IsTestAvailable && !IsVaccineAvailable && IsRecoveryAvailable;

        public bool IsMoreThanOneAvailable => ItemsViewModel.IsMoreThanOneEuPassportAvailable;

        private string _vaccineHeaderValue;

        public string VaccineHeaderValue
        {
            get => _vaccineHeaderValue;
            set
            {
                _vaccineHeaderValue = value;
                OnPropertyChanged(nameof(VaccineHeaderValue));
            }
        }

        private string _negativeTestHeaderValue;

        public string NegativeTestHeaderValue
        {
            get => _negativeTestHeaderValue;
            set
            {
                _negativeTestHeaderValue = value;
                OnPropertyChanged(nameof(NegativeTestHeaderValue));
            }
        }

        private EuPassportType _singleEuPassportType;

        public EuPassportType SingleEuPassportType
        {
            get => _singleEuPassportType;
            set
            {
                _singleEuPassportType = value;
                OnPropertyChanged(nameof(SingleEuPassportType));
            }
        }

        public bool IsInternationalPage
        {
            get
            {
                return _isInternationalCode;
            }
            set
            {
                _isInternationalCode = value;
                OnPropertyChanged(nameof(IsInternationalPage));
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        private bool _showManuelFetchSpinner;
        public bool ShowManuelFetchSpinner
        {
            get => _showManuelFetchSpinner;
            set
            {
                _showManuelFetchSpinner = value;
                OnPropertyChanged(nameof(ShowManuelFetchSpinner));
            }
        }

        public string RecoveryHeaderValue =>
            ItemsViewModel?.SelectedFamilyMemberPassport.EuRecoveryPassports?.FirstOrDefault().PassportData
                .RecoveryDisease;

        #endregion
    }
}