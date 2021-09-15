using NodaTime;
using SSICPAS.Services;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Base;
using System;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Configuration;
using SSICPAS.Enums;
using SSICPAS.Services.Translator;
using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;
using System.Globalization;

namespace SSICPAS.ViewModels.Certificates
{
    public class InfoRecoveryTextViewModel : BaseViewModel
    {
        #region Bindable properties

        public string RecoveryHeaderText => ShowTextInEnglish ? "INTERNATIONAL_RECOVERY_HEADER_TEXT".Translate() : "RECOVERY_HEADER_TEXT".Translate();
        public string RecoveryCertificateHeaderText => ShowTextInEnglish ? "INTERNATIONAL_RECOVERY_CERTIFICATE_HEADER_TEXT".Translate() : "RECOVERY_CERTIFICATE_HEADER_TEXT".Translate();
        public string RecoveryHeaderValue => ShowTextInEnglish ? "INTERNATIONAL_INFO_CERTIFICATE_PASSPORT_HEADER_TEXT".Translate() : "INFO_CERTIFICATE_PASSPORT_HEADER_TEXT".Translate();
        
        public string RecoveryTimeAfterTextInEnglish => "INFO_RECOVERY_TIME_AFTER_TEXT_EN".Translate();
        public string RecoveryTimeAfterTextInDanishOrAppLanguage => ShowCertificate ? "INFO_RECOVERY_TIME_AFTER_TEXT_DK".Translate() : "RECOVERY_TIME_AFTER_TEXT".Translate();

        public string RecoveryValidTextInEnglish => "INFO_RECOVERY_VALID_TEXT_EN".Translate();
        public string RecoveryValidTextInDanishOrAppLanguage => ShowCertificate ? "INFO_RECOVERY_VALID_TEXT_DK".Translate() : "RECOVERY_VALID_TEXT_".Translate();

        public string RecoveryValidFromTextInEnglish => "INFO_RECOVERY_VALID_FROM_TEXT_EN".Translate();
        public string RecoveryValidFromTextInDanishOrAppLanguage => ShowCertificate ? "INFO_RECOVERY_VALID_FROM_TEXT_DK".Translate() : "RECOVERY_VALID_FROM_TEXT".Translate();

        public string RecoveryValidToTextInEnglish => "INFO_RECOVERY_VALID_TO_TEXT_EN".Translate();
        public string RecoveryValidToTextInDanishOrAppLanguage => ShowCertificate ? "INFO_RECOVERY_VALID_TO_TEXT_DK".Translate() : "RECOVERY_VALID_TO_TEXT".Translate();

        public string RecoveryDateTextInEnglish => "INFO_RECOVERY_DATE_TEXT_EN".Translate();
        public string RecoveryDateTextInDanishOrAppLanguage => ShowCertificate ? "INFO_RECOVERY_DATE_TEXT_DK".Translate() : "RECOVERY_DATE_TEXT".Translate();

        public string RecoveryDiseaseTextInEnglish => "INFO_RECOVERY_DISEASE_TEXT_EN".Translate();
        public string RecoveryDiseaseTextInDanishOrAppLanguage => ShowCertificate ? "INFO_RECOVERY_DISEASE_TEXT_DK".Translate() : "RECOVERY_DISEASE_TEXT".Translate();

        public string RecoveryCountryTextInEnglish => "INFO_RECOVERY_COUNTRY_TEXT_EN".Translate();
        public string RecoveryCountryTextInDanishOrAppLanguage => ShowCertificate ? "INFO_RECOVERY_COUNTRY_TEXT_DK".Translate() : "RECOVERY_COUNRTY_TEXT".Translate();

        public string RecoveryIssuerTextInEnglish => "INFO_RECOVERY_CERTIFICATE_ISSUER_TEXT_EN".Translate();
        public string RecoveryIssuerTextInDanishOrAppLanguage => ShowCertificate ? "INFO_RECOVERY_CERTIFICATE_ISSUER_TEXT_DK".Translate() : "RECOVERY_CERTIFICATE_ISSUER_TEXT".Translate();

        public string RecoveryCertificateIdTextInEnglish => "INFO_RECOVERY_CERTIFICATE_ID_TEXT_EN".Translate();
        public string RecoveryCertificateIdTextInDanishOrAppLanguage => ShowCertificate ? "INFO_RECOVERY_CERTIFICATE_ID_TEXT_DK".Translate() : "INFO_CERTIFICATE_PASSPORT_NUMBER_TEXT".Translate();

        public string RecoveryDayTextInEnglish => "INFO_RECOVERY_DAY_TEXT_EN".Translate();
        public string RecoveryDayTextInDanishOrAppLanguage => ShowCertificate ? "INFO_RECOVERY_DAY_TEXT_EN".Translate() : "INFO_RECOVERY_DAY_TEXT_EN".Translate();

