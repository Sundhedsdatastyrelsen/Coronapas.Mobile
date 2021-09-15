using SSICPAS.iOS;
using SSICPAS.Services.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(StatusBarService))]
namespace SSICPAS.iOS
{
    public class StatusBarService : IStatusBarService
    {
        public StatusBarService()
        {
        }

        public void SetStatusBarColor(Color backgroundColor, Color textColor)
        {
            //This is handled automatically by setting BarTextColor on NavigationPage in Forms.
        }

    }
}
