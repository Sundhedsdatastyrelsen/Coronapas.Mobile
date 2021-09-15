using SSICPAS.ViewModels.Certificates;
using Xamarin.Forms;

namespace SSICPAS.Views.Certificates
{
    public partial class InfoBorderControlPage : ContentPage
    {
        public InfoBorderControlPage()
        {
            InitializeComponent();
            BindingContext = InfoBorderControlViewModel.CreateInfoBorderControlViewModel();
        }
    }
}