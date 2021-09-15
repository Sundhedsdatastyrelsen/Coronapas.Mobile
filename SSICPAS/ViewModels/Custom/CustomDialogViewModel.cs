using AiForms.Dialogs.Abstractions;
using SSICPAS.Enums;
using SSICPAS.ViewModels.Base;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.Custom
{
    public class CustomDialogViewModel : BaseViewModel
    {
        public IDialogNotifier Notifier { get; set; }
        public string Title { get; protected set; }
        public string Body { get; protected set; }
        public string OkButtonText { get; protected set; }
        public string CancelButtonText { get; }
        public bool IsCanceledOnTouchOutside { get; protected set; }
        public bool IsCancelTheFirstButton { get; protected set; }
        public StackOrientation ButtonStackOrientation { get; protected set; }       
        public bool CancelButtonVisible { get; }
        public bool ImageVisible { get; set; }
        public string ImageName { get; set; }
        public string ImageHeight { get; set; }
        public string ImageWidth { get; set; }

        public CustomDialogViewModel(string title, string message, bool isCanceledOnTouchOutside, StackOrientation buttonStackOrientation, bool isCancelTheFirstButton = true, string okButtonText = null,
            string cancelButtonText = null, DialogStyle style = DialogStyle.Default)
        {
            Title = title;
            Body = message;
            IsCanceledOnTouchOutside = isCanceledOnTouchOutside;
            IsCancelTheFirstButton = isCancelTheFirstButton;
            ButtonStackOrientation = buttonStackOrientation;
            OkButtonText = okButtonText;
            CancelButtonText = cancelButtonText;
            CancelButtonVisible = CancelButtonText != null;
            SetImageStatus(style);
        }

        public void Complete()
        {
            Notifier.Complete();
        }
        public void Cancel()
        {
            Notifier.Cancel();
        }

        private void SetImageStatus(DialogStyle style)
        {
            ImageVisible = style != DialogStyle.Default;
            switch (style)
            {
                case DialogStyle.Info:
                    ImageName = SSICPASImage.ExclamationMark.Image();
                    ImageHeight = "40";
                    ImageWidth = "40";
                    break;
                case DialogStyle.Notification:
                    ImageName = SSICPASImage.NotificationImage.Image();
                    ImageHeight = "64";
                    ImageWidth = "64";
                    break;
                case DialogStyle.InternetConnectivityIssue:
                    ImageName = SSICPASImage.InternetConnectivityIssueIcon.Image();
                    ImageHeight = "64";
                    ImageWidth = "64";
                    break;
                default:
                    ImageName = string.Empty;
                    break;
            }
        }
    }
}