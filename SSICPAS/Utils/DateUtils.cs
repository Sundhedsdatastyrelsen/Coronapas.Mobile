using System;
using System.Globalization;
using SSICPAS.Services;
using SSICPAS.ViewModels.Certificates;

namespace SSICPAS.Utils
{
    public static class DateUtils
    {
        public static string LocaleFormatDate(this DateTime date, bool ForceDenmarksTimeFormat = false)
        {
            var culture = new CultureInfo("LANG_DATEUTIL".Translate());

            if (culture.Name == "da-DK" || ForceDenmarksTimeFormat)
            {
                culture = new CultureInfo("da-DK");
                var fdate = date.ToString("d MMM yyyy", culture);
                var dayArray  = fdate.Split(' ');
                return string.Format("{0}. {1} {2}", dayArray[0], dayArray[1].Substring(0,3), dayArray[2]);
            }

            // Fallback to use english formatting
            return date.ToString("MMM d, yyyy", culture);
        }

        public static string LocaleFormatTime(this DateTime date, bool isEnglish = false, bool toLocalTime = false)
        {
            var culture = new CultureInfo("LANG_DATEUTIL".Translate());

            if (toLocalTime)
            {
                date = date.ToLocalTime();
            }

            if (culture.Name == "da-DK" && isEnglish == false) 
            {
                return date.ToString("HH:mm ", culture);
            }
            // Fallback to use english formatting
            return date.ToString("hh:mm tt", CultureInfo.InvariantCulture).ToUpper();
        }
        
        public static string FormatEuDate(this DateTime date, bool toLocalTime = false)
        {
            if (toLocalTime)
            {
                date = date.ToLocalTime();
            }
            return date.ToString("MMM d, yyyy", CultureInfo.InvariantCulture);
        }

        public static string ToLocaleDateFormat(this DateTime date, bool isEnglish = false, bool toLocalTime = false)
        {
            if (isEnglish)
            {
                return FormatEuDate(date, toLocalTime);
            }
            return LocaleFormatDate(date);
        }
        
        public static string ParseDateOfBirth(
            FamilyPassportItemsViewModel passportItemsViewModel,
            Func<DateTime, string> datePostProcessingAction,
            bool withMandatoryPassport = true)
        {
            bool correctParse = DateTime.TryParse(
                passportItemsViewModel?.SelectedPassport?.BirthDate,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal,
                out var dateOfBirth);
            
            string result;
            if (correctParse)
            {
                result = datePostProcessingAction?.Invoke(dateOfBirth);
                if (!string.IsNullOrEmpty(result)) return result;
            }
            else
            {
                if (!string.IsNullOrEmpty(passportItemsViewModel?.SelectedPassport?.BirthDate))
                {
                    return passportItemsViewModel.SelectedPassport.BirthDate;
                }
            }

            if (withMandatoryPassport)
            {
                bool correctMandatoryParse = DateTime.TryParse(
                    passportItemsViewModel?.SelectedPassport?.BirthDate,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal,
                    out var mandatoryDateOfBirth);
                if (correctMandatoryParse)
                {
                    result = datePostProcessingAction?.Invoke(mandatoryDateOfBirth);
                    if (!string.IsNullOrEmpty(result)) return result;
                }
                else
                {
                    if (!string.IsNullOrEmpty(passportItemsViewModel?.SelectedPassport?.BirthDate))
                    {
                        return passportItemsViewModel?.SelectedPassport?.BirthDate;
                    }
                }
            }

            return string.Empty;
        }
    }
}
