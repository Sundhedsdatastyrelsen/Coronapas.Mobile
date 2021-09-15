using System;
using Newtonsoft.Json;
using SSICPAS.Core.Services.Model.Converter;
using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;

namespace SSICPAS.Core.Services.Model.EuDCCModel._1._0._x
{
    public class Recovery
    {
        [JsonProperty("tg")]
        //disease or agent targeted
        [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.Disease)]
        public string Disease { get; set; }
        
        [JsonProperty("fr")]
        public DateTime? DateOfFirstPositiveResult{ get; set; }
        
        [JsonProperty("co")]
        [JsonConverter(typeof(CountryCodeConverter))]
        public string CountryOfTest { get; set; }

        public string CountryOfTestDK { get => CountryCodeTranslator.GetDanishCountryName(CountryCodeTranslator.GetCountryCode(CountryOfTest)); }

        [JsonProperty("is")]
        public string CertificateIssuer { get; set; }
        
        [JsonProperty("ci")]
        public string CertificateId { get; set; }
        
        [JsonProperty("df")]
        [JsonConverter(typeof(EpochDatetimeConverter))]
        public DateTime? ValidFrom{ get; set; }
        
        [JsonProperty("du")]
        [JsonConverter(typeof(EpochDatetimeConverter))]
        public DateTime? ValidTo{ get; set; }
    }
}