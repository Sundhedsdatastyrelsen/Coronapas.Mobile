using System.Windows.Input;
using SSICPAS.ViewModels.Menu;
using SSICPAS.Views.Menu;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.QrScannerViewModels
{
    public class QRScannerMenuViewModel: MenuPageViewModel
    {
        public QRScannerMenuViewModel()
        {
            IsLoggedIn = false;
        }

        public override ICommand OpenSettingsPage => new Command(
            async () => await ExecuteOnceAsync(
                async () => await _navigationService.PushPage(
                    new SettingsPage(QRScannerSettingsViewModel.CreateQRScannerSettingsViewModel()))));
    }
}
