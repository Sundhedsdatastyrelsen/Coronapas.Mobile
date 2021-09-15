using SSICPAS.Configuration;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Onboarding;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.Onboarding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MockLoginNemIdPage : ContentPage
    {
        public MockLoginNemIdPage()
        {
            InitializeComponent();
            BindingContext = LoginNemIdPageViewModel.CreateLoginNemIdPageViewModel();
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            IoCContainer.Resolve<INavigationService>().SetStatusBar(SSICPASColor.DefaultBackgroundColor.Color(), Color.Black);

        }
    }
}
