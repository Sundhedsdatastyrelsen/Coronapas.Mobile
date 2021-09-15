using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Logging;
using SSICPAS.Data;
using SSICPAS.Enums;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.WebServices;
using SSICPAS.Tests.NavigationTests;
using SSICPAS.Tests.TestMocks;
using Assert = NUnit.Framework.Assert;

namespace SSICPAS.Tests.ServiceTests
{
    public class TextServiceTests
    {
        private Mock<ILoggingService> loggingService;
        private Mock<ISettingsService> settingsService;
        private IPreferencesService preferencesService;

        private const Environment.SpecialFolder FILE_DIRECTORY = Environment.SpecialFolder.Personal;

        private ITextService textService;

        [SetUp]
        public void SetUp()
        {
            IoCContainer.RegisterInterface<ISettingsService, MockSettingsService>();
            IoCContainer.RegisterInterface<IPreferencesService, MockPreferencesService>();

            preferencesService = new MockPreferencesService();

            loggingService = new Mock<ILoggingService>();
            settingsService = new Mock<ISettingsService>();
            textService = new TextService(preferencesService, new Mock<ITextRepository>().Object, loggingService.Object, new MockNavigationTaskManager(), new MockDateTimeService());
        }

        [TearDown]
        public void TearDown()
        {
            loggingService.Reset();
        }

        [Test]
        public void SetLocale_LocaleIsSet()
        {
            textService.SetLocale("en");
            Assert.AreEqual(LanguageSelection.English, LocaleService.Current.GetLanguage());
        }

        [Test]
        public async Task LoadSavedLocales_LoadsSuccessfully()
        {
            await textService.LoadSavedLocales();
            loggingService.Verify(l => l.LogException(It.IsAny<LogSeverity>(), It.IsAny<Exception>(), "Failed to load embedded locales", It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public async Task EnsureDKAndENHasSameNumberOfText()
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(TextService)).Assembly;
            Stream dkStream = assembly.GetManifestResourceStream("SSICPAS.Locales.dk.json");
            Stream enStream = assembly.GetManifestResourceStream("SSICPAS.Locales.en.json");
            string dkText = "";
            string enText = "";

            using (var reader = new System.IO.StreamReader(dkStream))
            {
                dkText = await reader.ReadToEndAsync();
            }
            using (var reader = new System.IO.StreamReader(enStream))
            {
                enText = await reader.ReadToEndAsync();
            }
            var DKDictionary = JsonConvert
                .DeserializeObject<Dictionary<string, string>>(dkText);
            var ENDictionary = JsonConvert
                .DeserializeObject<Dictionary<string, string>>(enText);

            List<string> EnMissingKey = new List<string>();
            List<string> DkMissingKey = new List<string>();
            foreach (KeyValuePair<string,string> pair in DKDictionary)
            {
                if (ENDictionary.ContainsKey(pair.Key))
                {
                    ENDictionary.Remove(pair.Key);
                }
                else
                {
                    EnMissingKey.Add(pair.Key);   
                }
            }

            DkMissingKey = ENDictionary.Select(x => x.Key).ToList();

            if (DkMissingKey.Any() || EnMissingKey.Any())
            {
                throw new AssertFailedException(
                    $"en.json and dk.json is not synchronized:\nen.json missing key are : {string.Join(", ",EnMissingKey)} \ndk.json missing key are : {string.Join(", ",DkMissingKey)} ");
            }
        }

        [Test]
        public void SetLocale_LocaleIsSetToEmbedded()
        {
            textService.SetLocale("en");
            Assert.True(LocaleService.Current.LoadedFromEmbeddedFile);
        }

        [TestCase("en", "1.8")]
        [TestCase("dk", "1.8")]
        public async Task SetLocale_LocaleIsSetToFetchedFile(string lang, string versionOfFetchedFile)
        {
            await SimulatePreviouslyDownloadedFilesFromBackend(lang, versionOfFetchedFile);
            textService.SetLocale(lang);
            Assert.False(LocaleService.Current.LoadedFromEmbeddedFile);
            Assert.NotNull(LocaleService.Current.GetClassValueForKey<string>("TEST_KEY"));
            Assert.NotNull(LocaleService.Current.GetClassValueForKey<string>("TEST_KEY2"));
        }

        [TestCase("en", "1.0")]
        [TestCase("dk", "1.0")]
        [TestCase("dk", "1.5")]
        [TestCase("en", "1.5")]
        public async Task SetLocale_LocaleIsSetToEmbeddedFileBecauseFetchedFileIsOld(string lang, string versionOfFetchedFile)
        {
            await SimulatePreviouslyDownloadedFilesFromBackend(lang, versionOfFetchedFile);
            textService.SetLocale(lang);
            Assert.True(LocaleService.Current.LoadedFromEmbeddedFile);
            Assert.Null(LocaleService.Current.GetClassValueForKey<string>("TEST_KEY"));
            Assert.Null(LocaleService.Current.GetClassValueForKey<string>("TEST_KEY2"));
        }

        private async Task SimulatePreviouslyDownloadedFilesFromBackend(string lang, string version)
        {
            byte[] bytes;
            Stream textFileStream = new MemoryStream(Encoding.UTF8.GetBytes(@"{""TEST_KEY"": ""TEST"", ""TEST_KEY2"": ""TEST2""}"));
            using var memoryStream = new MemoryStream();
            await textFileStream.CopyToAsync(memoryStream);
            bytes = memoryStream.ToArray();
            var path = Path.Combine(Environment.GetFolderPath(FILE_DIRECTORY), $"{lang}_{version}.json");
            File.WriteAllBytes(path, bytes);

            preferencesService.SetUserPreference(PreferencesKeys.CURRENT_TEXT_VERSION, version);
            preferencesService.SetUserPreference(PreferencesKeys.LANGUAGE_SETTING, lang);
        }
    }
}
