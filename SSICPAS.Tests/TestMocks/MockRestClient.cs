using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using SSICPAS.Core.Auth;
using SSICPAS.Core.Data;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.WebServices;

namespace SSICPAS.Tests.TestMocks
{
    public class MockRestClient : IRestClient
    {
        public bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public Task<ApiResponse<T>> Get<T>(string uri, bool shouldCheckAccessToken = true)
        {
            return Task.FromResult(new ApiResponse<T>((ApiResponse) null));
        }

        public Task<ApiResponse<Stream>> GetFileAsStreamAsync(string url)
        {
            return Task.FromResult(new ApiResponse<Stream>((ApiResponse) null));
        }

        public Task RegisterAccessTokenHeader()
        {
            return Task.FromResult(true);
        }

        public void ClearAccessTokenHeader()
        {
            
        }

        public Task RefreshAccessToken(string refreshToken)
        {
            return Task.FromResult(true);
        }

        public void SetAdditionalHeader(string key, string value)
        {
            
        }

        public Task RegisterForceToken()
        {
            return Task.FromResult(true);
        }

        public void RegisterLocalesRequestHeaders(string versionNumber)
        {
        }

        public void ClearLocalesRequestHeaders()
        {
        }

        public void ClearAdditionalHeader(string key)
        {
        }

        public Task ValidateAuthTokensAsync()
        {
            return Task.FromResult(true);
        }

        public void Init(ISecureStorageService<AuthData> secureStorageService, ILoggingService loggingService, IAuthenticationManager authenticationManager, IAuthRenewalService authRenewalService, ISslCertificateService sslCertificateService, IAssemblyService assemblyService)
        {
        }
    }
}