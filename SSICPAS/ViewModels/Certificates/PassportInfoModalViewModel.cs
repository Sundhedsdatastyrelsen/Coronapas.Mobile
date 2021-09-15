using SSICPAS.Enums;
using SSICPAS.Services.Interfaces;
using SSICPAS.ViewModels.Base;
using SSICPAS.Configuration;

namespace SSICPAS.ViewModels.Certificates
{
    public class PassportInfoModalViewModel : ContentSheetPageNoBackButtonOnIOSViewModel
    {
        public FamilyPassportItemsViewModel PassportItemsViewModel { get; set; }
        public EuPassportType EuPassportType { get; set; }
        public SinglePassportViewModel SelectedPassportViewModel { get; set; }
        
        private readonly IGyroscopeService _gyroscopeService;
        private readonly IScreenshotDetectionService _screenshotDetectionService;

        public static PassportInfoModalViewModel CreatePassportInfoModalViewModel()
        {
            return new PassportInfoModalViewModel(
                IoCContainer.Resolve<IGyroscopeService>(),
                IoCContainer.Resolve<IScreenshotDetectionService>()
            );
        }

        public PassportInfoModalViewModel(IGyroscopeService gyroscopeService, IScreenshotDetectionService screenshotDetectionService)
        {
            _gyroscopeService = gyroscopeService;
            _screenshotDetectionService = screenshotDetectionService;
        }

        public void StartGyroService()
        {
            _gyroscopeService.TurnOnOrientation();
        }
        public void StopGyroService()
        {
            _gyroscopeService.TurnOffOrientation();
        }

        public async void OnScreenshotTaken(object sender)
        {
            await _screenshotDetectionService.ShowPassportPageScreenshotProtectionDialog();
        }
    }
}
