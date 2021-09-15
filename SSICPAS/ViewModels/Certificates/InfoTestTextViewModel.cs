using System;
using System.Globalization;
using SSICPAS.Configuration;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;
using SSICPAS.Enums;
using SSICPAS.Services;
using SSICPAS.Services.Translator;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Base;
using SSICPAS.ViewModels.Models;

namespace SSICPAS.ViewModels.Certificates
{
    public class InfoTestTextViewModel : BaseViewModel
    {
        private LanguageSelection _selectedLanguage => LocaleService.Current.GetLanguage();
        private bool EnglishSelected => _selectedLanguage == LanguageSelection.English;

        #region Bindable properties
        public string NegativeTestHeaderText => ShowTextInEnglish ? "INTERNATIONAL_NEGATIVE_TEST_HEADER_TEXT".Translate() : "NEGATIVE_TEST_HEADER_TEXT".Translate();
        public string NegativeTestCertificateHeaderText => ShowTextInEnglish ? "INTERNATIONAL_INFO_CERTIFICATE_HEADER_TEXT".Translate() : "INFO_CERTIFICATE_HEADER_TEXT".Translate();
        
        public string TestDateOfTestTextInEnglish => "INFO_TEST_DATE_OF_TEST_TEXT_EN".Translate();
        public string TestDateOfTestTextInDanishOrAppLanguage => ShowCertificate ? "INFO_TEST_DATE_OF_TEST_TEXT_DK".Translate() : "NEGATIVE_TEST_CONDUCTED_TEXT".Translate();

        public string TestResultOfTestTextInEnglish => "INFO_TEST_RESULT_OF_TEST_TEXT_EN".Translate();
        public string TestResultOfTestTextInDanishOrAppLanguage => ShowCertificate ? "INFO_TEST_RESULT_OF_TEST_TEXT_DK".Translate() : "NEGATIVE_TEST_RESULT_TEXT".Translate();

        public string TestTypeOfTestTextInEnglish => "INFO_TEST_TYPE_OF_TEST_TEXT_EN".Translate();
        public string TestTypeOfTestTextInDanishOrAppLanguage => ShowCertificate ? "INFO_TEST_TYPE_OF_TEST_TEXT_DK".Translate() : "NEGATIVE_TEST_TYPE_OF_TEST_TEXT".Translate();

        public string TestDateAndTimeTextInEnglish => "INFO_TEST_DATE_AND_TIME_OF_TEST_TEXT_EN".Translate();
        public string TestDateAndTimeTextInDanishOrAppLanguage => ShowCertificate ? "INFO_TEST_DATE_AND_TIME_OF_TEST_TEXT_DK".Translate() : "NEGATIVE_TEST_SAMPLE_COLLECTED_DATE".Translate();

        public string TestNameTextInEnglish => "INFO_TEST_TEST_NAME_TEXT_EN".Translate();
        public string TestNameTextInDanishOrAppLanguage => ShowCertificate ? "INFO_TEST_TEST_NAME_TEXT_DK".Translate() : "NEGATIVE_TEST_TEST_NAME".Translate();

        public string TestManufacturerTextInEnglish => "INFO_TEST_TEST_MANUFACTURER_TEXT_EN".Translate();
        public string TestManufacturerTextInDanishOrAppLanguage => ShowCertificate ? "INFO_TEST_TEST_MANUFACTURER_TEXT_DK".Translate() : "NEGATIVE_TEST_MANUFACTURER_TEXT".Translate();

        public string TestDiseaseTextInEnglish => "INFO_TEST_DISEASE_TEXT_EN".Translate();
        public string TestDiseaseTextInDanishOrAppLanguage => ShowCertificate ? "INFO_TEST_DISEASE_TEXT_DK".Translate() : "NEGATIVE_TEST_DISEASE_TEXT".Translate();

