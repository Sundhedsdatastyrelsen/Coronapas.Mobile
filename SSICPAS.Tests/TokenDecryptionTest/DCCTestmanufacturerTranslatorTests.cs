using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SSICPAS.Core.Services.Model.Converter;
using SSICPAS.Core.Services.Model.EuDCCModel.Shared;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.WebServices;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SSICPAS.Tests.TokenDecryptionTest
{
    public class DCCTestmanufacturerTranslatorTests
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

            DigitalCovidValueSetTranslatorFactory.DccValueSetTestDevicesTranslator = new DigitalCovidValueSetTestAndTestManufacturerNameTranslator(MockRatListService.Object);
            DigitalCovidValueSetTranslatorFactory.DccValueSetTestDevicesTranslator.InitValueSetAsync().Wait();
        }
        [Test]
        [TestCase("1833","COVID-VIRO", "AAZ-LMB")]
        [TestCase("1232","Panbio Covid-19 Ag Rapid Test", "Abbott Rapid Diagnostics")]
        [TestCase("1833","COVID-VIRO", "AAZ-LMB")]
        public void TestCountryCodeTranslation(string code, string testName, string manufacturer)
        {
            string json = $"{{\"code\":\"{code}\"}}";
            var deserializedObject = JsonConvert.DeserializeObject<TestManufacturer>(json);
            Assert.AreEqual(deserializedObject.Code.TestName, testName);
            Assert.AreEqual(deserializedObject.Code.ManufacturerName, manufacturer);
        }
        public class TestManufacturer
        {
            [JsonConverter(typeof(DigitalCovidValueSetTestDevicesConverter))]
            public TestDevice Code { get; set; }
        }
    }
}