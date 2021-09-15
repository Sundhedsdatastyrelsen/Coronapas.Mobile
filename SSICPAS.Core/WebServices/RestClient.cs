using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SSICPAS.Core.Data;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Auth;
using SSICPAS.Core.WebServices.Exceptions;
using Xamarin.Forms;

namespace SSICPAS.Core.WebServices
{
    public class RestClient : IRestClient
    {
        private ISecureStorageService<AuthData> _authStorage;
        private ILoggingService _loggingService;
        private ISettingsService _settingsService;
        private IConnectivityService _connectivityService;
        private IAuthenticationManager _authManager;
        private IAuthRenewalService _authRenewalService;
        private ISslCertificateService _sslCertificateService;
        private IAssemblyService _assemblyService;
        private IDateTimeService _dateTimeService;

        private SemaphoreSlim _semaphoreSlim;
        public HttpClient HttpClient { get; private set; }

        public static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };

        public RestClient(
            ISecureStorageService<AuthData> authStorage,
            ILoggingService loggingService,
            ISettingsService settingsService,
            IConnectivityService connectivityService, 
            IAuthenticationManager authManager,
            IAuthRenewalService authRenewalService,
            ISslCertificateService sslCertificateService,
            IAssemblyService assemblyService, 
            IDateTimeService dateTimeService)
        {
            _settingsService = settingsService;
            _connectivityService = connectivityService;
            _dateTimeService = dateTimeService;

            Init(authStorage, loggingService, authManager, authRenewalService, sslCertificateService, assemblyService);
        }

        // This method is called both from app start, and when logging out to reset.
        public void Init(
            ISecureStorageService<AuthData> authStorage,
            ILoggingService loggingService,
            IAuthenticationManager authManager,
            IAuthRenewalService authRenewalService,
            ISslCertificateService sslCertificateService,
            IAssemblyService assemblyService)
        {
            _authStorage = authStorage;
            _loggingService = loggingService;
            _authManager = authManager;
            _authRenewalService = authRenewalService;
            _sslCertificateService = sslCertificateService;
            _assemblyService = assemblyService;

            HttpClient = null;
            SetupTrustedCertificate();

            if (Device.RuntimePlatform == Device.Android)
            {
                HttpClient = new HttpClient(new CertificateValidationHandler(this));
            }
            else
            {
                HttpClient = new HttpClient(new NativeMessageHandler(throwOnCaptiveNetwork: false, customSSLVerification: true));

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
            }

            HttpClient.Timeout = TimeSpan.FromSeconds(_settingsService.DefaultTimeout);
            HttpClient.BaseAddress = new Uri(_settingsService.BaseUrl);
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpClient.DefaultRequestHeaders.Add("Authorization", _settingsService.AuthorizationHeader); //Only first line of defense. Expected to get leaked.

            RegisterAccessTokenHeader();
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        void SetupTrustedCertificate()
        {
            string certPrefix = _assemblyService.CertificatesFolderPath;
            string cert = _settingsService.TrustedSSLCertificateFileName;

            if (cert != null)
            {
                string certManifestResource = $"{certPrefix}.{cert}";
                try
                {
                    Stream certStream = _assemblyService.GetSharedFormsAssembly().GetManifestResourceStream(certManifestResource);
                    _sslCertificateService.SetTrustedCertificate(certStream);
                }
                catch (Exception e)
                {
                    _loggingService.LogException(LogSeverity.ERROR, e, $"{nameof(RestClient)}.{nameof(SetupTrustedCertificate)}: Failed to load trusted certificate");
                }
            }
        }

        public bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            try
            {
                X509Certificate trustedCert = _sslCertificateService.GetTrustedCertificate();
                
                if (trustedCert != null)
                {
                    // NOTE: Return true here if you want to skip SSL pinning during development
                    return string.Equals(trustedCert.GetPublicKeyString(), certificate.GetPublicKeyString());
                }
                return false;
            }
            catch (Exception e)
            {
                Debug.Print($"{nameof(RestClient)}.{nameof(ServerCertificateValidationCallback)}: Failed to compare trusted certificates");
                throw;
            }
        }

        public async Task RegisterAccessTokenHeader()
        {
            ClearAccessTokenHeader();
            
            AuthData token = await _authStorage.GetSecureStorageAsync(CoreSecureStorageKeys.AUTH_TOKEN);

            if (token?.AccessToken != null)
            {
                HttpClient.DefaultRequestHeaders.Add("AccessToken", token.AccessToken);
            }
        }

        public void ClearAccessTokenHeader()
        {
            if (HttpClient.DefaultRequestHeaders.Contains("AccessToken"))
            {
                HttpClient.DefaultRequestHeaders.Remove("AccessToken");
            }
        }
        public void ClearAdditionalHeader(string key)
        {
            if (HttpClient.DefaultRequestHeaders.Contains(key))
            {
                HttpClient.DefaultRequestHeaders.Remove(key);
            }
        }

        public void SetAdditionalHeader(string key, string value)
        {
            ClearAdditionalHeader(key);
            HttpClient.DefaultRequestHeaders.Add(key, value);
        }

