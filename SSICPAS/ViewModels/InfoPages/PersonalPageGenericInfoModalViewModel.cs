using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;
using SSICPAS.Enums;
using SSICPAS.Services;
using SSICPAS.Services.Translator;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Base;
using SSICPAS.ViewModels.Certificates;

namespace SSICPAS.ViewModels.InfoPages
{
    public class PersonalPageGenericInfoModalViewModel : ContentSheetPageNoBackButtonOnIOSViewModel
    {
        public bool IsTestSelected => _euPassportType == EuPassportType.TEST;
        public bool IsVaccineSelected => _euPassportType == EuPassportType.VACCINE;
        public bool IsRecoverySelected => _euPassportType == EuPassportType.RECOVERY;

        public SinglePassportViewModel PassportViewModel { get; set; }

        public FamilyPassportItemsViewModel PassportItemsViewModel { get; set; }

        public string InfoHeader { get; set; }
        public string InfoTypeHeader { get; set; }
        public string InfoHeaderAndroid { get; set; }
        public string Image { get; set; }
        public string ValidDate { get; set; }

        private EuPassportType _euPassportType { get; set; }

        public PersonalPageGenericInfoModalViewModel(SinglePassportViewModel passportViewModel, EuPassportType euPassportType)
        {
            PassportViewModel = passportViewModel;
            _euPassportType = euPassportType;
            UpdateText();
        }

        public void UpdateView()
        {
            OnPropertyChanged(nameof(IsTestSelected));
            OnPropertyChanged(nameof(IsRecoverySelected));
            OnPropertyChanged(nameof(IsVaccineSelected));
            OnPropertyChanged(nameof(PassportViewModel));
        }

        public void UpdateText()
        {
            if(IsVaccineSelected)
            {
                InfoHeader = "INFO_VACCINE_HEADER_TEXT".Translate();
                InfoTypeHeader = PassportViewModel.PassportData?.MarketingAuthorizationHolder ?? "-";
                InfoHeaderAndroid = "MY_PAGE_VACCINE_TEXT".Translate();
                Image = SSICPASImage.Covid19VaccineIcon.Image();
                ValidDate = PassportViewModel.PassportData?.CertificateValidFrom?.LocaleFormatDate() ?? "-";
            }
            if (IsTestSelected)
            {
                InfoHeader = "MY_PAGE_NEGATIVE_TEST_HEADER".Translate();
                InfoTypeHeader = DCCValueSetTranslator.ToLocale(PassportViewModel.PassportData?.TypeOfTest, DCCValueSetEnum.TypeOfTest);
                InfoHeaderAndroid = "MY_PAGE_TEST_TEXT".Translate();
                Image = SSICPASImage.Covid19NegativeTestIcon.Image();
                ValidDate = PassportViewModel.PassportData?.CertificateValidFrom?.LocaleFormatDate() ?? "-";
            }
            if (IsRecoverySelected)
            {
                InfoHeader = "RECOVERY_HEADER_TEXT".Translate();
                InfoTypeHeader = DCCValueSetTranslator.ToLocale(PassportViewModel.PassportData?.RecoveryDisease, DCCValueSetEnum.Disease);
                InfoHeaderAndroid = "MY_PAGE_IMMUNITY_TEXT".Translate();
                Image = SSICPASImage.Covid19RecoveryIcon.Image();
                ValidDate = PassportViewModel.PassportData?.CertificateValidTo?.LocaleFormatDate() ?? "-";
            }
        }
    }
}
