using Android.OS;
using SSICPAS.Droid;
using SSICPAS.Services.Interfaces;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Views;

[assembly: Dependency(typeof(StatusBarService))]
namespace SSICPAS.Droid
{
    public class StatusBarService : IStatusBarService
    {
        public StatusBarService()
        {
        }

        public void SetStatusBarColor(Color backgroundColor, Color textColor)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if (CrossCurrentActivity.Current?.Activity?.Window != null)
                {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                    {
                        if (textColor == Color.White)
                        {
                            CrossCurrentActivity.Current.Activity.Window.DecorView.SystemUiVisibility
                                = StatusBarVisibility.Visible;

                        }
                        else
                        {
                            CrossCurrentActivity.Current.Activity.Window.DecorView.SystemUiVisibility
                                = (StatusBarVisibility)SystemUiFlags.LightStatusBar;

                        }

                        CrossCurrentActivity.Current.Activity.Window.SetStatusBarColor(backgroundColor.ToAndroid());
                    }
                }
            });
        }
    }
}