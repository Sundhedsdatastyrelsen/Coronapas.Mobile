using SSICPAS.iOS;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof (BrightnessService))]
namespace SSICPAS.iOS
{
    public class BrightnessService : BaseBrightnessService, IBrightnessService
    {
        private float? _defaultBrightness = null;

        public BrightnessService()
        {
        }

        public override void SetBrightness(float brightness)
        {
            if (!_defaultBrightness.HasValue)
            {
                _defaultBrightness = (float)UIScreen.MainScreen.Brightness;
            }
            UIScreen.MainScreen.Brightness = brightness;

            if (brightness == 1)
            {
                UIApplication.SharedApplication.IdleTimerDisabled = true;
            }
            else
            {
                UIApplication.SharedApplication.IdleTimerDisabled = false;
            }
        }

        public override void ResetBrightness()
        {
            if (_defaultBrightness.HasValue)
            {
                SetBrightness(_defaultBrightness.Value);
            }
        }

        public override void SetDefaultBrightness()
        {
            if ((float)UIScreen.MainScreen.Brightness != 1)
            {
                _defaultBrightness = (float)UIScreen.MainScreen.Brightness;
            }
        }
    }
}
