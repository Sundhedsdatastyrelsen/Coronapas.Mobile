using Android.Content;
using Android.Util;
using Android.Widget;
using SSICPAS.Controls;
using SSICPAS.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LabelWithNoFontPadding), typeof(LabelWithNoFontPaddingRenderer))]
namespace SSICPAS.Droid
{
    public class LabelWithNoFontPaddingRenderer : LabelRenderer
    {
        public LabelWithNoFontPaddingRenderer(Context context) : base(context)
        {
            
        }
        
        
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (Control != null && Control is TextView textView)
            {
                textView.SetIncludeFontPadding(false);
            }
        }
    }
}