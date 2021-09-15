using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Foundation;
using SSICPAS.Services.Interfaces;
using UIKit;

namespace SSICPAS.iOS.Services
{
    public class IosDeeplinkingService : IDeeplinkingService
    {
        public async Task GoToAppSettings()
        {
            try
            {
                await UIApplication.SharedApplication.OpenUrlAsync(new NSUrl(UIApplication.OpenSettingsUrlString),
                    new UIApplicationOpenUrlOptions());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error attempting to deep-link to application settings: {ex}");
            }
        }
    }
}