        public virtual async Task<ApiResponse<T>> Get<T>(string uri, bool shouldCheckAccessToken = true)
        {
            Debug.Print($"GET {uri} [with access token check: {shouldCheckAccessToken}]");
            ApiResponse<T> result = new ApiResponse<T>(uri);

            try
            {
                if (shouldCheckAccessToken) await ValidateAuthTokensAsync();

                HttpResponseMessage response = await InternalGetAsync(uri, shouldCheckAccessToken);

                result.StatusCode = (int)response.StatusCode;

                if (!response.IsSuccessStatusCode)
                {
                    Debug.Print(response.ReasonPhrase);
                    result.ResponseText = response.ReasonPhrase;
                    HandleErrors(ref result);
                    return result;
                }

                result.ResponseText = await response.Content.ReadAsStringAsync();
                Debug.Print($"Response: {result.ResponseText}");
                result.Data = JsonConvert.DeserializeObject<T>(result.ResponseText, JsonSerializerSettings);
                PrettyPrintJsonObject("Response", result.Data);
            }
            catch (Exception e)
            {
                HandleErrors(ref result, e);
            }
            
            return result;
        }

        private void HandleErrors<T>(ref ApiResponse<T> result, Exception e)
        {
            result.Exception = e;

            if (!_connectivityService.HasInternetConnection())
            {
                result.ErrorType = ServiceErrorType.NoInternetConnection;
            }
            else if (IsBadConnectionError(e))
            {
                result.ErrorType = ServiceErrorType.BadInternetConnection;
            }
            else if (e.InnerException is AuthenticationException)
            {
                result.ErrorType = ServiceErrorType.TrustFailure;
                _loggingService.LogApiError(LogSeverity.SECURITY_WARNING, result, "Service call failed because of SSL pinning protection");
            }
            else if (e is TaskCanceledException)
            {
                //As long as we don't cancel tasks manually, then TaskCanceledException will only be thrown for timeouts
                result.ErrorType = ServiceErrorType.Timeout;
                _loggingService.LogApiError(LogSeverity.WARNING, result, "Service call timed out");
            }
            else if (e is RefreshTokenExpiredException)
            {
                result.ErrorType = ServiceErrorType.UserSessionExpired;
                _loggingService.LogApiError(LogSeverity.INFO, result, "RefreshToken Expired. The user has to login with NemID again");
            }
            else if (e is InvalidGrantAuthException)
            {
                result.ErrorType = ServiceErrorType.RefreshTokenRenewalFailed;
                _loggingService.LogApiError(LogSeverity.WARNING, result, "AccessToken/RefreshToken Renewal failed. The user will be asked to log in with NemID");
            }
            else
            {
                result.ErrorType = ServiceErrorType.InternalAppError;
                _loggingService.LogApiError(LogSeverity.ERROR, result);
            }
        }

        private void HandleErrors<T>(ref ApiResponse<T> result)
        {
            if (result.IsSuccessfull)
                return;

            if (result.StatusCode == 401)
            {
                _loggingService.LogApiError(LogSeverity.WARNING, result);
                return;
            }

            if (result.StatusCode == 410)
            {
                result.ErrorType = ServiceErrorType.Gone;
                _loggingService.LogApiError(LogSeverity.INFO, result);
                return;
            }

            result.ErrorType = ServiceErrorType.ServerError;
            _loggingService.LogApiError(LogSeverity.WARNING, result);
        }

        bool IsBadConnectionError(Exception e)
        {
            bool isIOSException = e?.InnerException is IOException
                && (e.InnerException.Message.Contains("the transport connection")
                || e.InnerException.Message.Contains("The server returned an invalid or unrecognized response"));

            //This will contain a more detailed error description in the Inner exception
            bool isWebException = e is WebException;

            return isIOSException || isWebException;
        }

