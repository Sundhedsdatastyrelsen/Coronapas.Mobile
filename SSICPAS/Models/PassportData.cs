using System;
using System.Globalization;
using System.Linq;
using SSICPAS.Configuration;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model.DK;
using DCCVersion_1_0_x = SSICPAS.Core.Services.Model.EuDCCModel._1._0._x;
using DCCVersion_1_3_0 = SSICPAS.Core.Services.Model.EuDCCModel._1._3._0;

namespace SSICPAS.Models
{
    public class PassportData
    {

        public PassportData()
        {
        }

        public PassportData(string token, DK2Payload payload)
        {
            SecureToken = token;
            CertificateValidFrom = payload.iat;
            CertificateValidTo = payload.exp;
            FullName = payload.LegalName;
            DateOfBirth = payload.DateOfBirth.ToString(CultureInfo.InvariantCulture);
        }

        public PassportData(string token, DK1Payload payload)
        {
            SecureToken = token;
            CertificateValidFrom = payload.iat;
            CertificateValidTo = payload.exp;
        }

        public PassportData(string token, ITokenPayload payload, string decodedJson)
        {
            switch (payload is DCCVersion_1_0_x.DCCPayload)
            {
                case true:
                    ProcessDCC1_1_x(token, (DCCVersion_1_0_x.DCCPayload)payload, decodedJson);
                    break;
                default:
                    ProcessDCC1_3_0(token, (DCCVersion_1_3_0.DCCPayload)payload, decodedJson);
                    break;
            }
        }

        public void ProcessDCC1_3_0(string token, DCCVersion_1_3_0.DCCPayload payload, string decodedJson)
        {
            bool isDKInfoToken = !string.IsNullOrEmpty(token) && token.Substring(0, 3).Equals("DK3");

            SecureToken = token;
            DecodedJson = decodedJson;
            bool dateParsedCorrectly = DateTime.TryParse(payload.DCCPayloadData.DCC.DateOfBirth, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dateOfBirth);
            DateOfBirth = dateParsedCorrectly ? dateOfBirth.ToString(CultureInfo.InvariantCulture) : payload.DCCPayloadData.DCC.DateOfBirth;
            FirstName = payload.DCCPayloadData.DCC.PersonName.GivenName;
            LastName = payload.DCCPayloadData.DCC.PersonName.FamilyName;
            FullName = isDKInfoToken ? payload.DCCPayloadData.DCC.PersonName.FullNameWithSpace : payload.DCCPayloadData.DCC.PersonName.FullNameTransliteratedReversedWithComma;

            CertificateValidTo = payload.ExpirationTime;
            CertificateValidFrom = payload.IssueAt;

            if (payload.DCCPayloadData.DCC != null)
            {
                if (payload.DCCPayloadData.DCC.Vaccinations != null && payload.DCCPayloadData.DCC.Vaccinations.Any())
                {
                    DCCVersion_1_3_0.Vaccination vaccine;

                    if (isDKInfoToken)
                    {
                        vaccine = payload.DCCPayloadData.DCC.Vaccinations
                            .OrderByDescending(x => x.CBSDefinedExpirationTime)
                            .FirstOrDefault(x => x.CBSDefinedExpirationTime > IoCContainer.Resolve<IDateTimeService>().Now);
                    }
                    else
                    {
                        vaccine = payload.DCCPayloadData.DCC.Vaccinations.OrderByDescending(x => x.DoseNumber).First();
                    }

                    SetVaccinePassportDataModel(vaccine);
                }
                if (payload.DCCPayloadData.DCC.Tests != null && payload.DCCPayloadData.DCC.Tests.Any())
                {
                    DCCVersion_1_3_0.TestResult testResult;

                    if (isDKInfoToken)
                    {
                        testResult = payload.DCCPayloadData.DCC.Tests
                            .OrderByDescending(x => x.CBSDefinedExpirationTime)
                            .FirstOrDefault(x => x.CBSDefinedExpirationTime > IoCContainer.Resolve<IDateTimeService>().Now);
                    }
                    else
                    {
                        testResult = payload.DCCPayloadData.DCC.Tests.First();
                    }

                    SetTestPassportDataModel(testResult);
                }
                if (payload.DCCPayloadData.DCC.Recovery != null && payload.DCCPayloadData.DCC.Recovery.Any())
                {
                    var recovery = payload.DCCPayloadData.DCC.Recovery.OrderBy(x => x.ValidFrom).First();

                    SetRecoveryPassportDataModel(recovery);
                }
            }
        }

