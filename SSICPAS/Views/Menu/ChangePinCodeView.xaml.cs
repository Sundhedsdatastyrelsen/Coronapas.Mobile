using SSICPAS.Configuration;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Menu;
using SSICPAS.ViewModels.Onboarding;
using Xamarin.Forms;
using static SSICPAS.ViewModels.Custom.CustomPincodeBulletsViewModel;

namespace SSICPAS.Views.Menu
{
    public partial class ChangePinCodeView : ContentPage
    {
        private BasePinCodeViewModel _viewModel;

        public ChangePinCodeView(BasePinCodeViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = _viewModel = viewModel;

            _viewModel.UpdatePincodeBullet += UpdatePincodeBullet;

        }

        protected override void OnAppearing()
        {
            if (_viewModel is ChangePinCodeViewModel changePinCodeViewModel)
            {
                changePinCodeViewModel.ResetView();
            }
            base.OnAppearing();
            IoCContainer.Resolve<INavigationService>().SetStatusBar(SSICPASColor.DefaultBackgroundColor.Color(), Color.Black);
        }

        private void UpdatePincodeBullet(PinCodeViewStatusEnum status)
        {
            PinCodeBulletView.ViewModel.UpdateBullets(status);
        }
    }
}
