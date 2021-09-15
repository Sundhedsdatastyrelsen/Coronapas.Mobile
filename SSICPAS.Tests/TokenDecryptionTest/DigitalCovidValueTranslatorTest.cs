using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SSICPAS.Core.Services.Model.Converter;
using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Translator;
using SSICPAS.Services.WebServices;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SSICPAS.Tests.TokenDecryptionTest
{
    public class DigitalCovidValueTranslatorTest
    {
        [SetUp]
        public void Setup()
        {
            var MockRatListService = new Mock<IRatListService>();
            MockRatListService.Setup(t => t.GetRatList()).Returns(Task.Run(async () =>
            {
                var assembly = typeof(RatListService).GetTypeInfo().Assembly;
                Stream ratlistStream = assembly.GetManifestResourceStream("SSICPAS.Valuesets.ratlist.json");
                using (var reader = new StreamReader(ratlistStream))
                    return await reader.ReadToEndAsync();
            }));
            MockRatListService.Setup(t => t.GetDCCValueSet()).Returns(Task.Run(async () =>
            {
                var assembly = typeof(RatListService).GetTypeInfo().Assembly;
                Stream valuesetsStream = assembly.GetManifestResourceStream("SSICPAS.Valuesets.valueset.json");
                using (var reader = new StreamReader(valuesetsStream))
                    return await reader.ReadToEndAsync();
            }));

            DigitalCovidValueSetTranslatorFactory.DccValueSetTranslator = new DCCValueSetTranslator(MockRatListService.Object);
            DigitalCovidValueSetTranslatorFactory.DccValueSetTestDevicesTranslator = new DigitalCovidValueSetTestAndTestManufacturerNameTranslator(MockRatListService.Object);
            DigitalCovidValueSetTranslatorFactory.DccValueSetTestDevicesTranslator.InitValueSetAsync().Wait();
            DigitalCovidValueSetTranslatorFactory.DccValueSetTranslator.InitValueSetAsync().Wait();
        }
        [TestCase("840539006","COVID-19")]
        public void DiseaseAgentValueTest(string code, string value)
        {
            string json = $"{{ \"code\" : \"{code}\"}}";
            var deserializedObject = JsonConvert.DeserializeObject<DiseaseAgentClass>(json);
            Assert.AreEqual(deserializedObject.Code, value);
        }
        public class DiseaseAgentClass
        {
            [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.Disease)]
            public string Code { get; set; }
        }
        
        [TestCase("260415000","Not detected")]
        [TestCase("260373001","Detected")]
        public void TestResultValueTest(string code, string value)
        {
            string json = $"{{ \"code\" : \"{code}\"}}";
            var deserializedObject = JsonConvert.DeserializeObject<TestResult>(json);
            Assert.AreEqual(deserializedObject.Code, value);
        }
        public class TestResult
        {
            [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.TestResult)]
            public string Code { get; set; }
        }
        
        [TestCase("ORG-100001699","AstraZeneca AB")]
        [TestCase("ORG-100030215","Biontech Manufacturing GmbH")]
        [TestCase("ORG-100001417","Janssen-Cilag International")]
        [TestCase("ORG-100031184","Moderna Biotech Spain S.L.")]
        [TestCase("ORG-100006270","Curevac AG")]
        [TestCase("ORG-100013793","CanSino Biologics")]
        [TestCase("ORG-100020693","China Sinopharm International Corp. - Beijing location")]
        [TestCase("ORG-100010771","Sinopharm Weiqida Europe Pharmaceutical s.r.o. - Prague location")]
        [TestCase("ORG-100024420","Sinopharm Zhijun (Shenzhen) Pharmaceutical Co. Ltd. - Shenzhen location")]
        [TestCase("ORG-100032020","Novavax CZ AS")]
        [TestCase("Gamaleya-Research-Institute","Gamaleya Research Institute")]
        [TestCase("Vector-Institute","Vector Institute")]
        [TestCase("Sinovac-Biotech","Sinovac Biotech")]
        [TestCase("Bharat-Biotech","Bharat Biotech")]
        public void VaccineAuthHolderValueTest(string code, string value)
        {
            string json = $"{{ \"code\" : \"{code}\"}}";
            var deserializedObject = JsonConvert.DeserializeObject<VaccineAuthHolder>(json);
            Assert.AreEqual(deserializedObject.Code, value);
        }
        public class VaccineAuthHolder
        {
            [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.VaccineAuthorityHolder)]
            public string Code { get; set; }
        }
        
        public class VaccineProphylaxis
        {
            [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.VaccineProphylaxis)]
            public string Code { get; set; }
        }
        
        public class VaccineMedicinalProduct
        {
            [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.VaccineMedicinalProduct)]
            public string Code { get; set; }
        }
        public class TypeOfTest
        {
            [JsonConverter(typeof(DigitalCovidValueSetConverter), DCCValueSetEnum.TypeOfTest)]
            public string Code { get; set; }
        }


    }
}