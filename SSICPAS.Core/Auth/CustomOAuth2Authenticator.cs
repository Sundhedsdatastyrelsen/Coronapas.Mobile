using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PCLCrypto;
using Xamarin.Auth;

namespace SSICPAS.Core.Auth
{
    public class CustomOAuth2Authenticator : OAuth2Authenticator
    {
        Uri _accessTokenUrl;
        private string _redirectUrl;
        private string _codeVerifier;

        private string _previouslySentCodeVerifier = "";

        public CustomOAuth2Authenticator(string clientId,
            string scope,
            Uri authorizeUrl,
            Uri redirectUrl,
            GetUsernameAsyncFunc getUsernameAsync = null,
            bool isUsingNativeUI = false) : base(clientId, scope, authorizeUrl, redirectUrl, getUsernameAsync, isUsingNativeUI)
        {
        }

        public CustomOAuth2Authenticator(
            string clientId,
            string clientSecret,
            string scope,
            Uri authorizeUrl,
            Uri redirectUrl,
            Uri accessTokenUrl,
            GetUsernameAsyncFunc getUsernameAsync = null,
            bool isUsingNativeUI = false) : base(clientId, clientSecret, scope, authorizeUrl, redirectUrl, accessTokenUrl, getUsernameAsync, isUsingNativeUI)
        {
            _accessTokenUrl = accessTokenUrl;
        }

        protected override void OnPageEncountered(Uri url, IDictionary<string, string> query, IDictionary<string, string> fragment)
        {
            var all = new Dictionary<string, string>(query);

            if (all.ContainsKey("error"))
            {
                foreach (KeyValuePair<string, string> item in all)
                {
                    Debug.Print($"{nameof(OnPageEncountered)}: Key: {item.Key}, Value: {item.Value}");
                }

                string description = all["error"];
                if (all.ContainsKey("error_description"))
                {
                    description = all["error_description"];
                }
#if TEST || DEVELOPMENT
                if (all.ContainsKey("state"))
                {
                    description += $" (state={all["state"]})";
                }
#endif
                OnError(string.Format("OAuth Error received from AuthServer = {0}", description));

                return;
            }
            
            base.OnPageEncountered(url, query, fragment);
        }

        protected override async void OnRedirectPageLoaded(Uri url, IDictionary<string, string> query, IDictionary<string, string> fragment)
        {
            Debug.Print($"{nameof(CustomOAuth2Authenticator)}.{nameof(OnRedirectPageLoaded)} is called");

            if (string.Equals(_codeVerifier, _previouslySentCodeVerifier))
            {
                return;
            }

            // Prepare code verifier and send them back to get access token
            query["code_verifier"] = _codeVerifier;
            query["client_id"] = ClientId;
            query["grant_type"] = "authorization_code";
            query["redirect_uri"] = _redirectUrl;

            _previouslySentCodeVerifier = _codeVerifier;

            // Get Access token before hand by our rule, then the package will not attemp to get the access token anymore later
            try
            {
                var token = await CustomRequestAccessTokenAsync(query);
                foreach (var tokenSegment in token)
                {
                    fragment.Add(tokenSegment);
                }
                base.OnRedirectPageLoaded(url, query, fragment);
            }
            catch (AuthException e)
            {
                Debug.Print(JsonConvert.SerializeObject(query));
                OnError(e);
            }
            finally
            {
                _previouslySentCodeVerifier = "";
                Debug.Print($"{nameof(CustomOAuth2Authenticator)}.{nameof(OnRedirectPageLoaded)}: Resetting _previouslySentCodeVerifier ");
            }
        }

        /// <summary>
        /// Asynchronously makes a request to the access token URL with the given parameters.
        /// </summary>
        /// <param name="queryValues">The parameters to make the request with.</param>
        /// <returns>The data provided in the response to the access token request.</returns>
        public async Task<IDictionary<string, string>> CustomRequestAccessTokenAsync(IDictionary<string, string> queryValues)
        {
            Debug.Print($"{nameof(CustomOAuth2Authenticator)}.{nameof(CustomRequestAccessTokenAsync)} is called");

            // mc++ changed protected to public for extension methods RefreshToken (Adrian Stevens) 
            var content = new FormUrlEncodedContent(queryValues);
            string text = "";

            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.PostAsync(_accessTokenUrl, content).ConfigureAwait(false);

                var fetchTask = response.Content?.ReadAsStringAsync() ?? null;
                text = fetchTask != null ? (await fetchTask.ConfigureAwait(false)) : "";

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Debug.Print($"{nameof(CustomOAuth2Authenticator)}.{nameof(CustomRequestAccessTokenAsync)}: Failed to get new access token");
                    Debug.Print($"Http Status: {response.StatusCode} - {response.ReasonPhrase}");
                    Debug.Print($"Content: {text}");
                }
            }
            catch (Exception e) {
                Debug.Print($"{nameof(CustomOAuth2Authenticator)}.{nameof(CustomRequestAccessTokenAsync)}: Failed to get new access token");
                Debug.Print(e.ToString());
                throw;
            }

            // Parse the response
            var data = text.Contains("{") ? WebEx.JsonDecode(text) : WebEx.FormDecode(text);

            if (data.ContainsKey("error"))
            {
                throw new AuthException("Error authenticating: " + data["error"]);
            }
            //---------------------------------------------------------------------------------------
            /// Pull Request - manually added/fixed
            ///		OAuth2Authenticator changes to work with joind.in OAuth #91
            ///		https://github.com/xamarin/Xamarin.Auth/pull/91
            ///		
            else if (data.ContainsKey(AccessTokenName))
            //---------------------------------------------------------------------------------------
            {
            }
            else
            {
                //---------------------------------------------------------------------------------------
                /// Pull Request - manually added/fixed
                ///		OAuth2Authenticator changes to work with joind.in OAuth #91
                ///		https://github.com/xamarin/Xamarin.Auth/pull/91
                ///		
                //throw new AuthException ("Expected access_token in access token response, but did not receive one.");
                throw new AuthException("Expected " + AccessTokenName + " in access token response, but did not receive one.");
                //---------------------------------------------------------------------------------------
            }

            return data;
        }

        protected override void OnCreatingInitialUrl(IDictionary<string, string> query)
        {
            _redirectUrl = Uri.UnescapeDataString(query["redirect_uri"]);
            _codeVerifier = CreateCodeVerifier();
            query["response_type"] = "code";
            query["nonce"] = Guid.NewGuid().ToString("N");
            query["code_challenge"] = CreateChallenge(_codeVerifier);
            query["code_challenge_method"] = "S256";

            base.OnCreatingInitialUrl(query);
        }

        private string CreateCodeVerifier()
        {
            var codeBytes = WinRTCrypto.CryptographicBuffer.GenerateRandom(64);
            return Convert.ToBase64String(codeBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        private string CreateChallenge(string code)
        {
            var codeVerifier = code;
            var sha256 = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(PCLCrypto.HashAlgorithm.Sha256);
            var challengeByteArray = sha256.HashData(WinRTCrypto.CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(codeVerifier)));
            WinRTCrypto.CryptographicBuffer.CopyToByteArray(challengeByteArray, out byte[] challengeBytes);
            return Convert.ToBase64String(challengeBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
    }
}
