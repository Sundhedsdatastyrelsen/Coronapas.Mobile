using SSICPAS.Services.Scanner;

namespace SSICPAS.Services.Interfaces
{
    public interface IImagerScanner
    {
        void Enable();
        bool IsEnabled { get; }
        void Disable();
        void SetConfig(IScannerConfig config);
        void SetSelectedScanner(ScannerModel scannerModel);
        IImagerReceiver Receiver { get; }
    }
}