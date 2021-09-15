namespace SSICPAS.Core.Interfaces
{
    public interface ISettingsService
    {
        bool UseMockServices { get; set; }

        string EnvironmentDescription { get; }

        string BaseUrl { get; }

        string TrustedSSLCertificateFileName { get; }

        string AuthorizationHeader { get; }

        string ApiVersion { get; }

        bool ShouldLogErrors { get; }

        int DefaultTimeout { get; }

        int TimeOutMinuteUntilReauthenticate { get; }
        
        string OAuthClientId { get; }
        
        string OAuthScopes { get; }
        
        string OAuthAuthorizeUrl { get; }
        
        string OAuthRedirectUrl { get; }

        string OAuthTokenUrl { get; }
        
        string OAuthSigningCertificate { get; }
        
        string BuildString { get; }

        string VersionString { get; }
        
        public double ScannerSuccessShownDurationMs { get; }
        public double ScannerInvalidShownDurationMs { get; }
        public double ScannerEUShownDurationMs { get; }

        int TextFileFetchIntervalInMinutes { get; }

        int RATValueSetsFilesFetchIntervalInHours { get; }

        string ForceUpdateLink { get; }

        string EmbeddedTextVersion { get; }
        string EmbeddedRATValueSetsFilesVersion { get; }

        double BusinessGroupLogThrottleFactor { get; }
        double SecurityGroupLogThrottleFactor { get; }
        double TransientGroupLogThrottleFactor { get; }
        double BlockingGroupLogThrottleFactor { get; }

        string ContinuousFetchingDelaysSeconds { get; }
        int SuspendedFetchingDelaySeconds { get; }

        int PublicKeyPeriodicFetchingIntervalInHours { get; }
    }
}