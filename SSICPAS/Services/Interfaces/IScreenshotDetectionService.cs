using System.Threading.Tasks;

namespace SSICPAS.Services.Interfaces
{
    public interface IScreenshotDetectionService
    {
        Task ShowPassportPageScreenshotProtectionDialog(int lockForSeconds = 0, bool setTimestamp = true);
        Task ShowResultPageScreenshotProtectionDialog(int lockForSeconds = 0, bool setTimestamp = true);
        Task ShowScannerPageScreenshotProtectionDialog(int lockForSeconds = 0, bool setTimeStamp = true);
        Task StartupShowScreenshotProtectionDialog(bool scannerOnly = false);
    }
}
