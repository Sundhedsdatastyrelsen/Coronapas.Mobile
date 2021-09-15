using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Core.Services.Enum;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model;
using DCCVersion_1_0_x = SSICPAS.Core.Services.Model.EuDCCModel._1._0._x;
using DCCVersion_1_3_0 = SSICPAS.Core.Services.Model.EuDCCModel._1._3._0;
using SSICPAS.Data;
using SSICPAS.Enums;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Translator;
using SSICPAS.ViewModels.Base;
using SSICPAS.Views.Menu;
using SSICPAS.Views.ScannerPages;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing;
using SSICPAS.Core.Services.Model.Converter;

namespace SSICPAS.ViewModels.QrScannerViewModels
{
    public class QRScannerViewModel : BaseViewModel, IScreenshotDetectorOnResultPage
    {
        private static readonly string _flashlightOnIconPath;
        private static readonly string _flashlightOffIconPath;

        private const double ScanFailureVibrationDuration = 500;
        private const double ScanSuccessVibrationDuration = 250;

        private readonly ITokenProcessorService _tokenProcessorService;
        private readonly IPopupService _popupService;
        private readonly IDeviceFeedbackService _deviceFeedbackService;
        private readonly IPreferencesService _preferencesService;
        private readonly IScreenshotDetectionService _screenshotDetectionService;

        private bool _inTabbar = false;
        private bool _isFlashlightSupported = true;
        private bool _isTorchOn;
        private string _currentFlashlightStateIconPath = _flashlightOffIconPath;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public bool IsFlashlightSupported
        {
            get => _isFlashlightSupported;
            set
            {
                _isFlashlightSupported = value;
                OnPropertyChanged(nameof(IsFlashlightSupported));
            }
        }
        public bool IsFlashlighthOn
        {
            get => _isTorchOn;
            private set
            {
                _isTorchOn = value;
                OnPropertyChanged(nameof(IsFlashlighthOn));
            }
        }
        public string CurrentFlashlightStateIconPath
        {
            get => _currentFlashlightStateIconPath;
            private set
            {
                _currentFlashlightStateIconPath = value;
                OnPropertyChanged(nameof(CurrentFlashlightStateIconPath));
            }
        }

        private bool _hasFlashlightPermissions;
        public bool HasFlashlightPermissions
        {
            get => _hasFlashlightPermissions;
            private set
            {
                _hasFlashlightPermissions = value;
                OnPropertyChanged(nameof(HasFlashlightPermissions));
            }
        }

        static QRScannerViewModel()
        {
            _flashlightOnIconPath = App.Current.Resources["ScannerFlashlightOnIcon"].ToString();
            _flashlightOffIconPath = App.Current.Resources["ScannerFlashlightOffIcon"].ToString();
        }

        public static QRScannerViewModel CreateQRScannerViewModel()
        {
            return new QRScannerViewModel(
                IoCContainer.Resolve<ITokenProcessorService>(),
                IoCContainer.Resolve<IPopupService>(),
                IoCContainer.Resolve<IDeviceFeedbackService>(),
                IoCContainer.Resolve<IPreferencesService>(),
                IoCContainer.Resolve<IRatListService>(),
                IoCContainer.Resolve<IScreenshotDetectionService>()
            );
        }

        public QRScannerViewModel(ITokenProcessorService tokenProcessorService,
            IPopupService popupService,
            IDeviceFeedbackService deviceFeedbackService,
            IPreferencesService preferencesService,
            IRatListService ratListService,
            IScreenshotDetectionService screenshotDetectionService)
        {
            _tokenProcessorService = tokenProcessorService;
            _popupService = popupService;
            _deviceFeedbackService = deviceFeedbackService;
            _preferencesService = preferencesService;
            _screenshotDetectionService = screenshotDetectionService;
            
            var translator = new DCCValueSetTranslator(ratListService);
            var ratListTranslator = new DigitalCovidValueSetTestAndTestManufacturerNameTranslator(ratListService);
            _tokenProcessorService.SetDCCValueSetTranslator(translator, ratListTranslator);
        }

