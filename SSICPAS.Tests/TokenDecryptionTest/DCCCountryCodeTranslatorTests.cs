using Newtonsoft.Json;
using NUnit.Framework;
using SSICPAS.Core.Services.Model.Converter;

namespace SSICPAS.Tests.TokenDecryptionTest
{
    public class DCCCountryCodeTranslatorTests
    {
        [Test]
        [TestCase("DK","Denmark")]
        [TestCase("GB","United Kingdom")]
        [TestCase("VN","Viet Nam")]
        public void TestCountryCodeTranslation(string code, string value)
        {
            string json = $"{{\"code\":\"{code}\"}}";
            var deserializedObject = JsonConvert.DeserializeObject<CountryCode>(json);
            Assert.AreEqual(deserializedObject.Code, value);
        }
        public class CountryCode
        {
            [JsonConverter(typeof(CountryCodeConverter))]
            public string Code { get; set; }
        }
    }
}