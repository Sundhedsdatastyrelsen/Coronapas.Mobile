using System.Globalization;
using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;
using SSICPAS.Enums;
using SSICPAS.Services;
using SSICPAS.Services.Translator;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Base;

namespace SSICPAS.ViewModels.Certificates
{
    public class InfoVaccineTextViewModel : BaseViewModel
    {
        #region Bindable properties
        public string VaccineHeaderText => ShowTextInEnglish ? "INTERNATIONAL_INFO_VACCINE_HEADER_TEXT".Translate() : "INFO_VACCINE_HEADER_TEXT".Translate();
        public string VaccineMarketingAuthorizationText => ShowTextInEnglish ? "INTERNATIONAL_INFO_VACCINE_MARKETING_AUTHORISATION_HOLDER_TEXT".Translate() : "INFO_VACCINE_MARKETING_AUTHORISATION_HOLDER_TEXT".Translate();
        public string VaccineCertificateHeaderText => ShowTextInEnglish ? "INTERNATIONAL_INFO_CERTIFICATE_HEADER_TEXT".Translate() : "INFO_CERTIFICATE_HEADER_TEXT".Translate();

        public string VaccineDateOfVaccinationTextInEnglish => "INFO_VACCINE_DATE_OF_VACCINATION_TEXT_EN".Translate();
        public string VaccineDateOfVaccinationTextInDanishOrAppLanguage => ShowCertificate ? "INFO_VACCINE_DATE_OF_VACCINATION_TEXT_DK".Translate() : "INFO_VACCINE_DATE_OF_VACCINATION_TEXT".Translate(); 

        public string VaccineMarketingAuthorizationTextInEnglish => "INFO_VACCINE_MARKETING_AUTHORISATION_HOLDER_TEXT_EN".Translate();
        public string VaccineMarketingAuthorizationTextInDanishOrAppLanguage => ShowCertificate ? "INFO_VACCINE_MARKETING_AUTHORISATION_HOLDER_TEXT_DK".Translate() : "INFO_VACCINE_MARKETING_AUTHORISATION_HOLDER_TEXT".Translate();

        public string VaccineDoseTitleTextInEnglish => "INFO_VACCINE_DOSE_TITLE_TEXT_EN".Translate();
        public string VaccineDoseTitleTextInDanishOrAppLanguage => ShowCertificate ? "INFO_VACCINE_DOSE_TITLE_TEXT_DK".Translate() : "INFO_VACCINE_DOSE_TITLE_TEXT".Translate();

        public string VaccineTypeTextInEnglish => "INFO_VACCINE_TYPE_TEXT_EN".Translate();
        public string VaccineTypeTextInDanishOrAppLanguage => ShowCertificate ? "INFO_VACCINE_TYPE_TEXT_DK".Translate() : "INFO_VACCINE_TYPE_TEXT".Translate();

        public string VaccineDiseaseTextInEnglish => "INFO_VACCINE_DISEASE_TARGETED_EN".Translate();
        public string VaccineDiseaseTextInDanishOrAppLanguage => ShowCertificate ? "INFO_VACCINE_DISEASE_TARGETED_DK".Translate() : "NEGATIVE_TEST_DISEASE_TEXT".Translate();

        public string VaccineVaccineNameTextInEnglish => "INFO_VACCINE_VACCINE_NAME_TEXT_EN".Translate();
        public string VaccineVaccineNameTextInDanishOrAppLanguage => ShowCertificate ? "INFO_VACCINE_VACCINE_NAME_TEXT_DK".Translate() : "INFO_VACCINE_VACCINE_NAME_TEXT".Translate();

        public string VaccineVaccinationCountryTextInEnglish => "INFO_VACCINE_VACCINATION_COUNTRY_TEXT_EN".Translate();
        public string VaccineVaccinationCountryTextInDanishOrAppLanguage => ShowCertificate ? "INFO_VACCINE_VACCINATION_COUNTRY_TEXT_DK".Translate() : "INFO_VACCINE_VACCINATION_COUNTRY_TEXT".Translate();

