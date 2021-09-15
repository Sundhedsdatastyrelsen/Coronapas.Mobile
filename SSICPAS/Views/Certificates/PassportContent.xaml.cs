using Xamarin.Forms;
using SSICPAS.ViewModels.Certificates;


namespace SSICPAS.Views.Certificates
{
    public partial class PassportContent : ContentPage
    {
        public PassportContent()
        {
            InitializeComponent();
            BindingContext = PassportContentViewModel.CreatePassportContentViewModel();
        }
    }
}