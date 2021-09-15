using SSICPAS.Configuration;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Onboarding;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.Onboarding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OnboardingBasePage : ContentPage
    {
        public OnboardingBasePage()
        {
            InitializeComponent();
            OnboardingBaseViewModel viewModel = new OnboardingBaseViewModel();
            BindingContext = viewModel;
            BaseCarouselView.ItemsSource = viewModel.GetInfoViewModels();
            BaseCarouselView.CurrentItemChanged += OnCurrentItemChanged;
        }

        protected override void OnAppearing()
        {
            IoCContainer.Resolve<INavigationService>().SetStatusBar(SSICPASColor.DefaultBackgroundColor.Color(), Color.Black);
            base.OnAppearing();
        }

        private void OnCurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {           
            if(e.CurrentItem is OnboardingInfoModel onboardingInfoViewModel)
            {
                onboardingInfoViewModel.ResetAnimation?.Execute(null);
            }
        }
    }
}