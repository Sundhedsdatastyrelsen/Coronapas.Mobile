
using SSICPAS.Services;

namespace SSICPAS.ViewModels.Onboarding
{
    public class OnboardingInfo2ViewModel : OnboardingInfoModel
    {
        public OnboardingInfo2ViewModel()
        {
            InitText();
        }

        protected override void InitText()
        {
            Title = "ONBOARDING_INFO_2_TITLE".Translate();
            Content = "ONBOARDING_INFO_2_CONTENT".Translate();
        }
    }
}
