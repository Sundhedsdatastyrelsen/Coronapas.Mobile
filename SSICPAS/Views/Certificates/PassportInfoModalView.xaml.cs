using SSICPAS.Data;
using SSICPAS.Enums;
using SSICPAS.ViewModels.Certificates;
using Xamarin.Forms;

namespace SSICPAS.Views.Certificates
{
    public partial class PassportInfoModalView : ContentSheetPageNoBackButtonOnIOS
    {
        private PassportInfoModalViewModel _viewModel;

        public PassportInfoModalView(FamilyPassportItemsViewModel viewModel, EuPassportType euPassportType)
        {
            BindingContext = _viewModel = PassportInfoModalViewModel.CreatePassportInfoModalViewModel();
            _viewModel.PassportItemsViewModel = viewModel;
            _viewModel.EuPassportType = euPassportType;
            InitializeComponent();
        }

        public PassportInfoModalView(FamilyPassportItemsViewModel viewModel, EuPassportType euPassportType, SinglePassportViewModel selectedPassport)
        {
            BindingContext = _viewModel = PassportInfoModalViewModel.CreatePassportInfoModalViewModel();
            _viewModel.PassportItemsViewModel = viewModel;
            _viewModel.EuPassportType = euPassportType;
            _viewModel.SelectedPassportViewModel = selectedPassport;
            InitializeComponent();
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            ((PassportInfoModalViewModel)BindingContext).StartGyroService();
            MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN, _viewModel.OnScreenshotTaken);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ((PassportInfoModalViewModel)BindingContext).StopGyroService();
            MessagingCenter.Unsubscribe<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN);
        }
        
    }
}
