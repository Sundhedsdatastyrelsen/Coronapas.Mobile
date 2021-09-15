using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Logging;
using Xamarin.Auth;

namespace SSICPAS.Core.Auth
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly ISettingsService _settingsService;
        private readonly ILoggingService _loggingService;
        private readonly ICustomOAuthNativeUIFlagResolver _customOAuthNativeUIFlagResolver;
        
        public EventHandler<AuthenticatorCompletedEventArgs> _completedHandler;
        public EventHandler<AuthenticatorErrorEventArgs> _errorHandler;
        public static JsonSerializer JsonSerializer = new JsonSerializer();

        public AuthenticationManager(
            ISettingsService settingsService,
            ILoggingService loggingService,
            ICustomOAuthNativeUIFlagResolver customOAuthNativeUIFlagResolver)
        {
            _settingsService = settingsService;
            _loggingService = loggingService;
            _customOAuthNativeUIFlagResolver = customOAuthNativeUIFlagResolver;
        }

        /*
            Client ID – this identifies the client that is making the request, and can be retrieved from the project in the Google API Console.
            Client Secret – this should be null or string.Empty.
            Scope – this identifies the API access being requested by the application, and the value informs the consent screen that is shown to the user. For more information about scopes, see Authorizing API request on Google's website.
            Authorize URL – this identifies the URL where the authorization code will be obtained from.
            Redirect URL – this identifies the URL where the response will be sent. The value of this parameter must match one of the values that appears in the Credentials tab for the project in the Google Developers Console.
            AccessToken Url – this identifies the URL used to request access tokens after an authorization code is obtained.
            GetUserNameAsync Func – an optional Func that will be used to asynchronously retrieve the username of the account after it's been successfully authenticated.
            Use Native UI – a boolean value indicating whether to use the device's web browser to perform the authentication request.
        */
        
        public void Setup(
            EventHandler<AuthenticatorCompletedEventArgs> completedHandler,
            EventHandler<AuthenticatorErrorEventArgs> errorHandler)
        {
            Cleanup();
            AuthenticationState.ErrorLogged = false;
            AuthenticationState.LockObject = new object();

            AuthenticationState.Authenticator = new CustomOAuth2Authenticator(
                _settingsService.OAuthClientId,
                null,
                _settingsService.OAuthScopes,
                new Uri(_settingsService.OAuthAuthorizeUrl),
                new Uri(_settingsService.OAuthRedirectUrl),
                new Uri(_settingsService.OAuthTokenUrl),
                null,
                isUsingNativeUI: _customOAuthNativeUIFlagResolver.ShouldEnableNativeUI()
                )
            {
                ClearCookiesBeforeLogin = true, 
                ShowErrors = false, 
                AllowCancel = true
            };

            Debug.Print($"Instantiating CustomOAuth2Authenticator with NativeUI: {_customOAuthNativeUIFlagResolver.ShouldEnableNativeUI()}");

            _completedHandler = completedHandler;
            AuthenticationState.Authenticator.Completed += _completedHandler;

            _errorHandler = errorHandler;
            AuthenticationState.Authenticator.Error += _errorHandler;
        }

        public void Cleanup()
        {
            if (AuthenticationState.Authenticator != null)
            {
                AuthenticationState.Authenticator.Completed -= _completedHandler;
                AuthenticationState.Authenticator.Error -= _errorHandler;
            }
            AuthenticationState.Authenticator = null;
        }

        public AuthData GetPayloadValidateJWTToken(string accessToken)
        {
            try
            {
                byte[] publicKey = Convert.FromBase64String(_settingsService.OAuthSigningCertificate);

                var cert = new X509Certificate2(publicKey);
                var alg = new RS256Algorithm(cert);
                
                string jsonPayload = JwtBuilder.Create()
                    .WithSerializer(new JsonNetSerializer(new JsonSerializer { DateTimeZoneHandling = DateTimeZoneHandling.Utc }))
                    .WithAlgorithm(alg)
                    .MustVerifySignature()
                    .Decode(accessToken);

                Debug.Print(jsonPayload);

                JObject obj = JObject.Parse(jsonPayload);

                AuthData dataModel = new AuthData();
                if (obj != null)
                {
                    dataModel = obj.ToObject<AuthData>(JsonSerializer);
                }

                dataModel.AccessToken = accessToken;

                return dataModel;

            }
            catch (Exception e)
            {
                _loggingService.LogException(LogSeverity.SECURITY_WARNING, e, $"{nameof(AuthenticationManager)}.{nameof(GetPayloadValidateJWTToken)} failed.");
                return null;
            }
        }
    }
}
