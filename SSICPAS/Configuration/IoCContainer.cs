using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSICPAS.Core.Auth;
using SSICPAS.Core.Data;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Services;
using SSICPAS.Core.Services.DecoderService;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.RandomService;
using SSICPAS.Core.WebServices;
using SSICPAS.Models;
using SSICPAS.Services;
using SSICPAS.Services.DataManagers;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Mocks;
using SSICPAS.Services.Navigation;
using SSICPAS.Services.Repositories;
using SSICPAS.Services.Translator;
using SSICPAS.Services.WebServices;
using SSICPAS.ViewModels.Certificates;
using TinyIoC;

namespace SSICPAS.Configuration
{
    public static class IoCContainer
    {
        private static TinyIoCContainer _container;

        private static readonly Dictionary<Type, Type> _singletons = new Dictionary<Type, Type>()
        {
            { typeof(INavigationService), typeof(NavigationService) },
            { typeof(ISessionManager), typeof(SessionManager) },
            { typeof(IGyroscopeService), typeof(GyroscopeService) },
            { typeof(IRestClient), typeof(RestClient) },
            { typeof(IOrientationService), typeof(OrientationService) },
            { typeof(IPassportDataManager), typeof(PassportDataManager) },
            { typeof(IPublicKeyService), typeof(PublicKeyDataManager) },
        };

        private static readonly Dictionary<Type, Type> _multiInstances = new Dictionary<Type, Type>()
        {
            { typeof(IAuthenticationManager), typeof(AuthenticationManager) },
            { typeof(IRestClient), typeof(RestClient) },
            { typeof(IPassportsRepository), typeof(PassportsRepository) },
            { typeof(IPublicKeyRepository), typeof(PublicKeyRepository) },
            { typeof(IPreferencesService), typeof(PreferencesService) },
            { typeof(IUserService), typeof(UserService) },
            { typeof(IDialogService), typeof(DialogService) },
            { typeof(IPassportsService), typeof(PassportsService) },
            { typeof(ISecureStorageService<UserAuthenticationToken>), typeof(SecureStorageService<UserAuthenticationToken>) },
            { typeof(ISecureStorageService<AuthData>), typeof(SecureStorageService<AuthData>) },
            { typeof(ISecureStorageService<PinCodeBiometricsModel>), typeof(SecureStorageService<PinCodeBiometricsModel>) },
            { typeof(ISecureStorageService<AdditionalDataViewModel>), typeof(SecureStorageService<AdditionalDataViewModel>) },
            { typeof(ISecureStorageService<DKPassportsViewModel>), typeof(SecureStorageService<DKPassportsViewModel>) },
            { typeof(ISecureStorageService<EUPassportsViewModel>), typeof(SecureStorageService<EUPassportsViewModel>) },
            { typeof(ISecureStorageService<FamilyPassportItemsViewModel>), typeof(SecureStorageService<FamilyPassportItemsViewModel>) },
            { typeof(ISecureStorageService<PublicKeyStorageModel>), typeof(SecureStorageService<PublicKeyStorageModel>) },
            { typeof(ISecureStorageService<UpdateNotificationModel>), typeof(SecureStorageService<UpdateNotificationModel>) },
            { typeof(ISecureStorageService<int>), typeof(SecureStorageService<int>) },
            { typeof(ISecureStorageService<string>), typeof(SecureStorageService<string>) },
            { typeof(ISecureStorageService<DateTime>), typeof(SecureStorageService<DateTime>) },
            { typeof(INavigationTaskManager), typeof(NavigationTaskManager) },
            { typeof(ILoggingService), typeof(LoggingService) },
            { typeof(IConnectivityService), typeof(ConnectivityService) },
            { typeof(IDateTimeService), typeof(DateTimeService) },
            { typeof(IFamilyPassportStorageRepository), typeof(FamilyPassportStorageRepository) },
            { typeof(IPublicKeyStorageRepository), typeof(PublicKeyStorageRepository) },
            { typeof(ITextService), typeof(TextService) },
            { typeof(ITextRepository), typeof(TextRepository) },
            { typeof(ICertificationService), typeof(CertificationService) },
            { typeof(IPopupService), typeof(PopupService) },
            { typeof(IDeviceFeedbackService), typeof(DeviceFeedbackService) },
            { typeof(IDCCValueSetTranslator), typeof(DCCValueSetTranslator) },
            { typeof(ITokenProcessorService), typeof(HcertTokenProcessorService) },
            { typeof(IAuthRenewalService), typeof(AuthRenewalService) },
            { typeof(IAssemblyService), typeof(AssemblyService) },
            { typeof(ISslCertificateService), typeof(SslCertificateService) },
            { typeof(IRandomService), typeof(RandomService) },
            { typeof(IScreenshotDetectionService), typeof(ScreenshotDetectionService) },
            { typeof(IMigrationService), typeof(MigrationService) },
            { typeof(INotificationService), typeof(NotificationService) },
            { typeof(IRatListService), typeof(RatListService) },
            { typeof(IRatListRepository), typeof(RatlistRepository) },
        };

