using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using SSICPAS.Configuration;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Auth;

namespace SSICPAS.Droid.OAuth
{
    /// <summary>
    /// This Activity is hit when redirecting from browser flow
    /// </summary>
    [Activity(
        Label = "AuthUrlSchemeInterceptorActivity",
        LaunchMode = LaunchMode.SingleTop,
        NoHistory = true,
        Name = "md52eac344ff43f7bff7f1301c3ba1d0d0c.AuthUrlSchemeInterceptorActivity")]
    [IntentFilter(
            new[] { Intent.ActionView },
            Categories = new[]
                {
                    Intent.CategoryDefault,
                    Intent.CategoryBrowsable
                },
            DataScheme = "dk.sum.ssicpas",
            DataPath = "/oauth2redirect"
    )]
    public class AuthUrlSchemeInterceptorActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                if (Intent != null)
                {
                    global::Android.Net.Uri uriAndroid = Intent.Data;

                    // Convert Android.Net.Url to C#/netxf/BCL System.Uri - common API
                    Uri uriNetfx = new Uri(uriAndroid.ToString());

                    CloseBrowser();

                    // load redirect_url Page for parsing
                    Console.WriteLine(uriNetfx.AbsolutePath);
                    AuthenticationState.Authenticator.OnPageLoading(uriNetfx);
                }

                Finish();
                return;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print("AuthUrlSchemeInterceptorActivity exception");
                IoCContainer.Resolve<ILoggingService>().LogException(LogSeverity.SECURITY_WARNING, e, nameof(AuthUrlSchemeInterceptorActivity) + " " + nameof(OnCreate) + " error when redirecting to app after authentication");

                CloseBrowser();

                // Redirect and hit OnAuthError
                AuthenticationState.Authenticator.OnPageLoading(new Uri("dk.sum.ssicpas:/oauth2redirect"));

                Finish();
            }
        }

        void CloseBrowser()
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NoAnimation);
            StartActivity(intent);
        }
    }
}