        public string TestTestingCentreTextInEnglish => "INFO_TEST_TESTING_CENTRE_TEXT_EN".Translate();
        public string TestTestingCentreTextInDanishOrAppLanguage => ShowCertificate ? "INFO_TEST_TESTING_CENTRE_TEXT_DK".Translate() : "NEGATIVE_TEST_TEST_CENTER".Translate();

        public string TestCountryTextInEnglish => "INFO_TEST_COUNTRY_TEXT_EN".Translate();
        public string TestCountryTextInDanishOrAppLanguage => ShowCertificate ? "INFO_TEST_COUNTRY_TEXT_DK".Translate() : "NEGATIVE_TEST_TESTING_COUNTRY_TEXT".Translate();

        public string TestCertificateIssuerTextInEnglish => "INFO_CERTIFICATE_ISSUER_TEXT_EN".Translate();
        public string TestCertificateIssuerTextInDanishOrAppLanguage => ShowCertificate ? "INFO_CERTIFICATE_ISSUER_TEXT_DK".Translate() : "INFO_CERTIFICATE_ISSUER_TEXT".Translate();

        public string TestPassportNumberTextInEnglish => "INFO_CERTIFICATE_PASSPORT_NUMBER_TEXT_EN".Translate();
        public string TestPassportNumberTextInDanishOrAppLanguage => ShowCertificate ? "INFO_CERTIFICATE_PASSPORT_NUMBER_TEXT_DK".Translate() : "INFO_CERTIFICATE_PASSPORT_NUMBER_TEXT".Translate();
        #endregion

        public string SampleConductedTimeHoursText => ShowTextInEnglish ? "INTERNATIONAL_SAMPLE_CONDUCTED_TIME_HOURS_TEXT".Translate() : "SAMPLE_CONDUCTED_TIME_HOURS_TEXT".Translate();
        public string SampleConductedTimeMinutesText => ShowTextInEnglish ? "INTERNATIONAL_SAMPLE_CONDUCTED_TIME_MINUTES_TEXT".Translate() : "SAMPLE_CONDUCTED_TIME_MINUTES_TEXT".Translate();
        public string SampleCounductedAgoText => ShowTextInEnglish ? "INTERNATIONAL_SAMPLE_CONDUCTED_TIME_AGO_TEXT".Translate() : "SAMPLE_CONDUCTED_TIME_AGO_TEXT".Translate();

        public string SampleConductedTimeHoursTextInEnglish => "SAMPLE_CONDUCTED_TIME_HOURS_TEXT_EN".Translate();
        public string SampleConductedTimeHoursTextInDanishOrAppLanguage => ShowCertificate ? "SAMPLE_CONDUCTED_TIME_HOURS_TEXT_DK".Translate() : "SAMPLE_CONDUCTED_TIME_HOURS_TEXT".Translate();

        public string SampleConductedTimeMinutesTextInEnglish => "SAMPLE_CONDUCTED_TIME_MINUTES_TEXT_EN".Translate();
        public string SampleConductedTimeMinutesTextInDanishOrAppLanguage => ShowCertificate ? "SAMPLE_CONDUCTED_TIME_MINUTES_TEXT_DK".Translate() : "SAMPLE_CONDUCTED_TIME_MINUTES_TEXT".Translate();

        public string SampleCounductedAgoTextInEnglish => "SAMPLE_CONDUCTED_TIME_AGO_TEXT_EN".Translate();
        public string SampleCounductedAgoTextInDanishOrAppLanguage => ShowCertificate ? "SAMPLE_CONDUCTED_TIME_AGO_TEXT_DK".Translate() : "SAMPLE_CONDUCTED_TIME_AGO_TEXT".Translate();

        public string TestCertificatIdentifierValue => ShowCertificate ? PassportViewModel.PassportData.CertificateIdentifier : "-";
        public string TestHeaderValue => ShowHeader ? ToLocaleTypeOfTest(PassportViewModel.PassportData.TypeOfTest, ShowCertificate) : "-";
        public string TestCenter => PassportViewModel.PassportData.TestCenter;

