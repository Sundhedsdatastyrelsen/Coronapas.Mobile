using System;
using System.Collections.Generic;
using SSICPAS.Configuration;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Menu;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HelpPage : ContentPage
    {
        public HelpPage()
        {
            InitializeComponent();
            BindingContext = IoCContainer.Resolve<MenuHelpPageViewModel>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            IoCContainer.Resolve<INavigationService>().SetStatusBar(SSICPASColor.NavigationHeaderBackgroundColor.Color(), Color.Black);
        }

        private void HelpEmail_OnTapped(object sender, EventArgs e)
        {
            ((MenuHelpPageViewModel) BindingContext).SendEmail(string.Empty, String.Empty, new List<string>());
        }
        
        private void HelpPhone_OnTapped(object sender, EventArgs e)
        {
            ((MenuHelpPageViewModel) BindingContext).PlacePhoneCall();
        }
    }
}