        private void SetVaccinePassportDataModel(DCCVersion_1_3_0.Vaccination vaccine)
        {
            if (vaccine == null) return;

            IsVaccineAvailable = true;
            MedicinialProduct = vaccine.VaccineMedicinalProduct ?? "-";
            Disease = vaccine.Disease ?? "-";
            MarketingAuthorizationHolder = vaccine.Manufacturer ?? "-";
            VaccinationType = vaccine.VaccineProphylaxis ?? "-";
            TotalNumberOfDose = vaccine.TotalSeriesOfDose.ToString() ?? "-";
            VaccinationCountry = vaccine.CountryOfVaccination ?? "-";
            VaccinationCountryDanish = vaccine.CountryOfVaccinationDK ?? "-";
            CertificateIdentifier = vaccine.CertificateId ?? "-";
            CertificateIssuer = vaccine.CertificateIssuer ?? "-";
            DoseNumber = vaccine.DoseNumber;
            VaccinationDate = DateTime.Parse(vaccine.DateOfVaccination, CultureInfo.InvariantCulture);
        }

        private void SetTestPassportDataModel(DCCVersion_1_3_0.TestResult testResult)
        {
            if (testResult == null) return;

            IsTestAvailable = true;
            Disease = testResult.Disease ?? "-";
            TypeOfTest = testResult.TypeOfTest ?? "-"; ;
            NAATestName = testResult.NAATestName ?? "-";
            TestName = testResult.TestManufacturer?.TestName ?? "-"; ;
            TestManufacturer = testResult.TestManufacturer?.ManufacturerName ?? "-";
            SampleCollectedTime = testResult.SampleCollectedTime;
            Result = testResult.ResultOfTest ?? "-"; ;
            TestCenter = testResult.TestingCentre ?? "-"; ;
            TestCountry = testResult.CountryOfTest ?? "-";
            TestCountryDanish = testResult.CountryOfTestDK ?? "-";
            CertificateIdentifier = testResult.CertificateId ?? "-";
            CertificateIssuer = testResult.CertificateIssuer ?? "-";
        }

        private void SetRecoveryPassportDataModel(DCCVersion_1_3_0.Recovery recovery)
        {
            if (recovery == null) return;

            IsRecoveryAvailable = true;
            RecoveryDisease = recovery.Disease ?? "-";
            DateFirstPositiveTest = DateTime.Parse(recovery.DateOfFirstPositiveResult, CultureInfo.InvariantCulture);
            CountryOfTest = recovery.CountryOfTest ?? "-";
            CountryOfTestDanish = recovery.CountryOfTestDK ?? "-";
            CertificateIdentifier = recovery.CertificateId ?? "-";
            CertificateIssuer = recovery.CertificateIssuer ?? "-";
            RecoveryValidFrom = DateTime.Parse(recovery.ValidFrom, CultureInfo.InvariantCulture);
            RecoveryValidTo = DateTime.Parse(recovery.ValidTo, CultureInfo.InvariantCulture);
        }

