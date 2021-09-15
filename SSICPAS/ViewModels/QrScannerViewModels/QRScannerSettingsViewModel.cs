using SSICPAS.Core.Data;
using SSICPAS.Models;
using SSICPAS.Services.Interfaces;
using SSICPAS.ViewModels.Menu;
using SSICPAS.Configuration;

namespace SSICPAS.ViewModels.QrScannerViewModels
{
    public class QRScannerSettingsViewModel: SettingsPageViewModel
    {
        public override bool FromLandingPage => true;

        public static QRScannerSettingsViewModel CreateQRScannerSettingsViewModel()
        {
            return new QRScannerSettingsViewModel(
                IoCContainer.Resolve<IUserService>(),
                IoCContainer.Resolve<ISecureStorageService<PinCodeBiometricsModel>>(),
                IoCContainer.Resolve<IDialogService>(),
                IoCContainer.Resolve<IPreferencesService>(),
                IoCContainer.Resolve<ITextService>()
            );
        }

        public QRScannerSettingsViewModel(IUserService userService, 
            ISecureStorageService<PinCodeBiometricsModel> pinCodeService, 
            IDialogService dialogService, 
            IPreferencesService preferencesService,
            ITextService textService): base(userService, pinCodeService, dialogService, textService, preferencesService)
        {
        }
    }
}
