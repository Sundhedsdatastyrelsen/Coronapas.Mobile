using SSICPAS.Core.Interfaces;
using UIKit;

namespace SSICPAS.iOS.Services
{
    public class IosVoiceOverManager: IVoiceOverManager
    {
        public bool IsVoiceOverEnabled => UIAccessibility.IsVoiceOverRunning;
    }
}