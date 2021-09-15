using System.Threading.Tasks;
using SSICPAS.Enums;
using SSICPAS.Models;
using Xamarin.Forms;

namespace SSICPAS.Services.Interfaces
{
    public interface INavigationService
    {
        int CurrentTab { get; set; }
        public Page FindCurrentPage(bool includeTabs = false);
        public Task<Page> FindCurrentPageAsync(bool includeTabs = false); //Use this method if you need to be sure all PushPage actions have been completed first.

        //Set the main page
        Task OpenTabbar();
        Task OpenVerifyPinCodePageAsync(bool setUiOnMainThread = true);
        Task OpenLandingPage();

        Task<bool> IsForceUpdateErrorPageShownAsync();

        //Push page
        Task PushPage(Page page, bool animated = true, PageNavigationStyle style = PageNavigationStyle.PushInNavigation, object data = null);
        void ChangePageInTab(Page page, TabPageLocationEnum locationEnum);

        //Pop page
        Task PopPage(bool animated = true);
        Task PopPage();
        void OnDidDismissSheetPageOnIOS();

        void ResetNavigationStack();

        //Others
        Task GoToErrorPage(ErrorPageModel data);
        void SetStatusBar(Color backgroundColor, Color textColor);
    }
}