using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using Plugin.CurrentActivity;
using SSICPAS.Configuration;
using SSICPAS.Services.Interfaces;

namespace SSICPAS.Droid
{
    [Activity(
        Label = "Coronapas",
        Icon = "@mipmap/icon",
        RoundIcon = "@mipmap/icon_round",
        Theme = "@style/SSICPAS.Splash",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleInstance
    )]
    public class SplashActivity : AppCompatActivity, ICloseButtonService
    {
        public SplashActivity() => IoCContainer.RegisterInstance<ICloseButtonService>(this);
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        public void ClickCloseButton()
        {
            if (IsFinishing && Lifecycle.CurrentState == Lifecycle.State.Destroyed)
            {
                // Continue after if to terminate
            }
            else if (IsFinishing || Lifecycle.CurrentState == Lifecycle.State.Started)
            {
                return;
            }

            if (CrossCurrentActivity.Current?.Activity?.IsFinishing == false)
            {
                CrossCurrentActivity.Current?.Activity?.FinishAffinity();
            }

            base.OnBackPressed();
        }
    }
}