        public SinglePassportViewModel PassportViewModel { get; set; } = new SinglePassportViewModel();

        public string TestConductedValueInEnglish => TimeSinceSampleTime(PassportViewModel.PassportData.SampleCollectedTime ?? DateTime.MinValue, true);
        public string TestConductedValueInDanishOrAppLanguage => TimeSinceSampleTime(PassportViewModel.PassportData.SampleCollectedTime ?? DateTime.MinValue);
        public string TestResultValueInEnglish => PassportViewModel.PassportData.Result;
        public string TestResultValueInDanishOrAppLanguage => ToLocaleTestResult(PassportViewModel.PassportData.Result, ShowCertificate ? false : (ShowTextInEnglish || EnglishSelected));
        public string TestTypeOfTestValueInEnglish => PassportViewModel.PassportData.TypeOfTest;
        public string TestTypeOfTestValueInDanishOrAppLanguage => ToLocaleTypeOfTest(PassportViewModel.PassportData.TypeOfTest, ShowCertificate ? false : (ShowTextInEnglish || EnglishSelected));
        public string TestSampleCollectionDateTimeValueInEnglish => TestSampleCollectionDateValueInEnglish + ", " + TestSampleCollectionTimeValueInEnglish;
        public string TestSampleCollectionDateTimeValueInDanishOrAppLanguage => TestSampleCollectionDateValueInDanishOrAppLanguage + ", " + TestSampleCollectionTimeValueInDanishOrAppLanguage;
        public string TestSampleCollectionDateValueInEnglish => PassportViewModel.PassportData.SampleCollectedTime?.ToLocaleDateFormat(true, true) ?? "-";
        public string TestSampleCollectionDateValueInDanishOrAppLanguage => PassportViewModel.PassportData.SampleCollectedTime?.ToLocalTime().LocaleFormatDate(ShowCertificate) ?? "-";
        public string TestSampleCollectionTimeValueInEnglish => PassportViewModel.PassportData.SampleCollectedTime?.LocaleFormatTime(true, true) ?? "-";
        public string TestSampleCollectionTimeValueInDanishOrAppLanguage => PassportViewModel.PassportData.SampleCollectedTime?.ToLocalTime().LocaleFormatTime(ShowCertificate) ?? "-";
        public string TestNameValueInEnglish =>
            string.IsNullOrEmpty(PassportViewModel.PassportData.TestName) || PassportViewModel.PassportData.TestName == "-" ?
            "TEST_INFORMATION_UNAVAILABLE_EN".Translate() : PassportViewModel.PassportData.TestName;
        public string TestNameValueInDanishOrAppLanguage =>
            string.IsNullOrEmpty(PassportViewModel.PassportData.TestName) || PassportViewModel.PassportData.TestName == "-" ?
            (ShowCertificate ? "TEST_INFORMATION_UNAVAILABLE_DK".Translate() : "TEST_INFORMATION_UNAVAILABLE".Translate()) : PassportViewModel.PassportData.TestName;
        public string TestManufacturerValueInEnglish => PassportViewModel.PassportData.TestManufacturer == "-" ?
            "TEST_INFORMATION_UNAVAILABLE_EN".Translate() : PassportViewModel.PassportData.TestManufacturer;
        public string TestManufacturerValueInDanishOrAppLanguage => PassportViewModel.PassportData.TestManufacturer == "-" ?
            (ShowCertificate ? "TEST_INFORMATION_UNAVAILABLE_DK".Translate() : "TEST_INFORMATION_UNAVAILABLE".Translate()) : PassportViewModel.PassportData.TestManufacturer;
        public string TestDiseaseValueInEnglish => PassportViewModel.PassportData.Disease;
        public string TestDiseaseValueInDanishOrAppLanguage => ToLocaleDisease(PassportViewModel.PassportData.Disease, ShowCertificate ? false : (ShowTextInEnglish || EnglishSelected));
        public string TestTestingCentreCountryValueInEnglish => PassportViewModel.PassportData.TestCountry;
        public string TestTestingCentreCountryValueInDanishOrAppLanguage => (ShowTextInEnglish || EnglishSelected) && !ShowCertificate ? PassportViewModel.PassportData.TestCountry : PassportViewModel.PassportData.TestCountryDanish;
        public string TestCertificateIssuerValueInEnglish => PassportViewModel.PassportData.CertificateIssuer;
        public string TestCertificateIssuerValueInDanishOrAppLanguage => ToLocaleCertificateIssuer(PassportViewModel.PassportData.CertificateIssuer, ShowCertificate ? false : (ShowTextInEnglish || EnglishSelected));

