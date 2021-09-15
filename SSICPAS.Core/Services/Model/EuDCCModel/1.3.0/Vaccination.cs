using System;
using Newtonsoft.Json;
using SSICPAS.Core.Services.Model.Converter;
using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;

namespace SSICPAS.Core.Services.Model.EuDCCModel._1._3._0
{
    public class Vaccination
    {
        [JsonProperty("tg")]
        [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.Disease)]
        public string Disease { get; set; }
        
        [JsonProperty("vp")]
        [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.VaccineProphylaxis)]
        public string VaccineProphylaxis { get; set; }
        
        [JsonProperty("mp")]
        [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.VaccineMedicinalProduct)]
        public string VaccineMedicinalProduct { get; set; }
        
        [JsonProperty("ma")]
        [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.VaccineAuthorityHolder)]
        public string Manufacturer { get; set; }
        
        [JsonProperty("dn")]
        public int DoseNumber { get; set; }
        
        [JsonProperty("sd")]
        public int TotalSeriesOfDose { get; set; }
        
        [JsonProperty("dt")]
        public string DateOfVaccination { get; set; }
        
        [JsonProperty("co")]
        [JsonConverter(typeof(CountryCodeConverter))]
        public string CountryOfVaccination { get; set; }

        public string CountryOfVaccinationDK { get => CountryCodeTranslator.GetDanishCountryName(CountryCodeTranslator.GetCountryCode(CountryOfVaccination)); }

        [JsonProperty("is")]
        public string CertificateIssuer { get; set; }
        
        [JsonProperty("ci")]
        public string CertificateId { get; set; }

        /// <summary>
        /// This is custom defined property for DK3 tokens from CBS. Represents
        /// absolute DateTime when the given vaccine expires. Used only internally for
        /// MyPage and is not present on HC1 EU tokens.
        /// </summary>
        [JsonProperty("xdu")]
        public DateTime? CBSDefinedExpirationTime { get; set; }
    }
}