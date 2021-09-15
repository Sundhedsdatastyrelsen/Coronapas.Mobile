using System;

namespace SSICPAS.Core.CustomExceptions
{
    public class TokenDecodeException : Exception
    {
        public TokenDecodeException(string message): base(message)
        {
            
        }

        public TokenDecodeException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}