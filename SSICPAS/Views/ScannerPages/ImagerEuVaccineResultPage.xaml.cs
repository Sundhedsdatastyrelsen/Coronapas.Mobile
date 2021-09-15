using System;
using System.Threading.Tasks;
using SSICPAS.Configuration;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Data;
using SSICPAS.Enums;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.QrScannerViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.ScannerPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImagerEuVaccineResultPage : ContentPage, IScanResultView, IExtraOrientationSupport
    {
        public ImagerEuVaccineResultPage(ITokenPayload payload, EuPassportType passportType)
        {
            InitializeComponent();
            ImagerEuVaccineResultViewModel viewModel = new ImagerEuVaccineResultViewModel(payload, passportType);

            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            ((ImagerEuVaccineResultViewModel) BindingContext).OnAttachTimer();
            IoCContainer.Resolve<INavigationService>().SetStatusBar(SSICPASColor.NavigationHeaderBackgroundColor.Color(), Color.Black);
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            ((ImagerEuVaccineResultViewModel) BindingContext).OnDetachTimer();
            base.OnDisappearing();
        }

        void ScrollView_OnScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            var transY = Convert.ToInt32(BannerView.TranslationY + IdentityView.TranslationY);
            var delta = e.VerticalDelta;
            if (transY == 0 && e.VerticalDelta > 230)
            {
                var trans = BannerView.Height + IdentityView.Height;
                IdentityViewSmall.IsVisible = true;
                HeaderView.HeightRequest = 60;
                Task.WhenAll(
                    BannerView.TranslateTo(0, -trans, 20, Easing.BounceIn),
                    BannerView.FadeTo(0, 0),
                    IdentityView.TranslateTo(0, -trans, 20, Easing.BounceIn),
                    IdentityView.FadeTo(0, 0),
                    HeaderTitle.TranslateTo(0, -trans, 20, Easing.BounceIn),
                    HeaderTitle.FadeTo(0, 0),
                    IdentityViewSmall.TranslateTo(0, 0, 20, Easing.BounceOut),
                    IdentityViewSmall.FadeTo(1, 0));
            }
            else if (transY != 0 && e.VerticalDelta <= 230)
            {
                var trans2 = IdentityViewSmall.Height;
                IdentityViewSmall.IsVisible = false;
                HeaderView.HeightRequest = -1;
                 Task.WhenAll(
                     BannerView.TranslateTo(0, 0, 20, Easing.BounceOut),
                     BannerView.FadeTo(1, 0),
                     IdentityView.TranslateTo(0, 0, 20, Easing.BounceOut),
                     IdentityView.FadeTo(1, 0),
                     HeaderTitle.TranslateTo(0, 0, 20, Easing.BounceOut),
                     HeaderTitle.FadeTo(1, 0),
                     IdentityViewSmall.TranslateTo(0, -trans2, 20, Easing.BounceIn),
                     IdentityViewSmall.FadeTo(0, 0));
            }
        }

        public SupportedOrientation SupportedOrientation => SupportedOrientation.SensorPortrait;
    }
}