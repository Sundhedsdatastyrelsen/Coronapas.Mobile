using System;
using Newtonsoft.Json;
using SSICPAS.Core.Services.Model.Converter;
using SSICPAS.Core.Services.Model.EuDCCModel.Shared;
using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;

namespace SSICPAS.Core.Services.Model.EuDCCModel._1._0._x
{
    public class TestResult
    {
        [JsonProperty("tg")]
        //disease or agent targeted
        [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.Disease)]
        public string Disease { get; set; }
        
        [JsonProperty("tt")]
        [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.TypeOfTest)]
        public string TypeOfTest { get; set; }

        [JsonProperty("nm")] 
        public string NAATestName { get; set; }
        
        [JsonProperty("ma")]
        [JsonConverter(typeof(DigitalCovidValueSetTestDevicesConverter))]
        public TestDevice TestManufacturer { get; set; }

        [JsonProperty("sc")]
        [JsonConverter(typeof(EpochDatetimeConverter))]
        public DateTime? SampleCollectedTime { get; set; }

        [JsonProperty("dr")]
        [JsonConverter(typeof(EpochDatetimeConverter))]
        public DateTime? ResultProducedTime { get; set; }

        [JsonProperty("tr")]
        [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.TestResult)]
        public string ResultOfTest { get; set; }
        
        [JsonProperty("tc")]
        public string TestingCentre { get; set; }
        
        [JsonProperty("co")]
        [JsonConverter(typeof(CountryCodeConverter))]
        public string CountryOfTest { get; set; }

        public string CountryOfTestDK { get => CountryCodeTranslator.GetDanishCountryName(CountryCodeTranslator.GetCountryCode(CountryOfTest)); }

        [JsonProperty("is")]
        public string CertificateIssuer { get; set; }
        
        [JsonProperty("ci")]
        public string CertificateId { get; set; }
    }
}