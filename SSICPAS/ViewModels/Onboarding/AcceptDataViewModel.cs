using Xamarin.Forms;
using System.Threading.Tasks;
using System.Windows.Input;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Core.WebServices;
using SSICPAS.Core.Auth;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.ViewModels.Base;
using SSICPAS.Views;
using SSICPAS.Views.Onboarding;
using Xamarin.Auth;
using Xamarin.Auth.Presenters;
using SSICPAS.Services.Navigation;
using Xamarin.Essentials;
using SSICPAS.Core.Logging;
using SSICPAS.Models;
using System.Diagnostics;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Services.Interface;

namespace SSICPAS.ViewModels.Onboarding
{
    public class AcceptDataViewModel : BaseViewModel
    {
        private IDateTimeService _dateTimeService;
        private ILoggingService _loggingService = IoCContainer.Resolve<ILoggingService>();
        private IDialogService _dialogService;
        IAuthenticationManager _authManager = IoCContainer.Resolve<IAuthenticationManager>();
        private OAuthLoginPresenter _loginPresenter = new OAuthLoginPresenter();
        private readonly ISecureStorageService<AuthData> _accessTokenStorageService;
        
        public string TitleText => "TERMS_AND_CONDITIONS_TITLE".Translate();
        public string AcceptText => "TERMS_AND_CONDITIONS_ACCEPTED".Translate();
        public string HelpText => "HELP".Translate();
        public string BackText => "BACK".Translate();
        public string NextText => "LOGIN_NEM_ID_HEADER".Translate();
        public string PageHelpText => "TERMS_AND_CONDITIONS_HELP".Translate();
        public string SwitchOnText => "ACCEPT_DATA_YES".Translate();
        public string SwitchOffText => "ACCEPT_DATA_NO".Translate();
        public string CheckBoxAccessibility => TermsAccepted ? "MY_PAGE_CHECKBOX".Translate() + " " + "TERMS_AND_CONDITIONS_CHECKBOX_CHECKED_ACCESSIBILLITY".Translate() : "MY_PAGE_CHECKBOX".Translate() + " " + "TERMS_AND_CONDITIONS_CHECKBOX_UNCHECKED_ACCESSIBILLITY".Translate();
        public string AcceptTextAccessibility => TermsAccepted ? "TERMS_AND_CONDITIONS_ACCEPTED_CHECKBOX".Translate() + " " + "TERMS_AND_CONDITIONS_CHECKBOX_CHECKED_ACCESSIBILLITY".Translate() : "TERMS_AND_CONDITIONS_ACCEPTED_CHECKBOX".Translate() + " " + "TERMS_AND_CONDITIONS_CHECKBOX_UNCHECKED_ACCESSIBILLITY".Translate();

        public string PrivacyPolicy => "ACCEPT_DATA_PRIVACY_POLICY".Translate();
        public string PrivacyPolicyUrl => "ACCEPT_DATA_PRIVACY_POLICY_URL".Translate();
        public string CookieNoticeUrl => "ACCEPT_DATA_DROPDOWN7_COOKIE_NOTICE_URL".Translate();
        public string CookieGuideUrl => "ACCEPT_DATA_DROPDOWN7_COOKIE_GUIDE_URL".Translate();
        public string AppPrivacyPolicy => "ACCEPT_DATA_DROPDOWN8_PRIVACY_POLICY_URL".Translate();

        public ICommand OpenLinkCommand => new Command(async () => await Launcher.OpenAsync(PrivacyPolicyUrl));

        public ICommand OpenCookieNoticeLinkCommand =>
            new Command(async () => await Launcher.OpenAsync(CookieNoticeUrl));

        public ICommand OpenCookieGuideLinkCommand => new Command(async () => await Launcher.OpenAsync(CookieGuideUrl));

        public ICommand OpenAppPrivacyPolicyLinkCommand =>
            new Command(async () => await Launcher.OpenAsync(AppPrivacyPolicy));

        public ICommand AcceptTermButton => new Command(async () => await OnApproveTermButtonClicked());

        public bool IsCheckBoxChecked { get; set; }
        public bool TermsAccepted { get; set; }

        public AcceptDataViewModel(IDialogService dialogService,
            ISecureStorageService<AuthData> accessTokenStorageService,
            IDateTimeService dateTimeService)
        {
            _dialogService = dialogService;
            _accessTokenStorageService = accessTokenStorageService;
            _dateTimeService = dateTimeService;
        }

        public void CheckBoxChanged(bool newValue)
        {
            TermsAccepted = newValue;
            OnPropertyChanged(nameof(CheckBoxAccessibility));
            OnPropertyChanged(nameof(AcceptTextAccessibility));
        }

        private async Task DisplayTermsNotApprovedDialog()
        {
            string title = "TERMS_AND_CONTITION_POPUP_TEXT".Translate();
            string accept = "TERMS_AND_CONTITION_POPUP_TEXT_OK".Translate();
            await _dialogService.ShowAlertAsync(title, null, true, true, StackOrientation.Horizontal, accept, null);
        }

