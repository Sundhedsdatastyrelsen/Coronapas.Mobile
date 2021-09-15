using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using SSICPAS.Core.Auth;
using SSICPAS.Core.Data;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Services.Interface;

namespace SSICPAS.Core.WebServices
{
    public interface IRestClient
    {
        void Init(
            ISecureStorageService<AuthData> secureStorageService,
            ILoggingService loggingService,
            IAuthenticationManager authenticationManager,
            IAuthRenewalService authRenewalService,
            ISslCertificateService sslCertificateService,
            IAssemblyService assemblyService);

        bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors);
        Task<ApiResponse<T>> Get<T>(string uri, bool shouldCheckAccessToken = true);
        Task<ApiResponse<Stream>> GetFileAsStreamAsync(string url);
        Task RegisterAccessTokenHeader();
        Task ValidateAuthTokensAsync();
        void ClearAccessTokenHeader();
        void RegisterLocalesRequestHeaders(string versionNumber);
        void ClearLocalesRequestHeaders();
        Task RefreshAccessToken(string refreshToken);
        void SetAdditionalHeader(string key, string value);
        void ClearAdditionalHeader(string key);

    }
}