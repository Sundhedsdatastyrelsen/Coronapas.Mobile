using SSICPAS.Core.Interfaces;

namespace SSICPAS.Tests.TestMocks
{
    public class MockSettingsService : ISettingsService
    {
        public MockSettingsService()
        {
        }

        public bool UseMockServices { get; set; } = true;

        public string EnvironmentString = "api-unittest";
        public string EnvironmentDescription => EnvironmentString;
        public string BaseUrl => "Https://baseurl.dk";
        public string AddJwtToGoogleUrl => "";

        public string TrustedSSLCertificateFileName => "local.crt";

        public string AuthorizationHeader => "Test-Subscription-Value";

        public bool LogErrors = false;
        public bool ShouldLogErrors => LogErrors;
        public string AndroidAppCenterKey => "";
        public string iOSAppCenterKey => "";

        public int DefaultTimeout => 15;

        public int PressedToReleasedCloseInterval => 100;

        public bool IsScrenShotAllowed => false;

        public int JwkValidForHours => 1;

        public int TimeOutMinuteUntilReauthenticate => 30;
        public string OAuthClientId => "";
        public string OAuthScopes => "";
        public string OAuthAuthorizeUrl => "";
        public string OAuthRedirectUrl => "";
        public string OAuthTokenUrl => "";
        public string OAuthSigningCertificate => "";
        public double ScannerEUShownDurationMs => 120000;
        public int TextFileFetchIntervalInMinutes => 24;
        public string ForceUpdateLink => "https://play.google.com/store";

        public string ApiV = "v1";
        public string ApiVersion => ApiV;

        public string Build = "14";
        public string BuildString => Build;

        public string Version = "1.0.0";
        public string VersionString => Version;
        public double ScannerSuccessShownDurationMs => 30000;
        public double ScannerInvalidShownDurationMs => 60000;

        public string EmbeddedTextVersion => "1.5";

        public double BusinessGroupLogThrottleFactor => 0.1;
        public double SecurityGroupLogThrottleFactor => 1;
        public double TransientGroupLogThrottleFactor => 0.05;
        public double BlockingGroupLogThrottleFactor => 1;

        public int PublicKeyPeriodicFetchingIntervalInHours => 24;

        public string ContinuousFetchingDelaysSeconds => "10, 15, 20";
        public int SuspendedFetchingDelaySeconds => 300;

        public int RATValueSetsFilesFetchIntervalInHours => 24;
        public string EmbeddedRATValueSetsFilesVersion => "1.00";
    }
}