        private static readonly Dictionary<Type, Type> _mockServices = new Dictionary<Type, Type>()
        {
            { typeof(IPassportsRepository), typeof(MockPassportsRepository) },
            { typeof(IPublicKeyRepository), typeof(MockPublicKeyRepository) },
            { typeof(ITextRepository), typeof(MockTextRepository) },
        };

        static IoCContainer()
        {
            _container = new TinyIoCContainer();
        }

        public static void Init()
        {
            _container.Register<ISettingsService, SettingsService>();
            
            RegisterMultiInstances();
            RegisterSingletons();
        }

        /// <summary>
        /// If you need a class to be a singleton, then add them to <see cref="_singletons"/>.
        /// It must implement IResetable to be able to reset data on logout.
        /// </summary>
        private static void RegisterSingletons()
        {
            foreach (KeyValuePair<Type, Type> singleton in _singletons)
            {
                _container.Register(singleton.Key, singleton.Value).AsSingleton();
            }
        }

        /// <summary>
        /// If you need a new instance to be created every time you resolve a class, then add them to <see cref="_multiInstances"/>.
        /// </summary>
        private static void RegisterMultiInstances()
        {
            foreach (KeyValuePair<Type, Type> multi in _multiInstances)
            {
                _container.Register(multi.Key, multi.Value).AsMultiInstance();
            }

            if (Resolve<SettingsService>().UseMockServices)
            {
                foreach (KeyValuePair<Type, Type> mock in _mockServices)
                {
                    _container.Register(mock.Key, mock.Value).AsMultiInstance();
                }
            }
        }

        public static void ResetSingletons()
        {
            Resolve<IPassportDataManager>().Reset();

            Resolve<IRestClient>().Init(
                    Resolve<ISecureStorageService<AuthData>>(),
                    Resolve<ILoggingService>(),
                    Resolve<IAuthenticationManager>(),
                    Resolve<IAuthRenewalService>(),
                    Resolve<ISslCertificateService>(),
                    Resolve<IAssemblyService>());
        }

        public static void RegisterSingleton<TInterface, T>() where TInterface : class where T : class, TInterface
        {
            _container.Register<TInterface, T>().AsSingleton();
        }

        public static void RegisterInterface<TInterface, T>() where TInterface : class where T : class, TInterface
        {
            _container.Register<TInterface, T>();
        }

        public static void RegisterInstance<TInterface>(TInterface instance) where TInterface : class
        {
            _container.Register<TInterface>(instance);
        }

        public static void RegisterSingleton<TInterface>(TInterface instance) where TInterface : class
        {
            _container.Register<TInterface>(instance).AsSingleton();
        }

        public static T Resolve<T>() where T : class
        {
            return _container.Resolve<T>();
        }
    }
}