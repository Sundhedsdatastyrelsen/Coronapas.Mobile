using Android.Views;
using SSICPAS.Droid;
using SSICPAS.Services.Interfaces;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using SSICPAS.Services;

[assembly: Dependency(typeof(BrightnessService))]
namespace SSICPAS.Droid
{
    public class BrightnessService : BaseBrightnessService, IBrightnessService
    {
        private float? _defaultBrightness = null;

        public override void SetBrightness(float brightness)
        {
            var window = CrossCurrentActivity.Current.Activity.Window;
            var attributesWindow = new WindowManagerLayoutParams();
            attributesWindow.CopyFrom(window?.Attributes);

            if (!_defaultBrightness.HasValue)
            {
                _defaultBrightness = attributesWindow.ScreenBrightness;
            }
            attributesWindow.ScreenBrightness = brightness;

            if (window != null) window.Attributes = attributesWindow;

            if (brightness == 1)
            {
                window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
            }
            else
            {
                window.ClearFlags(WindowManagerFlags.KeepScreenOn);
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
            var window = CrossCurrentActivity.Current.Activity.Window;
            var attributesWindow = new WindowManagerLayoutParams();
            attributesWindow.CopyFrom(window?.Attributes);

            if (attributesWindow.ScreenBrightness != 1)
            {
                _defaultBrightness = attributesWindow.ScreenBrightness;
            }
        }
    }
}
