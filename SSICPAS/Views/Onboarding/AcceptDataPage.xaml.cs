using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SSICPAS.Configuration;
using static SSICPAS.Views.Elements.CustomConsentSwitch;
using SSICPAS.Utils;

namespace SSICPAS.Views.Onboarding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AcceptDataPage : ContentPage
    {
        private readonly ViewModels.Onboarding.AcceptDataViewModel _viewModel;
        public AcceptDataPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = IoCContainer.Resolve<ViewModels.Onboarding.AcceptDataViewModel>();
            DisableLoginButton();
        }

        void ScrollView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            var transY = Convert.ToInt32(ConsentHeader.TranslationY);
            var scrollingSpace = ScrollView.ContentSize.Height - ScrollView.Height - 50;

            if (transY == 0 && e.VerticalDelta > 20)
            {
                ConsentHeaderText.IsVisible = true;
                ConsentHeader.HasShadow = true;

                Task.WhenAll(
                    ConsentHeaderText.TranslateTo(0, 0, 20, Easing.BounceIn),
                    ConsentHeaderText.FadeTo(1, 0));
            }
            else
            {
                ConsentHeaderText.IsVisible = false;
                ConsentHeader.HasShadow = false;

                Task.WhenAll(
                    ConsentHeaderText.TranslateTo(0, 0, 20, Easing.BounceOut),
                    ConsentHeaderText.FadeTo(0, 0));
            }
            if (scrollingSpace < e.VerticalDelta)
            {
                AcceptFrame.HasShadow = false;
                 
            }
            if (scrollingSpace > e.VerticalDelta)
            {
                AcceptFrame.HasShadow = false;
               
            }
        }

        void CheckBox_IsCheckedChanged(object sender, TappedEventArgs e)
        {
            bool isAccepted = e.Parameter.Equals(true);
            _viewModel.CheckBoxChanged(isAccepted);

            if (isAccepted)
            {
                EnableLoginButton();
            }
            else
            {
                DisableLoginButton();
            }
        }

        void DisableLoginButton()
        {
            LogInButton.IsEnabled = false;
            LogInButton.BackgroundColor = SSICPASColor.SSILoginButtonDisable.Color();
        }

        void EnableLoginButton()
        {
            LogInButton.IsEnabled = true;
            LogInButton.BackgroundColor = SSICPASColor.SSIButtonBlue.Color();
        }

        void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            CheckBox.IsChecked = !CheckBox.IsChecked;
        }

    }
}