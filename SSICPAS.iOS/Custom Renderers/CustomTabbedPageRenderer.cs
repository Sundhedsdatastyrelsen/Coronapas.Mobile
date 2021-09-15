using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using SSICPAS.iOS.CustomRenderers;
using CoreGraphics;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(CustomTabbedPageRenderer))]
namespace SSICPAS.iOS.CustomRenderers
{
    public class CustomTabbedPageRenderer : TabbedRenderer
    {
        public const float bottomMargin = 5f;
        public static readonly UIColor itemColor = Color.FromHex("#24215F").ToUIColor();
        public static readonly UIColor selectedItemColor = Color.FromHex("#3B37E2").ToUIColor();

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            TabBar.ClipsToBounds = false;
            CGRect bounds = View.Bounds;
            bounds.Y = bottomMargin;
            View.Bounds = bounds;
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            if (TabBar?.Items == null)
            {
                return;
            }

            TabbedPage tabbedPage = Element as TabbedPage;

            if (tabbedPage != null)
            {
                for (int i = 0; i < TabBar.Items.Length; i++)
                {
                    UITabBarItem item = TabBar.Items[i];
                    UIFont font = UIFont.FromName("IBM Plex Sans", 13);
                    item.SetTitleTextAttributes(new UITextAttributes() { Font = font, TextColor = itemColor }, UIControlState.Normal);
                    item.SetTitleTextAttributes(new UITextAttributes() { Font = font, TextColor = selectedItemColor }, UIControlState.Selected);
                }
            }
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
        }
    }
}
