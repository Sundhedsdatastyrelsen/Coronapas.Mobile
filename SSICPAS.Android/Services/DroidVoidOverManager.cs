using Android.Content;
using Android.Views.Accessibility;
using Plugin.CurrentActivity;
using SSICPAS.Core.Interfaces;

namespace SSICPAS.Droid.Services
{
    public class DroidVoidOverManager: IVoiceOverManager
    {
        public bool IsVoiceOverEnabled
        {
            get
            {
                AccessibilityManager am = (AccessibilityManager) CrossCurrentActivity.Current?.Activity?.GetSystemService(Context.AccessibilityService);
                return am is { IsEnabled: true };
            }
        }
    }
}