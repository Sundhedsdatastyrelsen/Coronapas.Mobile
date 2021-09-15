using System.Windows.Input;
using SSICPAS.Configuration;
using SSICPAS.Services.Interfaces;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.Error
{
    public class NemIDErrorViewModel : BaseErrorViewModel
    {
        IUserService _userService;
        
        public override ICommand OkButtonCommand => new Command(() => _userService.UserLogoutAsync(false));

        public override ICommand BackCommand => new Command(() => { });

        public static NemIDErrorViewModel CreateNemIDErrorViewModel()
        {
            return new NemIDErrorViewModel(
                IoCContainer.Resolve<IUserService>()
            );
        }

        public NemIDErrorViewModel(IUserService userService)
        {
            _userService = userService;
        }
    }
}
