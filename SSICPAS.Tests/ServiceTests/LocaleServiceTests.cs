using System.IO;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using SSICPAS.Enums;
using SSICPAS.Services;

#nullable enable

namespace SSICPAS.Tests.ServiceTests
{
    public class LocaleServiceTests
    {
        private Stream testStream;
        private LocaleService localeService = LocaleService.Current;

        [SetUp]
        public void SetUp()
        {
            testStream = new MemoryStream(Encoding.UTF8.GetBytes(@"{""TEST_KEY"": ""TEST"", ""TEST_KEY_2"": ""TEST {0} TEST"", ""DynamicSettingDouble"": ""0.1"", ""DynamicSettingInt"": ""10"", ""DynamicSettingString"": ""DYNAMIC_SETTING_STRING""}"));
        }

        [Test]
        public void Translate_KeyNotFoundReturnsKeyWithNotFoundSymbol()
        {
            string translated = localeService.Translate("ANOTHER_TEST_KEY");
            Assert.AreEqual("$ANOTHER_TEST_KEY$", translated);
        }

        [Test]
        public void Translate_KeyFoundReturnsValueForCurrentLocale()
        {
            localeService.LoadLocale("dk", testStream, false);
            string translated = localeService.Translate("TEST_KEY");
            Assert.AreEqual("TEST", translated);
        }

        [Test]
        public void Translate_WithArgsReturnsFormatted()
        {
            localeService.LoadLocale("dk", testStream, false);
            string translated = localeService.Translate("TEST_KEY_2", "TEST");
            Assert.AreEqual("TEST TEST TEST", translated);
        }

        [Test]
        public void GetLanguage_ReturnsCorrectLanguage()
        {
            localeService.LoadLocale("dk", testStream, false);
            Assert.AreEqual(LanguageSelection.Danish, LocaleService.Current.GetLanguage());
        }

        [Test]
        public void LoadLocale_InvalidJsonThrowsException()
        {
            Stream invalidStream = new MemoryStream(Encoding.UTF8.GetBytes("\"TEST_KEY\": \"TEST\" \"TEST_KEY_2\": \"TEST {0} TEST\""));
            Assert.Throws<JsonReaderException>(() =>
            {
                localeService.LoadLocale("dk", invalidStream, false);
            });
        }

        [Test]
        public void LoadLocale_OverwritesOldLocales()
        {
            Stream anotherStream = new MemoryStream(Encoding.UTF8.GetBytes("{\"TEST_KEY\": \"TEST2\"}"));
            localeService.LoadLocale("dk", testStream, false);
            localeService.LoadLocale("en", anotherStream, false);
            Assert.AreEqual(LanguageSelection.English, LocaleService.Current.GetLanguage());
            Assert.AreEqual("TEST2", LocaleService.Current.Translate("TEST_KEY"));
        }

        [TestCase("DynamicSettingDouble", 0.1)]
        [TestCase("NonExistingDynamicSettingDouble", null)]
        public void GetValueForKeyTests(string key, double? output)
        {
            localeService.LoadLocale("dk", testStream, false);
            double? setting = localeService.GetValueForKey<double>(key);
            Assert.AreEqual(setting, output);
        }

        [TestCase("DynamicSettingInt", 10)]
        [TestCase("NonExistingDynamicSettingInt", null)]
        public void GetValueForKeyTests(string key, int? output)
        {
            localeService.LoadLocale("dk", testStream, false);
            int? setting = localeService.GetValueForKey<int>(key);
            Assert.AreEqual(setting, output);
        }

        [TestCase("DynamicSettingString", "DYNAMIC_SETTING_STRING")]
        [TestCase("NonExistingDynamicSettingString", null)]
        public void GetClassValueForKeyTests(string key, string? output)
        {
            localeService.LoadLocale("dk", testStream, false);
            string? setting = localeService.GetClassValueForKey<string>(key);
            Assert.AreEqual(setting, output);
        }
    }
}
