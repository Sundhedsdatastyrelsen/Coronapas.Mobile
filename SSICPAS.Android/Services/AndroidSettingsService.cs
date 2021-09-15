using Android.Content;
using Plugin.CurrentActivity;
using SSICPAS.Services.Interfaces;
using System.Threading.Tasks;

namespace SSICPAS.Droid.Services
{
    public class AndroidSettingsService : IPlatformSettingsService
    {
        public Task OpenWirelessSettings()
        {
            Intent intent = new Intent(Android.Provider.Settings.ActionWirelessSettings).AddFlags(ActivityFlags.NewTask);
            CrossCurrentActivity.Current.AppContext.StartActivity(intent);
            return Task.CompletedTask;
        }
    }
}