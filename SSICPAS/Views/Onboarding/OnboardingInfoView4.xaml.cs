using SSICPAS.Configuration;
using SSICPAS.ViewModels.Onboarding;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.Onboarding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OnboardingInfoView4 : ScrollView
    {
        public OnboardingInfoView4()
        {
            InitializeComponent();
            BindingContext = IoCContainer.Resolve<OnboardingInfo4ViewModel>();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is OnboardingInfoModel viewModel)
            {
                viewModel.ResetAnimation = new Command(() => ResetAnimation());
            }
        }

        public void ResetAnimation()
        {
            animation.Progress = 0;
            animation.Play();
        }
    }
}