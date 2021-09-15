using System;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using SSICPAS.Configuration;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Services.Interface;
using SSICPAS.ViewModels.QrScannerViewModels;
using Xamarin.Forms;

namespace SSICPAS.Views.Elements
{
    public partial class ScanSuccessResultPopup : PopupPage
    {
        public static async Task ShowResult(ITokenPayload payload)
        {
            await PopupNavigation.Instance.PushAsync(new ScanSuccessResultPopup(payload));
        }

        public ScanSuccessResultPopup(ITokenPayload payload)
        {
            InitializeComponent();
            AndroidTalkbackAccessibilityWorkaround = true;

            ScanSuccessResultPopupViewModel viewModel = new ScanSuccessResultPopupViewModel();
            viewModel.InitializeAsync(payload);
            BindingContext = viewModel;
            
            ProgressBar.ProgressTo(1.0,
                Convert.ToUInt32(IoCContainer.Resolve<ISettingsService>().ScannerSuccessShownDurationMs), Easing.Linear);
        }

        protected override void OnDisappearing()
        {
            ((ScanSuccessResultPopupViewModel) BindingContext).Timer.Enabled = false;
            base.OnDisappearing();
        }
    }
}