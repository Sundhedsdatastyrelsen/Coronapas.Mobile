using System.Threading.Tasks;
using SSICPAS.Enums;
using SSICPAS.Models;
using SSICPAS.Services.Interfaces;
using Xamarin.Forms;

namespace SSICPAS.Tests.TestMocks
{
    public class MockNavigationService : INavigationService
    {
        public MockNavigationService()
        {
        }

        public int CurrentTab { get; set; }

        public void ChangePageInTab(Page page, TabPageLocationEnum locationEnum)
        {
        }

        public Page FindCurrentPage(bool includeTabs = false)
        {
            return new Page();
        }

        public Task<Page> FindCurrentPageAsync(bool includeTabs = false)
        {
            return Task.FromResult(new Page());
        }

        public Task GoToErrorPage(ErrorPageModel data)
        {
            return Task.CompletedTask;
        }

        public Task<bool> IsForceUpdateErrorPageShownAsync()
        {
            return Task.FromResult(false);
        }

        public void OnDidDismissSheetPageOnIOS()
        {
        }

        public Task OpenLandingPage()
        {
            return Task.CompletedTask;
        }

        public Task OpenTabbar()
        {
            return Task.CompletedTask;
        }

        public Task OpenVerifyPinCodePageAsync(bool setUiOnMainThread = true)
        {
            return Task.CompletedTask;
        }

        public Task PopPage(bool animated = true)
        {
            return Task.CompletedTask;
        }

        public Task PopPage()
        {
            return Task.CompletedTask;
        }

        public Task PushPage(Page page, bool animated = true, PageNavigationStyle style = PageNavigationStyle.PushInNavigation, object data = null)
        {
            return Task.CompletedTask;
        }

        public void ResetNavigationStack()
        {
            
        }

        public void SetStatusBar(Color backgroundColor, Color textColor)
        {
        }
    }
}