        public string RecoveryDaysTextInEnglish => "INFO_RECOVERY_DAYS_TEXT_EN".Translate();
        public string RecoveryDaysTextInDanishOrAppLanguage => ShowCertificate ? "INFO_RECOVERY_DAYS_TEXT_DK".Translate() : "RECOVERY_DAYS_TEXT".Translate();

        public string RecoveryValidMonthsValueInEnglish => "INFO_RECOVERY_VALID_VALUE_EN".Translate();
        public string RecoveryValidMonthsValueInDanishOrAppLanguage => ShowCertificate ? "INFO_RECOVERY_VALID_VALUE_DK".Translate() : "RECOVERY_VALID_VALUE".Translate();
        #endregion

        private LanguageSelection _selectedLanguage => LocaleService.Current.GetLanguage();
        private bool EnglishSelected => _selectedLanguage == LanguageSelection.English;

        public string RecoveryCertIdValue => ShowCertificate ? PassportViewModel.PassportData.CertificateIdentifier : "-";

        public string RecoveryTimeAfterValueInEnglish => TimeSinceRecoveryTime(PassportViewModel.PassportData.DateFirstPositiveTest ?? DateTime.MinValue, true);
        public string RecoveryTimeAfterValueInDanishOrAppLanguage => TimeSinceRecoveryTime(PassportViewModel.PassportData.DateFirstPositiveTest ?? DateTime.MinValue);

        public string RecoveryValidFromValueInEnglish => PassportViewModel.PassportData.RecoveryValidFrom?.ToLocaleDateFormat(true) ?? "-";
        public string RecoveryValidFromValueInDanishOrAppLanguage => PassportViewModel.PassportData.RecoveryValidFrom?.ToLocalTime().LocaleFormatDate(ShowCertificate) ?? "-";

        public string RecoveryValidToValueInEnglish => PassportViewModel.PassportData.RecoveryValidTo?.ToLocaleDateFormat(true) ?? "-";
        public string RecoveryValidToValueInDanishOrAppLanguage => PassportViewModel.PassportData.RecoveryValidTo?.ToLocalTime().LocaleFormatDate(ShowCertificate) ?? "-";

        public string RecoveryDateOfFirstPositiveResultValueInEnglish => PassportViewModel.PassportData.DateFirstPositiveTest?.ToLocaleDateFormat(true) ?? "-";
        public string RecoveryDateOfFirstPositiveResultValueInDanishOrAppLanguage => PassportViewModel.PassportData.DateFirstPositiveTest?.ToLocalTime().LocaleFormatDate(ShowCertificate) ?? "-";

        public string RecoveryDiseaseValueInEnglish => PassportViewModel.PassportData.RecoveryDisease;
        public string RecoveryDiseaseValueInDanishOrAppLanguage => ToLocaleDisease(PassportViewModel.PassportData.RecoveryDisease, ShowCertificate ? false : (ShowTextInEnglish || EnglishSelected));

        public string RecoveryCountryValueInEnglish => PassportViewModel.PassportData.CountryOfTest;
        public string RecoveryCountryValueInDanishOrAppLanguage => (ShowTextInEnglish || EnglishSelected) && !ShowCertificate ? PassportViewModel.PassportData.CountryOfTest : PassportViewModel.PassportData.CountryOfTestDanish;

        public string RecoveryCertificateIssuerValueInEnglish => PassportViewModel.PassportData.CertificateIssuer;
        public string RecoveryCertificateIssuerValueInDanishOrAppLanguage => ToLocaleCertificateIssuer(PassportViewModel.PassportData.CertificateIssuer, ShowCertificate ? false : (ShowTextInEnglish || EnglishSelected));

        public SinglePassportViewModel PassportViewModel { get; set; } = new SinglePassportViewModel();

        public bool ShowCertificate { get; set; }
        public bool ShowHeader { get; set; }
        public bool ShowTextInEnglish { get; set; }
        public bool OnlyOneEUPassport { get; set; }
        public bool ShouldUseDanishForAccessibility => CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "da" || !ShowCertificate;

        string ToLocaleDisease(string disease, bool ShouldUseEnglish) => DCCValueSetTranslator.ToLocale(disease, DCCValueSetEnum.Disease, ShouldUseEnglish);
        string ToLocaleCertificateIssuer(string certIssuer, bool ShouldUseEnglish) => DCCValueSetTranslator.ToLocale(certIssuer, DCCValueSetEnum.CertificateIssuer, ShouldUseEnglish);

