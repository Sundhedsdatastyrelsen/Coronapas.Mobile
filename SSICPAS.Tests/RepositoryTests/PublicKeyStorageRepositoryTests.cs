using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model;
using SSICPAS.Models;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Repositories;
using SSICPAS.Tests.TestMocks;

namespace SSICPAS.Tests.RepositoryTests
{
    public class PublicKeyStorageRepositoryTests
    {
        private const string SHORT_STRING = "AAA";
        private const string MEDIUM_STRING = "Medium length string for testing of storage service chunks";
        private const string LONG_STRING =
            "NCF3TD2RBAADUSOUNQJVTN+DIM5GLP769/$H3XHS-56R5Y81Q:I*CM1Z4L$I.5E/GPWBIMDBDCA6JC*ZS2%KYZP-1S.MKLWL9TQXO2TH10VP8Q95SAF$BWTATJ1ENA+J4+0S%YKCPCARA.T8VS8BK4RXS2N62%K:XFMFGZ952W4VZ0K1H$$05QN*Y0SY0H*APZ04R2.XI5TBSYS3DJZEB+KKEIIPBJ7OIPHS*8BWFLS7B NIX3C6DBCHJQZJBQK/Q1XHM2A7H87BQA$S4PCNAHLW 70SO:GOLIROGO3T5*XK YQ *PT*OE+Q:CO3KL6T4615+*P2$P-8RYKM094+V4YC5/HQS OHCR7XB8EQEA7IB65C94JBSJDRDAI-B9JAK/BT3E%B9K.C9FED.B4JBTC9 795AO9O96LF:D9Y7MVBHB7L2FM:2CUDBQEAJJKHHGQA8RK4B/IGLA$O2%VRH:EXUSL:A7P6IDNX:U*O7DQVH 0E.NKJ232WI-0:2S-1NU5F5GNH2WS:IEZLJXLVVAH$7$/BAV8Y OL*K06H*AT6X5TY7IVU9NR6SLLBN9I3HG1PVM% BR/MLB8EJINJ6N/T%-Q 2S5$R.BT5K1+JNYNI$VSDZSG1W%IFO6T0I41WDF4T:3LH6MS1CT/A $NB1A1GJ6AQH NL0R5WDYDSY-RHBPT7VNFWY+V /BL+A 7VZXEVIOUBR6RH+*85-3MS82WA40R WAEQ7*QKZC2RA27LJM/2$TP+3KJ4LH12T-Q*1VOXF9EJYNSY9T92OJ1QQ7TX1O-2N21VQ0H-B7U0M2BQSEEWJ61JS%3EX8KM5V:6FW03MGUVAP:BQ/6EC3OADR YE3H03Z0V6S";

        private const string VERY_LONG_STRING = LONG_STRING + LONG_STRING + LONG_STRING + LONG_STRING + LONG_STRING +
                                                LONG_STRING + LONG_STRING + LONG_STRING + LONG_STRING + LONG_STRING;
        
        [SetUp]
        public void SetUp()
        {
            IoCContainer.RegisterSingleton<IDateTimeService, MockDateTimeService>();
            ((MockDateTimeService)IoCContainer.Resolve<IDateTimeService>()).Now = new DateTime(2021,06,14);
            
            IoCContainer
                .RegisterSingleton<ISecureStorageService<int>,
                    MockSecureStorageService<int>>();
            IoCContainer
                .RegisterSingleton<ISecureStorageService<DateTime>,
                    MockSecureStorageService<DateTime>>();
            IoCContainer
                .RegisterSingleton<ISecureStorageService<string>,
                    MockSecureStorageService<string>>();
            IoCContainer
                .RegisterSingleton<ISecureStorageService<PublicKeyStorageModel>,
                    MockSecureStorageServiceThrowingOnTooLargeValue<PublicKeyStorageModel>>();
            IoCContainer.RegisterSingleton<IPublicKeyStorageRepository, PublicKeyStorageRepository>();
        }

