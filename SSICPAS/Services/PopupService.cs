using System.Threading.Tasks;
using AiForms.Dialogs;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Services.Interfaces;
using SSICPAS.Views.Elements;

namespace SSICPAS.Services
{
    public class PopupService : IPopupService
    {
        private const int ToastPopupTimeInMs = 3000;

        public Task ShowScanSuccessPopup(ITokenPayload payload)
        {
            return ScanSuccessResultPopup.ShowResult(payload);
        }
        public void ShowSuccessToast(string successText, int? durationInMs = null)
        {
            Toast.Instance.Show<CustomToast>(new { Title = successText, Duration = durationInMs ?? ToastPopupTimeInMs});
        }
    }
}