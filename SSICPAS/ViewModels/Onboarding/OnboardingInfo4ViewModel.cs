using SSICPAS.Services;

namespace SSICPAS.ViewModels.Onboarding
{
    public class OnboardingInfo4ViewModel : OnboardingInfoModel
    {
        public OnboardingInfo4ViewModel()
        {
            InitText();
        }

        protected override void InitText()
        {
            Title = "ONBOARDING_INFO_4_TITLE".Translate();
            Content = "ONBOARDING_INFO_4_CONTENT".Translate();
        }
    }
}