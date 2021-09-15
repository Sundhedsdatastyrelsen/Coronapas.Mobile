using SSICPAS.Utils;
using SSICPAS.ViewModels.Onboarding;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.Onboarding
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OnboardingInfoView : ScrollView
    {
        public OnboardingInfoView()
        {
            InitializeComponent();
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
