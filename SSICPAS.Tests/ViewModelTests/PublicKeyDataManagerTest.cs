using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SSICPAS.Configuration;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model;
using SSICPAS.Core.WebServices;
using SSICPAS.Models;
using SSICPAS.Services;
using SSICPAS.Services.DataManagers;
using SSICPAS.Services.Interfaces;
using SSICPAS.Tests.NavigationTests;
using SSICPAS.Tests.TestMocks;

namespace SSICPAS.Tests.ViewModelTests
{
    public class PublicKeyDataManagerTest
    {
        public PublicKeyDataManagerTest()
        {
            IoCContainer.RegisterInterface<ISettingsService, MockSettingsService>();
            IoCContainer.RegisterInterface<IDateTimeService, MockDateTimeService>();
            IoCContainer.RegisterInterface<INavigationTaskManager, MockNavigationTaskManager>();
        }
        [TestCase("2021-1-1 10:00", "2021-1-8 10:00")]
        [TestCase("2021-1-1 10:00", "2021-1-2 10:00:01")]
        public async Task TestFetchingPublicKey_CanCall(DateTime lastFetch, DateTime now)
        {
            var dateTimeService = IoCContainer.Resolve<IDateTimeService>() as MockDateTimeService;
            dateTimeService.Now = now;
            
            var publicKeySecureStorage = new Mock<IPublicKeyStorageRepository>();
            publicKeySecureStorage.Setup(x => x.GetPublicKeyFromSecureStorage()).ReturnsAsync(
                new PublicKeyStorageModel()
                {
                    LastFetchTimestamp = lastFetch
                });
            var publicKeyService = new Mock<IPublicKeyRepository>();
            publicKeyService.Setup(x => x.GetPublicKey()).ReturnsAsync(new ApiResponse<List<PublicKeyDto>>(new List<PublicKeyDto>()));

            var dataManager = new PublicKeyDataManager(
                publicKeyService.Object,
                IoCContainer.Resolve<ISettingsService>(),
                IoCContainer.Resolve<INavigationTaskManager>(),
                IoCContainer.Resolve<IDateTimeService>(),
                publicKeySecureStorage.Object);
            await dataManager.FetchPublicKeyFromBackend();
            publicKeyService.Verify(x => x.GetPublicKey(), Times.Once);



        }
        [TestCase("2021-1-1 10:00", "2021-1-1 11:00")]
        [TestCase("2021-1-1 10:00", "2021-1-2 10:00:00")]
        public async Task TestFetchingPublicKey_CannotCall(DateTime lastFetch, DateTime now)
        {
            var dateTimeService = IoCContainer.Resolve<IDateTimeService>() as MockDateTimeService;
            dateTimeService.Now = now;
            
            var publicKeySecureStorage = new Mock<IPublicKeyStorageRepository>();
            publicKeySecureStorage.Setup(x => x.GetPublicKeyFromSecureStorage()).ReturnsAsync(
                new PublicKeyStorageModel()
                {
                    PublicKeys = new List<PublicKeyDto>(){new PublicKeyDto() {Kid = "test", PublicKey = "test"}},
                    LastFetchTimestamp = lastFetch
                });
            var publicKeyService = new Mock<IPublicKeyRepository>();

            var dataManager = new PublicKeyDataManager(
                publicKeyService.Object,
                IoCContainer.Resolve<ISettingsService>(),
                IoCContainer.Resolve<INavigationTaskManager>(),
                IoCContainer.Resolve<IDateTimeService>(),
                publicKeySecureStorage.Object);
            await dataManager.FetchPublicKeyFromBackend();
            publicKeyService.Verify(x => x.GetPublicKey(), Times.Never);

        }
        [Test]
        public async Task TestFetchingPublicKeyFromSecureStorage()
        {
            var publicKeySecureStorage = new Mock<IPublicKeyStorageRepository>();
            publicKeySecureStorage.Setup(x => x.GetPublicKeyFromSecureStorage()).ReturnsAsync(
                new PublicKeyStorageModel()
                {
                    PublicKeys = new List<PublicKeyDto>(){new PublicKeyDto() {Kid = "test", PublicKey = "testpublickey"}},
                    LastFetchTimestamp = DateTime.Now
                });
            var publicKeyService = new Mock<IPublicKeyRepository>();

            var dataManager = new PublicKeyDataManager(
                publicKeyService.Object,
                IoCContainer.Resolve<ISettingsService>(),
                IoCContainer.Resolve<INavigationTaskManager>(),
                IoCContainer.Resolve<IDateTimeService>(),
                publicKeySecureStorage.Object);
            await dataManager.FetchPublicKeyFromBackend();
            List<string> publickeys = await dataManager.GetPublicKeyByKid("test");
            Assert.AreEqual( publickeys.First(), "testpublickey");
        }
    }
}