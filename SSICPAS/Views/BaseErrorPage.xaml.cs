using SSICPAS.Configuration;
using SSICPAS.Enums;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Error;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BaseErrorPage : ContentPage
    {
        public ErrorPageType Type { get; set; }

        public BaseErrorPage(ErrorPageType type = ErrorPageType.Default)
        {
            Type = type;
            InitializeComponent();
          
            
            switch (type) {
                case ErrorPageType.NemID:
                    BindingContext = NemIDErrorViewModel.CreateNemIDErrorViewModel();
                    break;
                case ErrorPageType.ForceUpdate:
                    BindingContext = new ForceUpdateViewModel();
                    break;
                default:
                    BindingContext = new BaseErrorViewModel();
                    break;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            IoCContainer.Resolve<INavigationService>().SetStatusBar(SSICPASColor.DefaultBackgroundColor.Color(), Color.Black);
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }
    }
}