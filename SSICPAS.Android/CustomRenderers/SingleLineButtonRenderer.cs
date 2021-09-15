using Android.Content;
using Android.Util;
using Plugin.CurrentActivity;
using SSICPAS.Controls;
using SSICPAS.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SingleLineButton), typeof(SingleLineButtonRenderer))]
namespace SSICPAS.Droid
{
    public class SingleLineButtonRenderer : ButtonRenderer
    {
        public SingleLineButtonRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            
            if (Control != null)
            {
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    // SetAutoSizeTextTypeUniformWithConfiguration() is available from API level 26.
                    // https://developer.android.com/guide/topics/ui/look-and-feel/autosizing-textview
                    // On older Android devices, attempting to call this function and it throws an exception.
                    var systemFontScaleValue = Android.Content.Res.Resources.System.Configuration.FontScale;
                    
                    if (Control.Text != null)
                    {
                        //Apply effect when FontScale valeu larger than a certain amount
                        if (systemFontScaleValue > 1.2f)
                        {
                            DisplayMetrics metrics = CrossCurrentActivity.Current.Activity.ApplicationContext.Resources.DisplayMetrics;
                            int defaultSpValue = (int) (Control.TextSize / metrics.ScaledDensity);
                            Control.SetAutoSizeTextTypeUniformWithConfiguration(1, defaultSpValue, 1, (int) ComplexUnitType.Sp);
                        }
                    }
                }
            }
        }
    }
}