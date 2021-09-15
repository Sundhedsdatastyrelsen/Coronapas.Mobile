#nullable enable
using SSICPAS.Services.Interfaces;

namespace SSICPAS.iOS.Services
{
    public class IosScannerFactory: IScannerFactory
    {
        public IImagerScanner? GetAvailableScanner()
        {
            return null; // currently no support for scanner in ios devices
        }

        public IScannerConfig? GetScannerConfig()
        {
            return null;
        }
    }
}