        private async Task<bool> HasAccessTokenExpiredWhileTransmitting(HttpResponseMessage httpResponse)
        {
            if (httpResponse.StatusCode == HttpStatusCode.Unauthorized && await IsAccessTokenExpired())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void PrettyPrintJsonObject(string prefix, object jsonObj)
        {
            try
            {
                Debug.Print($"{prefix}: {JsonConvert.SerializeObject(jsonObj, Formatting.Indented, JsonSerializerSettings)}");
            }
            catch { }
        }

        public virtual async Task<ApiResponse<Stream>> GetFileAsStreamAsync(string url)
        {
            Debug.Print($"Downloading file: {url}");
            ApiResponse<Stream> result = new ApiResponse<Stream>(url);
            try
            {
                HttpResponseMessage response = await InternalGetAsync(url, false);
               
                result.StatusCode = (int)response.StatusCode;

                if (!response.IsSuccessStatusCode)
                {
                    result.ResponseText = response.ReasonPhrase;
                    HandleErrors(ref result);
                    return result;
                }

                Stream content = await response.Content.ReadAsStreamAsync();
                result.Headers = response.Headers;
                if (content.Length > 0)
                    result.Data = content;

                Debug.WriteLine("Page content: " + content);
            }
            catch (Exception e)
            {
                HandleErrors(ref result, e);
            }

            return result;
        }

        private async Task<HttpResponseMessage> InternalGetAsync(string url, bool shouldRenewAccessToken = true)
        {
            HttpResponseMessage response = default;
            bool tryAgainDueToBadConnection = false;
            bool tryAgainDueToAccessTokenExpired = false;
            try
            {
                response = await HttpClient.GetAsync(url);

                if (shouldRenewAccessToken && await HasAccessTokenExpiredWhileTransmitting(response))
                {
                    tryAgainDueToAccessTokenExpired = true;
                }
            }
            catch (Exception e)
            {
                if (IsBadConnectionError(e))
                {
                    tryAgainDueToBadConnection = true;
                }
                else
                {
                    throw;
                }
            }

            if (tryAgainDueToAccessTokenExpired)
            {
                Debug.Print("Failed http call due to access token expired during request. Retrying...");
                await ValidateAuthTokensAsync();
                response = await HttpClient.GetAsync(url);
            }
            else if (tryAgainDueToBadConnection)
            {
                Debug.Print("Failed http call due to bad connection. Retrying...");
                Thread.Sleep(100);
                response = await HttpClient.GetAsync(url);
            }

            if (response == default)
            {
                throw new HttpRequestException($"No response");
            }
            Debug.Print($"Statuscode: {(int)response.StatusCode} {response.StatusCode.ToString()}");

            return response;
        }

        public async Task ValidateAuthTokensAsync()
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                AuthData token = await _authStorage.GetSecureStorageAsync(CoreSecureStorageKeys.AUTH_TOKEN);

                if (token == null)
                {
                    throw new RefreshTokenExpiredException($"{nameof(RestClient)}.{nameof(RefreshAccessToken)}: RefreshToken is missing");
                }

                if (_dateTimeService.Now <= token.AccessTokenExpiration)
                {
                    return;
                }

                if (_dateTimeService.Now > token.RefreshTokenExpiration)
                {
                    throw new RefreshTokenExpiredException($"{nameof(RestClient)}.{nameof(RefreshAccessToken)}: RefreshToken is expired");
                }
                else
                {
                    await RefreshAccessToken(token.RefreshToken);
                }
            }
            catch (InvalidGrantAuthException e)
            {
                Debug.WriteLine(e);
                throw;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async Task<bool> IsAccessTokenExpired()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                AuthData token = await _authStorage.GetSecureStorageAsync(CoreSecureStorageKeys.AUTH_TOKEN);
                if (token == null) return true;

                if (_dateTimeService.Now > token.AccessTokenExpiration)
                {
                    return true;
                }
                return false;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task RefreshAccessToken(string refreshToken)
        {
            Debug.Print("Refreshing access token");

            Dictionary<string, string> queryValues = new Dictionary<string, string>
            {
                {"refresh_token", refreshToken},
                {"client_id", _settingsService.OAuthClientId},
                {"grant_type", "refresh_token"}
            };

            try
            {
                IDictionary<string, string> refreshResponse = await _authRenewalService.RenewAccessToken(queryValues);

                int.TryParse(refreshResponse["expires_in"], out int atExpiry);
                int.TryParse(refreshResponse["refresh_expires_in"], out int rtExpiry);
                
                AuthData payload = _authManager.GetPayloadValidateJWTToken(refreshResponse["access_token"]);
                payload.RefreshToken = refreshResponse["refresh_token"];
                payload.AccessTokenExpiration = _dateTimeService.Now.AddSeconds(atExpiry);
                payload.RefreshTokenExpiration = _dateTimeService.Now.AddSeconds(rtExpiry);
                
                await _authStorage.SetSecureStorageAsync(CoreSecureStorageKeys.AUTH_TOKEN, payload);
                await RegisterAccessTokenHeader();
                Debug.Print("Access token was renewed.");
            }
            catch (Xamarin.Auth.AuthException e)
            {
                if (e.Message.Contains("invalid_grant"))
                {
                    throw new InvalidGrantAuthException($"{nameof(RestClient)}.{nameof(RefreshAccessToken)}: Failed to renew access token", e);
                }
                else
                {
                    _loggingService.LogException(LogSeverity.WARNING, e, "AuthException is raised not due to invalid_grant", false);
                }
            }
            catch (Exception e)
            {
                _loggingService.LogException(LogSeverity.WARNING, e, "Communication error with Auth Server", false);
            }
        }

        public void RegisterLocalesRequestHeaders(string versionNumber)
        {
            ClearLocalesRequestHeaders();

            HttpClient.DefaultRequestHeaders.Add("CurrentVersionNo", versionNumber);
        }

        public void ClearLocalesRequestHeaders()
        {
            if (HttpClient.DefaultRequestHeaders.Contains("CurrentVersionNo"))
            {
                HttpClient.DefaultRequestHeaders.Remove("CurrentVersionNo");
            }
        }
    }
}