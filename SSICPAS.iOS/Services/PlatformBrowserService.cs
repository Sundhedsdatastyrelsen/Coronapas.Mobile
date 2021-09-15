using System.Threading.Tasks;
using SSICPAS.Core.Interfaces;
using SSICPAS.iOS.Services;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(PlatformBrowserService))]
namespace SSICPAS.iOS.Services
{
    public class PlatformBrowserService : IPlatformBrowserService
    {
        public async Task CloseBrowser(bool animated)
        {
            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            UIViewController vc = window.RootViewController;

            await vc.DismissViewControllerAsync(animated);
        }
    }
}