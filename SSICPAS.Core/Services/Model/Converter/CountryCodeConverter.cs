using System;
using Newtonsoft.Json;

namespace SSICPAS.Core.Services.Model.Converter
{
    public class CountryCodeConverter : JsonConverter<string>
    {
        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
        {
            writer.WriteRawValue(CountryCodeTranslator.GetCountryCode(value));
        }

        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return CountryCodeTranslator.GetCountryName(reader.Value.ToString());
        }
    }
}