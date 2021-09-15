using Android.Content.PM;
using SSICPAS.Enums;

namespace SSICPAS.Droid.Services
{
    public static class SupportedOrientationToAndroidOrientationExtension 
    {
        public static ScreenOrientation ToAndroidScreenOrientation(this SupportedOrientation orientation)
        {
            switch (orientation)
            {
                case SupportedOrientation.SensorPortrait:
                    return ScreenOrientation.SensorPortrait;
                case SupportedOrientation.Portrait:
                default:
                    return ScreenOrientation.Portrait;
            }
        }
    }
}