using SSICPAS.Enums;
using UIKit;

namespace SSICPAS.iOS.Services
{
    public static class SupportedOrientationToIosInterfaceOrientationMaskExtension
    {
        public static UIInterfaceOrientationMask ToIOSInterfaceOrientationMask(this SupportedOrientation orientation)
        {
            switch (orientation)
            {
                case SupportedOrientation.SensorPortrait:
                    return UIInterfaceOrientationMask.Portrait | UIInterfaceOrientationMask.PortraitUpsideDown;
                case SupportedOrientation.Portrait:
                default:
                    return UIInterfaceOrientationMask.Portrait;
            }
        }
    }
}