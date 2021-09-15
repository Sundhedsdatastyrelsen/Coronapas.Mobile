using Newtonsoft.Json;
using SSICPAS.Core.Services.Model.Converter;
using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;

namespace SSICPAS.Core.Services.Model.EuDCCModel._1._3._0
{
    public class Recovery
    {
        [JsonProperty("tg")]
        [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.Disease)]
        public string Disease { get; set; }
        
        [JsonProperty("fr")]
        public string DateOfFirstPositiveResult{ get; set; }
        
        [JsonProperty("co")]
        [JsonConverter(typeof(CountryCodeConverter))]
        public string CountryOfTest { get; set; }

        public string CountryOfTestDK { get => CountryCodeTranslator.GetDanishCountryName(CountryCodeTranslator.GetCountryCode(CountryOfTest)); }

        [JsonProperty("is")]
        public string CertificateIssuer { get; set; }
        
        [JsonProperty("ci")]
        public string CertificateId { get; set; }

        [JsonProperty("df")]
        public string ValidFrom{ get; set; }

        [JsonProperty("du")]
        public string ValidTo{ get; set; }
    }
}