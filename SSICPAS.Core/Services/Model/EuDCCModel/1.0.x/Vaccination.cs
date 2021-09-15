using System;
using Newtonsoft.Json;
using SSICPAS.Core.Services.Model.Converter;
using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;

namespace SSICPAS.Core.Services.Model.EuDCCModel._1._0._x
{
    public class Vaccination
    {
        [JsonProperty("tg")]
        //disease or agent targeted
        [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.Disease)]
        public string Disease { get; set; }
        
        [JsonProperty("vp")]
        //vaccine or prophylaxis
        [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.VaccineProphylaxis)]
        public string VaccineProphylaxis { get; set; }
        
        [JsonProperty("mp")]
        //vaccine medicinal product
        [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.VaccineMedicinalProduct)]
        public string VaccineMedicinalProduct { get; set; }
        
        [JsonProperty("ma")]
        //Marketing Authorization Holder - if no MAH present, then manufacturer
        [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.VaccineAuthorityHolder)]
        public string Manufacturer { get; set; }
        
        [JsonProperty("dn")]
        public int DoseNumber { get; set; }
        
        [JsonProperty("sd")]
        public int TotalSeriesOfDose { get; set; }
        
        [JsonProperty("dt")]
        [JsonConverter(typeof(EpochDatetimeConverter))]
        public DateTime? DateOfVaccination { get; set; }
        
        [JsonProperty("co")]
        [JsonConverter(typeof(CountryCodeConverter))]
        public string CountryOfVaccination { get; set; }

        public string CountryOfVaccinationDK { get => CountryCodeTranslator.GetDanishCountryName(CountryCodeTranslator.GetCountryCode(CountryOfVaccination)); }

        [JsonProperty("is")]
        public string CertificateIssuer { get; set; }
        
        [JsonProperty("ci")]
        public string CertificateId { get; set; }
    }
}