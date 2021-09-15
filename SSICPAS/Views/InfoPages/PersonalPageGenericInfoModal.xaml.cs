using System;
using System.Threading.Tasks;
using SSICPAS.Enums;
using SSICPAS.ViewModels.Certificates;
using SSICPAS.ViewModels.InfoPages;
using Xamarin.Forms;

namespace SSICPAS.Views.InfoPages
{
    public partial class PersonalPageGenericInfoModal : ContentSheetPageNoBackButtonOnIOS
    {
        private PersonalPageGenericInfoModalViewModel _viewModel;

        public PersonalPageGenericInfoModal(SinglePassportViewModel viewModel, EuPassportType euPassportType)
        {
            InitializeComponent();
            BindingContext = _viewModel = new PersonalPageGenericInfoModalViewModel(viewModel, euPassportType);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.UpdateView();
        }

        void ScrollView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            var transY = Convert.ToInt32(InfoHeader.TranslationY);
            if (transY == 0 && e.VerticalDelta > 15)
            {
                var trans = InfoHeader.Height;
                InfoHeaderSmall.IsVisible = true;
                iOSHeader.HeightRequest = 50;

                Task.WhenAll(
                    InfoHeader.TranslateTo(0, -trans, 20, Easing.BounceIn),
                    InfoHeader.FadeTo(0, 0),
                    InfoHeaderSmall.TranslateTo(0, 0, 20, Easing.BounceOut),
                    InfoHeaderSmall.FadeTo(1, 0));
            }
            else if (transY != 0 && e.VerticalDelta <= 0)
            {
                var trans2 = InfoHeaderSmall.Height;
                iOSHeader.HeightRequest = -1;
                Task.WhenAll(
                    InfoHeader.TranslateTo(0, 0, 20, Easing.BounceOut),
                    InfoHeader.FadeTo(1, 0),
                    InfoHeaderSmall.TranslateTo(0, -trans2, 20, Easing.BounceIn),
                    InfoHeaderSmall.FadeTo(0, 0));
            }
        }
    }
}
