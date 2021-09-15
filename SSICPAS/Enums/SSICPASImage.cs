using System;
using Xamarin.Forms;

namespace SSICPAS.Enums
{
    public enum SSICPASImage
    {
        ErrorMaintenance,
        ErrorQueue,
        ErrorUnknown,
        ErrorLock,
        BottombarPassport,
        BottombarScan,
        BottombarInfo,
        ToastTick,
        Covid19VaccineIcon,
        Covid19NegativeTestIcon,
        Covid19RecoveryIcon,
        ExclamationMark,
        ExclamationIconBlue,
        InternationalPassportIcon,
        ValidPassportIcon,
        PassportNotSelected,
        PassportSelected,
        PassportDKIconSelected,
        PassportDKIcon,
        PassportEUIconSelected,
        PassportEUIcon,
        NotificationImage,
        InternetConnectivityIssueIcon
    }

    public static class SSICPASSImageExtensions
    {
        public static string Image(this SSICPASImage img)
        {
            string imageString = Enum.GetName(typeof(SSICPASImage), img);
            string image = Application.Current.Resources[imageString] as string;
            return image;
        }
    }
}