        string ToLocaleTestResult(string result, bool ShouldUseEnglish) => DCCValueSetTranslator.ToLocale(result, DCCValueSetEnum.TestResult, ShouldUseEnglish);
        string ToLocaleTypeOfTest(string typeOfTest, bool ShouldUseEnglish) => DCCValueSetTranslator.ToLocale(typeOfTest, DCCValueSetEnum.TypeOfTest, ShouldUseEnglish);
        string ToLocaleDisease(string disease, bool ShouldUseEnglish) => DCCValueSetTranslator.ToLocale(disease, DCCValueSetEnum.Disease, ShouldUseEnglish);
        string ToLocaleCertificateIssuer(string certIssuer, bool ShouldUseEnglish) => DCCValueSetTranslator.ToLocale(certIssuer, DCCValueSetEnum.CertificateIssuer, ShouldUseEnglish);

        public bool ShouldUseDanishForAccessibility => CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "da" || !ShowCertificate;
        public bool ShowCertificate { get; set; }
        public bool ShowHeader { get; set; }
        public bool ShowTextInEnglish { get; set; }
        public bool OnlyOneEUPassport { get; set; }

        public InfoTestTextViewModel()
        {
        }

        public void UpdateView()
        {
            OnPropertyChanged(nameof(NegativeTestHeaderText));
            OnPropertyChanged(nameof(NegativeTestCertificateHeaderText));
            OnPropertyChanged(nameof(SampleConductedTimeMinutesText));
            OnPropertyChanged(nameof(SampleConductedTimeHoursText));
            OnPropertyChanged(nameof(SampleCounductedAgoText));

            OnPropertyChanged(nameof(TestDateOfTestTextInEnglish));
            OnPropertyChanged(nameof(TestDateOfTestTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(TestResultOfTestTextInEnglish));
            OnPropertyChanged(nameof(TestResultOfTestTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(TestTypeOfTestTextInEnglish));
            OnPropertyChanged(nameof(TestTypeOfTestTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(TestDateAndTimeTextInEnglish));
            OnPropertyChanged(nameof(TestDateAndTimeTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(TestNameTextInEnglish));
            OnPropertyChanged(nameof(TestNameTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(TestManufacturerTextInEnglish));
            OnPropertyChanged(nameof(TestManufacturerTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(TestDiseaseTextInEnglish));
            OnPropertyChanged(nameof(TestDiseaseTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(TestTestingCentreTextInEnglish));
            OnPropertyChanged(nameof(TestTestingCentreTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(TestCountryTextInEnglish));
            OnPropertyChanged(nameof(TestCountryTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(TestCertificateIssuerTextInEnglish));
            OnPropertyChanged(nameof(TestCertificateIssuerTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(TestPassportNumberTextInEnglish));
            OnPropertyChanged(nameof(TestPassportNumberTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(SampleConductedTimeHoursTextInEnglish));
            OnPropertyChanged(nameof(SampleConductedTimeHoursTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(SampleConductedTimeHoursTextInEnglish));
            OnPropertyChanged(nameof(SampleConductedTimeMinutesTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(SampleConductedTimeHoursTextInEnglish));
            OnPropertyChanged(nameof(SampleCounductedAgoTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(ShowCertificate));
            OnPropertyChanged(nameof(PassportViewModel));
            OnPropertyChanged(nameof(ShowHeader));
            OnPropertyChanged(nameof(ShouldUseDanishForAccessibility));

            if (PassportViewModel == null) return;
            OnPropertyChanged(nameof(TestCenter));

            OnPropertyChanged(nameof(TestNameValueInEnglish));
            OnPropertyChanged(nameof(TestNameValueInDanishOrAppLanguage));

            OnPropertyChanged(nameof(TestManufacturerValueInEnglish));
            OnPropertyChanged(nameof(TestManufacturerValueInDanishOrAppLanguage));

            OnPropertyChanged(nameof(TestConductedValueInEnglish));
            OnPropertyChanged(nameof(TestConductedValueInDanishOrAppLanguage));

            OnPropertyChanged(nameof(TestResultValueInEnglish));
            OnPropertyChanged(nameof(TestResultValueInDanishOrAppLanguage));

            OnPropertyChanged(nameof(TestTypeOfTestValueInEnglish));
            OnPropertyChanged(nameof(TestTypeOfTestValueInDanishOrAppLanguage));

            OnPropertyChanged(nameof(TestSampleCollectionDateTimeValueInEnglish));
            OnPropertyChanged(nameof(TestSampleCollectionDateTimeValueInEnglish));

            OnPropertyChanged(nameof(TestSampleCollectionTimeValueInEnglish));
            OnPropertyChanged(nameof(TestSampleCollectionTimeValueInDanishOrAppLanguage));

            OnPropertyChanged(nameof(TestSampleCollectionDateTimeValueInEnglish));
            OnPropertyChanged(nameof(TestSampleCollectionDateTimeValueInDanishOrAppLanguage));

            OnPropertyChanged(nameof(TestDiseaseValueInEnglish));
            OnPropertyChanged(nameof(TestDiseaseValueInDanishOrAppLanguage));

            OnPropertyChanged(nameof(TestTestingCentreCountryValueInEnglish));
            OnPropertyChanged(nameof(TestTestingCentreCountryValueInDanishOrAppLanguage));

            if (ShowCertificate)
            {
                OnPropertyChanged(nameof(TestCertificateIssuerValueInEnglish));
                OnPropertyChanged(nameof(TestCertificateIssuerValueInDanishOrAppLanguage));
                OnPropertyChanged(nameof(TestCertificatIdentifierValue));
            }
            if(ShowHeader)
            {
                OnPropertyChanged(nameof(TestHeaderValue));
            }

        }

        private string TimeSinceSampleTime(DateTime SampleCollectedTime, bool ShouldForceEnglish = false)
        {
            if (SampleCollectedTime == DateTime.MinValue)
            {
                return "-";
            }

            HoursAndMinutesModel diff = HoursAndMinutesDifference(SampleCollectedTime);

            string returnValue;

            if (diff.Hours > 0)
            {
                returnValue = $"{diff.Hours} " +
                    $"{(ShouldForceEnglish ? SampleConductedTimeHoursTextInEnglish : SampleConductedTimeHoursTextInDanishOrAppLanguage)} " +
                    $"{diff.Minutes} " +
                    $"{(ShouldForceEnglish ? SampleConductedTimeMinutesTextInEnglish : SampleConductedTimeMinutesTextInDanishOrAppLanguage)} " +
                    $"{(ShouldForceEnglish ? SampleCounductedAgoTextInEnglish : SampleCounductedAgoTextInDanishOrAppLanguage)}";
            }
            else
            {
                returnValue = $"{diff.Minutes} {SampleConductedTimeMinutesText} {SampleCounductedAgoText}";
            }
            return returnValue;
        }

        private HoursAndMinutesModel HoursAndMinutesDifference(DateTime SampleCollectedTime)
        {
            DateTime utcNow = IoCContainer.Resolve<IDateTimeService>().Now;

            int diff = (int)(utcNow - SampleCollectedTime).TotalMinutes;
            return new HoursAndMinutesModel
            {
                Hours = diff / 60,
                Minutes = diff % 60
            };
        }
    }
}
