using System.Threading.Tasks;
using SSICPAS.Configuration;
using SSICPAS.Enums;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.ViewModels;
using SSICPAS.ViewModels.Base;
using SSICPAS.ViewModels.Certificates;
using SSICPAS.Views.Certificates;
using SSICPAS.Views.InfoPages;
using SSICPAS.Views.ScannerPages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainTabbedPage : TabbedPage
    {
        private readonly INavigationService _navigationService = IoCContainer.Resolve<INavigationService>();
        private readonly IBrightnessService _brightnessService = IoCContainer.Resolve<IBrightnessService>();
        private readonly IScreenshotDetectionService _screenshotDetectionService = IoCContainer.Resolve<IScreenshotDetectionService>();
        private readonly INotificationService _notificationService = IoCContainer.Resolve<INotificationService>();
        private IScannerFactory _scannerFactoryService = IoCContainer.Resolve<IScannerFactory>();

        private QRScannerPage _qrScannerPage;
        private ImagerScannerPage _imagerScannerPage;
        private bool _scannerPageIsShowing = false;

        private string[] _tabIcons = new string[3] {
            SSICPASImage.BottombarPassport.Image(),
            SSICPASImage.BottombarInfo.Image(),
            SSICPASImage.BottombarScan.Image()
        };
        private string[] _tabTitle => new string[3] {
            "TABBAR_ITEM_TITLE_CERTIFICATE".Translate(),
            "TABBAR_ITEM_TITLE_INFO".Translate(),
            "TABBAR_ITEM_TITLE_SCANNER".Translate()
        };

        public MainTabbedPage()
        {
            InitializeComponent();
            AutomationProperties.SetIsInAccessibleTree(this, false);

            BarBackgroundColor = Color.White;
            BindingContext = new MainTabbedPageViewModel();

            NavigationPage certificatePage = CreateTabNavigationPage(
                PassportPageViewModel.FetchCurrentPassportPage(),
                TabPageLocationEnum.PassportPage);
            Children.Add(certificatePage);

            NavigationPage infoPage = CreateTabNavigationPage(new PersonalPage(), TabPageLocationEnum.InfoPage);
            Children.Add(infoPage);


            if (_scannerFactoryService.GetAvailableScanner() == null)
            {
                _qrScannerPage = new QRScannerPage();
                _qrScannerPage.SetFromTabbar();
                NavigationPage scannerView = CreateTabNavigationPage(_qrScannerPage, TabPageLocationEnum.ScannerPage);
                Children.Add(scannerView);
            }
            else
            {
                _imagerScannerPage = new ImagerScannerPage();
                _imagerScannerPage.SetFromTabbar();
                NavigationPage scannerView = CreateTabNavigationPage(_imagerScannerPage, TabPageLocationEnum.ScannerPage);
                Children.Add(scannerView);
            }

            SetStatusBarColorDefault();
            UpdateTabPage();
            Device.InvokeOnMainThreadAsync(async () => await InitializePages());
            ShowNotificationOnStartup();
        }

        private async void ShowNotificationOnStartup()
        {
            await Device.InvokeOnMainThreadAsync(_notificationService.NotificationOnStartUp);
        }
        
        public void ChangeRootPageInTab(Page page, TabPageLocationEnum locationEnum)
        {
            Children[(int)locationEnum] = CreateTabNavigationPage(page, locationEnum);
            if (locationEnum == TabPageLocationEnum.PassportPage)
            {
                UpdateTabPage();
            }
            UpdateChildrenLayout();
        }

        private void UpdateTabPage()
        {
            switch ((CurrentPage as NavigationPage)?.CurrentPage)
            {
                case null:
                    return;
                case PassportPageEuView _:
                    if (Children.Count == 3)
                    {
                        Children.RemoveAt(1);
                    }
                    break;
                case PassportPageDkView _:
                    if (Children.Count == 2)
                    {
                        NavigationPage infoPage = CreateTabNavigationPage(new PersonalPage(), TabPageLocationEnum.InfoPage);
                        Children.Insert(1, infoPage);
                    }
                    break;
                default:
                    break;
            }
        }

        void SetStatusBarColorDefault()
        {
            _navigationService.SetStatusBar(Color.White, Color.Black);
        }

        private NavigationPage CreateTabNavigationPage(Page rootPage, TabPageLocationEnum locationEnum)
        {
            return new NavigationPage(rootPage)
            {
                IconImageSource = _tabIcons[(int)locationEnum],
                Title = _tabTitle[(int)locationEnum]
            };
        }

        public async Task InitializePages()
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                foreach (Page page in Children)
                {
                    await ((BaseViewModel)page.InternalChildren[0].BindingContext).InitializeAsync(null);
                }
            });

            await _screenshotDetectionService.StartupShowScreenshotProtectionDialog();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(_scannerPageIsShowing)
            {
                _qrScannerPage.OnViewDisplayed();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if(_scannerPageIsShowing)
            {
                _qrScannerPage.OnViewHidden();
            }
        }

        protected override void OnCurrentPageChanged()
        {
            _navigationService.CurrentTab = Children.IndexOf(CurrentPage);

            base.OnCurrentPageChanged();
            SetStatusBarColorDefault();
            _brightnessService.UpdateBrightness();

            switch ((CurrentPage as NavigationPage)?.CurrentPage)
            {
                case null:
                    return;
                case QRScannerPage _:
                    _qrScannerPage.OnViewDisplayed();
                    _scannerPageIsShowing = true;
                    break;
                default:
                    SetStatusBarColorDefault();
                    if (_scannerPageIsShowing)
                    {
                        _scannerPageIsShowing = false;
                        _qrScannerPage.OnViewHidden();
                    }
                    break;
            }
        }
    }
}
