using System.Threading.Tasks;
using SSICPAS.Configuration;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using NUnit.Framework;
using Xamarin.Forms;
using SSICPAS.Views;
using System.Linq;
using System.Collections.Generic;
using System;
using SSICPAS.Enums;
using Xamarin.Forms.Mocks;
using SSICPAS.ViewModels.Base;
using SSICPAS.Services.Navigation;
using SSICPAS.ViewModels.Error;

namespace SSICPAS.Tests.NavigationTests
{
    /// <summary>
    /// Testing the navigation service and that state is preserved after a variety of navigation actions.
    ///
    /// When writing new tests after new methods are added, then be sure to test the following:
    ///
    /// - Push page in navigation
    /// - Push page modally fullscreen
    /// -> Perform your new method
    /// - Push page in navigation
    /// - Push page modally fullscreen
    /// - Pop page
    /// - Pop page
    /// - Pop page
    /// For each step, test that the state is preserved. See examples from other tests.
    /// 
    /// </summary>
    public class NavigationServiceTests : BaseUITests
    {
        INavigationService _navigationService;

        public override void AfterInit()
        {
            _navigationService = IoCContainer.Resolve<INavigationService>();
        }

        public override void RegisterUserService()
        {
            //Use the real UserService to test logout
            IoCContainer.RegisterInterface<IUserService, UserService>();
        }