        public string VaccineCertificateIssuerTextInEnglish => "INFO_CERTIFICATE_ISSUER_TEXT_EN".Translate();
        public string VaccineCertificateIssuerTextInDanishOrAppLanguage => ShowCertificate ? "INFO_CERTIFICATE_ISSUER_TEXT_DK".Translate() : "INFO_CERTIFICATE_ISSUER_TEXT".Translate();

        public string VaccinePassportNumberTextInEnglish => "INFO_CERTIFICATE_PASSPORT_NUMBER_TEXT_EN".Translate();
        public string VaccinePassportNumberTextInDanishOrAppLanguage => ShowCertificate ? "INFO_CERTIFICATE_PASSPORT_NUMBER_TEXT_DK".Translate() : "INFO_CERTIFICATE_PASSPORT_NUMBER_TEXT".Translate();

        public string CurrentDoseTemplateInEnglish => "INFO_VACCINE_CURRENT_DOSE_TEXT_EN".Translate();
        public string CurrentDoseTemplateInDanishOrAppLanguage => ShowCertificate  ? "INFO_VACCINE_CURRENT_DOSE_TEXT_DK".Translate() : "INFO_VACCINE_CURRENT_DOSE_TEXT".Translate();

        #endregion

        private LanguageSelection _selectedLanguage => LocaleService.Current.GetLanguage();
        private bool EnglishSelected => _selectedLanguage == LanguageSelection.English;
        public string VaccineMarketingAuthorizationValue => PassportViewModel.PassportData.MarketingAuthorizationHolder;

        public string VaccineCurrentDoseValueInEnglish => string.Format(CurrentDoseTemplateInEnglish, 
            PassportViewModel.PassportData.DoseNumber,
            PassportViewModel.PassportData.TotalNumberOfDose);
        public string VaccineCurrentDoseValueInDanishOrAppLanguage => string.Format(CurrentDoseTemplateInDanishOrAppLanguage,
            PassportViewModel.PassportData.DoseNumber,
            PassportViewModel.PassportData.TotalNumberOfDose);

        public string VaccineVaccinationDateValueInEnglish => PassportViewModel.PassportData.VaccinationDate?.ToLocaleDateFormat(ShowCertificate) ?? "-";
        public string VaccineVaccinationDateValueInDanishOrAppLanguage => PassportViewModel.PassportData.VaccinationDate?.ToLocalTime().LocaleFormatDate(ShowCertificate) ?? "-";

        public string VaccineTypeValue => PassportViewModel.PassportData.VaccinationType;

        public string VaccineTargetDiseaseInDanishOrAppLanguage => ToLocaleDisease(PassportViewModel.PassportData.Disease, ShowCertificate ? false : (ShowTextInEnglish || EnglishSelected));
        public string VaccineTargetDiseaseInEnglish => PassportViewModel.PassportData.Disease;

        public string VaccineVaccineNameValue => PassportViewModel.PassportData.MedicinialProduct;
        public string VaccineVaccinationCountryValueInDanishOrAppLanguage => (ShowTextInEnglish || EnglishSelected) && !ShowCertificate ? PassportViewModel.PassportData.VaccinationCountry : PassportViewModel.PassportData.VaccinationCountryDanish;
        public string VaccineVaccinationCountryValueInEnglish => PassportViewModel.PassportData.VaccinationCountry;
        public string VaccineCertificateIssuerValueInEnglish => PassportViewModel.PassportData.CertificateIssuer;
        public string VaccineCertificateIssuerValueInDanishOrAppLanguage => ToLocaleCertificateIssuer(PassportViewModel.PassportData.CertificateIssuer, ShowCertificate ? false : (ShowTextInEnglish || EnglishSelected));
        public string VaccinePassportNumberValue => ShowCertificate ? PassportViewModel.PassportData.CertificateIdentifier : "-";
        public string VaccineHeaderValue => ShowHeader ? PassportViewModel.PassportData.MarketingAuthorizationHolder : "-";

