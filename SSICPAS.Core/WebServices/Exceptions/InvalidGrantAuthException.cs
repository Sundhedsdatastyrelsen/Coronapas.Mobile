using System;

namespace SSICPAS.Core.WebServices.Exceptions
{
    public class InvalidGrantAuthException : Exception
    {
        public InvalidGrantAuthException(string message, Exception innerE) : base(message, innerE)
        {

        }
    }
}