using System;
using Xamarin.Forms;

namespace SSICPAS.Enums
{
    public enum SSICPASLottieAnimation
    {
        ValidDKPassport,
        ValidEUPassport,
    }

    public static class SSICPASSLottieAnimationExtensions
    {
        public static string GetFilePath(this SSICPASLottieAnimation animation)
        {
            string animationString = Enum.GetName(typeof(SSICPASLottieAnimation), animation);
            string animationFilePath = Application.Current.Resources[animationString] as string;
            return animationFilePath;
        }
    }
}
