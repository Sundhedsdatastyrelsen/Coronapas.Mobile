using System;
using System.Windows.Input;
using SSICPAS.Configuration;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.ViewModels.Base;
using SSICPAS.Views.Onboarding;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.Onboarding
{
    public class LoginNemIdPageViewModel : BaseViewModel
    {
        public ICommand Next => new Command(async () => await ExecuteOnceAsync(async () =>
        {
            if (!String.IsNullOrWhiteSpace(UserName) && !String.IsNullOrWhiteSpace(Password))
            {
                await _navigationService.PushPage(
                    new RegisterPinCodeView(RegisterPinCodeViewModel.CreateRegisterPinCodeViewModel()));
            }
        }));

        public string LoginNemIdHeaderText { get; set; }
        public string LoginNemIdSubHeaderText { get; set; }
        public string LoginNemIdCovidPassText { get; set; }
        public string LoginNemIdUserIdText { get; set; }
        public string LoginNemIdForgotPasswordText { get; set; }
        public string LoginNemIdPasswordText { get; set; }
        public string LoginNemIdNextText { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        INavigationTaskManager _navigationTaskManager;

        public static LoginNemIdPageViewModel CreateLoginNemIdPageViewModel()
        {
            return new LoginNemIdPageViewModel(
                IoCContainer.Resolve<INavigationTaskManager>()
            );
        }

        public LoginNemIdPageViewModel(INavigationTaskManager navigationTaskManager)
        {
            _navigationTaskManager = navigationTaskManager;
            InitText();
        }

        private void InitText()
        {
            LoginNemIdHeaderText = "LOGIN_NEM_ID_HEADER".Translate();
            LoginNemIdSubHeaderText = "LOGIN_NEM_ID_SUB_HEADER".Translate();
            LoginNemIdCovidPassText = "LOGIN_NEM_ID_COVID_PASS_TEXT".Translate();
            LoginNemIdUserIdText = "LOGIN_NEM_ID_USER_ID_TEXT".Translate();
            LoginNemIdForgotPasswordText = "LOGIN_NEM_ID_FORGOT_PASSWORD_TEXT".Translate();
            LoginNemIdPasswordText = "LOGIN_NEM_ID_PASSWORD_TEXT".Translate();
            LoginNemIdNextText = "LOGIN_NEM_ID_NEXT_TEXT".Translate();
        }
    }
}
