using Android.Util;
using Android.Widget;
using Plugin.CurrentActivity;
using SSICPAS.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("SSICPAS")]
[assembly: ExportEffect(typeof(SSICPAS.Droid.FontSizeLabelEffect), nameof(FontSizeLabelEffect))]
namespace SSICPAS.Droid
{
    public class FontSizeLabelEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            
            var min = (int) FontSizeLabelEffectParams.GetMinFontSize(this.Element);
            var max = (int) FontSizeLabelEffectParams.GetMaxFontSize(this.Element);

            var metrics = CrossCurrentActivity.Current.Activity.ApplicationContext.Resources.DisplayMetrics;

            if (max <= min)
                return;

            if (this.Control is TextView textView)
            {
                int currentFontSize = PxToSp(textView.TextSize, metrics);

                if (currentFontSize > max)
                {
                    textView.SetTextSize(ComplexUnitType.Sp, max);
                }
                else if (currentFontSize < min)
                {
                    textView.SetTextSize(ComplexUnitType.Sp, min);
                }

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    // SetAutoSizeTextTypeUniformWithConfiguration() is available from API level 26.
                    // https://developer.android.com/guide/topics/ui/look-and-feel/autosizing-textview
                    // On older Android devices, attempting to call this function and it throws an exception.

                    var systemFontScaleValue = Android.Content.Res.Resources.System.Configuration.FontScale;
                    
                    //The sp value on Android is already a scalable value
                    var defaultSpValue = PxToSp(textView.TextSize, metrics);

                    if (systemFontScaleValue > 1.45f && defaultSpValue > 5)
                    {
                        //Apply effect when FontScale valeu larger than a certain amount
                        textView.SetAutoSizeTextTypeUniformWithConfiguration(5, defaultSpValue, 1,
                            (int) ComplexUnitType.Sp);
                    }
                }
            }
            else if (this.Control is Android.Widget.Button button)
            {
                int currentFontSize = PxToSp(button.TextSize, metrics);

                if(currentFontSize > max)
                {
                    button.SetTextSize(ComplexUnitType.Sp, max);
                }
                else if (currentFontSize < min)
                {
                    button.SetTextSize(ComplexUnitType.Sp, min);
                }

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                   var systemFontScaleValue = Android.Content.Res.Resources.System.Configuration.FontScale;

                    //The sp value on Android is already a scalable value
                    var defaultSpValue = PxToSp(button.TextSize, metrics);

                    if (systemFontScaleValue > 1.45f)
                    {
                        //Apply effect when FontScale valeu larger than a certain amount
                        button.SetAutoSizeTextTypeUniformWithConfiguration(5, defaultSpValue, 1,
                            (int)ComplexUnitType.Sp);
                    }
                }
            }
        }

        protected override void OnDetached()
        {
        }

        public int PxToSp(float px, DisplayMetrics metrics)
        {
            return (int) (px / metrics.ScaledDensity);
        }
    }
}