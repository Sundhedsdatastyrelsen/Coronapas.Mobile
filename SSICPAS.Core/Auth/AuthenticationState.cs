namespace SSICPAS.Core.Auth
{
    //Only use an Authenticator once.
    //This way you can use ErrorLogged to make sure that only one error gets logged per login attempt.
    //Call Authenticator.Cleanup and Setup before making another login attempt.
    public static class AuthenticationState
    {
        public static CustomOAuth2Authenticator Authenticator;
        public static bool ErrorLogged;
        public static object LockObject;
    }
}