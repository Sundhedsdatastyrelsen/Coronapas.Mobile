using System;
namespace SSICPAS.ViewModels
{
    public interface IScreenshotDetectorOnResultPage
    {
        public void OnScreenshotTaken(object sender);
        public void OnScreenshotTimerElapsed(object sender);
    }
}
