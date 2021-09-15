using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Lifecycle;
using Plugin.CurrentActivity;
using Plugin.Fingerprint;
using Sentry;
using SSICPAS.Configuration;
using SSICPAS.Core.CustomExceptions;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Logging;
using SSICPAS.Droid.Services;
using SSICPAS.Enums;
using SSICPAS.Models.Exceptions;
using SSICPAS.Services.Interfaces;
using SSICPAS.ViewModels.Base;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace SSICPAS.Droid
{
    
    [Activity(
        Label = "Coronapas",
        Theme = "@style/MainTheme",
        LaunchMode = LaunchMode.SingleTop,
        NoHistory = false,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : FormsAppCompatActivity
    {
        INavigationService _navigationService;
        
        public MainActivity()
        {
            
        }

        public MainActivity(IntPtr ptr, JniHandleOwnership handle)
        {
            
        }
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
#if !DEBUG && !TEST
            Window.SetFlags(WindowManagerFlags.Secure, WindowManagerFlags.Secure);
#endif
            SentryXamarin.Init(options =>
            {
#if APPSTORE
                options.Dsn = "REDACTED_BEFORE_PUBLISHING_THE_SOURCE_CODE";
                options.Environment = "Production";
#elif DEVELOPMENT
                options.Dsn = "REDACTED_BEFORE_PUBLISHING_THE_SOURCE_CODE";
                options.Environment = "Development";
#elif TEST
                options.Dsn = "REDACTED_BEFORE_PUBLISHING_THE_SOURCE_CODE";
                options.Environment = "Test";
#elif APPSTOREBETA
                options.Dsn = "REDACTED_BEFORE_PUBLISHING_THE_SOURCE_CODE";
                options.Environment = "AppStore Beta (pre-production)";
#endif
                options.AddXamarinFormsIntegration();
                options.DisableAppDomainUnhandledExceptionCapture(); //We want to capture exceptions ourselves, so we can anonymize stacktraces.
            });

            global::Xamarin.Forms.Forms.SetFlags(new string[] { "Expander_Experimental" });

            CrossFingerprint.SetCurrentActivityResolver(() => this);
            CrossCurrentActivity.Current.Activity = this;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            }

            base.OnCreate(savedInstanceState);
            Rg.Plugins.Popup.Popup.Init(this);

            ZXing.Net.Mobile.Forms.Android.Platform.Init();

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            AndroidEnvironment.UnhandledExceptionRaiser += OnUnhandledAndroidException;

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            global::Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, savedInstanceState);
            CustomTabsConfiguration.CustomTabsClosingMessage = null;
            AiForms.Dialogs.Dialogs.Init(this);
            RegisterAndroidServices();

            LoadApplication(new App());
            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

            var settingsService = IoCContainer.Resolve<ISettingsService>();
            _navigationService = IoCContainer.Resolve<INavigationService>();

            IoCContainer.Resolve<IOrientationService>().OnChangeSupportedOrientation += SetSupportedOrientation;
        }

        private void SetSupportedOrientation(SupportedOrientation orientation)
        {
            RequestedOrientation = orientation.ToAndroidScreenOrientation();
        }

        private void RegisterAndroidServices()
        {
            IoCContainer.RegisterSingleton<IConfigurationProvider, ConfigurationProvider>();
            IoCContainer.RegisterSingleton<IStatusBarService, StatusBarService>();
            IoCContainer.RegisterSingleton<IBrightnessService, BrightnessService>();
            IoCContainer.RegisterInterface<IScannerFactory, AndroidScannerFactory>();
            IoCContainer.RegisterInterface<IDeeplinkingService, AndroidDeeplinkingService>();
            IoCContainer.RegisterInterface<IPlatformSettingsService, AndroidSettingsService>();
            IoCContainer.RegisterInterface<ICustomOAuthNativeUIFlagResolver, AndroidCustomOAuthNativeUIFlagResolver>();
            IoCContainer.RegisterInterface<IVoiceOverManager, DroidVoidOverManager>();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnDestroy()
        {
            AndroidEnvironment.UnhandledExceptionRaiser -= OnUnhandledAndroidException;
            base.OnDestroy();
        }

        private void OnUnhandledAndroidException(object sender, RaiseThrowableEventArgs e)
        {
            if (e?.Exception != null)
            {
                var loggingService = IoCContainer.Resolve<ILoggingService>();

                string message;
                LogSeverity logLevel;

                if (e.Exception is MissingSettingException)
                {
                    message = $"{nameof(MainActivity)}.{nameof(OnUnhandledAndroidException)}: {e.Exception.Message}";
                    logLevel = LogSeverity.FATAL;
                }
                else
                {
                    message = $"{nameof(MainActivity)}.{nameof(OnUnhandledAndroidException)}: "
                                       + (!e.Handled
                                       ? "Native unhandled crash"
                                       : "Native unhandled exception - not crashing");
                    logLevel = e.Handled
                        ? LogSeverity.WARNING
                        : LogSeverity.ERROR;
                }

                loggingService.LogException(logLevel, e.Exception, message, !e.Handled);

                if (e.Exception is FailedOperationSecureStorageException)
                {
                    _ = IoCContainer.Resolve<IUserService>().UserLogoutAsync(false).Wait(TimeSpan.FromSeconds(5));
                }
            }
        }

        public override async void OnBackPressed()
        {
            if (_navigationService == null || !Lifecycle.CurrentState.IsAtLeast(Lifecycle.State.Resumed)) return;
            Page currentPage = await _navigationService.FindCurrentPageAsync(true);
            if (currentPage is { BindingContext: BaseViewModel vm } &&
                Lifecycle.CurrentState.IsAtLeast(Lifecycle.State.Resumed))
            {
                //The BackCommand on the current active VM will be executed instead of the native back command.
                Rg.Plugins.Popup.Popup.SendBackPressed(() =>
                {
                    if (Lifecycle.CurrentState.IsAtLeast(Lifecycle.State.Resumed))
                    {
                        vm.BackCommand.Execute(null);
                    }
                });
            }
        }
    }
}