        private async Task OnApproveTermButtonClicked()
        {
            if (TermsAccepted)
            {
                if (_settingsService.UseMockServices)
                {
                    await _navigationService.PushPage(new MockLoginNemIdPage());
                }
                else
                {
                    _authManager.Setup(OnAuthCompleted, OnAuthError);
                    _loginPresenter.Login(AuthenticationState.Authenticator);

                    await _navigationService.PushPage(new SplashPage(), false);
                }
            }
            else
            {
                await DisplayTermsNotApprovedDialog();
            }
        }

        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            Debug.Print(nameof(OnAuthCompleted));
            
            await CloseBrowserIOS(true);
            
            string errorLogPrefix =
                $"{nameof(AcceptDataViewModel)}.{nameof(OnAuthCompleted)}: Failed to authenticate user - ";
            _authManager.Cleanup();

            if ((e?.IsAuthenticated ?? false) && e.Account?.Properties != null &&
                e.Account.Properties.ContainsKey("access_token"))
            {
                //Access_token
                string token = e.Account?.Properties["access_token"];
                AuthData payload = _authManager.GetPayloadValidateJWTToken(token);

                if (payload == null)
                {
                    GoToErrorPageAndRemoveSplashPage(Errors.UnknownError, $"{errorLogPrefix}Payload was null");
                    return;
                }

                // Extract Refresh Token
                if (e.Account.Properties.TryGetValue("refresh_token", out string refreshToken))
                {
                    payload.RefreshToken = refreshToken;
                }
                else
                {
                    GoToErrorPageAndRemoveSplashPage(Errors.UnknownError, $"{errorLogPrefix}Missing refresh token");
                    return;
                }

                // Extract Access Token Expiration Time
                if (e.Account.Properties.TryGetValue("expires_in", out string atExpires))
                {
                    int.TryParse(atExpires, out int atExpiresSeconds);
                    if (atExpiresSeconds > 0)
                    {
                        payload.AccessTokenExpiration = _dateTimeService.Now.AddSeconds(atExpiresSeconds);
                    }
                }
                else
                {
                    GoToErrorPageAndRemoveSplashPage(Errors.UnknownError,
                        $"{errorLogPrefix}Missing Access Token Expiration Time");
                    return;
                }

                // Extract Refresh Token Expiration Time
                if (e.Account.Properties.TryGetValue("refresh_expires_in", out string rtExpires))
                {
                    int.TryParse(rtExpires, out int rtExpiresSeconds);
                    if (rtExpiresSeconds > 0)
                    {
                        payload.RefreshTokenExpiration = _dateTimeService.Now.AddSeconds(rtExpiresSeconds);
                    }
                }
                else
                {
                    GoToErrorPageAndRemoveSplashPage(Errors.UnknownError,
                        $"{errorLogPrefix}Missing Refresh Token Expiration Time");
                    return;
                }


                if (!payload.Validate())
                {
                    GoToErrorPageAndRemoveSplashPage(Errors.UnknownError, $"{errorLogPrefix}Invalid JWT");
                    return;
                }

                // Success
                await _accessTokenStorageService.SetSecureStorageAsync(CoreSecureStorageKeys.AUTH_TOKEN, payload);
                await IoCContainer.Resolve<IRestClient>().RegisterAccessTokenHeader();
                await _navigationService.PopPage(false);
                await _navigationService.PushPage(new RegisterPinCodeView(IoCContainer.Resolve<RegisterPinCodeViewModel>()));
            }
            else
            {
                Debug.Print("The user clicked back or something failed");
                await _navigationService.PopPage();
            }
        }

        async void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            lock (AuthenticationState.LockObject)
            {
                if (AuthenticationState.ErrorLogged)
                {
                    Debug.Print(nameof(OnAuthError) + $" ({e?.Message}) method called again and ignored");
                    return;
                }

                AuthenticationState.ErrorLogged = true;
            }

            Debug.Print(nameof(OnAuthError) + $" ({e?.Message})");
            
            await CloseBrowserIOS(false);
            
            _authManager.Cleanup();

            string logPrefix = $"{nameof(AcceptDataViewModel)}.{nameof(OnAuthError)}";
            GoToErrorPageAndRemoveSplashPage(Errors.UnknownError, $"{logPrefix}: Failed to authenticate user.",
                e?.Message +
                $". {(e?.Exception != null ? e?.Exception?.Message + " " + e?.Exception?.StackTrace : "")}");
        }

        void GoToErrorPageAndRemoveSplashPage(ErrorPageModel error, string errorLogMessage,
            string additionalLogInfo = null)
        {
            Device.BeginInvokeOnMainThread(async () => { await _navigationService.PopPage(false); });
            Device.BeginInvokeOnMainThread(async () => { await _navigationService.GoToErrorPage(error); });
            _loggingService.LogMessage(LogSeverity.WARNING, errorLogMessage, additionalLogInfo);
        }

        public async Task CloseBrowserIOS(bool animated)
        {
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                await IoCContainer.Resolve<IPlatformBrowserService>().CloseBrowser(animated);
            }
        }
        
        public bool IsVoiceOverEnabled => IoCContainer.Resolve<IVoiceOverManager>().IsVoiceOverEnabled;
    }
}