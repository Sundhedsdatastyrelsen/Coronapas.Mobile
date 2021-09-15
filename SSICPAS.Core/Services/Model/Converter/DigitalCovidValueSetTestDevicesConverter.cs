using System;
using Newtonsoft.Json;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model.EuDCCModel.Shared;
using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;

namespace SSICPAS.Core.Services.Model.Converter
{
    public class DigitalCovidValueSetTestDevicesConverter : JsonConverter<TestDevice>
    {
        private readonly IDCCValueSetTranslator _dccValueSetTranslator;

        public DigitalCovidValueSetTestDevicesConverter()
        {
            _dccValueSetTranslator = DigitalCovidValueSetTranslatorFactory.DccValueSetTestDevicesTranslator;
        }

        public override void WriteJson(JsonWriter writer, TestDevice value, JsonSerializer serializer)
        {
            writer.WriteRawValue(_dccValueSetTranslator.GetDCCCode(DCCValueSetEnum.TestManufacturer, value));
        }

        public override TestDevice ReadJson(JsonReader reader, Type objectType, TestDevice existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return (TestDevice)_dccValueSetTranslator.Translate(DCCValueSetEnum.TestManufacturer, reader.Value.ToString());
        }
    }
}