using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Android.Content;
using Plugin.CurrentActivity;
using SSICPAS.Services.Interfaces;

namespace SSICPAS.Droid.Services
{
    public class AndroidDeeplinkingService : IDeeplinkingService
    {
        public Task GoToAppSettings()
        {
            try
            {
                var intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
                intent.AddFlags(ActivityFlags.NewTask);
                string package_name = "dk.sum.ssicpas";
                var uri = Android.Net.Uri.FromParts("package", package_name, null);
                intent.SetData(uri);
                CrossCurrentActivity.Current.AppContext.StartActivity(intent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error attempting to deep-link to application settings: {ex}");
            }

            return Task.CompletedTask;
        }
    }
}