        public bool HasCameraPermissions => Permissions.CheckStatusAsync<Permissions.Camera>().Result == PermissionStatus.Granted;
        
        public async Task<bool> CheckFlashlightPermissions() 
        {
            var hasPermissions = await Permissions.CheckStatusAsync<Permissions.Flashlight>() == PermissionStatus.Granted;
            HasFlashlightPermissions = hasPermissions;
            return hasPermissions;
        }

        public ICommand ToggleFlashlight => new Command(async () =>
        {
            if (await CheckFlashlightPermissions())
            {
                IsFlashlighthOn = !IsFlashlighthOn;
                CurrentFlashlightStateIconPath = IsFlashlighthOn ? _flashlightOnIconPath : _flashlightOffIconPath;
                if (Device.RuntimePlatform == Device.iOS)
                {
                    //This part of the code should not be ran in android
                    //Xamarin essential flashlight require camera permission in android, which we are not explicitly declaring in AndroidManifest
                    //This cause crashing in some device due to how OEM config their device os and android version 
                    if (IsFlashlighthOn)
                    {
                        await Flashlight.TurnOnAsync();
                    }
                    else
                    {
                        await Flashlight.TurnOffAsync();
                    }
                }

            }
        });

        public async void ResetFlashlightState(object sender = null)
        {
            if (Device.RuntimePlatform == Device.iOS && IsFlashlighthOn)
            {
                await Flashlight.TurnOffAsync();
            }
            CurrentFlashlightStateIconPath = _flashlightOffIconPath;
            IsFlashlighthOn = false;
        }

        public bool InTabbar
        {
            get => _inTabbar;
            set
            {
                _inTabbar = value;
                OnPropertyChanged(nameof(InTabbar));
            }
        }

        public override ICommand BackCommand => new Command(async () =>
        {
            await ThreadSafeExecuteOnceAsync(async () =>
            {
                if (!InTabbar)
                {
                    if (_navigationService.FindCurrentPage() is QRScannerPage qr)
                    {
                        qr.DestroyScannerView();
                    }
                    
                    await _navigationService.OpenLandingPage();
                }
            });
        });

        public ICommand OpenMenuCommand => new Command(async () =>
        {
            await ExecuteOnceAsync(async () =>
            {
                if (!InTabbar)
                {
                    await _navigationService.PushPage(new MenuPage(new QRScannerMenuViewModel()));
                }
            });
        });

        public ICommand OpenSettingsCommand => new Command(async () =>
        {
            await ExecuteOnceAsync(async () =>
            {
                await IoCContainer.Resolve<IDeeplinkingService>().GoToAppSettings();
            });
        });

