using System;
using Newtonsoft.Json;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;

namespace SSICPAS.Core.Services.Model.Converter
{
    public class DigitalCovidValueSetConverter : JsonConverter<string>
    {
        private readonly DCCValueSetEnum _key;
        private readonly IDCCValueSetTranslator _dccValueSetTranslator;

        public DigitalCovidValueSetConverter(DCCValueSetEnum key)
        {
            _dccValueSetTranslator = DigitalCovidValueSetTranslatorFactory.DccValueSetTranslator;
            _key = key;
        }
        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
        {
            writer.WriteRawValue(_dccValueSetTranslator.GetDCCCode(_key, value));
        }

        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return (string) _dccValueSetTranslator.Translate(_key, reader.Value.ToString());
        }
    }
}