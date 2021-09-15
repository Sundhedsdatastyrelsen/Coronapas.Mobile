using SSICPAS.ViewModels.Base;
using System.Threading.Tasks;
using System.Windows.Input;
using SSICPAS.Models;
using SSICPAS.Services;

namespace SSICPAS.ViewModels.Error
{
    public class BaseErrorViewModel : BaseViewModel
    {
        public virtual ICommand OkButtonCommand => BackCommand;

        private string _titleMessage;
        private string _subtitleMessage;
        private string _imageSource;
        private string _buttonTitle;

        public string HelpButton { get; set; }
        
        public string ImageSource
        {
            get
            {
                return _imageSource;
            }
            set
            {
                _imageSource = value;
                RaisePropertyChanged(() => ImageSource);
            }
        }
       
        public string ErrorTitle
        {
            get
            {
                return _titleMessage;
            }
            set
            {
                _titleMessage = value;
                RaisePropertyChanged(() => ErrorTitle);
            }
        }

        public string ErrorSubtitle
        {
            get
            {
                return _subtitleMessage;
            }
            set
            {
                _subtitleMessage = value;
                RaisePropertyChanged(() => ErrorSubtitle);
            }
        }

        public string ButtonTitle
        {
            get
            {
                return _buttonTitle;
            }
            set
            {
                _buttonTitle = value;
                RaisePropertyChanged(() => ButtonTitle);
            }
        }

        public BaseErrorViewModel()
        {
            HelpButton = "HELP".Translate();
        }

        public override Task InitializeAsync(object navigationData)
        {
            if (navigationData is ErrorPageModel data)
            {
                if (!string.IsNullOrEmpty(data.Title)) ErrorTitle = data.Title;
                if (!string.IsNullOrEmpty(data.Message)) ErrorSubtitle = data.Message;
                if (!string.IsNullOrEmpty(data.Image)) ImageSource = data.Image;
                if (!string.IsNullOrEmpty(data.ButtonTitle)) ButtonTitle = data.ButtonTitle;
            }

            return base.InitializeAsync(navigationData);
        }
    }
}
