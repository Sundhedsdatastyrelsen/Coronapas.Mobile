using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Data;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.RootCheck;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SSICPAS
{
    public partial class App : Application
    {
        private IMigrationService _migrationService;
        private ISessionManager _sessionManager;
        private IBrightnessService _brightnessService;
        private IUserService _userService;
        private IDialogService _dialogService;
        private IPreferencesService _preferencesService;
        private ITextService _textService;
        private IRatListService _ratListService;
        private IPublicKeyService _publicKeyDataManager;

        public App()
        {
            Debug.Print("Initializing app.");
            InitializeComponent();
            IoCContainer.Init();
            Init();
        }

        public App(bool mock)
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            _migrationService = IoCContainer.Resolve<IMigrationService>();
            _userService = IoCContainer.Resolve<IUserService>();
            _sessionManager = IoCContainer.Resolve<ISessionManager>();
            _brightnessService = IoCContainer.Resolve<IBrightnessService>();
            _dialogService = IoCContainer.Resolve<IDialogService>();
            _preferencesService = IoCContainer.Resolve<IPreferencesService>();
            _textService = IoCContainer.Resolve<ITextService>();
            _ratListService = IoCContainer.Resolve<IRatListService>();
            _publicKeyDataManager = IoCContainer.Resolve<IPublicKeyService>();
            MainPage = new NavigationPage(new SplashPage());
            RunMigration();
            ConfigureApp();
        }

        public async void PerformRootCheck()
        {
            var consent = _preferencesService.GetUserPreferenceAsBoolean(PreferencesKeys.ROOTCHECK_CONSENT);
            if (consent)
            {
                // If the user user already have given their consent, there is no need to perform root checking.
                Debug.Print("user has previously given consent using this app on a possible rooted device.");
                return;
            }

            var isDeviceRooted = RootCheck.Current.IsDeviceRooted();
            Debug.Print("is device rooted: {0}", isDeviceRooted);

            if (!isDeviceRooted)
            {
                return;
            }

            IoCContainer.Resolve<ILoggingService>().LogMessage(LogSeverity.INFO, $"{nameof(App)}.{nameof(PerformRootCheck)}: Detected rooted device");

            var title = "ROOTCHECK_TITLE".Translate();
            var description = "ROOTCHECK_DESCRIPTION".Translate();
            var accept = "ROOTCHECK_INFO_BUTTON".Translate();
            var cancel = "ROOTCHECK_PROCEED_BUTTON".Translate();
            var showInfoAboutRootedDevice = await _dialogService.ShowAlertAsync(title, description, false, true, StackOrientation.Horizontal, accept, cancel);

            if (showInfoAboutRootedDevice)
            {
                // open browser with help
                var url = "ROOTCHECK_HELP_URL".Translate();
                await Launcher.OpenAsync(url);
            }
            else
            {
                // User has given their consent that they are aware the app is running in an unsafe environment.
                _preferencesService.SetUserPreference(PreferencesKeys.ROOTCHECK_CONSENT, true);
            }
        }

        protected override async void OnStart()
        {
            await _textService.LoadSavedLocales();
            await _ratListService.LoadSavedFiles();
            await _userService?.CleanupAfterFreshInstallAsync();
            await _publicKeyDataManager.FetchPublicKeyFromBackend();
            await _userService?.InitializeAsync();
            base.OnStart();
            PerformRootCheck();
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            _sessionManager?.StartTrackSessionAsync();
            _brightnessService?.ResetBrightness();
            MessagingCenter.Send<object>(this, MessagingCenterKeys.GOING_TO_BACKGROUND);
        }

        protected override void OnResume()
        {
            base.OnResume();
            _sessionManager?.EndTrackSession();
            _brightnessService?.SetDefaultBrightness();
            MessagingCenter.Send<object>(this, MessagingCenterKeys.BACK_FROM_BACKGROUND);
            PerformRootCheck();
        }

        private void ConfigureApp()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
        }

        private void RunMigration()
        {
            _migrationService.Migrate();
        }
    }
}
