using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using SSICPAS.Configuration;
using SSICPAS.Core.Services.Enum;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model;
using SSICPAS.Core.Services.Model.Converter;
using SSICPAS.Data;
using SSICPAS.Enums;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Scanner;
using SSICPAS.Services.Status;
using SSICPAS.Services.Translator;
using SSICPAS.ViewModels.Base;
using SSICPAS.Views.Menu;
using SSICPAS.Views.ScannerPages;
using Xamarin.Forms;

#nullable enable
namespace SSICPAS.ViewModels.QrScannerViewModels
{
    public class ImagerScannerViewModel : BaseViewModel
    {
        private const double ScanFailureVibrationDuration = 500;
        private const double ScanSuccessVibrationDuration = 250;
        
        private readonly ITokenProcessorService _tokenProcessorService;
        private readonly IDeviceFeedbackService _deviceFeedbackService;
        private readonly IScreenshotDetectionService _screenshotDetectionService;

        private bool _inTabbar = false;
        private static bool _handlingResult = false;
        private IScannerFactory _scannerFactoryService;
        private readonly IImagerScanner? _scanner;

        private async Task OnBackButtonPressed()
        {
            DisableScanner();
            await _navigationService.PopPage();
        }

        public static ImagerScannerViewModel CreateImagerScannerViewModel()
        {
            return new ImagerScannerViewModel(
                IoCContainer.Resolve<IScannerFactory>(),
                IoCContainer.Resolve<ITokenProcessorService>(),
                IoCContainer.Resolve<IDeviceFeedbackService>(),
                IoCContainer.Resolve<IRatListService>(),
                IoCContainer.Resolve<IScreenshotDetectionService>()
            );
        }

        public ImagerScannerViewModel(IScannerFactory scannerFactoryService,
            ITokenProcessorService tokenProcessorService, IDeviceFeedbackService deviceFeedbackService, IRatListService ratListService, IScreenshotDetectionService screenshotDetectionService)
            : base()
        {
            _tokenProcessorService = tokenProcessorService;
            var translator = new DCCValueSetTranslator(ratListService);
            DigitalCovidValueSetTestAndTestManufacturerNameTranslator ratListTranslator = new DigitalCovidValueSetTestAndTestManufacturerNameTranslator(ratListService);
            _tokenProcessorService.SetDCCValueSetTranslator(translator,ratListTranslator);
            _scannerFactoryService = scannerFactoryService;
            _deviceFeedbackService = deviceFeedbackService;
            _screenshotDetectionService = screenshotDetectionService;
            _scanner = _scannerFactoryService.GetAvailableScanner();
            _scanner.SetSelectedScanner(new ScannerModel(id: "INTERNAL_IMAGER", name: "Imager", connectionState: true));
        }

        public void EnableScanner(bool isQuickScan = false)
        {
            if (_scanner.IsEnabled == false)
            {
                _scanner.Enable();
                _scanner.Receiver.OnBarcodeScanned += BarcodeScanned;
                _scanner.SetConfig(_scannerFactoryService.GetScannerConfig());
            }
        }

        public void DisableScanner()
        {
            _scanner.Disable();
            _scanner.Receiver.OnBarcodeScanned -= BarcodeScanned;
        }

        private async void BarcodeScanned(object sender, StatusEventArgs e)
        {
            if (_handlingResult) return;
            _handlingResult = true;
            Debug.Print(e.Data);
            if (_navigationService.FindCurrentPage() is IScanResultView)
            {
                //Pop to scanning page for quickscan
                await _navigationService.PopPage();
            }

            await ThreadSafeExecuteOnceAsync(() =>  HandleScanResult(e.Data));
        }

        public async Task HandleScanResult(String result)
        {
            if (PopupNavigation.Instance.PopupStack.Any()) return;

            try
            {
                TokenValidateResultModel model = await _tokenProcessorService.DecodePassportTokenToModel(result);

                if (model == null) return;                   

                if ((model.DecodedModel is Core.Services.Model.EuDCCModel._1._0._x.DCCPayload
                    || model.DecodedModel is Core.Services.Model.EuDCCModel._1._3._0.DCCPayload)
                    && model.ValidationResult == TokenValidateResult.Valid)
                {
                    _deviceFeedbackService.Vibrate(ScanSuccessVibrationDuration);
                    _deviceFeedbackService.PlaySound(SoundKeys.VALID_SCAN_SOUND);
                    await OnScanEUFinish(model);
                }
                else
                {
                    if (model.ValidationResult == TokenValidateResult.Valid)
                    {                    
                        _deviceFeedbackService.Vibrate(ScanSuccessVibrationDuration);
                        _deviceFeedbackService.PlaySound(SoundKeys.VALID_SCAN_SOUND);
                    }
                    else
                    {
                        _deviceFeedbackService.Vibrate(ScanFailureVibrationDuration);
                        _deviceFeedbackService.PlaySound(SoundKeys.INVALID_SCAN_SOUND);
                    }
                    await OnScanFinish(model);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                _handlingResult = false;
            }
        }

        private async Task OnScanFinish(TokenValidateResultModel model)
        {
            await _navigationService.PushPage(new ImagerSuccessResultPage(model), true, PageNavigationStyle.PushModallyFullscreen);
        }
        
        private async Task OnScanEUFinish(TokenValidateResultModel model)
        {
            bool isTest;
            bool isResult;
            if ( model.DecodedModel is Core.Services.Model.EuDCCModel._1._0._x.DCCPayload)
            {
                var euDccPayload = model.DecodedModel as Core.Services.Model.EuDCCModel._1._0._x.DCCPayload;
                isTest = euDccPayload.DCCPayloadData.DCC.Tests?.Any() ?? false;
                isResult = euDccPayload.DCCPayloadData.DCC.Recovery?.Any() ?? false;
            }
            else
            {
                var euDccPayload = model.DecodedModel as Core.Services.Model.EuDCCModel._1._3._0.DCCPayload;
                isTest = euDccPayload.DCCPayloadData.DCC.Tests?.Any() ?? false;
                isResult = euDccPayload.DCCPayloadData.DCC.Recovery?.Any() ?? false;
            }
            
            EuPassportType passportType = isTest ? EuPassportType.TEST :
                isResult ? EuPassportType.RECOVERY : EuPassportType.VACCINE;
            await _navigationService.PushPage(new ImagerEuVaccineResultPage(model.DecodedModel, passportType), true, PageNavigationStyle.PushModallyFullscreen);
        }

        private async Task OnScanFailure(TokenValidateResultModel model)
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                await _navigationService.PushPage(new ScannerErrorPage(), true, PageNavigationStyle.PushModallyFullscreen, model);
            });
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
    }
}