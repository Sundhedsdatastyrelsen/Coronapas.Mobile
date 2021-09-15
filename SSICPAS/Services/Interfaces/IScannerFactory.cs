#nullable enable
namespace SSICPAS.Services.Interfaces
{
    public interface IScannerFactory
    {
        IImagerScanner? GetAvailableScanner();
        IScannerConfig? GetScannerConfig();
    }
}