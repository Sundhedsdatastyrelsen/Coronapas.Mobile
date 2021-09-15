using System.Threading.Tasks;
using SSICPAS.Core.Services.Interface;

namespace SSICPAS.Services.Interfaces
{
    public interface IPopupService
    {
        Task ShowScanSuccessPopup(ITokenPayload payload);
        void ShowSuccessToast(string successText, int? durationInMs = null);
    }
}