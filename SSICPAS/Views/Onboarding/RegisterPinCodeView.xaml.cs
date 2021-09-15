using SSICPAS.Configuration;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Onboarding;
using Xamarin.Forms;
using static SSICPAS.ViewModels.Custom.CustomPincodeBulletsViewModel;

namespace SSICPAS.Views.Onboarding
{
    public partial class RegisterPinCodeView : ContentPage
    {
        private readonly IStatusBarService _statusBarService = IoCContainer.Resolve<IStatusBarService>();
        private BasePinCodeViewModel _viewModel;
        public RegisterPinCodeView(BasePinCodeViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = _viewModel = viewModel;

            _viewModel.UpdatePincodeBullet += UpdatePincodeBullet;


        }

        protected override void OnAppearing()
        {
            if (_viewModel is RegisterPinCodeViewModel registerPinCodeViewModel)
            {
                registerPinCodeViewModel.ResetView();
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
