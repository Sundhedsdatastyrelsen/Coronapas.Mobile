using SSICPAS.Services;

namespace SSICPAS.ViewModels.Onboarding
{
    public class OnboardingInfo3ViewModel : OnboardingInfoModel
    {
        public OnboardingInfo3ViewModel()
        {
            InitText();
        }

        protected override void InitText()
        {
            Title = "ONBOARDING_INFO_3_TITLE".Translate();
            Content = "ONBOARDING_INFO_3_CONTENT".Translate();
        }
    }
}
