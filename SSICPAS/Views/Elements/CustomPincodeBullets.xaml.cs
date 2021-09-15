using SSICPAS.ViewModels.Custom;
using SSICPAS.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomPincodeBullets : ContentView
    {
        public CustomPincodeBulletsViewModel ViewModel;
        public CustomPincodeBullets()
        {
            InitializeComponent();

            BindingContext = ViewModel = new CustomPincodeBulletsViewModel();

            ViewModel.BeginShake += BeginShake;
        }

        private void BeginShake()
        {
            AnimationUtils.ShakeAnimation(ShakingAnimationGrid);
        }
    }
}
