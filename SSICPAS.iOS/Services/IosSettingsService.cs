using System.Threading.Tasks;
using Foundation;
using SSICPAS.Services.Interfaces;
using UIKit;

namespace SSICPAS.iOS.Services
{
    public class IosSettingsService : IPlatformSettingsService
    {
        public Task OpenWirelessSettings() => UIApplication.SharedApplication.OpenUrlAsync(new NSUrl(UIApplication.OpenSettingsUrlString), new UIApplicationOpenUrlOptions());
    }
}
