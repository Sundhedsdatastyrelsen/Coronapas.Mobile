using System;
namespace SSICPAS.Models.Exceptions
{
    public class TextServiceException: Exception
    {
        public TextServiceException()
        {
        }

        public TextServiceException(string message)
            : base(message)
        {
        }

        public TextServiceException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
