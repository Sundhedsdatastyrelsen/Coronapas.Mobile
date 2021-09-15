using Android.Content;
using Android.Graphics;
using Android.Widget;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Internal;
using SSICPAS.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;


[assembly: ExportRenderer(typeof(TabbedPage), typeof(CustomTabbedPageRenderer))]
namespace SSICPAS.Droid.CustomRenderers
{
    public class CustomTabbedPageRenderer : TabbedPageRenderer
    {
        Xamarin.Forms.TabbedPage tabbedPage;
        BottomNavigationView bottomNavigationView;
        BottomNavigationMenuView bottomNavigationMenuView;

        public CustomTabbedPageRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                tabbedPage = e.NewElement as TabbedPage;
                bottomNavigationView = (GetChildAt(0) as Android.Widget.RelativeLayout).GetChildAt(1) as BottomNavigationView;
                bottomNavigationMenuView = bottomNavigationView.GetChildAt(0) as BottomNavigationMenuView;

                ChangeFont();
            }
        }

        private void ChangeFont()
        {
            Typeface fontFace = Typeface.CreateFromAsset(Context.Assets, "fonts/IBMPlexSans-Regular.ttf");

            for (int i = 0; i < bottomNavigationMenuView.ChildCount; i++)
            {
                BottomNavigationItemView item = bottomNavigationMenuView.GetChildAt(i) as BottomNavigationItemView;
                Android.Views.View itemTitle = item.GetChildAt(1);

                TextView smallTextView = ((TextView)((BaselineLayout)itemTitle).GetChildAt(0));
                TextView largeTextView = ((TextView)((BaselineLayout)itemTitle).GetChildAt(1));

                smallTextView.SetTypeface(fontFace, TypefaceStyle.Normal);
                largeTextView.SetTypeface(fontFace, TypefaceStyle.Normal);

                smallTextView.SetTextSize(Android.Util.ComplexUnitType.Sp, 13);
                largeTextView.SetTextSize(Android.Util.ComplexUnitType.Sp, 13);
            }
        }
    }
}