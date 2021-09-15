using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.Error
{
    public class ForceUpdateViewModel: BaseErrorViewModel
    {
        public override ICommand OkButtonCommand =>
            new Command(async () => await Launcher.OpenAsync(_settingsService.ForceUpdateLink));

        public override ICommand BackCommand => new Command(() => { });
    }
}
