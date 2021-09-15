using Xamarin.Forms;

namespace SSICPAS.Services.Interfaces
{
    public interface IBrightnessService
    {
        void UpdateBrightness(Page page = null);
        void SetDefaultBrightness();
        void SetBrightness(float factor);
        void ResetBrightness();
    }
}
