using SSICPAS.Configuration;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Menu;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
            BindingContext = IoCContainer.Resolve<AboutPageViewModel>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            IoCContainer.Resolve<INavigationService>().SetStatusBar(SSICPASColor.NavigationHeaderBackgroundColor.Color(), Color.Black);
        }
    }
}