        public void ProcessDCC1_1_x(string token, DCCVersion_1_0_x.DCCPayload payload, string decodedJson)
        {
            SecureToken = token;
            DecodedJson = decodedJson;
            DateOfBirth = payload.DCCPayloadData.DCC.DateOfBirth.ToString(CultureInfo.InvariantCulture);
            FirstName = payload.DCCPayloadData.DCC.PersonName.GivenName;
            LastName = payload.DCCPayloadData.DCC.PersonName.FamilyName;
            FullName = payload.DCCPayloadData.DCC.PersonName.FullNameTransliteratedReversedWithComma;

            CertificateValidTo = payload.ExpirationTime;
            CertificateValidFrom = payload.IssueAt;
            if (payload.DCCPayloadData.DCC != null)
            {
                if (payload.DCCPayloadData.DCC.Vaccinations?.Any() ?? false)
                {
                    var latestVaccination = payload.DCCPayloadData.DCC.Vaccinations.OrderByDescending(x => x.DoseNumber).First();
                    IsVaccineAvailable = true;
                    MedicinialProduct = latestVaccination.VaccineMedicinalProduct ?? "-";
                    Disease = latestVaccination.Disease ?? "-";
                    MarketingAuthorizationHolder = latestVaccination.Manufacturer ?? "-";
                    VaccinationType = latestVaccination.VaccineProphylaxis ?? "-";
                    TotalNumberOfDose = latestVaccination?.TotalSeriesOfDose.ToString() ?? "-";
                    VaccinationCountry = latestVaccination?.CountryOfVaccination ?? "-";
                    VaccinationCountryDanish = latestVaccination?.CountryOfVaccinationDK ?? "-";
                    CertificateIdentifier = latestVaccination.CertificateId ?? "-";
                    CertificateIssuer = latestVaccination.CertificateIssuer ?? "-";
                    DoseNumber = latestVaccination.DoseNumber;
                    VaccinationDate = latestVaccination.DateOfVaccination;
                }
                if (payload.DCCPayloadData.DCC.Tests != null && payload.DCCPayloadData.DCC.Tests.Any())
                {
                    var result = payload.DCCPayloadData.DCC.Tests.First();
                    IsTestAvailable = true;
                    Disease = result.Disease ?? "-";
                    TypeOfTest = result.TypeOfTest ?? "-"; ;
                    NAATestName = result.NAATestName ?? "-";
                    TestName = result.TestManufacturer?.TestName ?? "-";
                    TestManufacturer = result.TestManufacturer?.ManufacturerName ?? "-";
                    SampleCollectedTime = result.SampleCollectedTime;
                    Result = result.ResultOfTest ?? "-"; ;
                    TestCenter = result.TestingCentre ?? "-"; ;
                    TestCountry = result.CountryOfTest ?? "-";
                    TestCountryDanish = result.CountryOfTestDK ?? "-";
                    CertificateIdentifier = result.CertificateId ?? "-";
                    CertificateIssuer = result.CertificateIssuer ?? "-";
                }
                if (payload.DCCPayloadData.DCC.Recovery != null && payload.DCCPayloadData.DCC.Recovery.Any())
                {
                    var recovery = payload.DCCPayloadData.DCC.Recovery.OrderBy(x => x.ValidFrom).First();
                    IsRecoveryAvailable = true;
                    RecoveryDisease = recovery.Disease ?? "-";
                    DateFirstPositiveTest = recovery.DateOfFirstPositiveResult;
                    CountryOfTest = recovery.CountryOfTest ?? "-";
                    CountryOfTestDanish = recovery.CountryOfTestDK ?? "-";
                    CertificateIdentifier = recovery.CertificateId ?? "-";
                    CertificateIssuer = recovery.CertificateIssuer ?? "-";
                    RecoveryValidFrom = recovery.ValidFrom;
                    RecoveryValidTo = recovery.ValidTo;
                }
            }
        }
        public string DecodedJson { get; set; }

        public string SecureToken { get; set; }

        public string? DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }

        public string MedicinialProduct { get; set; }
        public string MarketingAuthorizationHolder { get; set; }
        public string VaccinationType { get; set; }
        public string TotalNumberOfDose { get; set; }
        public int DoseNumber { get; set; }
        public DateTime? VaccinationDate { get; set; }
        public string VaccinationCountry { get; set; }
        public string VaccinationCountryDanish { get; set; }
        [Obsolete]
        public string TestName { get; set; }
        public string TypeOfTest { get; set; }
        public string Result { get; set; }
        public DateTime? SampleCollectedTime { get; set; }
        public string Disease { get; set; }
        public string TestManufacturer { get; set; }
        public string SampleOrigin { get; set; }
        public string TestCountry { get; set; }
        public string TestCountryDanish { get; set; }
        public string TestCenter { get; set; }
        public string NAATestName { get; set; }

        public string RecoveryDisease { get; set; }
        public DateTime? DateFirstPositiveTest { get; set; }
        public string CountryOfTest { get; set; }
        public string CountryOfTestDanish { get; set; }
        public DateTime? RecoveryValidTo { get; set; }
        public DateTime? RecoveryValidFrom { get; set; }

        public string CertificateIssuer { get; set; }
        public string CertificateIdentifier { get; set; }
        public DateTime? CertificateValidFrom { get; set; }
        public DateTime? CertificateValidTo { get; set; }

        public bool IsVaccineAvailable { get; set; } = false;
        public bool IsTestAvailable { get; set; } = false;
        public bool IsRecoveryAvailable { get; set; } = false;
    }
}
