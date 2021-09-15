using SSICPAS.Configuration;
using SSICPAS.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using SSICPAS.Views.Onboarding;
using SSICPAS.Views.Menu;
using SSICPAS.Services;

namespace SSICPAS.ViewModels.Onboarding
{
    public class OnboardingBaseViewModel : BaseViewModel
    {
        public ObservableCollection<BaseViewModel> ViewModels;
        public int CarouselPosition { get; set; }
        public string BackButton { get; set; }
        public string NextButton { get; set; }
        public string HelpButton { get; set; }
        
        public override ICommand BackCommand => new Command(async () => await ExecuteOnceAsync(async () =>
        {
            if (CarouselPosition > 0)
                GoToNextCarouselItem(-1);

            else
                await _navigationService.PopPage();
        }));

        public ICommand HelpButtonCommand => new Command(async () => await ExecuteOnceAsync(async () => await _navigationService.PushPage(new HelpPage())));

        public ICommand NextCommand => new Command(async () => await ExecuteOnceAsync(async () =>
        {
            if (CarouselPosition < ViewModels.Count - 1)
                GoToNextCarouselItem(1);
            else
                await _navigationService.PushPage(new AcceptDataPage());
        }));

        public OnboardingBaseViewModel()
        {
            CarouselPosition = 0;
            InitText();
            InitializeViewModels();
        }

        private void InitText()
        {
            BackButton = "BACK".Translate();
            NextButton = "NEXT".Translate();
            HelpButton = "HELP".Translate();
        }

        private void InitializeViewModels()
        {
            var infoVm = new OnboardingInfoViewModel();
            infoVm.ParentVM = this;

            var info2Vm = IoCContainer.Resolve<OnboardingInfo2ViewModel>();
            info2Vm.ParentVM = this;

            var info3Vm = IoCContainer.Resolve<OnboardingInfo3ViewModel>();
            info3Vm.ParentVM = this;

            var info4Vm = IoCContainer.Resolve<OnboardingInfo4ViewModel>();
            info3Vm.ParentVM = this;

            var info5Vm = IoCContainer.Resolve<OnboardingInfo5ViewModel>();
            info3Vm.ParentVM = this;

            ViewModels = new ObservableCollection<BaseViewModel>()
            {
                infoVm,
                info2Vm,
                info3Vm,
                info4Vm,
                info5Vm,
            };
        }

        public ObservableCollection<BaseViewModel> GetInfoViewModels()
        {
            return ViewModels;
        }

        public void GoToNextCarouselItem(int move = 1)
        {
            CarouselPosition += move;
            RaisePropertyChanged(() => CarouselPosition);
        }
    }
}
