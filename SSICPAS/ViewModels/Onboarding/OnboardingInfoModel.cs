using SSICPAS.ViewModels.Base;
using System.Windows.Input;

namespace SSICPAS.ViewModels.Onboarding
{
    public abstract class OnboardingInfoModel : BaseViewModel
    {
        public OnboardingBaseViewModel ParentVM { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public ICommand ResetAnimation { get; set; }

        protected virtual void InitText() { }
    }
}
