using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Core.Services.Model;
using SSICPAS.Models;
using SSICPAS.Tests.TestMocks;
using SSICPAS.Utils;

namespace SSICPAS.Tests.ServiceTests
{
    public class SecureStorageServiceTests
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
            IoCContainer.RegisterSingleton<ISecureStorageService<string>, MockSecureStorageService<string>>();
            IoCContainer.RegisterSingleton<ISecureStorageService<int>, MockSecureStorageService<int>>();
        }

        [TestCase(SHORT_STRING)]
        [TestCase(MEDIUM_STRING)]
        [TestCase(LONG_STRING)]
        [TestCase(VERY_LONG_STRING)]
        [TestCase(SHORT_STRING, true)]
        [TestCase(MEDIUM_STRING, true)]
        [TestCase(LONG_STRING, true)]
        [TestCase(VERY_LONG_STRING, true)]
        public async Task GetLongStringValue_ShouldBeTheSame(string str, bool withTooLargeException = false)
        {
            ISecureStorageService<string> secureStorageService = withTooLargeException ?
                new MockSecureStorageServiceThrowingOnTooLargeValue<string>():
                IoCContainer.Resolve<ISecureStorageService<string>>();
            await secureStorageService.SetLongStringValue("KEY", str);
            string longStringValue = await secureStorageService.GetLongStringValue("KEY");
            
            Assert.That(longStringValue, Is.EqualTo(str));
        }

        [TestCase(SHORT_STRING)]
        [TestCase(MEDIUM_STRING)]
        [TestCase(LONG_STRING)]
        [TestCase(VERY_LONG_STRING)]
        [TestCase(SHORT_STRING, true)]
        [TestCase(MEDIUM_STRING, true)]
        [TestCase(LONG_STRING, true)]
        [TestCase(VERY_LONG_STRING, true)]
        public async Task RemoveLongStringValue_ShouldRemoveValue(string str, bool withTooLargeException = false)
        {
            ISecureStorageService<string> secureStorageService = withTooLargeException ?
                new MockSecureStorageServiceThrowingOnTooLargeValue<string>():
                IoCContainer.Resolve<ISecureStorageService<string>>();
            ISecureStorageService<string> intSecureStorageService = IoCContainer.Resolve<ISecureStorageService<string>>();
            await secureStorageService.SetLongStringValue("KEY", str);
            await secureStorageService.RemoveLongStringValue("KEY");
            
            Assert.False(await secureStorageService.HasValue("KEY"));
            Assert.False(await intSecureStorageService.HasValue(SecureStorageHelper.NumberOfChunkKeys("KEY")));
            Assert.AreEqual(
                await intSecureStorageService.GetSecureStorageAsync(SecureStorageHelper.NumberOfChunkKeys("KEY")),
                null);
        }
        
        [TestCase(SHORT_STRING)]
        [TestCase(MEDIUM_STRING)]
        [TestCase(LONG_STRING)]
        [TestCase(VERY_LONG_STRING)]
        [TestCase(SHORT_STRING, true)]
        [TestCase(MEDIUM_STRING, true)]
        [TestCase(LONG_STRING, true)]
        [TestCase(VERY_LONG_STRING, true)]
        public async Task HasLongStingValueAsync_ShouldReturnProperValue(string str, bool withTooLargeException = false)
        {
            ISecureStorageService<string> secureStorageService = withTooLargeException ?
                new MockSecureStorageServiceThrowingOnTooLargeValue<string>():
                IoCContainer.Resolve<ISecureStorageService<string>>();
            await secureStorageService.SetLongStringValue("KEY", str);
            
            Assert.True(await secureStorageService.HasLongStringValueAsync("KEY"));

            await secureStorageService.RemoveLongStringValue("KEY");
            
            Assert.False(await secureStorageService.HasLongStringValueAsync("KEY"));
        }
        
        [TestCase(SHORT_STRING)]
        [TestCase(MEDIUM_STRING)]
        [TestCase(LONG_STRING)]
        [TestCase(VERY_LONG_STRING)]
        [TestCase(SHORT_STRING, true)]
        [TestCase(MEDIUM_STRING, true)]
        [TestCase(LONG_STRING, true)]
        [TestCase(VERY_LONG_STRING, true)]
        public async Task GetLargeValue_ShouldBeTheSame(string str, bool withTooLargeException = false)
        {
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
            
            ISecureStorageService<string> secureStorageService = withTooLargeException ?
                new MockSecureStorageServiceThrowingOnTooLargeValue<string>():
                IoCContainer.Resolve<ISecureStorageService<string>>();
            await secureStorageService.SetLargeValue("TEST", publicKeyStorageModel);

            PublicKeyStorageModel keyStorageModel = await secureStorageService.GetLargeValue<PublicKeyStorageModel>("TEST");
            
            Assert.AreEqual(keyStorageModel.LastFetchTimestamp, publicKeyStorageModel.LastFetchTimestamp);
            Assert.AreEqual(keyStorageModel.PublicKeys.Count, publicKeyStorageModel.PublicKeys.Count);
            Assert.True(keyStorageModel.PublicKeys.Select(key => key.Kid)
                .SequenceEqual(publicKeyStorageModel.PublicKeys.Select(key => key.Kid)));
            Assert.True(keyStorageModel.PublicKeys.Select(key => key.PublicKey)
                .SequenceEqual(publicKeyStorageModel.PublicKeys.Select(key => key.PublicKey)));
        }

        [TestCase(SHORT_STRING)]
        [TestCase(MEDIUM_STRING)]
        [TestCase(LONG_STRING)]
        [TestCase(VERY_LONG_STRING)]
        [TestCase(SHORT_STRING, true)]
        [TestCase(MEDIUM_STRING, true)]
        [TestCase(LONG_STRING, true)]
        [TestCase(VERY_LONG_STRING, true)]
        public async Task RemoveLargeValue_ShouldRemoveValue(string str, bool withTooLargeException = false)
        {
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
            
            ISecureStorageService<string> secureStorageService = withTooLargeException ?
                new MockSecureStorageServiceThrowingOnTooLargeValue<string>():
                IoCContainer.Resolve<ISecureStorageService<string>>();
            await secureStorageService.SetLargeValue("TEST", publicKeyStorageModel);
            
            Assert.True(await secureStorageService.HasLargeValueAsync("TEST"));

            await secureStorageService.RemoveLargeValue("TEST");
            
            Assert.False(await secureStorageService.HasLargeValueAsync("TEST"));
            Assert.IsNull(await secureStorageService.GetLargeValue<PublicKeyStorageModel>("TEST"));
        }

    }
}