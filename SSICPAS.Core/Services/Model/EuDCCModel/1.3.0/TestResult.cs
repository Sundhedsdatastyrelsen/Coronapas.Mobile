using System;
using Newtonsoft.Json;
using SSICPAS.Core.Services.Model.Converter;
using SSICPAS.Core.Services.Model.EuDCCModel.Shared;
using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;

namespace SSICPAS.Core.Services.Model.EuDCCModel._1._3._0
{
    public class TestResult
    {
        [JsonProperty("tg")]
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

        /// <summary>
        /// This is custom defined property for DK3 tokens from CBS. Represents
        /// absolute DateTime when the given test expires. Used only internally for
        /// MyPage and is not present on HC1 EU tokens.
        /// </summary>
        [JsonProperty("xdu")]
        public DateTime? CBSDefinedExpirationTime { get; set; }
    }
}