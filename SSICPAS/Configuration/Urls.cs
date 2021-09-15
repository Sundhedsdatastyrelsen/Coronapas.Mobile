using SSICPAS.Core.Interfaces;

namespace SSICPAS.Configuration
{
    public static class Urls
    {
        public static string _baseUrl => IoCContainer.Resolve<ISettingsService>().BaseUrl;

        public static string _apiVersion => IoCContainer.Resolve<ISettingsService>().ApiVersion;

        public static string URL_GET_PASSPORTS => $"{_baseUrl}{_apiVersion}/passport";
        public static string URL_GET_PUBLIC_KEY => $"{_baseUrl}{_apiVersion}/publickey";
        public static string URL_GET_TEXTS => $"{_baseUrl}{_apiVersion}/text";
        public static string URL_GET_RATLIST => $"{_baseUrl}{_apiVersion}/euratvalueset";
    }
}
