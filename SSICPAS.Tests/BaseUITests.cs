using NUnit.Framework;
using SSICPAS.Configuration;
using SSICPAS.Core.Auth;
using SSICPAS.Core.Data;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.WebServices;
using SSICPAS.Models;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.Tests.TestMocks;
using Xamarin.Forms;
using Xamarin.Forms.Mocks;

namespace SSICPAS.Tests
{
    public class BaseUITests
    {
        public virtual string RuntimeDevice { get; set; } = Device.iOS;

        [SetUp]
        public void Init()
        {
            //See navigation mocks details on https://github.com/jonathanpeppers/Xamarin.Forms.Mocks
            MockForms.Init(RuntimeDevice);

            IoCContainer.RegisterInterface<IConfigurationProvider, MockConfigurationProvider>();
            IoCContainer.RegisterInterface<ISettingsService, MockSettingsService>();
            IoCContainer.RegisterInterface<IStatusBarService, MockStatusBarService>();
            IoCContainer.RegisterInterface<IPreferencesService, MockPreferencesService>();
            IoCContainer.RegisterInterface<IRestClient, MockRestClient>();
            IoCContainer.RegisterInterface<IBrightnessService, MockBrightnessService>();
            IoCContainer.RegisterInterface<IDialogService, MockDialogService>();
            IoCContainer.RegisterInterface<IScannerFactory, MockScannerFactory>();
            IoCContainer.RegisterInterface<ITextService, MockTextService>();
            IoCContainer.RegisterInterface<IRandomService, MockRandomService>();
            IoCContainer.RegisterInterface<ISecureStorageService<PinCodeBiometricsModel>, SecureStorageService<PinCodeBiometricsModel>>();
            IoCContainer.RegisterInterface<ILoggingService, LoggingService>();
            IoCContainer.RegisterInterface<IDateTimeService, MockDateTimeService>();
            IoCContainer.RegisterInterface<ISessionManager, SessionManager>();
            IoCContainer.RegisterInterface<IPublicKeyService, MockPublicKeyDataManager>();
            IoCContainer.RegisterInterface<INavigationService, NavigationService>();
            IoCContainer.RegisterInterface<IOrientationService, MockOrientationService>();
            IoCContainer.RegisterInterface<IPassportDataManager, MockPassportDataManager>();
            IoCContainer.RegisterInterface<ISecureStorageService<AuthData>, MockSecureStorageService<AuthData>>();
            IoCContainer.RegisterInterface<IAuthenticationManager, MockAuthenticationManager>();
            IoCContainer.RegisterInterface<IAuthRenewalService, MockAuthRenewalService>();
            IoCContainer.RegisterInterface<ISslCertificateService, MockSslCertificateService>();
            IoCContainer.RegisterInterface<IAssemblyService, MockAssemblyService>();
            IoCContainer.RegisterInterface<IMigrationService, MockMigrationService>();
            IoCContainer.RegisterInterface<ISecureStorageService<UpdateNotificationModel>, SecureStorageService<UpdateNotificationModel>>();
            IoCContainer.RegisterInterface<IRatListService, MockRatListService>();         
            RegisterUserService();

            Application.Current = new App(mock: true);

            AfterInit();
        }

        /// <summary>
        /// Extend this to run code after the app is initialized before each test
        /// </summary>
        public virtual void AfterInit()
        {
        }

        public virtual void RegisterUserService()
        {
            IoCContainer.RegisterInterface<IUserService, UserService>();
        }
    }
}