        public void UpdateView()
        {
            OnPropertyChanged(nameof(RecoveryHeaderText));
            OnPropertyChanged(nameof(RecoveryCertificateHeaderText));
            OnPropertyChanged(nameof(RecoveryHeaderValue));

            OnPropertyChanged(nameof(RecoveryTimeAfterTextInEnglish));
            OnPropertyChanged(nameof(RecoveryTimeAfterTextInDanishOrAppLanguage));
            OnPropertyChanged(nameof(RecoveryValidTextInEnglish));
            OnPropertyChanged(nameof(RecoveryValidTextInDanishOrAppLanguage));
            OnPropertyChanged(nameof(RecoveryValidFromTextInEnglish));
            OnPropertyChanged(nameof(RecoveryValidFromTextInDanishOrAppLanguage));
            OnPropertyChanged(nameof(RecoveryValidToTextInEnglish));
            OnPropertyChanged(nameof(RecoveryValidToTextInDanishOrAppLanguage));
            OnPropertyChanged(nameof(RecoveryDateTextInEnglish));
            OnPropertyChanged(nameof(RecoveryDateTextInDanishOrAppLanguage));
            OnPropertyChanged(nameof(RecoveryDiseaseTextInEnglish));
            OnPropertyChanged(nameof(RecoveryDiseaseTextInDanishOrAppLanguage));
            OnPropertyChanged(nameof(RecoveryCountryTextInEnglish));
            OnPropertyChanged(nameof(RecoveryCountryTextInDanishOrAppLanguage));
            OnPropertyChanged(nameof(RecoveryIssuerTextInEnglish));
            OnPropertyChanged(nameof(RecoveryIssuerTextInDanishOrAppLanguage));
            OnPropertyChanged(nameof(RecoveryCertificateIdTextInEnglish));
            OnPropertyChanged(nameof(RecoveryCertificateIdTextInDanishOrAppLanguage));
            OnPropertyChanged(nameof(RecoveryDayTextInEnglish));
            OnPropertyChanged(nameof(RecoveryDayTextInDanishOrAppLanguage));
            OnPropertyChanged(nameof(RecoveryDaysTextInEnglish));
            OnPropertyChanged(nameof(RecoveryDaysTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(PassportViewModel));
            OnPropertyChanged(nameof(ShowCertificate));
            OnPropertyChanged(nameof(ShowHeader));
            OnPropertyChanged(nameof(ShowTextInEnglish));
            OnPropertyChanged(nameof(ShouldUseDanishForAccessibility));

            if (PassportViewModel == null) return;
            OnPropertyChanged(nameof(RecoveryValidMonthsValueInEnglish));
            OnPropertyChanged(nameof(RecoveryValidMonthsValueInDanishOrAppLanguage));

            OnPropertyChanged(nameof(RecoveryTimeAfterValueInEnglish));
            OnPropertyChanged(nameof(RecoveryTimeAfterValueInDanishOrAppLanguage));

            OnPropertyChanged(nameof(RecoveryValidFromValueInEnglish));
            OnPropertyChanged(nameof(RecoveryValidFromValueInDanishOrAppLanguage));

            OnPropertyChanged(nameof(RecoveryValidToValueInEnglish));
            OnPropertyChanged(nameof(RecoveryValidToValueInDanishOrAppLanguage));

            OnPropertyChanged(nameof(RecoveryDateOfFirstPositiveResultValueInEnglish));
            OnPropertyChanged(nameof(RecoveryDateOfFirstPositiveResultValueInDanishOrAppLanguage));

            OnPropertyChanged(nameof(RecoveryDiseaseValueInEnglish));
            OnPropertyChanged(nameof(RecoveryDiseaseValueInDanishOrAppLanguage));

            OnPropertyChanged(nameof(RecoveryCountryValueInEnglish));
            OnPropertyChanged(nameof(RecoveryCountryValueInDanishOrAppLanguage));

            if (ShowCertificate)
            {
                OnPropertyChanged(nameof(RecoveryCertificateIssuerValueInEnglish));
                OnPropertyChanged(nameof(RecoveryCertificateIssuerValueInDanishOrAppLanguage));
                OnPropertyChanged(nameof(RecoveryCertIdValue));
            }
            if (ShowHeader)
            {
                OnPropertyChanged(nameof(RecoveryHeaderValue));
            }

        }
        public string TimeSinceRecoveryTime(DateTime RecoveryValidFrom, bool ShouldForceEnglish = false)

        {
            if (RecoveryValidFrom == DateTime.MinValue)
            {
                return "-";
            }

            DateTime utcNow = IoCContainer.Resolve<IDateTimeService>().Now;

            LocalDate today = new LocalDate(utcNow.Year, utcNow.Month, utcNow.Day);
            LocalDate sampleTime = new LocalDate(RecoveryValidFrom.Year, RecoveryValidFrom.Month, RecoveryValidFrom.Day);

            Period days = Period.Between(sampleTime, today, PeriodUnits.Days);
            
            return days.Days == 1
                ?  days.Days + $" {(ShouldForceEnglish ? RecoveryDayTextInEnglish : RecoveryDayTextInDanishOrAppLanguage)}"
                :  days.Days + $" {(ShouldForceEnglish ? RecoveryDaysTextInEnglish : RecoveryDaysTextInDanishOrAppLanguage)}";
        }
    }
}