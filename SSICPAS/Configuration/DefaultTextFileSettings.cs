namespace SSICPAS.Configuration
{
    /// <summary>
    /// This class should contain all the defaults for dynamic settings that are
    /// obtained in a runtime from the fetched text files.
    /// </summary>
    public static class DefaultTextFileSettings
    {
        // Log throttling
        public static readonly double BUSINESS_GROUP_LOG_THROTTLE_FACTOR = 1;
        public static readonly double SECURITY_GROUP_LOG_THROTTLE_FACTOR = 1;
        public static readonly double TRANSIENT_GROUP_LOG_THROTTLE_FACTOR = 1;
        public static readonly double BLOCKING_GROUP_LOG_THROTTLE_FACTOR = 1;

        // Passport fetching intervals
        public static readonly string CONTINUOUS_FETCHING_DELAYS_SECONDS = "10, 15, 20";
        public static readonly int SUSPENDED_FETCHING_DELAY_SECONDS = 300;
    }
}