        [TestCase(SHORT_STRING)]
        [TestCase(MEDIUM_STRING)]
        [TestCase(LONG_STRING)]
        [TestCase(VERY_LONG_STRING)]
        public async Task SavePublicKeyToSecureStorage_ShouldSaveCorrectData(string str)
        {
            IPublicKeyStorageRepository repository = IoCContainer.Resolve<IPublicKeyStorageRepository>();
            await repository.DeletePublicKeyFromSecureStorage();

            PublicKeyStorageModel publicKeyStorageModel = new PublicKeyStorageModel
            {
                LastFetchTimestamp = new DateTime(2021, 07,07),
                PublicKeys = new List<PublicKeyDto>
                {
                    new PublicKeyDto
                    {
                        Kid = "kid_1",
                        PublicKey = str
                    },
                    new PublicKeyDto
                    {
                        Kid = "kid_2",
                        PublicKey = str
                    },
                }
            };

            await repository.SavePublicKeyToSecureStorage(publicKeyStorageModel);

            PublicKeyStorageModel data = await repository.GetPublicKeyFromSecureStorage();
            
            Assert.AreNotEqual(data.LastFetchTimestamp, default(DateTime));
            Assert.AreEqual(
                data.LastFetchTimestamp,
                new DateTime(2021, 07,07));
            Assert.AreEqual(
                data.PublicKeys.Count,
                2);
            Assert.AreEqual(data.PublicKeys[0].Kid, publicKeyStorageModel.PublicKeys[0].Kid);
            Assert.AreEqual(data.PublicKeys[1].Kid, publicKeyStorageModel.PublicKeys[1].Kid);
            Assert.AreEqual(data.PublicKeys[0].PublicKey, publicKeyStorageModel.PublicKeys[0].PublicKey);
            Assert.AreEqual(data.PublicKeys[1].PublicKey, publicKeyStorageModel.PublicKeys[1].PublicKey);
        }

        [TestCase(SHORT_STRING)]
        [TestCase(MEDIUM_STRING)]
        [TestCase(LONG_STRING)]
        [TestCase(VERY_LONG_STRING)]
        public async Task GetPublicKeyFromSecureStorage_ShouldReturnCorrectData(string str)
        {
            IPublicKeyStorageRepository repository = IoCContainer.Resolve<IPublicKeyStorageRepository>();
            await repository.DeletePublicKeyFromSecureStorage();

            PublicKeyStorageModel publicKeyStorageModel = new PublicKeyStorageModel
            {
                LastFetchTimestamp = new DateTime(2021, 07,07),
                PublicKeys = new List<PublicKeyDto>
                {
                    new PublicKeyDto
                    {
                        Kid = "kid_1",
                        PublicKey = str
                    },
                    new PublicKeyDto
                    {
                        Kid = "kid_2",
                        PublicKey = str
                    },
                }
            };

            await repository.SavePublicKeyToSecureStorage(publicKeyStorageModel);
            
            PublicKeyStorageModel publicKeyFromSecureStorage = await repository.GetPublicKeyFromSecureStorage();
            
            Assert.AreEqual(publicKeyFromSecureStorage.LastFetchTimestamp, new DateTime(2021, 07,07));
            Assert.AreEqual(publicKeyFromSecureStorage.PublicKeys.Count, 2);
            Assert.AreEqual(publicKeyFromSecureStorage.PublicKeys[0].Kid, "kid_1");
            Assert.AreEqual(publicKeyFromSecureStorage.PublicKeys[1].Kid, "kid_2");
            Assert.AreEqual(publicKeyFromSecureStorage.PublicKeys[0].PublicKey, str);
            Assert.AreEqual(publicKeyFromSecureStorage.PublicKeys[1].PublicKey, str);
        }

        [TestCase(SHORT_STRING)]
        [TestCase(MEDIUM_STRING)]
        [TestCase(LONG_STRING)]
        [TestCase(VERY_LONG_STRING)]
        public async Task DeletePublicKeyFromSecureStorage_ShouldRemoveData(string str)
        {
            IPublicKeyStorageRepository repository = IoCContainer.Resolve<IPublicKeyStorageRepository>();
            PublicKeyStorageModel publicKeyStorageModel = new PublicKeyStorageModel
            {
                LastFetchTimestamp = new DateTime(2021, 07,07),
                PublicKeys = new List<PublicKeyDto>
                {
                    new PublicKeyDto
                    {
                        Kid = "kid_1",
                        PublicKey = str
                    },
                    new PublicKeyDto
                    {
                        Kid = "kid_2",
                        PublicKey = str
                    },
                }
            };
            await repository.SavePublicKeyToSecureStorage(publicKeyStorageModel);
            
            PublicKeyStorageModel publicKeyFromSecureStorage = await repository.GetPublicKeyFromSecureStorage();
            
            Assert.AreEqual(publicKeyFromSecureStorage.LastFetchTimestamp, new DateTime(2021, 07,07));
            Assert.AreEqual(publicKeyFromSecureStorage.PublicKeys.Count, 2);
            Assert.AreEqual(publicKeyFromSecureStorage.PublicKeys[0].Kid, "kid_1");
            Assert.AreEqual(publicKeyFromSecureStorage.PublicKeys[1].Kid, "kid_2");
            Assert.AreEqual(publicKeyFromSecureStorage.PublicKeys[0].PublicKey, str);
            Assert.AreEqual(publicKeyFromSecureStorage.PublicKeys[1].PublicKey, str);
            
            await repository.DeletePublicKeyFromSecureStorage();
            
            publicKeyFromSecureStorage = await repository.GetPublicKeyFromSecureStorage();

            Assert.IsEmpty(publicKeyFromSecureStorage.PublicKeys);
            Assert.AreEqual(publicKeyFromSecureStorage.LastFetchTimestamp, default(DateTime));
        }
    }
}