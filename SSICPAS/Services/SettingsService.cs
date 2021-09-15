using System.IO;
using Newtonsoft.Json.Linq;
using SSICPAS.Configuration;
using SSICPAS.Core.Interfaces;
using SSICPAS.Models.Exceptions;
using Xamarin.Essentials;
using Xamarin.Forms;

#nullable enable

namespace SSICPAS.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly JObject _settings;

        public SettingsService()
        {
            UseMockServices = true;
        }

        public SettingsService(IConfigurationProvider configurationProvider)
        {
            using var reader = new StreamReader(configurationProvider.GetConfiguration());
            var json = reader.ReadToEnd();
            _settings = JObject.Parse(json);
            UseMockServices = configurationProvider.GetEnvironment() == "local";
        }

        public bool UseMockServices { get; set; }

        public string EnvironmentDescription => GetSetting<string>(nameof(EnvironmentDescription));

        public string BaseUrl => GetSetting<string>(nameof(BaseUrl));

        public string TrustedSSLCertificateFileName => GetSetting<string>(nameof(TrustedSSLCertificateFileName));

        public string AuthorizationHeader => GetSetting<string>(nameof(AuthorizationHeader));

        public string ApiVersion => GetSetting<string>(nameof(ApiVersion));

        public bool ShouldLogErrors => GetSetting<bool>(nameof(ShouldLogErrors));

        public int DefaultTimeout => GetSetting<int>(nameof(DefaultTimeout));

        public string OAuthClientId => GetSetting<string>(nameof(OAuthClientId));

        public string OAuthScopes => GetSetting<string>(nameof(OAuthScopes));

        public string OAuthAuthorizeUrl => GetSetting<string>(nameof(OAuthAuthorizeUrl));

        public string OAuthRedirectUrl => GetSetting<string>(nameof(OAuthRedirectUrl));

        public string OAuthTokenUrl => GetSetting<string>(nameof(OAuthTokenUrl));

        public string OAuthSigningCertificate => GetSetting<string>(nameof(OAuthSigningCertificate));

        public int TimeOutMinuteUntilReauthenticate => GetSetting<int>(nameof(TimeOutMinuteUntilReauthenticate));

        public string BuildString => AppInfo.BuildString;

        public string VersionString => AppInfo.VersionString;

        public double ScannerSuccessShownDurationMs => GetSetting<double>(nameof(ScannerSuccessShownDurationMs));
        
        public double ScannerInvalidShownDurationMs => GetSetting<double>(nameof(ScannerInvalidShownDurationMs));
       
        public double ScannerEUShownDurationMs => GetSetting<double>(nameof(ScannerEUShownDurationMs));

        public int TextFileFetchIntervalInMinutes => GetSetting<int>(nameof(TextFileFetchIntervalInMinutes));

        public int RATValueSetsFilesFetchIntervalInHours => GetSetting<int>(nameof(RATValueSetsFilesFetchIntervalInHours));

        public string EmbeddedTextVersion => GetSetting<string>(nameof(EmbeddedTextVersion));

        public string EmbeddedRATValueSetsFilesVersion => GetSetting<string>(nameof(EmbeddedRATValueSetsFilesVersion));

        public int PublicKeyPeriodicFetchingIntervalInHours => GetSetting<int>(nameof(PublicKeyPeriodicFetchingIntervalInHours));

        public double BusinessGroupLogThrottleFactor =>
            GetSettingFromTextFile<double>(nameof(BusinessGroupLogThrottleFactor)) ?? DefaultTextFileSettings.BUSINESS_GROUP_LOG_THROTTLE_FACTOR;

        public double SecurityGroupLogThrottleFactor =>
            GetSettingFromTextFile<double>(nameof(SecurityGroupLogThrottleFactor)) ?? DefaultTextFileSettings.SECURITY_GROUP_LOG_THROTTLE_FACTOR;

        public double TransientGroupLogThrottleFactor =>
            GetSettingFromTextFile<double>(nameof(TransientGroupLogThrottleFactor)) ?? DefaultTextFileSettings.TRANSIENT_GROUP_LOG_THROTTLE_FACTOR;

        public double BlockingGroupLogThrottleFactor =>
            GetSettingFromTextFile<double>(nameof(BlockingGroupLogThrottleFactor)) ?? DefaultTextFileSettings.BLOCKING_GROUP_LOG_THROTTLE_FACTOR;

        public string ContinuousFetchingDelaysSeconds =>
            GetClassSettingFromTextFile<string>(nameof(ContinuousFetchingDelaysSeconds)) ?? DefaultTextFileSettings.CONTINUOUS_FETCHING_DELAYS_SECONDS;

        public int SuspendedFetchingDelaySeconds =>
            GetSettingFromTextFile<int>(nameof(SuspendedFetchingDelaySeconds)) ?? DefaultTextFileSettings.SUSPENDED_FETCHING_DELAY_SECONDS;

        public string ForceUpdateLink {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                    case Device.macOS:
                        return GetSetting<string>("AppStoreLink");
                    case Device.Android:
                    default:
                        return GetSetting<string>("GooglePlayLink");
                }
            }
        }

       

        private T GetSetting<T>(string key)
        {
            var value = _settings.SelectToken(key);

            if (value == null)
            {
                throw new MissingSettingException($"Key '{key}' does not exist in current settings file.");
            }

            return value.Value<T>();
        }

        /// <summary>
        /// Generic method to retrieve the primitive type setting for a given key from the text file that is loaded
        /// on each app launch. These settings are possible to adjust live for all the users, compared
        /// to the one from appsettings.json files that ship together with the app bundle. Since locale file
        /// might not load or the property might be missing in it, this method operates on nullable. The caller
        /// of this method should be provided a default for this type of nullable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        private T? GetSettingFromTextFile<T>(string key) where T: struct
        {
            var value = LocaleService.Current.GetValueForKey<T>(key);

            // We want to know if something is wrong with text file settings in non-prod environments,
            // so we crash the app. In production envs - we fallback to default values in DefaultTextFileSettings
#if !(APPSTORE || APPSTOREBETA)
            if (value == null)
            {
                throw new MissingSettingException($"Key '{key}' does not exist in current text file.");
            }
#endif
            return value;
        }

        /// <summary>
        /// Generic method to retrieve the reference type setting for a given key from the text file that is loaded
        /// on each app launch. These settings are possible to adjust live for all the users, compared
        /// to the one from appsettings.json files that ship together with the app bundle. Since locale file
        /// might not load or the property might be missing in it, this method operates on nullable. The caller
        /// of this method should be provided a default for this type of nullable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        private T? GetClassSettingFromTextFile<T>(string key) where T : class
        {
            var value = LocaleService.Current.GetClassValueForKey<T>(key);

            // We want to know if something is wrong with text file settings in non-prod environments,
            // so we crash the app. In production envs - we fallback to default values in DefaultTextFileSettings
#if !(APPSTORE || APPSTOREBETA)
            if (value == null)
            {
                throw new MissingSettingException($"Key '{key}' does not exist in current text file.");
            }
#endif
            return value;
        }
    }
}