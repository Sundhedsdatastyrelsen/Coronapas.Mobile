using System;
using SSICPAS.Core.Auth;
using Xamarin.Auth;

namespace SSICPAS.Core.Interfaces
{
    public interface IAuthenticationManager
    {
        void Setup(EventHandler<AuthenticatorCompletedEventArgs> completedHandler, EventHandler<AuthenticatorErrorEventArgs> errorHandler);
        void Cleanup();
        AuthData GetPayloadValidateJWTToken(string accessToken);
    }
}