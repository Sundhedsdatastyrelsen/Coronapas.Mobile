using Newtonsoft.Json;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;
using SSICPAS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSICPAS.Services.Translator
{
    public class DCCValueSetTranslator : IDCCValueSetTranslator
    {
        private readonly IRatListService _ratListService;
        
        private ValueSetObject _valueSetModel = new ValueSetObject();

        public DCCValueSetTranslator(IRatListService ratListService)
        {
            _ratListService = ratListService;
        }

        public async Task InitValueSetAsync()
        { 
            var resourceReader = await _ratListService.GetDCCValueSet();
            _valueSetModel = JsonConvert.DeserializeObject<ValueSetObject>(resourceReader);
        }

        public object Translate(DCCValueSetEnum key, string code)
        {
            bool canGetSupportedKey = EnumTostringValueMap.TryGetValue(key, out var valueSetId);
            if (!canGetSupportedKey) 
                return code;
           
            ValueSetModel valueSetModel = _valueSetModel.DeviceList.First(x => x.ValueSetId == valueSetId);
            
            if (valueSetModel.ValueSetId == null)
                return code;

            valueSetModel.ValueSetValues.TryGetValue(code, out var translatedValue);

            return translatedValue?.Active ?? false ? translatedValue.Display : code;
        }

        public string GetDCCCode(DCCValueSetEnum key, object value)
        {
            EnumTostringValueMap.TryGetValue(key, out var valueSetId);
            ValueSetModel valueSetModel = _valueSetModel.DeviceList.First(x => x.ValueSetId == valueSetId);
            return (string)(valueSetModel.ValueSetValues.FirstOrDefault(x => x.Value.Display == value).Key ?? value);
        }

        public static Dictionary<DCCValueSetEnum, string> EnumTostringValueMap = new Dictionary<DCCValueSetEnum, string>()
        {
            {DCCValueSetEnum.Disease, "disease-agent-targeted"},
            {DCCValueSetEnum.VaccineProphylaxis, "sct-vaccines-covid-19"},
            {DCCValueSetEnum.VaccineMedicinalProduct, "vaccines-covid-19-names"},
            {DCCValueSetEnum.VaccineAuthorityHolder, "vaccines-covid-19-auth-holders"},
            {DCCValueSetEnum.TestResult, "covid-19-lab-result"},
            {DCCValueSetEnum.TypeOfTest, "covid-19-lab-test-type"}
        };

        /// <summary>
        /// Localizes the given value from valuesets to Danish. If English is set as app language,
        /// the value from valuesets will be used by default. Use it when localisation is needed.
        /// Translation is done by TextService and text files are therefore used. Add more translations
        /// to the maps used by this method and update text files if more transaltions will be needed.
        /// Note, by default valuesets and RAT have only English texts that are used everywhere in the code.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="type"></param>
        /// <param name="_ShowTextInEnglish"></param>
        /// <returns></returns>
        public static string ToLocale(string result, DCCValueSetEnum type, bool _ShowTextInEnglish = false)
        {
            if (_ShowTextInEnglish || string.IsNullOrEmpty(result)) return result;

            bool canGetTranslatedValue = false;
            string key = result + " Danish";
            string output = result;

            switch (type)
            {
                case DCCValueSetEnum.TypeOfTest:
                    canGetTranslatedValue = TypeOfTestTranslationMap.TryGetValue(key, out output);
                    break;
                case DCCValueSetEnum.TestResult:
                    canGetTranslatedValue = TestResultTranslationMap.TryGetValue(key, out output);
                    break;
                case DCCValueSetEnum.Disease:
                    canGetTranslatedValue = DiseaseTranslationMap.TryGetValue(key, out output);
                    break;
                case DCCValueSetEnum.CertificateIssuer:
                    return result.Equals("DANISH_CERTIFICATE_ISSUER_EN".Translate()) ? "DANISH_CERTIFICATE_ISSUER_DK".Translate() : result;
                default:
                    break;
            }

            if (canGetTranslatedValue)
            {
                // Verify that tranlsatedValue is set in text file, otherwise, show English from valueset
                string tranlsatedValue = output.Translate();
                return tranlsatedValue.StartsWith("$") ? result : tranlsatedValue;
            }

            return result;
        }

        private static Dictionary<string, string> TypeOfTestTranslationMap = new Dictionary<string, string>()
        {
            {"Rapid immunoassay", "RAPID_IMMUNOASSAY"},
            {"Nucleic acid amplification with probe detection", "NUCLEIC_ACID_AMPLIFICATION_WITH_PROBE_DETECTION"},
            {"Rapid immunoassay Danish", "RAPID_IMMUNOASSAY_DK"},
            {"Nucleic acid amplification with probe detection Danish", "NUCLEIC_ACID_AMPLIFICATION_WITH_PROBE_DETECTION_DK"}
        };

        private static Dictionary<string, string> TestResultTranslationMap = new Dictionary<string, string>()
        {
            {"Not detected", "NOT_DETECTED"},
            {"Detected", "DETECTED"},
            {"Not detected Danish", "NOT_DETECTED_DK"},
            {"Detected Danish", "DETECTED_DK"}
        };

        private static Dictionary<string, string> DiseaseTranslationMap = new Dictionary<string, string>()
        {
            {"COVID-19", "COVID-19"},
            {"COVID-19 Danish", "COVID-19_DK"},
        };
    }

    public class ValueSetObject
    {
        [JsonProperty("valueset")]
        public List<ValueSetModel> DeviceList { get; set; } = new List<ValueSetModel>();
    }

    public class ValueSet
    {
        public string ValueSetId { get; set; }
        public DateTime ValueSetDate { get; set; }
        public List<ValueSetValues> Values { get; set; } = new List<ValueSetValues>();
    }

    public class ValueSetValues
    {
        public DCCValueSetEnum Group { get; set; }
        public string Lang { get; set; }
        public bool Active { get; set; }
        public string Display { get; set; }
        public string Version { get; set; }
        public string System { get; set; }
    }
}