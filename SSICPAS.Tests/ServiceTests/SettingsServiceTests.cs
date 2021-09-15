using System.IO;
using System.Text;
using Moq;
using NUnit.Framework;
using SSICPAS.Models.Exceptions;
using SSICPAS.Services;
using SSICPAS.Tests.TestMocks;

namespace SSICPAS.Tests.ServiceTests
{
    public class SettingsServiceTests
    {
        private Mock<SettingsService> settingsServiceMock;
        SettingsService _settingsService => settingsServiceMock.Object;

        [SetUp]
        public void SetUp()
        {
            MockConfigurationProvider mockConfigurationProvider = new MockConfigurationProvider();
            settingsServiceMock = new Mock<SettingsService>(mockConfigurationProvider);
        }

        [Test]
        public void GetEnvironmentDescriptionSetting()
        {
            var setting = _settingsService.EnvironmentDescription;
            Assert.AreEqual(setting, "api-local");
        }

        [Test]
        public void GetTrustedSSLCertificateFileNameSetting()
        {
            var setting = _settingsService.TrustedSSLCertificateFileName;
            Assert.AreEqual(setting, "local.crt");
        }

        [Test]
        public void GetNonExistingSettingFromSettingsFile()
        {
            var ex = Assert.Throws<MissingSettingException>(() => _ = _settingsService.ApiVersion);
            Assert.AreEqual(ex.Message, "Key 'ApiVersion' does not exist in current settings file.");
        }

        [Test]
        public void GetExistingSettingFromTextFile()
        {
            Stream dkTestStream = new MemoryStream(Encoding.UTF8.GetBytes(@"{""TEST_KEY"": ""TEST"", ""BlockingGroupLogThrottleFactor"": ""1""}"));
            LocaleService.Current.LoadLocale("dk", dkTestStream, false);
            var result = _settingsService.BlockingGroupLogThrottleFactor;

            Assert.AreEqual(result, 1);

            LocaleService.Current.Reset();
        }

        [Test]
        public void GetNonExistingSettingFromTextFile()
        {
            var ex = Assert.Throws<MissingSettingException>(() => _ = _settingsService.BlockingGroupLogThrottleFactor);
            Assert.AreEqual(ex.Message, "Key 'BlockingGroupLogThrottleFactor' does not exist in current text file.");
        }


        [Test]
        public void GetExistingRefTypeSettingFromTextFile()
        {
            Stream dkTestStream = new MemoryStream(Encoding.UTF8.GetBytes(@"{""ContinuousFetchingDelaysSeconds"": ""20;45;100""}"));
            LocaleService.Current.LoadLocale("dk", dkTestStream, false);
            var result = _settingsService.ContinuousFetchingDelaysSeconds;

            Assert.AreEqual(result, "20;45;100");

            LocaleService.Current.Reset();
        }

        [Test]
        public void GetNonExistingRefTypeSettingFromTextFile()
        {
            var ex = Assert.Throws<MissingSettingException>(() => _ = _settingsService.ContinuousFetchingDelaysSeconds);
            Assert.AreEqual(ex.Message, "Key 'ContinuousFetchingDelaysSeconds' does not exist in current text file.");
        }

        [TearDown]
        public void TearDown()
        {
            settingsServiceMock.Reset();
        }
    }
}
