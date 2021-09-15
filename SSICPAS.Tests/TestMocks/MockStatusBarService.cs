using SSICPAS.Services.Interfaces;
using Xamarin.Forms;

namespace SSICPAS.Tests.TestMocks
{
    public class MockStatusBarService : IStatusBarService
    {
        public MockStatusBarService()
        {
        }

        public void RenderQrScannerStatusBarColor(bool isLandingPage)
        {
        }

        public void SetStatusBarColor(Color color)
        {
        }

        public void SetStatusBarColor(Color backgroundColor, Color textColor)
        {
        }
    }
}
