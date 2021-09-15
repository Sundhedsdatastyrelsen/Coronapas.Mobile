using System;
using SSICPAS.Configuration;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Menu;
using SSICPAS.Views.Elements;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage(SettingsPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            IoCContainer.Resolve<INavigationService>().SetStatusBar(SSICPASColor.NavigationHeaderBackgroundColor.Color(), Color.Black);
        }

        private void VibrationConsentSwitch_OnSelectionChanged(object sender, EventArgs e)
        {
            ((SettingsPageViewModel) BindingContext).VibrationSettingChanged((CustomConsentSwitch.CustomConsentSwitchEventArgs) e);
        }

        private void SoundConsentSwitch_OnSelectionChanged(object sender, EventArgs e)
        {
            ((SettingsPageViewModel) BindingContext).SoundSettingChanged((CustomConsentSwitch.CustomConsentSwitchEventArgs) e);
        }

        private void BiometricSwitch_OnSelectionChanged(object sender, EventArgs e)
        {
            ((SettingsPageViewModel) BindingContext).BiometricSettingChanged((CustomConsentSwitch.CustomConsentSwitchEventArgs) e);
        }
    }
}