        [Test]
        public async Task NavigationServiceTest_VerifyPincodeAndDropNavigationStack()
        {
            await _navigationService.OpenLandingPage();

            //When opening any page
            await _navigationService.PushPage(new TestPage1());

            //Then the current page returns this page and the stack contains this page on top of the Landing page.
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage1)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsTrue(CurrentPageIs(typeof(TestPage1)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(_navigationService.FindCurrentPage() is TestPage1);

            //And then restart the app
            await _navigationService.OpenLandingPage();

            //Then only the Verify pincode page is in the stack, wrapped by a NavigationPage.
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(CurrentPageIs(typeof(LandingPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
        }

        [Test]
        public async Task NavigationServiceTest_GoToLandingPage()
        {
            //When opening the LandingPage
            await _navigationService.OpenLandingPage();

            //And then pushing another page
            await _navigationService.PushPage(new TestPage1());

            //And then pushing another page
            await _navigationService.PushPage(new TestPage2());

            //Then all pages are in the stack
            Assert.IsTrue(CurrentPageIs(typeof(TestPage2)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage2)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);

            //When opening a modal
            await _navigationService.PushPage(new TestPage3(), false, Enums.PageNavigationStyle.PushModallyFullscreen);

            //Then all pages are there but there is a new NavigationPage with a modal page in it.
            Assert.IsTrue(CurrentPageIs(typeof(TestPage3)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2), typeof(TestPage3)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage2), typeof(TestPage3)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestPage3)), ModalPagesContainCheck_ErrorMessage);

            //When calling logout
            await _navigationService.OpenLandingPage();

            //Then the only the LandingPage is there
            Assert.IsTrue(CurrentPageIs(typeof(LandingPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);
        }

        [Test]
        public async Task NavigationServiceTest_OpenLandingPageAndCloseAppWithBackBtn()
        {
            await _navigationService.OpenLandingPage();

            //When opening the LandingPage
            //And then pushing another page
            await _navigationService.PushPage(new TestPage1());

            //Then the landingpage and page1 are in the stack
            Assert.IsTrue(CurrentPageIs(typeof(TestPage1)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage1)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);

            //Then opening the Landingpage
            await _navigationService.OpenLandingPage();

            //Then only the stack is dropped and only the landingpage is present.
            Assert.IsTrue(CurrentPageIs(typeof(LandingPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);

            //When calling the back function from here
            await _navigationService.PopPage();

            //Then nothing happened in the navigation.
            Assert.IsTrue(CurrentPageIs(typeof(LandingPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);
        }

        [Test]
        public async Task NavigationServiceTest_AddPagesToStackAndPopThemAgain()
        {
            //When opening the Landing page
            await _navigationService.OpenLandingPage();

            //And opening a modal
            await _navigationService.PushPage(new TestPage1(), false, PageNavigationStyle.PushModallyFullscreen);

            //And pushing a page inside the Modals navigation
            await _navigationService.PushPage(new TestPage2(), false, PageNavigationStyle.PushInNavigation);

            //And adding another modal
            await _navigationService.PushPage(new TestPage3(), false, PageNavigationStyle.PushModallyFullscreen);

            //Then all these pages are present in the stack
            Assert.IsTrue(CurrentPageIs(typeof(TestPage3)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2), typeof(TestPage3)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage), typeof(TestPage2), typeof(TestPage3)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestPage1), typeof(TestPage3)), ModalPagesContainCheck_ErrorMessage);

            //When calling PopPage
            await _navigationService.PopPage();

            //Then the modal is removed
            Assert.IsTrue(CurrentPageIs(typeof(TestPage2)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage), typeof(TestPage2)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestPage1)), ModalPagesContainCheck_ErrorMessage);

            //When calling PopPage
            await _navigationService.PopPage();

            //Then the Page 2 is removed
            Assert.IsTrue(CurrentPageIs(typeof(TestPage1)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage), typeof(TestPage1)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestPage1)), ModalPagesContainCheck_ErrorMessage);

            //When calling PopPage
            await _navigationService.PopPage();

            //Then the last modal is removed
            Assert.IsTrue(CurrentPageIs(typeof(LandingPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);

            //When calling PopPage
            await _navigationService.PopPage();

            //Then nothing happened in the navigation.
            Assert.IsTrue(CurrentPageIs(typeof(LandingPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);
        }

        [TestCase(Device.iOS)]
        [TestCase(Device.Android)]
        public async Task NavigationServiceTest_AddPagesToStackAndPopThemAgain2(string deviceType)
        {
            MockForms.Init(deviceType);
            Application.Current = new App(mock: true);
            AfterInit();

            //When opening the Landing page
            await _navigationService.OpenLandingPage();
            Assert.IsTrue(CurrentPageIs(typeof(LandingPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);

            //Then adding another page
            await _navigationService.PushPage(new TestPage1());
            Assert.IsTrue(CurrentPageIs(typeof(TestPage1)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage1)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);

            //Then adding another page
            await _navigationService.PushPage(new TestPage2());
            Assert.IsTrue(CurrentPageIs(typeof(TestPage2)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage2)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);

            //Then adding a modal page(fullscreen)
            await _navigationService.PushPage(new TestPage3(), false, PageNavigationStyle.PushModallyFullscreen);
            Assert.IsTrue(CurrentPageIs(typeof(TestPage3)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2), typeof(TestPage3)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage2), typeof(TestPage3)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestPage3)), ModalPagesContainCheck_ErrorMessage);

            //Then adding a modal page(cardView)
            await _navigationService.PushPage(new TestSheetPage(), true, PageNavigationStyle.PushModallySheetPageIOS);
            Assert.IsTrue(CurrentPageIs(typeof(TestSheetPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2), typeof(TestPage3), typeof(TestSheetPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            if (deviceType == Device.iOS)
            {
                //Then the sheetpage on iOS does NOT have a navpage for the statusbar
                Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage2), typeof(TestPage3)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            }
            else
            {
                //Then the sheetpage on Android DOES have a navpage for the statusbar
                Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage2), typeof(TestPage3), typeof(TestSheetPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            }
            Assert.IsTrue(ModalPagesContain(typeof(TestPage3), typeof(TestSheetPage)), ModalPagesContainCheck_ErrorMessage);

            //Then popping the sheet page
            await _navigationService.PopPage();
            Assert.IsTrue(CurrentPageIs(typeof(TestPage3)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2), typeof(TestPage3)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage2), typeof(TestPage3)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestPage3)), ModalPagesContainCheck_ErrorMessage);

            //Then pushing another sheet page
            await _navigationService.PushPage(new TestSheetPage2(), true, PageNavigationStyle.PushModallySheetPageIOS);
            Assert.IsTrue(CurrentPageIs(typeof(TestSheetPage2)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2), typeof(TestPage3), typeof(TestSheetPage2)), NavigationStackContainsPagesCheck_ErrorMessage);
            if (deviceType == Device.iOS)
            {
                //Then the sheetpage on iOS does NOT have a navpage for the statusbar
                Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage2), typeof(TestPage3)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            }
            else
            {
                //Then the sheetpage on Android DOES have a navpage for the statusbar
                Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage2), typeof(TestPage3), typeof(TestSheetPage2)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            }
            Assert.IsTrue(ModalPagesContain(typeof(TestPage3), typeof(TestSheetPage2)), ModalPagesContainCheck_ErrorMessage);

