using Moq;
using NUnit.Framework;
using SSICPAS.Configuration;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Services.DecoderService;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model.Converter;
using SSICPAS.Core.Services.Model.CoseModel;
using SSICPAS.Models;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Translator;
using SSICPAS.Services.WebServices;
using SSICPAS.Tests.TestMocks;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DCCVersion_1_3_0 = SSICPAS.Core.Services.Model.EuDCCModel._1._3._0;

namespace SSICPAS.Tests.ModelTests
{
    public class PassportDataTests
    {
        private HcertTokenProcessorService verifier;
        private Mock<ICertificationService> MockCertificationService { get; set; }

        public PassportDataTests()
        {
            IoCContainer.RegisterInterface<ILoggingService, MockLoggingService>();
            IoCContainer.RegisterInterface<IDateTimeService, MockDateTimeService>();

            MockCertificationService = new Mock<ICertificationService>();
            MockCertificationService.Setup(x => x.VerifyCoseSign1Object(It.IsAny<CoseSign1Object>()));

            verifier = new HcertTokenProcessorService(MockCertificationService.Object,
                IoCContainer.Resolve<ILoggingService>(),
                IoCContainer.Resolve<IDateTimeService>());
            
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
            IDCCValueSetTranslator translator = new DCCValueSetTranslator(MockRatListService.Object);
            IDCCValueSetTranslator testmanufacturerTranslator = new DigitalCovidValueSetTestAndTestManufacturerNameTranslator(MockRatListService.Object);
            verifier.SetDCCValueSetTranslator(translator, testmanufacturerTranslator).Wait();
        }

        [Test]
        public async Task ProcessDCC1_3_0_ProcessingDK3_VaccineWithLongestExpirationTimeIsKept()
        {
            string prefixCompressedCose =
                "DK3:NCFOXN%TSMAHN-H9QCGDSB5QPN9OO3:D4K1CNDC+NEM/C3K9RXRUXI.I565TR1BF/8X*G3M9CXP5+AZW4Z*AK.GNNVR*G0C7PHBO335KN QB9E3PQN:436J3/FJSA3NQN%234NJT53IMJJKBUA3OJJ8E3FIN$3HSZ4ZI00T9UKP0T9WC5PF6846A$Q$76QW6%V98T5WBI$E9$UPV3Q.GUQ$9WC5R7ACB97C968ELZ5$DP6PP5IL*PP:+P*.1D9R+Q6646G%63ZMZE9KZ56DE/.QC$Q3J62:6QW6G0A++9-G9+E93ZM$96PZ6+Q6X46+E5+DP:Q67ZMA$6BVUARI6IAHM9*7VIFT+F3423BNBO13BBAJ.AU53Q57HI7JON O7QHBNOJPLN*6B3SJ2SJ 73 IJ523L83$974E5EJOQQOGIUVB3VZ37JD:$8-BHUV0Y 3HD0X1LVS139H$ QHJP7NVXCB$ZAI984LT+LJ/9TL4T.B9NVPLEE:*P.B9C9Q4*17$PB/9BL5GFEJ/U1C9P8QRA9YKUEB9UM97H98$QP3R8BHGLVQQ94KTNRMEGLDZHN4BICFAC9B37/CW9KBU-RDVV44RRYT3/9EQ12+52OTW1OXC5Q71PJB-Y790OHS6Z:C5H10ES+DEFAST86+IE:00KJUG3";
            var result = await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            var resultDCC = result.DecodedModel as DCCVersion_1_3_0.DCCPayload;

            var vaccinesfromDK3Token = resultDCC.DCCPayloadData.DCC.Vaccinations;

            DCCVersion_1_3_0.Vaccination longestExpTimeVaccine = new DCCVersion_1_3_0.Vaccination();
            longestExpTimeVaccine.CBSDefinedExpirationTime = DateTime.MinValue;
            foreach (DCCVersion_1_3_0.Vaccination vaccine in vaccinesfromDK3Token)
            {
                if (vaccine.CBSDefinedExpirationTime > longestExpTimeVaccine.CBSDefinedExpirationTime)
                {
                    longestExpTimeVaccine = vaccine;
                }
            }

            PassportData pd = new PassportData(prefixCompressedCose, resultDCC, "someDecodedJson");
            Assert.AreEqual(longestExpTimeVaccine.CertificateId, pd.CertificateIdentifier);
        }

        [Test]
        public async Task ProcessDCC1_3_0_ProcessingDK3_VaccineWithLongestExpirationIsInThePast_NoVaccineOnMyPage()
        {
            string prefixCompressedCose =
                "DK3:NCFOXN%TSMAHN-H9QCGDSB5QPN9OO3:D4K1CNDC+NEM/C3K9RXRUXI.I565TR1BF/8X*G3M9CXP5+AZW4Z*AK.GNNVR*G0C7PHBO335KN QB9E3PQN:436J3/FJSA3NQN%234NJT53IMJJKBUA3OJJ8E3FIN$3HSZ4ZI00T9UKP0T9WC5PF6846A$Q$76QW6%V98T5WBI$E9$UPV3Q.GUQ$9WC5R7ACB97C968ELZ5$DP6PP5IL*PP:+P*.1D9R+Q6646G%63ZMZE9KZ56DE/.QC$Q3J62:6QW6G0A++9-G9+E93ZM$96PZ6+Q6X46+E5+DP:Q67ZMA$6BVUARI6IAHM9*7VIFT+F3423BNBO13BBAJ.AU53Q57HI7JON O7QHBNOJPLN*6B3SJ2SJ 73 IJ523L83$974E5EJOQQOGIUVB3VZ37JD:$8-BHUV0Y 3HD0X1LVS139H$ QHJP7NVXCB$ZAI984LT+LJ/9TL4T.B9NVPLEE:*P.B9C9Q4*17$PB/9BL5GFEJ/U1C9P8QRA9YKUEB9UM97H98$QP3R8BHGLVQQ94KTNRMEGLDZHN4BICFAC9B37/CW9KBU-RDVV44RRYT3/9EQ12+52OTW1OXC5Q71PJB-Y790OHS6Z:C5H10ES+DEFAST86+IE:00KJUG3";
            var result = await verifier.DecodePassportTokenToModel(prefixCompressedCose);
            var resultDCC = result.DecodedModel as DCCVersion_1_3_0.DCCPayload;

            var vaccinesfromDK3Token = resultDCC.DCCPayloadData.DCC.Vaccinations;

            DCCVersion_1_3_0.Vaccination longestExpTimeVaccine = new DCCVersion_1_3_0.Vaccination();
            longestExpTimeVaccine.CBSDefinedExpirationTime = DateTime.MinValue;
            foreach (DCCVersion_1_3_0.Vaccination vaccine in vaccinesfromDK3Token)
            {
                // Set expiration Date for one year ago.
                vaccine.CBSDefinedExpirationTime = vaccine.CBSDefinedExpirationTime.Value.AddDays(-365);
                if (vaccine.CBSDefinedExpirationTime > longestExpTimeVaccine.CBSDefinedExpirationTime)
                {
                    longestExpTimeVaccine = vaccine;
                }
            }

            PassportData pd = new PassportData(prefixCompressedCose, resultDCC, "someDecodedJson");
            Assert.Null(pd.CertificateIdentifier);
            Assert.False(pd.IsVaccineAvailable);
        }
    }
}
