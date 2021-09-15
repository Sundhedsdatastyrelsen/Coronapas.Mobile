namespace SSICPAS.Enums
{
    public enum LanguageSelection
    {
        Danish,
        English
    }

    public static class LanguageSelectionExtensions
    {
        public static string ToISOCode(this LanguageSelection languageSelection)
        {
            return languageSelection switch
            {
                LanguageSelection.English => "en",
                LanguageSelection.Danish => "dk",
                _ => "dk",
            };
        }

        public static LanguageSelection FromISOCode(this string isoCode)
        {
            return isoCode switch
            {
                "en" => LanguageSelection.English,
                "dk" => LanguageSelection.Danish,
                _ => LanguageSelection.Danish,
            };
        }
    }
}
