using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SSICPAS.Configuration;
using SSICPAS.Enums;
using SSICPAS.Models;
using SSICPAS.Services.Interfaces;
using SSICPAS.ViewModels.Base;
using SSICPAS.ViewModels.Onboarding;
using SSICPAS.Views;
using SSICPAS.Views.Onboarding;
using Xamarin.Forms;

namespace SSICPAS.Services
{
    public class NavigationService : INavigationService
    {
        //Be careful not to get a deadlock. I.e. you cannot call PushPage, PopPage or FindCurrentPageAsync inside each other.
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1,1);

        //State: Keeps track of the current tab that is visible in the MainTabbedPage. Be sure to update the state if you edit something in the tabbar.
        public int CurrentTab { get; set; } = 1;
        
        //Keeps a reference to the MainTabbedPage so that we can handle and replace tabs inside it at any time. Be sure this is garbage collected when Going to the frontpage, See ResetNavigationStack();
        public MainTabbedPage TabbedPage;

        //State: This is used to hold the state of the modal pages. We need it so FindCurrentPage() will work.
        public static Stack<Page> ModalPages = new Stack<Page>();

        //State: This is used to hold the state of the navigation pages. This is needed both to get FindCurrentPage() to work, but the main purpose is for the StatusBarService to set the color on Android.
        public static Stack<NavigationPage> NavPagesWithStatusBar = new Stack<NavigationPage>();

        IStatusBarService _androidStatusBarService;
        IBrightnessService _brightnessService;
        private static bool IsPushing = false;

        public NavigationService(IStatusBarService statusBarService, IBrightnessService brightnessService)
        {
            _androidStatusBarService = statusBarService;
            _brightnessService = brightnessService;
        }

        /// <summary>
        /// Opens the landing page and resets the navigation stack.
        /// The LandingPage will be the only page in the stack.
        /// </summary>
        public async Task OpenLandingPage()
        {
            // In case of ForceUpdate we do not want to show landing page.
            if (await IsForceUpdateErrorPageShownAsync())
            {
                return;
            }

            ResetNavigationStack();
            NavigationPage newPage = new NavigationPage(new LandingPage());

            Device.BeginInvokeOnMainThread(() => {
                NavPagesWithStatusBar.Push(newPage);
                Application.Current.MainPage = newPage;
            });
        }

