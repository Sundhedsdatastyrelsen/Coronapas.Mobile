using System;
using Foundation;
using Sentry;
using SSICPAS.Configuration;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Auth;
using SSICPAS.Data;
using SSICPAS.Enums;
using SSICPAS.iOS.Services;
using SSICPAS.Services.Interfaces;
using UIKit;
using Xamarin.Forms;

namespace SSICPAS.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private SupportedOrientation _supportedOrientation = SupportedOrientation.Portrait;

        UIVisualEffectView _blurWindow = null;

        private NSObject _screenshotObserver = null;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
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

            Rg.Plugins.Popup.Popup.Init();
            global::Xamarin.Forms.Forms.Init();
            AiForms.Dialogs.Dialogs.Init();
#if UITEST
            Xamarin.Calabash.Start();
#endif
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();

            global::Xamarin.Auth.Presenters.XamarinIOS.AuthenticationConfiguration.Init();
            RegisterIosServices();

            LoadApplication(new App());
            IoCContainer.Resolve<IOrientationService>().OnChangeSupportedOrientation += OnChangeSupportedOrientation;

            return base.FinishedLaunching(app, options);
        }

        private void OnChangeSupportedOrientation(SupportedOrientation obj)
        {
            _supportedOrientation = obj;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, UIWindow forWindow)
        {
            return _supportedOrientation.ToIOSInterfaceOrientationMask();
        }

        private void RegisterIosServices()
        {
            IoCContainer.RegisterSingleton<IConfigurationProvider, ConfigurationProvider>();
            IoCContainer.RegisterSingleton<IStatusBarService, StatusBarService>();
            IoCContainer.RegisterSingleton<IBrightnessService, BrightnessService>();
            IoCContainer.RegisterSingleton<IPlatformBrowserService, PlatformBrowserService>(); 
            IoCContainer.RegisterInterface<IScannerFactory, IosScannerFactory>(); 
            IoCContainer.RegisterInterface<IDeeplinkingService, IosDeeplinkingService>();
            IoCContainer.RegisterInterface<IPlatformSettingsService, IosSettingsService>();
            IoCContainer.RegisterInterface<ICustomOAuthNativeUIFlagResolver, IosCustomOAuthNativeUIFlagResolver>();
            IoCContainer.RegisterInterface<IVoiceOverManager, IosVoiceOverManager>();
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            return OpenUrl(url);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            return OpenUrl(url);
        }

        bool OpenUrl(NSUrl url)
        {
            try
            {
                // Convert iOS NSUrl to C#/netxf/BCL System.Uri - common API
                Uri uri_netfx = new Uri(url.AbsoluteString);

                // load redirect_url Page for parsing
                AuthenticationState.Authenticator.OnPageLoading(uri_netfx);
            }
            catch (Exception e)
            {
                IoCContainer.Resolve<ILoggingService>().LogException(LogSeverity.SECURITY_WARNING, e,
                    $"{nameof(AppDelegate)}.{nameof(OpenUrl)}: Failed to redirect the user to the app after browser flow");
            }

            return true;
        }

        public override void OnActivated(UIApplication uiApplication)
        {
             base.OnActivated(uiApplication);
            
            _blurWindow?.RemoveFromSuperview();
            _blurWindow?.Dispose();
            _blurWindow = null;

#if !DEBUG && !TEST
            if(_screenshotObserver == null)
            {
                _screenshotObserver = NSNotificationCenter.DefaultCenter.AddObserver(new NSString("UIApplicationUserDidTakeScreenshotNotification"),
                    _ => { MessagingCenter.Send<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN); });
            }
#endif
            MessagingCenter.Send<object>(this, MessagingCenterKeys.BACK_FROM_BACKGROUND);
        }

        public override void OnResignActivation(UIApplication uiApplication)
        {
            base.OnResignActivation(uiApplication);

            using (var blurEffect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Prominent))
            {
                _blurWindow = new UIVisualEffectView(blurEffect)
                {
                    Frame = UIApplication.SharedApplication.KeyWindow.RootViewController.View.Bounds
                };
            UIApplication.SharedApplication.KeyWindow.RootViewController.View.AddSubview(_blurWindow);
            }

#if !DEBUG && !TEST
            if(_screenshotObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_screenshotObserver);
                _screenshotObserver = null;
            }
#endif
        }
    }
}
