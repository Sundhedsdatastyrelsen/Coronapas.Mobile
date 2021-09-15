using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SSICPAS.Core.Services.Model.Converter;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Translator;
using SSICPAS.Services.WebServices;

namespace SSICPAS.Tests.TokenDecryptionTest
{
    public class DCCCsvTranslatorTests
    {
        [SetUp]
        public void Setup()
        {
            var testStream = new MemoryStream(Encoding.UTF8.GetBytes(@"{""TEST_KEY"": ""TEST"", ""TEST_KEY_2"": ""TEST {0} TEST"", ""LANG_DATEUTIL"": ""en-GB""}"));
            LocaleService.Current.LoadLocale("en", testStream, false);
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
        [Test]
        [TestCase("840539006","COVID-19")]
        public void TestDiseaseTranslation(string code, string value)
        {
            string json = $"{{\"code\":\"{code}\"}}";
            var deserializedObject = JsonConvert.DeserializeObject<DigitalCovidValueTranslatorTest.DiseaseAgentClass>(json);
            Assert.AreEqual(deserializedObject.Code, value);
        }
        [Test]
        [TestCase("1119305005","SARS-CoV-2 antigen vaccine")]
        public void TestVaccineProphylaxisTranslation(string code, string value)
        {
            string json = $"{{\"code\":\"{code}\"}}";
            var deserializedObject = JsonConvert.DeserializeObject<DigitalCovidValueTranslatorTest.VaccineProphylaxis>(json);
            Assert.AreEqual(deserializedObject.Code, value);
        }
        [Test]
        [TestCase("EU/1/20/1528","Comirnaty")]
        public void TestVaccineMedicinalProductTranslation(string code, string value)
        {
            string json = $"{{\"code\":\"{code}\"}}";
            var deserializedObject = JsonConvert.DeserializeObject<DigitalCovidValueTranslatorTest.VaccineMedicinalProduct>(json);
            Assert.AreEqual(deserializedObject.Code, value);
        }
        
        [Test]
        [TestCase("ORG-100001699","AstraZeneca AB")]
        public void TestVaccineAuthHolderTranslation(string code, string value)
        {
            string json = $"{{\"code\":\"{code}\"}}";
            var deserializedObject = JsonConvert.DeserializeObject<DigitalCovidValueTranslatorTest.VaccineAuthHolder>(json);
            Assert.AreEqual(deserializedObject.Code, value);
        }
        [Test]
        [TestCase("LP6464-4","Nucleic acid amplification with probe detection")]
        public void TestTypeOfTestTranslation(string code, string value)
        {
            string json = $"{{\"code\":\"{code}\"}}";
            var deserializedObject = JsonConvert.DeserializeObject<DigitalCovidValueTranslatorTest.TypeOfTest>(json);
            Assert.AreEqual(deserializedObject.Code, value);
        }
        [Test]
        [TestCase("260415000","Not detected")]
        public void TestResultTranslation(string code, string value)
        {
            string json = $"{{\"code\":\"{code}\"}}";
            var deserializedObject = JsonConvert.DeserializeObject<DigitalCovidValueTranslatorTest.TestResult>(json);
            Assert.AreEqual(deserializedObject.Code, value);
        }      
    }
}