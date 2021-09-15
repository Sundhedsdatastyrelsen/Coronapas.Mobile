using SSICPAS.Configuration;
using SSICPAS.Data;
using SSICPAS.Services.Interfaces;
using SSICPAS.ViewModels.Certificates;
using SSICPAS.Views.Certificates;
using Xamarin.Forms;

namespace SSICPAS.Services
{
    public abstract class BaseBrightnessService : IBrightnessService
    {
        public BaseBrightnessService()
        {
            MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.BACK_FROM_BACKGROUND, OnShouldUpdateBrightness);
            MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.PASSPORT_UPDATED, OnShouldUpdateBrightness);
        }

        public abstract void SetDefaultBrightness();
        public abstract void SetBrightness(float factor);
        public abstract void ResetBrightness();

        private void OnShouldUpdateBrightness(object obj)
        {
            UpdateBrightness();
        }

        public void UpdateBrightness(Page page = null)
        {
            Page currentPage = page ?? IoCContainer.Resolve<INavigationService>().FindCurrentPage(true);

            if (ShouldShowFullBrightnessOnPage(currentPage))
            {
                Device.BeginInvokeOnMainThread(() => { SetBrightness(1); });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() => { ResetBrightness(); });
            }
        }

        public bool ShouldShowFullBrightnessOnPage(Page page)
        {
            bool nationalQRCodeIsDisplayed = page is PassportPageDkView
                && ((page.BindingContext as PassportPageViewModel)?.IsPassportAvailable ?? false);
            bool euQRCodeIsDisplayedInModal = page is PassportInfoModalView;
            bool euQRCodeIsDisplayedInEUBaseView = page is PassportPageEuView
                && (((page.BindingContext as PassportPageViewModel)?.IsPassportAvailable ?? false)
                && !((page.BindingContext as PassportPageViewModel)?.IsMoreThanOneAvailable ?? false)); 

            return nationalQRCodeIsDisplayed || euQRCodeIsDisplayedInModal || euQRCodeIsDisplayedInEUBaseView;
        }
    }
}
