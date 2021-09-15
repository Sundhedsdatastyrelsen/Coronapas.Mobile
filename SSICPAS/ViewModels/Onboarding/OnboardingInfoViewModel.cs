using SSICPAS.Services;

namespace SSICPAS.ViewModels.Onboarding
{
    public class OnboardingInfoViewModel : OnboardingInfoModel
    {
        public OnboardingInfoViewModel()
        {
            InitText();
        }

        protected override void InitText()
        {
            Title = "ONBOARDING_INFO_1_TITLE".Translate();
            Content = "ONBOARDING_INFO_1_CONTENT".Translate();
        }
    }
}
