using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using SSICPAS.Core.Logging;
using SSICPAS.Services;
using SSICPAS.ViewModels.Base;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.Menu
{
    public class MenuHelpPageViewModel : BaseViewModel
    {
        private LoggingService _loggingService;
        
        public string HelpUrl => "HELP_URL".Translate();
        public string HelpEmail => "HELP_MAIL".Translate();
        public string HelpPhoneNumber => "HELP_PHONE".Translate();

        public ICommand OpenLinkCommand => new Command(async () => await Launcher.OpenAsync(HelpUrl));
        
        public async Task SendEmail(string subject, string body, List<string> recipients)
        {
            recipients.Add(HelpEmail);
            try
            {
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = recipients,
                 };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                _loggingService.LogException(LogSeverity.WARNING, fbsEx, $"Email is not supported on this device");
            }
            catch (Exception ex)
            {
                _loggingService.LogException(LogSeverity.ERROR, ex);
            }
        }

        public void PlacePhoneCall()
        {
            var number = HelpPhoneNumber.Trim();
            try
            {
                PhoneDialer.Open(number);
            }
            catch (FeatureNotSupportedException anEx)
            {
                _loggingService.LogException(LogSeverity.WARNING, anEx, $"Phonecall is not supported on this device");
            }
            catch (Exception ex)
            { 
                _loggingService.LogException(LogSeverity.ERROR, ex);
            }
        }
    }
}