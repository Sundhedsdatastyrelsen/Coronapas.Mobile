using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace SSICPAS.Core.Services.Model.Converter
{
    public static class CountryCodeTranslator
    {
        private static List<CountryCode> ValueSetModels = new List<CountryCode>();

        static CountryCodeTranslator()
        {
            Init();
        }
        
        private static async void Init()
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(CountryCodeTranslator)).Assembly;
            var embededResources = assembly.GetManifestResourceNames();
            string testAndManufacturerResource =
                embededResources.SingleOrDefault(x => x.Contains("country_code.json"));
            if (string.IsNullOrEmpty(testAndManufacturerResource)) return;
            using (Stream resourceStream = assembly.GetManifestResourceStream(testAndManufacturerResource))
            {
                using (var streamReader = new StreamReader(resourceStream))
                {
                    var json = await streamReader.ReadToEndAsync();

                    ValueSetModels = JsonConvert
                        .DeserializeObject<List<CountryCode>>(json);
                }  
            }
        }

        public static string GetCountryName(string countryCode)
        {
            return ValueSetModels.FirstOrDefault(x => x.Code == countryCode)?.Name ?? countryCode;
        }

        public static string GetDanishCountryName(string countryCode)
        {
            return ValueSetModels.FirstOrDefault(x => x.Code == countryCode)?.DanishName ?? countryCode;
        }
        
        public static string GetCountryCode(string countryName)
        {
            return ValueSetModels.FirstOrDefault(x => x.Name == countryName)?.Code ?? countryName;
        }
        
    }
}