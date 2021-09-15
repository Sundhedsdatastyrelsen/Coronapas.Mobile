using System;

namespace SSICPAS.Core.WebServices.Exceptions
{
    public class RefreshTokenExpiredException : Exception
    {
        public RefreshTokenExpiredException(string message) : base(message)
        {

        }
    }
}