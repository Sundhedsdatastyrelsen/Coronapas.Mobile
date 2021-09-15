using System.Linq;
using System.Threading.Tasks;
using SSICPAS.Configuration;
using SSICPAS.Enums;
using SSICPAS.Services.Interfaces;
using SSICPAS.ViewModels.Base;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace SSICPAS.Services
{
    static class PageNavigationExtensions
    {
        public static bool IsModal(this Xamarin.Forms.Page page)
        {
            return (NavigationService.ModalPages.Any() && NavigationService.ModalPages.Peek() == page);
        }

        public static void ResetOrientation(this Xamarin.Forms.Page page)
        {
            if (page is IExtraOrientationSupport pageWithExtraOrientationSupport)
            {
                IoCContainer.Resolve<IOrientationService>().SetSupportedOrientation(pageWithExtraOrientationSupport.SupportedOrientation);
            }
            else
            {
                IoCContainer.Resolve<IOrientationService>().SetSupportedOrientation(SupportedOrientation.Portrait);
            }
        }

        public static async Task Initialize(this Xamarin.Forms.Page page, object data)
        {
            if (page is IExtraOrientationSupport pageWithExtraOrientationSupport)
            {
                IoCContainer.Resolve<IOrientationService>().SetSupportedOrientation(pageWithExtraOrientationSupport.SupportedOrientation);
            }
            if (data != null)
            {
                await ((BaseViewModel)page.BindingContext).InitializeAsync(data);
            }
        }

        public static void SetNavigationStyleFullscreen(this Xamarin.Forms.Page page)
        {
            page.On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FullScreen);
        }

        public static void SetNavigationStyleCardView(this Xamarin.Forms.Page page)
        {
            page.On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.PageSheet);
        }
    }
}
