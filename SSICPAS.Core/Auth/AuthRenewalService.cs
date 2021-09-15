using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSICPAS.Core.Interfaces;

namespace SSICPAS.Core.Auth
{
    public class AuthRenewalService: IAuthRenewalService
    {
        private ISettingsService _settingsService;

        public AuthRenewalService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<IDictionary<string, string>> RenewAccessToken(Dictionary<string, string> queryValues)
        {
            bool renewalAuthenticatorUsed = false;

            if (AuthenticationState.Authenticator == null)
            {
                renewalAuthenticatorUsed = true;

                AuthenticationState.Authenticator = new CustomOAuth2Authenticator(
                _settingsService.OAuthClientId,
                null,
                _settingsService.OAuthScopes,
                new Uri(_settingsService.OAuthAuthorizeUrl),
                new Uri(_settingsService.OAuthRedirectUrl),
                new Uri(_settingsService.OAuthTokenUrl));
            }

            IDictionary<string, string> refreshResponse = await AuthenticationState.Authenticator.CustomRequestAccessTokenAsync(queryValues);

            if (renewalAuthenticatorUsed)
            {
                AuthenticationState.Authenticator = null;
            }

            return refreshResponse;
        }
    }
}
