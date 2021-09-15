namespace SSICPAS.Core.Logging
{
    public enum LogSeverity
    {
        INFO,
        WARNING,
        ERROR,

        SECURITY_ERROR,
        SECURITY_WARNING,

        /// <summary>
        /// Use only in non-prod environments
        /// </summary>
        FATAL
    }
}
