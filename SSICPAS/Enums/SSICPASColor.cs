using System;
using System.Linq;
using SkiaSharp;
using Xamarin.Forms;

namespace SSICPAS.Utils
{
    public enum SSICPASColor
    {
        SSILinkColor,
        SSILightTextColor,
        SSIContentTextColor,
        SSITitleTextColor,
        SSIBaseTextColor,
        SSILandingPageStatusBarColor,
        DefaultBackgroundColor,
        NavigationHeaderBackgroundColor,

        SSIButtonBlue,
        SSIButtonDisable,
        SSILoginButtonDisable,

        LandingPageColorStart,
        SuccessBorderColor,
        InvalidBorderColor,
        ExpiredBorderColor,

        LandingPageColorEnd,
    }

    public enum SSICPASColorGradient
    {
        //Gradient
        MainPageCrownGradient,
        MainPageStatusGradientGreen,
        MainPageStatusGradientBlue,
        ClockGradient
    }

    public static class SSICPASColorExtensions
    {
        public static Color Color(this SSICPASColor color)
        {
            string colourString = Enum.GetName(typeof(SSICPASColor), color);
            Color? colour = Application.Current.Resources[colourString] as Color?;
            return colour ?? Xamarin.Forms.Color.White;
        }

        public static SKColor[] Gradient (this SSICPASColorGradient color){
            string gradientString = Enum.GetName(typeof(SSICPASColorGradient), color);
            Color[]? gradient = Application.Current.Resources[gradientString] as Color[];
            return gradient?.Select(x=>SKColor.Parse(x.ToHex())).ToArray() ?? new SKColor[] { };
        }
    }
}
