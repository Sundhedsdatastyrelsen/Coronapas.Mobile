using Newtonsoft.Json;

namespace SSICPAS.Core.Services.Model.EuDCCModel
{
    public class PersonName
    {
        [JsonProperty("gn")]
        public string GivenName { get; set; }
        [JsonProperty("gnt")]
        public string GivenNameTransliterated { get; set; }
        [JsonProperty("fn")]
        public string FamilyName { get; set; }
        [JsonProperty("fnt")]
        public string FamilyNameTransliterated { get; set; }

        public string FullNameTransliteratedReversedWithComma => $"{FamilyNameTransliterated}, {GivenNameTransliterated}";
        public string FullNameWithSpace => $"{GivenName} {FamilyName}";
    }
}