using System.Windows.Input;
using SSICPAS.ViewModels.Base;
using SSICPAS.Views.Menu;
using Xamarin.Forms;
using SSICPAS.Configuration;
using SSICPAS.Core.Interfaces;

namespace SSICPAS.ViewModels.Menu
{
    public class MenuPageViewModel : BaseViewModel
    {
        public ICommand OpenAboutPage => new Command(async () => await ExecuteOnceAsync(async () => await _navigationService.PushPage(new AboutPage())));

        public ICommand OpenTermsAndUsePage => new Command(async () => await ExecuteOnceAsync(async () => await _navigationService.PushPage(new TermsPage())));

        public virtual ICommand OpenSettingsPage => new Command(async () => await ExecuteOnceAsync(async () => await _navigationService.PushPage(new SettingsPage(IoCContainer.Resolve<SettingsPageViewModel>()))));

        public ICommand OpenSupportPage => new Command(async () => await ExecuteOnceAsync(async () => await _navigationService.PushPage(new HelpPage())));

        public string VersionNumber
        {
            get
            {
                ISettingsService settings = _settingsService;
                return $"{settings.VersionString} ({settings.BuildString}) - {settings.EnvironmentDescription}";
            }
        }
        
        private bool _isLoggedIn = true;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged(nameof(IsLoggedIn));
            }
        }

    }
}