        public SinglePassportViewModel PassportViewModel { get; set; } = new SinglePassportViewModel();

        public bool ShowCertificate { get; set; }
        public bool ShowHeader { get; set; }
        public bool ShowTextInEnglish { get; set; }
        public bool OnlyOneEUPassport { get; set; }
        public bool ShowInfoInBothEnglishAndDanish { get; set; }
        public bool ShouldUseDanishForAccessibility => CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "da" || !ShowCertificate;

        string ToLocaleDisease(string disease, bool ShouldUseEnglish) => DCCValueSetTranslator.ToLocale(disease, DCCValueSetEnum.Disease, ShouldUseEnglish);
        string ToLocaleCertificateIssuer(string certIssuer, bool ShouldUseEnglish) => DCCValueSetTranslator.ToLocale(certIssuer, DCCValueSetEnum.CertificateIssuer, ShouldUseEnglish);

        public void UpdateView()
        {
            OnPropertyChanged(nameof(VaccineHeaderText));
            OnPropertyChanged(nameof(VaccineMarketingAuthorizationText));
            OnPropertyChanged(nameof(VaccineCertificateHeaderText));

            OnPropertyChanged(nameof(VaccineDateOfVaccinationTextInEnglish));
            OnPropertyChanged(nameof(VaccineDateOfVaccinationTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(VaccineMarketingAuthorizationTextInEnglish));
            OnPropertyChanged(nameof(VaccineMarketingAuthorizationTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(VaccineDoseTitleTextInEnglish));
            OnPropertyChanged(nameof(VaccineDoseTitleTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(VaccineTypeTextInEnglish));
            OnPropertyChanged(nameof(VaccineTypeTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(VaccineDiseaseTextInEnglish));
            OnPropertyChanged(nameof(VaccineDiseaseTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(VaccineVaccineNameTextInEnglish));
            OnPropertyChanged(nameof(VaccineVaccineNameTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(VaccineVaccinationCountryTextInEnglish));
            OnPropertyChanged(nameof(VaccineVaccinationCountryTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(VaccineCertificateIssuerTextInEnglish));
            OnPropertyChanged(nameof(VaccineCertificateIssuerTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(VaccinePassportNumberTextInEnglish));
            OnPropertyChanged(nameof(VaccinePassportNumberTextInDanishOrAppLanguage));

            OnPropertyChanged(nameof(PassportViewModel));
            OnPropertyChanged(nameof(ShowCertificate));
            OnPropertyChanged(nameof(ShowHeader));
            OnPropertyChanged(nameof(ShouldUseDanishForAccessibility));
            if (PassportViewModel == null) return;
            OnPropertyChanged(nameof(VaccineMarketingAuthorizationValue));

            OnPropertyChanged(nameof(VaccineCurrentDoseValueInEnglish));
            OnPropertyChanged(nameof(VaccineCurrentDoseValueInDanishOrAppLanguage));

            OnPropertyChanged(nameof(VaccineVaccinationDateValueInEnglish));
            OnPropertyChanged(nameof(VaccineVaccinationDateValueInDanishOrAppLanguage));

            OnPropertyChanged(nameof(VaccineTypeValue));
            OnPropertyChanged(nameof(VaccineVaccineNameValue));

            OnPropertyChanged(nameof(VaccineTargetDiseaseInEnglish));
            OnPropertyChanged(nameof(VaccineTargetDiseaseInDanishOrAppLanguage));

            OnPropertyChanged(nameof(VaccineVaccinationCountryValueInDanishOrAppLanguage));
            OnPropertyChanged(nameof(VaccineVaccinationCountryValueInEnglish));
            if (ShowCertificate)
            {
                OnPropertyChanged(nameof(VaccineCertificateIssuerValueInEnglish));
                OnPropertyChanged(nameof(VaccineCertificateIssuerValueInDanishOrAppLanguage));
                OnPropertyChanged(nameof(VaccinePassportNumberValue));
            }
            if (ShowHeader)
            {
                OnPropertyChanged(nameof(VaccineHeaderValue));
            }
        }
    }
}