        public async Task HandleScanResult(Result result)
        {
            await _semaphore.WaitAsync();
            try
            {
                
                if (PopupNavigation.Instance.PopupStack.Any()) return;
                if (await IsResultOpen()) return;

                TokenValidateResultModel model = await _tokenProcessorService.DecodePassportTokenToModel(result.Text);

                if (model == null) return;
                
                if (model.ValidationResult == TokenValidateResult.Valid)
                {
                    _deviceFeedbackService.Vibrate(ScanSuccessVibrationDuration);
                    _deviceFeedbackService.PlaySound(SoundKeys.VALID_SCAN_SOUND);
                    await OnScanSuccess(model.DecodedModel);
                }
                else
                {
                    _deviceFeedbackService.Vibrate(ScanFailureVibrationDuration);
                    _deviceFeedbackService.PlaySound(SoundKeys.INVALID_SCAN_SOUND);
                    await OnScanFailure(model);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task OnScanSuccess(ITokenPayload payload)
        {
            if (payload is DCCVersion_1_0_x.DCCPayload cwtPayload_1_0_x)
            {
                await showResultFromCWT(cwtPayload_1_0_x);
                
            }
            else if (payload is DCCVersion_1_3_0.DCCPayload cwtPayload_1_3_0)
            {
                await showResultFromCWT(cwtPayload_1_3_0);

            }
            else
            {
                await _popupService.ShowScanSuccessPopup(payload);
            }
        }

        private async Task showResultFromCWT(DCCVersion_1_0_x.DCCPayload cwtPayload)
        {
            bool anyVaccinations = cwtPayload.DCCPayloadData.DCC.Vaccinations?.Any() ?? false;
            bool anyTestResults = cwtPayload.DCCPayloadData.DCC.Tests?.Any() ?? false;
            bool anyRecovery = cwtPayload.DCCPayloadData.DCC.Recovery?.Any() ?? false;

            if (anyVaccinations)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (await IsResultOpen()) return;
                    await _navigationService.PushPage(new ScanEuResultView(cwtPayload, EuPassportType.VACCINE), true, Enums.PageNavigationStyle.PushModallyFullscreen);
                });
            }
            else if (anyTestResults)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (await IsResultOpen()) return;
                    await _navigationService.PushPage(new ScanEuResultView(cwtPayload, EuPassportType.TEST), true, Enums.PageNavigationStyle.PushModallyFullscreen);
                });
            }
            else if (anyRecovery)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (await IsResultOpen()) return;
                    await _navigationService.PushPage(new ScanEuResultView(cwtPayload, EuPassportType.RECOVERY), true, Enums.PageNavigationStyle.PushModallyFullscreen);
                });
            }
            else
            {
                await OnScanFailure(new TokenValidateResultModel() { ValidationResult = TokenValidateResult.Invalid });
            }
        }

        private async Task showResultFromCWT(DCCVersion_1_3_0.DCCPayload cwtPayload)
        {
            bool anyVaccinations = cwtPayload.DCCPayloadData.DCC.Vaccinations?.Any() ?? false;
            bool anyTestResults = cwtPayload.DCCPayloadData.DCC.Tests?.Any() ?? false;
            bool anyRecovery = cwtPayload.DCCPayloadData.DCC.Recovery?.Any() ?? false;

            if (anyVaccinations)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (await IsResultOpen()) return;
                    await _navigationService.PushPage(new ScanEuResultView(cwtPayload, EuPassportType.VACCINE), true, Enums.PageNavigationStyle.PushModallyFullscreen);
                });
            }
            else if (anyTestResults)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (await IsResultOpen()) return;
                    await _navigationService.PushPage(new ScanEuResultView(cwtPayload, EuPassportType.TEST), true, Enums.PageNavigationStyle.PushModallyFullscreen);
                });
            }
            else if (anyRecovery)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (await IsResultOpen()) return;
                    await _navigationService.PushPage(new ScanEuResultView(cwtPayload, EuPassportType.RECOVERY), true, Enums.PageNavigationStyle.PushModallyFullscreen);
                });
            }
            else
            {
                await OnScanFailure(new TokenValidateResultModel() { ValidationResult = TokenValidateResult.Invalid });
            }
        }


        private async Task OnScanFailure(TokenValidateResultModel model)
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                if (await IsResultOpen()) return;
                await _navigationService.PushPage(new ScannerErrorPage(), true, Enums.PageNavigationStyle.PushModallyFullscreen, model);
            });
        }

        private async Task<bool> IsResultOpen()
        {
            return await _navigationService.FindCurrentPageAsync() is IScanResultView;
        }

        public async void OnScreenshotTaken(object sender)
        {
            Debug.Print($"{nameof(QRScannerViewModel)}.{nameof(OnScreenshotTaken)} is called");

            try
            {
                await _semaphore.WaitAsync();

                if (PopupNavigation.Instance.PopupStack.Any()) return;
                if (await IsResultOpen()) return;

                if (_navigationService.FindCurrentPage(InTabbar) is QRScannerPage qr)
                {
                    qr.DisableScannerView();
                }

                await _screenshotDetectionService.ShowScannerPageScreenshotProtectionDialog();
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async void OnScreenshotTimerElapsed(object sender)
        {
            Debug.Print($"{nameof(QRScannerViewModel)}.{nameof(OnScreenshotTimerElapsed)} is called");

            if (await _navigationService.FindCurrentPageAsync(InTabbar) is QRScannerPage qr)
            {
                qr.EnableScannerView();
            }
        }
    }
}