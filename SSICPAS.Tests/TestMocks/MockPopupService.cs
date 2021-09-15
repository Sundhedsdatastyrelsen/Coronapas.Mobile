using System.Threading.Tasks;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Services.Interfaces;

namespace SSICPAS.Tests.TestMocks
{
    public class MockPopupService : IPopupService
    {
        public Task ShowScanSuccessPopup(ITokenPayload payload)
        {
            return Task.FromResult(true);
        }

        public void ShowSuccessToast(string successText, int? durationInMs = null)
        {
        }
    }
}