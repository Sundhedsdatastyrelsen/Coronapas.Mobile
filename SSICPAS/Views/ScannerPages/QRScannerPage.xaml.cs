using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSICPAS.Configuration;
using SSICPAS.Core.Logging;
using SSICPAS.Data;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.QrScannerViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace SSICPAS.Views.ScannerPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QRScannerPage : ContentPage
    {
        private static int DelayBetweenContinousScans = 100;
        private static int DelayBetweenAnalyzingFrames = 100;
        private static int MinResolutionHeightThreshold = 720;
        private static bool LanguageChanged = false;
        
        private bool _hasAskedForCameraPermission = false;
        private bool _inTabbar = false;
        private static ZXingScannerView _scannerView;
        private MobileBarcodeScanningOptions _scanningOptions = new MobileBarcodeScanningOptions
        {
            DelayBetweenAnalyzingFrames = DelayBetweenAnalyzingFrames,
            DelayBetweenContinuousScans = DelayBetweenContinousScans,
            PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE },
            TryHarder = true,
            UseNativeScanning = false,
            InitialDelayBeforeAnalyzingFrames = 0,
            CameraResolutionSelector = SelectLowestResolution,
            TryInverted = true,
        };

        private TimeSpan _focusAssistTimespan = new TimeSpan(0, 0, 2);

        public QRScannerPage()
        {
            InitializeComponent();
            BindingContext = QRScannerViewModel.CreateQRScannerViewModel();
            MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.LANGUAGE_CHANGED, OnLanguageChanged);
        }

        ~QRScannerPage()
        {
            MessagingCenter.Unsubscribe<object>(this, MessagingCenterKeys.LANGUAGE_CHANGED);
        }
        
        public void SetFromTabbar()
        {
            _inTabbar = true;
            ((QRScannerViewModel)BindingContext).InTabbar = _inTabbar;
        }

        public async void OnViewDisplayed()
        {
            //Initialize camera so it only turns on when the page is visible. Only in tabbar.
            if (!_inTabbar) return;
            if (_scannerView != null) return;

            if (Device.RuntimePlatform == Device.iOS)
            {
                IoCContainer.Resolve<INavigationService>().SetStatusBar(Color.Transparent, Color.White);
            }

            await IoCContainer.Resolve<IScreenshotDetectionService>().StartupShowScreenshotProtectionDialog(true);
            await CreateScannerView();
        }

        public void OnViewHidden()
        {
            //Tear down camera completely. Only in tabbar.
            if (!_inTabbar) return;
            if (_scannerView == null) return;

            DestroyScannerView();
        }

        protected override async void OnAppearing()
        {
            MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.GOING_TO_BACKGROUND,(BindingContext as QRScannerViewModel).ResetFlashlightState);

            if (_inTabbar) return;
            if (_scannerView != null && !LanguageChanged)
            {
                _scannerView.IsAnalyzing = true;
                return;
            }

            LanguageChanged = false;
            //Initialize camera from the front page
            IoCContainer.Resolve<INavigationService>().SetStatusBar(SSICPASColor.NavigationHeaderBackgroundColor.Color(), SSICPASColor.SSIContentTextColor.Color());
            await IoCContainer.Resolve<IScreenshotDetectionService>().StartupShowScreenshotProtectionDialog(true);
            await CreateScannerView();

            base.OnAppearing();
        }

        private void OnLanguageChanged(object _)
        {
            LanguageChanged = true;
        }
        
        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<object>(this, MessagingCenterKeys.GOING_TO_BACKGROUND);

            ((QRScannerViewModel)BindingContext).ResetFlashlightState();
            if (_inTabbar) return;
            if (_scannerView == null) return;

            // Front page - stop analyzing but keep scanning, so the scanner does not have to restart on every
            // result or when the menu is opened. Destroy logic will be handled when back is executed.
            _scannerView.IsAnalyzing = false;
            base.OnDisappearing();
        }

        private async void OnScanResult(Result result)
        {
            await ((QRScannerViewModel)BindingContext).HandleScanResult(result);
        }

        private async Task CreateScannerView()
        {
            try
            {
                if (((QRScannerViewModel)BindingContext).HasCameraPermissions)
                {
                    ((QRScannerViewModel)BindingContext).RaisePropertyChanged(() => ((QRScannerViewModel)BindingContext).HasCameraPermissions);
                    // Note: We only want to show the flashlight switch in the view, if we have both Camera and Flashlight permissions
                    await Task.Run(() => (BindingContext as QRScannerViewModel)?.CheckFlashlightPermissions() ?? Task.CompletedTask);
                    MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN, ((QRScannerViewModel)BindingContext).OnScreenshotTaken);
                    MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN_TIMER_ELAPSED, ((QRScannerViewModel)BindingContext).OnScreenshotTimerElapsed);

                    _scannerView ??= new ZXingScannerView();
                    _scannerView.OnScanResult += OnScanResult;
                    _scannerView.Options = _scanningOptions;
                    _scannerView.SetBinding(ZXingScannerView.IsTorchOnProperty, nameof(QRScannerViewModel.IsFlashlighthOn));

                    try
                    {
                        // Check that Flashlight feature is available by triggering ZXingScannerView.IsTorchOnProperty
                        (BindingContext as QRScannerViewModel)?.ResetFlashlightState();
                    }
                    catch (FeatureNotSupportedException)
                    {
                        // Handle not supported on device, by disabling flashlight functionality from view.
                        ((QRScannerViewModel)BindingContext).IsFlashlightSupported = false;
                    }

                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        if (_scannerView == null) return;
                        ScannerContainer.Children.Add(_scannerView);
                        _scannerView.IsScanning = true;
                        _scannerView.IsAnalyzing = true;
                    });
                    
                    Device.StartTimer(_focusAssistTimespan, () =>
                    {
                        if (_scannerView == null)
                        {
                            return false;
                        }

                        _scannerView.AutoFocus();
                        return _scannerView.IsScanning;
                    });
                }
                else
                {
                    if (_hasAskedForCameraPermission) return;
                    
                    if (await Permissions.RequestAsync<Permissions.Camera>() == PermissionStatus.Granted)
                    {
                        _hasAskedForCameraPermission = true;
                        ((QRScannerViewModel)BindingContext).RaisePropertyChanged(() => ((QRScannerViewModel)BindingContext).HasCameraPermissions);
                        await Task.Run(() => (BindingContext as QRScannerViewModel)?.CheckFlashlightPermissions() ?? Task.CompletedTask);
                        await CreateScannerView();
                    }
                    
                    _hasAskedForCameraPermission = true;
                }
            }
            catch (Exception e)
            {
                IoCContainer.Resolve<ILoggingService>().LogException(LogSeverity.WARNING, e, $"{nameof(QRScannerPage)}.{nameof(CreateScannerView)}: error when creating scanner");
            }
        }

        /// <summary>
        /// Prevents camera preview distortion. Selects the lowest resolution within the tolerance of device aspect ratio.
        /// Lowest resolution is selected as the lower the resolution, the faster QR detection should be.
        /// </summary>
        /// <param name="availableResolutions">
        /// API generated list of available camera resolutions for the scanner view.
        /// </param>
        /// <returns>
        /// Lowest resolution within tolerance.
        /// </returns>
        private static CameraResolution SelectLowestResolution(List<CameraResolution> availableResolutions)
        {            
            CameraResolution result = null;
            double aspectTolerance = 0.1;
            var targetRatio = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Width;
            var targetHeight = DeviceDisplay.MainDisplayInfo.Height;
            var minDiff = double.MaxValue;
            
            availableResolutions
                .Where(r => Math.Abs(((double) r.Width / r.Height) - targetRatio) < aspectTolerance)
                .ForEach(
                    res =>
                    {
                        if (Math.Abs(res.Height - targetHeight) < minDiff && res.Height >= MinResolutionHeightThreshold)
                        {
                            minDiff = Math.Abs(res.Height - targetHeight);
                            result = res;
                        }
                    });
            
            return result;
        }

        public void DestroyScannerView()
        {
            MessagingCenter.Unsubscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN);
            MessagingCenter.Unsubscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN_TIMER_ELAPSED);

            if (_scannerView == null) return;

            _scannerView.IsScanning = false;
            _scannerView.IsAnalyzing = false;
            _scannerView.OnScanResult -= OnScanResult;
            ScannerContainer.Children.Remove(_scannerView);
            _scannerView = null;
        }

        public void DisableScannerView()
        {
            if (_scannerView == null) return;

            _scannerView.IsAnalyzing = false;
            _scannerView.OnScanResult -= OnScanResult;
        }

        public void EnableScannerView()
        {
            if (_scannerView == null) return;

            _scannerView.IsAnalyzing = true;
            _scannerView.OnScanResult += OnScanResult;
        }

        protected override bool OnBackButtonPressed()
        {
            ((QRScannerViewModel)BindingContext).BackCommand.Execute(null);
            return true;
        }
    }
}