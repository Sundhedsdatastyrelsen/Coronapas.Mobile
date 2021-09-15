using Android.Content;
using Android.OS;
using SSICPAS.Controls;
using SSICPAS.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(FrameWithShadow), typeof(FrameShadowRenderer))]
namespace SSICPAS.Droid.CustomRenderers
{
    public class FrameShadowRenderer : Xamarin.Forms.Platform.Android.AppCompat.FrameRenderer
    {
        public FrameShadowRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null && Build.VERSION.SdkInt >= BuildVersionCodes.P)
            {
                SetOutlineAmbientShadowColor(Android.Graphics.Color.Argb(26, 26, 67, 67));
                SetOutlineSpotShadowColor(Android.Graphics.Color.Argb(26, 26, 67, 67));
                Elevation = 1.0f;
                TranslationZ = 6.0f;
            }
        }
    }
}
