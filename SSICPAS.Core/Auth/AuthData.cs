using System;

namespace SSICPAS.Core.Auth
{
    public class AuthData
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? AccessTokenExpiration { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }

        public bool Validate()
        {
            bool notExpired = AccessTokenExpiration != null && AccessTokenExpiration > DateTime.UtcNow;
            return notExpired;
        }
    }
}
