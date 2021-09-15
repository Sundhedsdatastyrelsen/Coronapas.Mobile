using SSICPAS.Services.Interfaces;

namespace SSICPAS.Tests.TestMocks
{
    public class MockScannerFactory: IScannerFactory
    {
        public MockScannerFactory()
        {
        }

        public IImagerScanner GetAvailableScanner()
        {
            return null;
        }

        public IScannerConfig GetScannerConfig()
        {
            return null;
        }
    }
}
