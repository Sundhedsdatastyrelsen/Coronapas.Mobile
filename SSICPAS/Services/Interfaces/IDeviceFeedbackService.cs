using Xamarin.Essentials;

namespace SSICPAS.Services.Interfaces
{
    public interface IDeviceFeedbackService
    {
        void Vibrate();
        void Vibrate(double durationMs);
        void PlaySound(string fileNameWithExtension);
        void PerformHapticFeedback(HapticFeedbackType type);
    }
}