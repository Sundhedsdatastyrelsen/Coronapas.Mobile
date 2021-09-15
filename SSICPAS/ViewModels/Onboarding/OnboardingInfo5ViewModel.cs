using SSICPAS.Services;

namespace SSICPAS.ViewModels.Onboarding
{
    public class OnboardingInfo5ViewModel : OnboardingInfoModel
    {
        public OnboardingInfo5ViewModel()
        {
            InitText();
        }

        protected override void InitText()
        {
            Title = "ONBOARDING_INFO_5_TITLE".Translate();
            Content = "ONBOARDING_INFO_5_CONTENT".Translate();
        }
    }
}