            //Then popping the sheet page
            await _navigationService.PopPage();
            Assert.IsTrue(CurrentPageIs(typeof(TestPage3)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2), typeof(TestPage3)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage2), typeof(TestPage3)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestPage3)), ModalPagesContainCheck_ErrorMessage);

            //Then popping the modal fullscreen
            await _navigationService.PopPage();
            Assert.IsTrue(CurrentPageIs(typeof(TestPage2)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage2)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);

            //Then popping Page 2
            await _navigationService.PopPage();
            Assert.IsTrue(CurrentPageIs(typeof(TestPage1)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage1)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);

        }

        [TestCase(Device.iOS)]
        [TestCase(Device.Android)]
        public async Task NavigationServiceTest_PushIOSSheetPage(string deviceType)
        {
            MockForms.Init(deviceType);
            Application.Current = new App(mock: true);
            AfterInit();

            //When opening the Landing page
            await _navigationService.OpenLandingPage();
            Assert.IsTrue(CurrentPageIs(typeof(LandingPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);

            //Then pushing a page modally sheet page style
            TestSheetPage sheetPage1 = new TestSheetPage();
            await _navigationService.PushPage(sheetPage1, true, PageNavigationStyle.PushModallySheetPageIOS);
            Assert.IsTrue(CurrentPageIs(typeof(TestSheetPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestSheetPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestSheetPage)), ModalPagesContainCheck_ErrorMessage);
            if (deviceType == Device.iOS)
            {
                //Then the sheetpage on iOS does NOT have a navpage for the statusbar
                Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            }
            else
            {
                //Then the sheetpage on Android DOES have a navpage for the statusbar
                Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage), typeof(TestSheetPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            }

            //Then popping the page by swipe (only iOS)
            if (deviceType == Device.iOS)
            {
                SimulateSwipeBackOnIOSSheetPage(sheetPage1);
            }
            else
            {
                SimulateClickBackButtonOniOSSheetPage(sheetPage1);
            }
            Assert.IsTrue(CurrentPageIs(typeof(LandingPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);

            //Then pushing Page2 modally sheet page style
            TestSheetPage2 sheetPage2 = new TestSheetPage2();
            await _navigationService.PushPage(sheetPage2, true, PageNavigationStyle.PushModallySheetPageIOS);
            Assert.IsTrue(CurrentPageIs(typeof(TestSheetPage2)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestSheetPage2)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestSheetPage2)), ModalPagesContainCheck_ErrorMessage);
            if (deviceType == Device.iOS)
            {
                //Then the sheetpage on iOS does NOT have a navpage for the statusbar
                Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            }
            else
            {
                //Then the sheetpage on Android DOES have a navpage for the statusbar
                Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage), typeof(TestSheetPage2)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            }

            //Then popping Page2 by button
            SimulateClickBackButtonOniOSSheetPage(sheetPage2);
            Assert.IsTrue(CurrentPageIs(typeof(LandingPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);

            //Then pushing Page1 normally
            await _navigationService.PushPage(new TestPage1());
            Assert.IsTrue(CurrentPageIs(typeof(TestPage1)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage1)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);

            //Then pushing Page2 modally sheet page style
            TestSheetPage2 sheetPage2again = new TestSheetPage2();
            await _navigationService.PushPage(sheetPage2again, true, PageNavigationStyle.PushModallySheetPageIOS);
            Assert.IsTrue(CurrentPageIs(typeof(TestSheetPage2)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestSheetPage2)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestSheetPage2)), ModalPagesContainCheck_ErrorMessage);
            if (deviceType == Device.iOS)
            {
                //Then the sheetpage on iOS does NOT have a navpage for the statusbar
                Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage1)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            }
            else
            {
                //Then the sheetpage on Android DOES have a navpage for the statusbar
                Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage1), typeof(TestSheetPage2)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            }

            //Then popping Page2 by swipe (IOS only)
            if (deviceType == Device.iOS)
            {
                SimulateSwipeBackOnIOSSheetPage(sheetPage2again);
            }
            else
            {
                SimulateClickBackButtonOniOSSheetPage(sheetPage2again);
            }
            Assert.IsTrue(CurrentPageIs(typeof(TestPage1)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage1)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);

            //Then pushing Page2 modally full screen
            await _navigationService.PushPage(new TestPage2());

            //Then pushing Page1 modally sheet page style
            TestSheetPage sheetPage1Again = new TestSheetPage();
            await _navigationService.PushPage(sheetPage1Again, true, PageNavigationStyle.PushModallySheetPageIOS);

            //Then popping Page1 by button
            SimulateClickBackButtonOniOSSheetPage(sheetPage1Again);

            //The the stack should include LandingPage, Page1 and Page2
            Assert.IsTrue(CurrentPageIs(typeof(TestPage2)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(TestPage2)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);
        }

        [Test]
        public async Task NavigationServiceTest_PushAndPopErrorPages_DefaultType()
        {
            //When opening the Landing page
            await _navigationService.OpenLandingPage();

            //And opening a modal
            await _navigationService.PushPage(new TestPage1(), false, PageNavigationStyle.PushModallyFullscreen);

            //And pushing a page inside the Modals navigation
            await _navigationService.PushPage(new TestPage2(), false, PageNavigationStyle.PushInNavigation);

            //And adding an error page
            await _navigationService.GoToErrorPage(Errors.UnknownError);

            //Then all these pages are present in the stack
            Assert.IsTrue(CurrentPageIs(typeof(BaseErrorPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2), typeof(BaseErrorPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage), typeof(TestPage2), typeof(BaseErrorPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestPage1), typeof(BaseErrorPage)), ModalPagesContainCheck_ErrorMessage);

            //When trying to push another error page
            await _navigationService.GoToErrorPage(Errors.UnknownError);

            //Then nothing happened. There is still only one error page.
            Assert.IsTrue(CurrentPageIs(typeof(BaseErrorPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2), typeof(BaseErrorPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage), typeof(TestPage2), typeof(BaseErrorPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestPage1), typeof(BaseErrorPage)), ModalPagesContainCheck_ErrorMessage);

            //When click back on error page
            (_navigationService.FindCurrentPage().BindingContext as BaseViewModel).BackCommand.Execute(null);

            //Then the error page is removed
            Assert.IsTrue(CurrentPageIs(typeof(TestPage2)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage), typeof(TestPage2)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestPage1)), ModalPagesContainCheck_ErrorMessage);
        }

        [Test]
        public async Task NavigationServiceTest_PushAndPopErrorPages_NemIDType()
        {
            //When opening the Landing page
            await _navigationService.OpenLandingPage();

            //And opening a modal
            await _navigationService.PushPage(new TestPage1(), false, PageNavigationStyle.PushModallyFullscreen);

            //And pushing a page inside the Modals navigation
            await _navigationService.PushPage(new TestPage2(), false, PageNavigationStyle.PushInNavigation);

            //And adding an error page
            await _navigationService.GoToErrorPage(Errors.SessionExpiredError);

            //Then all these pages are present in the stack
            Assert.IsTrue(CurrentPageIs(typeof(BaseErrorPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2), typeof(BaseErrorPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage), typeof(TestPage2), typeof(BaseErrorPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestPage1), typeof(BaseErrorPage)), ModalPagesContainCheck_ErrorMessage);

            //When trying to push another error page
            await _navigationService.GoToErrorPage(Errors.UnknownError);

            //Then nothing happened. There is still only one error page.
            Assert.IsTrue(CurrentPageIs(typeof(BaseErrorPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2), typeof(BaseErrorPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage), typeof(TestPage2), typeof(BaseErrorPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestPage1), typeof(BaseErrorPage)), ModalPagesContainCheck_ErrorMessage);

            //When click back on NemID error page
            (_navigationService.FindCurrentPage().BindingContext as BaseViewModel).BackCommand.Execute(null);

            //Nothing happened, you cannot navigate back.
            Assert.IsTrue(CurrentPageIs(typeof(BaseErrorPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2), typeof(BaseErrorPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage), typeof(TestPage2), typeof(BaseErrorPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestPage1), typeof(BaseErrorPage)), ModalPagesContainCheck_ErrorMessage);

            //When clicking OK on NemID error page
            (_navigationService.FindCurrentPage().BindingContext as NemIDErrorViewModel).OkButtonCommand.Execute(null);

            //Then you are sent to the front page to log in again.
            Assert.IsTrue(CurrentPageIs(typeof(LandingPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsEmpty(NavigationService.ModalPages);
        }

        [Test]
        public async Task NavigationServiceTest_PushAndPopErrorPages_ForceUpdateRequired()
        {
            //When opening the Landing page
            await _navigationService.OpenLandingPage();

            //And opening a modal
            await _navigationService.PushPage(new TestPage1(), false, PageNavigationStyle.PushModallyFullscreen);

            //And pushing a page inside the Modals navigation
            await _navigationService.PushPage(new TestPage2(), false, PageNavigationStyle.PushInNavigation);

            //And adding an error page
            await _navigationService.GoToErrorPage(Errors.ForceUpdateRequiredError);

            //Then all these pages are present in the stack
            Assert.IsTrue(CurrentPageIs(typeof(BaseErrorPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2), typeof(BaseErrorPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage), typeof(TestPage2), typeof(BaseErrorPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestPage1), typeof(BaseErrorPage)), ModalPagesContainCheck_ErrorMessage);

            //When trying to push another error page
            await _navigationService.GoToErrorPage(Errors.UnknownError);

            //Then nothing happened. There is still only one error page.
            Assert.IsTrue(CurrentPageIs(typeof(BaseErrorPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2), typeof(BaseErrorPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage), typeof(TestPage2), typeof(BaseErrorPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestPage1), typeof(BaseErrorPage)), ModalPagesContainCheck_ErrorMessage);

            //When click back on Force update error page
            (_navigationService.FindCurrentPage().BindingContext as BaseViewModel).BackCommand.Execute(null);

            //Nothing happened, you cannot navigate back.
            Assert.IsTrue(CurrentPageIs(typeof(BaseErrorPage)), CurrentPageIsCheck_ErrorMessage);
            Assert.IsTrue(NavigationStackContainsPages(typeof(LandingPage), typeof(TestPage1), typeof(TestPage2), typeof(BaseErrorPage)), NavigationStackContainsPagesCheck_ErrorMessage);
            Assert.IsTrue(NavPagesWithStatusBarContains(typeof(LandingPage), typeof(TestPage2), typeof(BaseErrorPage)), NavPagesWithStatusBarContainsCheck_ErrorMessage);
            Assert.IsTrue(ModalPagesContain(typeof(TestPage1), typeof(BaseErrorPage)), ModalPagesContainCheck_ErrorMessage);

            //Since ForceUpdate is always shown on top of the stack and prevent users from further actions,
            //we need to reset the NavigationStack after this test
            _navigationService.ResetNavigationStack();
        }

        public static string NavigationStackContainsPagesCheck_ErrorMessage = "";
        public bool NavigationStackContainsPages(params Type[] pageTypes)
        {
            NavigationStackContainsPagesCheck_ErrorMessage = "";
            int index = 0;

            List<NavigationPage> navPagesList = NavigationService.NavPagesWithStatusBar.ToList();
            navPagesList.Reverse();
            foreach (NavigationPage navPage in navPagesList)
            {
                List<Page> navStack = navPage.Navigation.NavigationStack.ToList();
                if (navStack == null || !navStack.Any())
                {
                    NavigationStackContainsPagesCheck_ErrorMessage = "There are no navigation pages in the stack. There should always be one";
                    return false;
                }

                foreach (Page page in navStack)
                {
                    if (pageTypes.Length <= index)
                    {
                        NavigationStackContainsPagesCheck_ErrorMessage = $"Expected no more pages, but found {page.GetType().Name}";
                        return false;
                    }

                    if (!(page.GetType() == pageTypes[index]))
                    {
                        NavigationStackContainsPagesCheck_ErrorMessage = $"Expected {pageTypes[index].Name} but found {page.GetType().Name}";
                        return false;
                    }
                    index++;
                }
            }

            List<Page> modalsWithoutStack = navPagesList.Last().Navigation.ModalStack.Where(modal => !(modal is NavigationPage)).ToList();
            if (modalsWithoutStack != null && modalsWithoutStack.Any())
            {
                Page page = modalsWithoutStack[0];
                if (!(page.GetType() == pageTypes[index]))
                {
                    //Only happens for iOS sheet pages. Other modals always have a navigation page under it.
                    NavigationStackContainsPagesCheck_ErrorMessage = $"Expected iOS sheet page {pageTypes[index].Name} but found {page.GetType().Name}";
                    return false;
                }
                index++;
            }

            if (pageTypes.Length > index)
            {
                NavigationStackContainsPagesCheck_ErrorMessage = $"Expected {pageTypes[index].Name} but found none";
                return false;
            }

            return true;
        }

        public static string NavPagesWithStatusBarContainsCheck_ErrorMessage = "";
        public bool NavPagesWithStatusBarContains(params Type[] pageTypes)
        {
            NavPagesWithStatusBarContainsCheck_ErrorMessage = "";

            int index = 0;

            List<NavigationPage> navPagesList = NavigationService.NavPagesWithStatusBar.ToList();
            navPagesList.Reverse();
            foreach (NavigationPage navPage in navPagesList)
            {
                Page currentPageInNavPage = navPage.CurrentPage;

                if (pageTypes.Length <= index)
                {
                    NavPagesWithStatusBarContainsCheck_ErrorMessage = $"Expected no more pages, but found {currentPageInNavPage.GetType().Name}";
                    return false;
                }

                if (!(currentPageInNavPage.GetType() == pageTypes[index]))
                {
                    NavPagesWithStatusBarContainsCheck_ErrorMessage = $"Expected {pageTypes[index].Name} but found {currentPageInNavPage.GetType().Name}";
                    return false;
                }
                index++;
            }

            if (pageTypes.Length > index)
            {
                NavPagesWithStatusBarContainsCheck_ErrorMessage = $"Expected {pageTypes[index].Name} but found none";
                return false;
            }

            return true;
        }

        public static string ModalPagesContainCheck_ErrorMessage = "";
        public bool ModalPagesContain(params Type[] pageTypes)
        {
            ModalPagesContainCheck_ErrorMessage = "";

            int index = 0;

            List<Page> modalsList = NavigationService.ModalPages.ToList();
            modalsList.Reverse();
            foreach (Page page in modalsList)
            {
                if (pageTypes.Length <= index)
                {
                    ModalPagesContainCheck_ErrorMessage = $"Expected no more pages, but found {page.GetType().Name}";
                    return false;
                }

                if (!(page.GetType() == pageTypes[index]))
                {
                    ModalPagesContainCheck_ErrorMessage = $"Expected {pageTypes[index].Name} but found {page.GetType().Name}";
                    return false;
                }
                index++;
            }

            if (pageTypes.Length > index)
            {
                NavigationStackContainsPagesCheck_ErrorMessage = $"Expected {pageTypes[index].Name} but found none";
                return false;
            }

            return true;
        }

        public static string CurrentPageIsCheck_ErrorMessage = "";
        public bool CurrentPageIs(Type pageType)
        {
            CurrentPageIsCheck_ErrorMessage = "";
            Page currentPage = _navigationService.FindCurrentPage();
            if (!(currentPage.GetType() == pageType))
            {
                CurrentPageIsCheck_ErrorMessage = $"Expected type {pageType.Name} but found type {currentPage.GetType().Name}";
                return false;
            }

            return true;
        }

        void SimulateSwipeBackOnIOSSheetPage(ContentSheetPageNoBackButtonOnIOS page)
        {
            page.Navigation.PopModalAsync();
            (page.BindingContext as ContentSheetPageNoBackButtonOnIOSViewModel)?.OnModalDismissed();
        }

        void SimulateClickBackButtonOniOSSheetPage(ContentSheetPageNoBackButtonOnIOS page)
        {
            (page.BindingContext as ContentSheetPageNoBackButtonOnIOSViewModel)?.BackCommand.Execute(null);
            (page.BindingContext as ContentSheetPageNoBackButtonOnIOSViewModel)?.OnModalDismissed();
        }
    }
}