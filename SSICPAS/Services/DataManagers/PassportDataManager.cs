using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model.Converter;
using SSICPAS.Core.Services.Model.EuDCCModel._1._3._0;
using SSICPAS.Data;
using SSICPAS.Enums;
using SSICPAS.Models;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Translator;
using SSICPAS.ViewModels.Certificates;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSICPAS.Services.DataManagers
{
    public class PassportDataManager : IPassportDataManager
    {
        private DateTime _lastFetchDataDateTime = DateTime.MinValue;
        private IPassportsService _passportsService;
        private IPreferencesService _preferencesService;
        private ISettingsService _settingsService;
        private IDateTimeService _dateTimeService;
        private ISessionManager _sessionManager;
        private IDialogService _dialogService;
        private IFamilyPassportStorageRepository _passportStorageRepository;

        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        private static List<TimeSpan> _continuousFetchingDelay;
        private static TimeSpan _suspendedFetchingDelay;
        private static bool _firstRunSafeCheck = true;

        private int _fetchingAttempt = 0;
        private TimeSpan _elapsedTimeUntilAbortFetching;
        private TimeSpan _fetchingTimeLeft => new TimeSpan(_continuousFetchingDelay.Sum(r => r.Ticks)) - IncreasingDelay(_fetchingAttempt);

        public bool IsContinuouslyFetchingPassport { get; set; }
        public Action StopContinuousFetching { get; set; }
        public Action StartContinuousFetching { get; set; }

        private FamilyPassportItemsViewModel _passportItemsViewModel { get; set; }

        public PassportDataManager()
        {
            Init();
            _sessionManager.OnSessionTrackEnded += OnAppResume;
            MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.LANGUAGE_CHANGED, CheckLanguageSetting);
        }

        public void Init()
        {
            _passportsService = IoCContainer.Resolve<IPassportsService>();
            _preferencesService = IoCContainer.Resolve<IPreferencesService>();
            _dateTimeService = IoCContainer.Resolve<IDateTimeService>();
            _settingsService = IoCContainer.Resolve<ISettingsService>();
            _sessionManager = IoCContainer.Resolve<ISessionManager>();
            _dialogService = IoCContainer.Resolve<IDialogService>();
            _passportStorageRepository = IoCContainer.Resolve<IFamilyPassportStorageRepository>();
            _lastFetchDataDateTime = DateTime.MinValue;
            _fetchingAttempt = 0;

            _continuousFetchingDelay = CreateListOfTimeSpansFromIntArray(_settingsService.ContinuousFetchingDelaysSeconds);
            _suspendedFetchingDelay = TimeSpan.FromSeconds(_settingsService.SuspendedFetchingDelaySeconds);

            IsContinuouslyFetchingPassport = false;
            StopContinuousFetching = null;
            StartContinuousFetching = null;
            _passportItemsViewModel = null;
            _elapsedTimeUntilAbortFetching = default(TimeSpan);
        }

        private List<TimeSpan> CreateListOfTimeSpansFromIntArray(string intsString)
        {
            // Convert string containing ints separated by , or ; into list of ints
            List<int> numbers = intsString
                .Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(stringNumber => Convert.ToInt32(stringNumber))
                .ToList();

            // Create a list of timespans from list of integers
            List<TimeSpan> intervals = new List<TimeSpan>();
            numbers.ForEach(number => intervals.Add(TimeSpan.FromSeconds(number)));

            return intervals;
        }

        public async Task<FamilyPassportItemsViewModel> FetchPassport(bool forced = false)
        {
            Debug.Print($"{nameof(PassportDataManager)}.{nameof(FetchPassport)}: Fetching passport");

            //we want to fetch new data when bumping versionnumber
            if (VersionTracking.IsFirstLaunchForCurrentBuild && _firstRunSafeCheck)
            {
                forced = true;
                _firstRunSafeCheck = false;
            }

            if (!forced && _passportItemsViewModel != null && string.IsNullOrEmpty(_passportItemsViewModel.AdditionalData.JobId) &&
                _passportItemsViewModel.IsDKPassportValid && !_passportItemsViewModel.ShouldPrefetchNewPassport)
            {
                Debug.Print("Returning existing passports from data manager");
                return _passportItemsViewModel;
            }
            else
            {
                if (!forced && _dateTimeService.Now - _lastFetchDataDateTime <= _suspendedFetchingDelay)
                {
                    Debug.Print("Fetching too soon. returning previous passports");
                    return _passportItemsViewModel;
                }

                _passportItemsViewModel = await FetchPassports(forced);
                SetupSavedPreference();
                //if the passport is being prepared, we start fetching
                if (!string.IsNullOrEmpty(_passportItemsViewModel.AdditionalData.JobId)
                    && _passportItemsViewModel.AdditionalData.JobStatus == PassportJobStatus.Inprogress)
                {
                    _fetchingAttempt = 0;
                    IsContinuouslyFetchingPassport = true;
                    //We want a fire and forget task
                    Task.Factory.StartNew(ContinuousFetchingInBackground).ConfigureAwait(false);
                }
                else if (_passportItemsViewModel.IsAnyPassportAvailable && _passportItemsViewModel.IsAllAvailablePassportValid)
                {
                    _fetchingAttempt = 0;
                }
                return _passportItemsViewModel;
            }
        }

        private async Task<FamilyPassportItemsViewModel> FetchPassports(bool forced)
        {
            _lastFetchDataDateTime = _preferencesService.GetUserPreferenceAsDateTime(PreferencesKeys.LATEST_PASSPORT_CALL_TO_BACKEND_TIMESTAMP);
            bool isForced = forced || (!_passportItemsViewModel?.IsDKPassportValid ?? false);
            return (await _passportsService.GetPassports(isForced)).Data;
        }

        private async void OnAppResume(object sender, SessionTrackingEventArgs e)
        {
            if (IsContinuouslyFetchingPassport && _elapsedTimeUntilAbortFetching + e.ElapsedTime <= _fetchingTimeLeft)
            {
                StartContinuousFetching?.Invoke();
                //if the total elapsed time is smaller than current delay, it will resume the fetching
                Task.Factory.StartNew(ContinuousFetchingInBackground).ConfigureAwait(false);
            }
            else if (IsContinuouslyFetchingPassport)
            {
                _fetchingAttempt = 0;
                IsContinuouslyFetchingPassport = false;
                _passportItemsViewModel = await FetchPassports(true);
                StopContinuousFetching?.Invoke();
                SetupSavedPreference();

                if (!string.IsNullOrEmpty(_passportItemsViewModel.AdditionalData.JobId) && _passportItemsViewModel.AdditionalData.JobStatus == PassportJobStatus.Inprogress)
                    ShowFailedToFetchPassportDialog();
            }
        }
        
        private async Task ContinuousFetchingInBackground()
        {
            // we only want one Continuous Fetching task
            await _semaphoreSlim.WaitAsync();
            try
            {
                do
                {
                    // We don't want to have a while loop running constantly every clock cycle
                    if (_dateTimeService.Now - _lastFetchDataDateTime <= IncreasingDelay(_fetchingAttempt))
                    {
                        try
                        {
                            await Task.Delay(
                                IncreasingDelay(_fetchingAttempt) - (_dateTimeService.Now - _lastFetchDataDateTime),
                                //this will make the session manager take control of execution of this task 
                                _sessionManager.RegisterCancellationToken());
                        }
                        catch (TaskCanceledException e)
                        {
                            _elapsedTimeUntilAbortFetching = _dateTimeService.Now - _lastFetchDataDateTime;
                            // on task canceled, we finish this immediately
                            return;
                        }
                    }
                    _fetchingAttempt++;
                    _passportItemsViewModel = await FetchPassports(true);
                    SetupSavedPreference();

                    if (_passportItemsViewModel.AdditionalData.JobStatus == PassportJobStatus.Done)
                    {
                        IsContinuouslyFetchingPassport = false;
                        _fetchingAttempt = 0;
                        StopContinuousFetching?.Invoke();
                        break;
                    }
                    else if (_fetchingAttempt >= _continuousFetchingDelay.Count)
                    {
                        IsContinuouslyFetchingPassport = false;
                        StopContinuousFetching?.Invoke();
                        ShowFailedToFetchPassportDialog();
                        // reach the end of continuous fetching. break and return to regular fetching 
                        break;
                    }
                } while (true);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public void UpdateSelectedPassportPreference(PassportType passportType)
        {
            _passportItemsViewModel.SelectedPassportType = passportType;
            _preferencesService.SetUserPreference(PreferencesKeys.PASSPORT_TYPE_SETTING, (int)_passportItemsViewModel.SelectedPassportType);
        }
        
        public TimeSpan IncreasingDelay(int attempts)
        {
            if (attempts < 0) throw new ArgumentOutOfRangeException();
            return attempts >= _continuousFetchingDelay.Count ? _suspendedFetchingDelay : _continuousFetchingDelay[attempts];
        }
        
        private void SetupSavedPreference()
        {
            int savedPassportType = _preferencesService.GetUserPreferenceAsInt(PreferencesKeys.PASSPORT_TYPE_SETTING);

            if (savedPassportType == -1)
            {
                _preferencesService.SetUserPreference(PreferencesKeys.PASSPORT_TYPE_SETTING, (int)_passportItemsViewModel.SelectedPassportType);
            }
            else
            {
                _passportItemsViewModel.SelectedPassportType = (PassportType)Enum.ToObject(typeof(PassportType), savedPassportType);
            }
        }

        public void Reset()
        {
            Init();
            Debug.Print("Reset PassportDataManager");
        }

        private async void CheckLanguageSetting(object sender)
        {
            if (_passportItemsViewModel == null)
            {
                _passportItemsViewModel = await _passportStorageRepository.GetFamilyPassportFromSecureStorage();
            }
            if (_passportItemsViewModel != null 
                && _passportItemsViewModel.AdditionalData.LanguageSelection != LocaleService.Current.GetLanguage())
            {
                Debug.Print("Language was changed, Begin re-translating");
                LanguageChanged();
                await _passportStorageRepository.SaveFamilyPassportToSecureStorage(_passportItemsViewModel);

                MessagingCenter.Send<object>(this, MessagingCenterKeys.PASSPORT_UPDATED);
            }
        }
        
        private void LanguageChanged()
        {
            ITokenProcessorService tokenProcessorService = IoCContainer.Resolve<ITokenProcessorService>();
            DCCValueSetTranslator translator = new DCCValueSetTranslator(IoCContainer.Resolve<IRatListService>());
            DigitalCovidValueSetTestAndTestManufacturerNameTranslator ratListTranslator = new DigitalCovidValueSetTestAndTestManufacturerNameTranslator(IoCContainer.Resolve<IRatListService>());
            tokenProcessorService.SetDCCValueSetTranslator(translator, ratListTranslator);
            string dkInfoToken = _passportItemsViewModel?.FamilyMembersGetParent?.PassportData?.SecureToken;
            string dkInfoJson = _passportItemsViewModel?.FamilyMembersGetParent?.PassportData?.DecodedJson;

            if (!string.IsNullOrEmpty(dkInfoJson))
            {
                ITokenPayload model = tokenProcessorService.MapToModelFromJson(dkInfoJson, dkInfoToken.Substring(0, 3));

                _passportItemsViewModel.FamilyMembersGetParent.PassportData =
                new PassportData(dkInfoToken, (DCCPayload)model, dkInfoJson);
            }
                
            _passportItemsViewModel.AdditionalData.LanguageSelection = LocaleService.Current.GetLanguage();
            
        }

        private void ShowFailedToFetchPassportDialog()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _dialogService.ShowStyleAlertAsync("DIALOG_PASSPORT_FAILED_TITLE".Translate(),
                    "DIALOG_PASSPORT_FAILED_BODY".Translate(), true, true, StackOrientation.Horizontal, "BIOMETRIC_DISMISS".Translate(), null, DialogStyle.Info);
            });
        }
    }
}