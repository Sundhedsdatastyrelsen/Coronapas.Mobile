using System;
namespace SSICPAS.Core.CustomExceptions
{
    public class FailedOperationSecureStorageException : Exception
    {
        public FailedOperationSecureStorageException()
        {
        }

        public FailedOperationSecureStorageException(string message)
            : base(message)
        {
        }

        public FailedOperationSecureStorageException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
