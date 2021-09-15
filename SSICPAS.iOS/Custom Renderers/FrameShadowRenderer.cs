using CoreGraphics;
using SSICPAS.Controls;
using SSICPAS.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FrameWithShadow), typeof(FrameShadowRenderer))]
namespace SSICPAS.iOS.CustomRenderers
{
    public class FrameShadowRenderer : FrameRenderer
    {
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            base.LayoutSubviews();
            Layer.ShadowOpacity = 0.1f;
            Layer.ShadowColor = UIColor.FromRGB(26, 67, 67).CGColor;
            Layer.ShadowRadius = 6;
        }
    }
}
