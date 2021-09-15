using SSICPAS.Core.Interfaces;

namespace SSICPAS.iOS.Services
{
    public class IosCustomOAuthNativeUIFlagResolver : ICustomOAuthNativeUIFlagResolver
    {
        public bool ShouldEnableNativeUI()
        {
            return true;
        }
    }
}
