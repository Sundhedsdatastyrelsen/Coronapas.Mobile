using Android.Content;
using Android.Content.PM;
using SSICPAS.Core.Interfaces;

namespace SSICPAS.Droid.Services
{
    public class AndroidCustomOAuthNativeUIFlagResolver : ICustomOAuthNativeUIFlagResolver
    {
        public bool ShouldEnableNativeUI()
        {
            Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://"));
            ResolveInfo resolveInfo = Android.App.Application.Context.PackageManager.ResolveActivity(browserIntent, PackageInfoFlags.MatchDefaultOnly);
            string defaultBrowserPackageName = resolveInfo?.ActivityInfo.PackageName;

            return defaultBrowserPackageName == "com.android.chrome"; // If Default browser is not Chrome - fallback to WebView
        }
    }
}
