using SSICPAS.Configuration;
using SSICPAS.ViewModels.InfoPages;
using Xamarin.Forms;

namespace SSICPAS.Views.InfoPages
{
    public partial class PersonalPage : ContentPage
    {
        private readonly PersonalPageViewModel viewModel;
        public PersonalPage()
        {
            InitializeComponent();
            this.viewModel = viewModel ?? IoCContainer.Resolve<PersonalPageViewModel>();
            BindingContext = this.viewModel;
        }
    }
}