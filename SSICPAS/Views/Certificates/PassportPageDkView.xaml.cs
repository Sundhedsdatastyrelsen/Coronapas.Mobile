using SSICPAS.Configuration;
using SSICPAS.Data;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Certificates;
using Xamarin.Forms;

namespace SSICPAS.Views.Certificates
{
    public partial class PassportPageDkView : ContentPage
    {
        private PassportPageViewModel viewModel;

        public PassportPageDkView(PassportPageViewModel viewModel = null)
        {
            InitializeComponent();
            this.viewModel = viewModel ?? PassportPageViewModel.CreatePassportPageViewModel();
            BindingContext = this.viewModel;
            AwaitingPassportView.BindingContext = BindingContext;
            ExpiredPassportView.BindingContext = BindingContext;

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            IoCContainer.Resolve<INavigationService>().SetStatusBar(SSICPASColor.NavigationHeaderBackgroundColor.Color(), Color.Black);

            await viewModel.FetchPassport();
            ((PassportPageViewModel)BindingContext).StartGyroService();
            MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN, viewModel.OnScreenshotTaken);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ((PassportPageViewModel)BindingContext).StopGyroService();
            MessagingCenter.Unsubscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN);
        }
    }
}