        /// <summary>
        /// Opens the RegisterPinCode page and resets the navigation stack.
        /// The RegisterPinCode page will be the only page in the stack so there are no pages behind it, to go back to.
        /// </summary>
        public async Task OpenVerifyPinCodePageAsync(bool setUiOnMainThread = true)
        {
            // In case of ForceUpdate we do not want to show VerifyPincodePage page.
            if (await IsForceUpdateErrorPageShownAsync())
            {
                return;
            }
            ResetNavigationStack();
            NavigationPage newPage = new NavigationPage(new RegisterPinCodeView(VerifyPinCodeViewModel.CreateVerifyPinCodeViewModel()));
            
            if (setUiOnMainThread)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    NavPagesWithStatusBar.Push(newPage);
                    Application.Current.MainPage = newPage;
                });
            }
            else
            {
                NavPagesWithStatusBar.Push(newPage);
                Application.Current.MainPage = newPage;
            }
        }

        /// <summary>
        /// Opens the MainTabbedPage and resets the navigation stack.
        /// The MainTabbedPage will be the only page in the stack, so the user cannot go back to onboarding.
        /// </summary>
        /// <returns></returns>
        public async Task OpenTabbar()
        {
            ResetNavigationStack();
            MainTabbedPage newTabbedPage = new MainTabbedPage();

            NavigationPage newPage = new NavigationPage(newTabbedPage);
            NavPagesWithStatusBar.Push(newPage);
            Application.Current.MainPage = newPage;

            TabbedPage = newTabbedPage;
            await newTabbedPage.InitializePages();
        }

        /// <summary>
        /// Resets the state of the navigation service.
        /// Should be called whenever the navigation stack is cleared.
        /// And afterwards, update the state with the new first page.
        /// </summary>
        public void ResetNavigationStack()
        {
            TabbedPage = null;
            ModalPages = new Stack<Page>();
            NavPagesWithStatusBar = new Stack<NavigationPage>();
        }

        /// <summary>
        /// Changes a page in a tab. The state logic is handled inside the MainTabbedPage.
        /// The TabbedPage also needs the TabbedPage and CurrentTab states to be maintained.
        /// Will only work when called from a place where the MainTabbedPage is visible.
        /// </summary>
        /// <param name="page">The new page to show</param>
        /// <param name="locationEnum">The location (index in tabbar) it should be added to</param>
        public void ChangePageInTab(Page page, TabPageLocationEnum locationEnum)
        {
            TabbedPage.ChangeRootPageInTab(page, locationEnum);
        }

        /// <summary>
        /// Use this method to push a new page, both with navigation and/or modally. Please check how it's used in other places.
        /// </summary>
        /// <param name="page">The page to push. Do not wrap this page in a NavigationPage. This is done automatically to keep track of all Nav Pages.</param>
        /// <param name="animated">Whether or not the transition should be animated (on iOS)</param>
        /// <param name="style">
        /// PageNavigationStyle.PushInNavigation: Pushes the page to the existing Navigation stack. If animation=true, then it will animate in from right to left.
        /// PageNavigationStyle.PushModallyFullscreen: Pushes the page modally and automatically wraps it in a NavigationPage. If animation=true, then it will animate up from below.
        /// PageNavigationStyle.PushModallySheetPageIOS: Pushes the page as a Sheet page on iOS, and ModallyFullscreen on Android. See PushSheetPageModalOnIOS() for more information.
        /// </param>
        /// <param name="data">Data that is sent to BaseViewModel.Initialize() in case you want to transfer data to the new page. Just make your new view model override the Initialize() method.</param>
        /// <returns></returns>
        public async Task PushPage(Page page, bool animated = true, PageNavigationStyle style = PageNavigationStyle.PushInNavigation, object data = null)
        {
            if (IsPushing) return;
            await _semaphore.WaitAsync();
            IsPushing = true;
            _brightnessService.UpdateBrightness(page);
            try
            {
                switch (style)
                {
                    case PageNavigationStyle.PushInNavigation:

                        await PushPageInNavigation(page, animated, data);
                        break;

                    case PageNavigationStyle.PushModallySheetPageIOS:

                        page.SetNavigationStyleCardView();
                        if (Device.RuntimePlatform == Device.iOS)
                            await PushSheetPageModalOnIOS(page, animated, data);
                        else
                            await PushModalWithNavigation(page, animated, data);
                        break;

                    case PageNavigationStyle.PushModallyFullscreen:

                        page.SetNavigationStyleFullscreen();
                        await PushModalWithNavigation(page, animated, data);
                        break;
                }
            }
            finally
            {
                IsPushing = false;
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Pushes the page to the existing Navigation stack. If animation=true, then it will animate in from right to left.
        /// Only use this method through PushPage, to be sure the state is handled properly.
        /// </summary>
        protected async Task PushPageInNavigation(Page page, bool animated = true, object data = null)
        {
            await page.Initialize(data);
            await FindCurrentPage().Navigation.PushAsync(page, animated);
        }

        /// <summary>
        /// Pushes the page modally and automatically wraps it in a NavigationPage. If animation=true, then it will animate up from below.
        /// Only use this method through PushPage, to be sure the state is handled properly.
        /// </summary>
        private async Task PushModalWithNavigation(Page page, bool animated = true, object data = null)
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                await page.Initialize(data);

                NavigationPage newPage = new NavigationPage(page);
                await FindCurrentPage().Navigation.PushModalAsync(newPage, animated);

                NavPagesWithStatusBar.Push(newPage);
                ModalPages.Push(page);
            });
        }

        /// <summary>
        /// Pushes the page as a Sheet page on iOS, and ModallyFullscreen on Android.
        /// Only use this method through PushPage, to be sure the state is handled properly.
        ///
        /// Read more about the Sheet modality here:
        /// https://developer.apple.com/design/human-interface-guidelines/ios/app-architecture/modality/
        /// 
        /// </summary>
        /// <param name="page">
        /// The page you want to present
        /// The page has to extend ContentSheetPageNoBackButtonOnIOS
        /// The page's viewmodel HAS to extend ContentSheetPageNoBackButtonOnIOSViewModel
        /// Also ensure that all back functions are using the inherited BackCommand function.
        /// Only this way, will the state be preserved in the NavigationService.
        /// </param>
        /// <param name="animated"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task PushSheetPageModalOnIOS(Page page, bool animated = true, object data = null)
        {
            await page.Initialize(data);

            await FindCurrentPage().Navigation.PushModalAsync(page, animated);

            ModalPages.Push(page);
        }

        /// <summary>
        /// Used by the ContentSheetPageNoBackButtonOnIOSViewModel to preserve state.
        /// </summary>
        public void OnDidDismissSheetPageOnIOS()
        {
            //In case we reset the stack when in a sheet page on IOS
            if (ModalPages.Any())
            {
                ModalPages.Pop();
            }
        }

        /// <summary>
        /// Uses the FindCurrentPage() method to find the right page to pop.
        /// This method preserves the state of the NavigationService for a wide variety of cases.
        /// Be very careful about editing this method.
        /// Rule of thumb when you want to edit it: DON'T
        /// If you cannot avoid it, then be very careful that you maintain all states and that you unittest it thoroughly with all the cases you can think of. Think about which navigation actions the user might perform AFTER your change.
        /// </summary>
        public async Task PopPage(bool animated = true)
        {
            await _semaphore.WaitAsync();

            try
            {

                Page pageToPop = FindCurrentPage();
                
                if (pageToPop.IsModal())
                {
                    if (pageToPop.Navigation.NavigationStack.Any()) //The modal has a navigation page
                    {
                        NavPagesWithStatusBar.Pop();
                    }
                    await pageToPop.Navigation.PopModalAsync(animated);
                    ModalPages.Pop();
                }
                else
                {
                    var poppedPage = await pageToPop.Navigation.PopAsync(animated);
                    if (poppedPage == null)
                    {
                        //There were no pages to close.
                        //Use default Android behaviour to close the app.
                        if (Device.RuntimePlatform == Device.Android)
                        {
                            IoCContainer.Resolve<ICloseButtonService>().ClickCloseButton();
                        }
                    }
                    else if (pageToPop.Navigation.NavigationStack.Count >= 1)
                    {
                        //A Nav page was popped
                        NavPagesWithStatusBar.Pop();
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }

            Page pageAfterPop = await FindCurrentPageAsync(true);
            await ((BaseViewModel) pageAfterPop.BindingContext)?.ExecuteOnReturn(null);
            pageAfterPop.ResetOrientation();
            _brightnessService.UpdateBrightness(pageAfterPop);
        }

        public async Task PopPage()
        {
            await PopPage(true);
        }

        /// <summary>
        /// Use this method when you need to check the current page BEFORE calling PushPage or PopPage.
        /// This will make sure that the check is done synchronously do avail race conditions.
        ///
        /// The PushPage and PopPage methods are using the same semaphore to avoid race conditions.
        /// </summary>
        /// <param name="includeTabs">
        /// Set to true, if you want the page inside the tab to return in case you have the MainTabbedPage open and visible.
        /// Set to false, if you the MainTabbedPage to be returned isntead of a page inside a tab.</param>
        /// <returns>The currently active page (excluding popups)</returns>
        public async Task<Page> FindCurrentPageAsync(bool includeTabs = false)
        {
            await _semaphore.WaitAsync();

            try
            {
                return FindCurrentPage(includeTabs);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Should only be called from inside the navigation service if possible.
        /// Otherwise try to use FindCurrentPageAsync instead. This method does not protect against race conditions.
        /// </summary>
        public Page FindCurrentPage(bool includeTabs = false)
        {
            //When a modal is pushed it has it's own navigation stack on top of another navigation stack.
            if (ModalPages.Any())
            {
                //If the last modal page, has a navigation stack, then return the top most item on the stack.
                Page modalRoot = ModalPages.Peek();
                if (modalRoot.Navigation.NavigationStack.Count > 1)
                {
                    return modalRoot.Navigation.NavigationStack.Last();
                }
                var modalNavPage = modalRoot as NavigationPage;
                if (modalNavPage != null)
                {
                    return modalNavPage.CurrentPage;
                }

                //Otherwise the modal it self must be the top.
                return modalRoot;
            }

            // The tabbar is not the highest root.
            if (NavPagesWithStatusBar.Any() && !(NavPagesWithStatusBar.Peek().CurrentPage is MainTabbedPage))
            {
                return NavPagesWithStatusBar.Peek().CurrentPage;
            }

            //The tabbar is the highest root
            if (TabbedPage != null)
            {
                if (includeTabs)
                {
                    var currentTab = TabbedPage.CurrentPage;
                    var tabbedNavPage = currentTab as NavigationPage;
                    if (tabbedNavPage != null)
                    {
                        return tabbedNavPage.CurrentPage;
                    }
                    else
                    {
                        return currentTab;
                    }
                }
                else
                {
                    return TabbedPage;
                }
            }

            //If there is no tabbar yet
            Page rootPage = Application.Current.MainPage;
            var navPage = rootPage as NavigationPage;
            if (navPage != null)
            {
                return navPage.CurrentPage;
            }
            return rootPage;
        }

        /// <summary>
        /// Can be used programmatically switch to another tab in the MainTabbedPage.
        /// </summary>
        /// <param name="index">The tab index to go to</param>
        public async Task GoToTab(int index)
        {
            if (TabbedPage == null) { return; }

            if (index >= 0 && index < TabbedPage.Children.Count)
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    TabbedPage.CurrentPage = TabbedPage.Children[index];
                });
                CurrentTab = index;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"Cannot navigate to specified tab index as it is outside the range of acceptable values. Attempted to navigate to index {index} (zero-based) on a tabbed page that has {TabbedPage.Children.Count} children.");
            }
        }

        /// <summary>
        /// Can be used programmatically switch to another tab in the MainTabbedPage.
        /// </summary>
        /// <param name="title">The title of the tab page to go to</param>
        public async Task GoToTab(string title = null)
        {
            int tabIndex = CurrentTab;

            if (title != null && TabbedPage != null)
            {
                var tabs = TabbedPage.Children.ToList();
                var index = tabs.FindIndex(x => x.Title == title);
                if (index >= 0 && index < tabs.Count)
                {
                    tabIndex = index;
                }
            }
            await GoToTab(tabIndex);

        }

        /// <summary>
        /// Use this to push an error page to the stack. It will be pushed modally fullscreen.
        /// It handles race conditions and will make sure to only show one error page at a time, in case multiple errors are thrown.
        /// Only the first error will be displayed.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task GoToErrorPage(ErrorPageModel data)
        {
            if ((await FindCurrentPageAsync()).GetType() != typeof(BaseErrorPage))
            {
                await PushPage(new BaseErrorPage(data.Type), true, PageNavigationStyle.PushModallyFullscreen, data);
            }
        }

        /// <summary>
        /// Sets the status bar color. This is done automatically and should only rarely be handled manually.
        /// </summary>
        public void SetStatusBar(Color backgroundColor, Color textColor)
        {
            if (!NavPagesWithStatusBar.Any()) return;

            NavPagesWithStatusBar.Peek().BarBackgroundColor = backgroundColor;
            NavPagesWithStatusBar.Peek().BarTextColor = textColor;
            _androidStatusBarService.SetStatusBarColor(backgroundColor, textColor);
        }

        /// <summary>
        /// Determins if the force update page is shown on the screen now
        /// </summary>
        public async Task<bool> IsForceUpdateErrorPageShownAsync()
        {
            var currentPage = await FindCurrentPageAsync();
            var isForceUpdateErrorPage = false;

            if (currentPage.GetType() == typeof(BaseErrorPage))
            {
                isForceUpdateErrorPage = (((BaseErrorPage)currentPage).Type == ErrorPageType.ForceUpdate);
            }

            return isForceUpdateErrorPage;
        }
    }
}