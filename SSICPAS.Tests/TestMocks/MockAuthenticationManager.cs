using System;
using SSICPAS.Core.Auth;
using SSICPAS.Core.Interfaces;
using Xamarin.Auth;

namespace SSICPAS.Tests.TestMocks
{
    public class MockAuthenticationManager : IAuthenticationManager
    {
        public MockAuthenticationManager()
        {
        }

        public void Cleanup()
        {
        }

        public AuthData GetPayloadValidateJWTToken(string accessToken)
        {
            return new AuthData();
        }

        public void Setup(EventHandler<AuthenticatorCompletedEventArgs> completedHandler, EventHandler<AuthenticatorErrorEventArgs> errorHandler)
        {
        }
    }
}
