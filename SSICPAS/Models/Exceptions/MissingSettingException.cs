using System;
namespace SSICPAS.Models.Exceptions
{
    public class MissingSettingException: Exception
    {
        public MissingSettingException()
        {
        }

        public MissingSettingException(string message)
            : base(message)
        {
        }

        public MissingSettingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
