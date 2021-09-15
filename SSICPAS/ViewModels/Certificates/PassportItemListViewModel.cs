using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SkiaSharp;
using SSICPAS.Enums;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Base;
using SSICPAS.Views.Certificates;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.Certificates
{
    public class PassportItemListViewModel : BaseViewModel
    {
        public SKColor[] TopGradientList => SSICPASColorGradient.MainPageStatusGradientBlue.Gradient();
        public SKColor[] TopGradient => SSICPASColorGradient.MainPageCrownGradient.Gradient();

        public ICommand OnQREuInfoButtonClicked => new Command(async () => await ExecuteOnceAsync(OpenQrEuInfoPage));

        public async Task OpenQrEuInfoPage() => await _navigationService.PushPage(new InfoBorderControlPage());

        public PassportItemListViewModel(FamilyPassportItemsViewModel passportItemsViewModel)
        {
            FullName = passportItemsViewModel?.SelectedPassport?.FullName ??
                               passportItemsViewModel?.SelectedMandatoryInfo?.FullName;

            Birthdate = DateUtils.ParseDateOfBirth(
                passportItemsViewModel,
                time => time.ToLocaleDateFormat(
                    passportItemsViewModel?.SelectedPassportType == PassportType.UNIVERSAL_EU));

            PassportItemsGrouped = new ObservableCollection<PassportItemsGroupViewModel>();

            List<SinglePassportViewModel> vaccinePassports = passportItemsViewModel.SelectedFamilyMemberPassport.EuVaccinePassports;
            vaccinePassports = vaccinePassports.OrderByDescending(x => x.PassportData.VaccinationDate).ToList();

            List<SinglePassportViewModel> testPassports = passportItemsViewModel.SelectedFamilyMemberPassport.EuTestPassports;
            testPassports = testPassports.OrderByDescending(x => x.PassportData.SampleCollectedTime).ToList();

            List<SinglePassportViewModel> recoveryPassports = passportItemsViewModel.SelectedFamilyMemberPassport.EuRecoveryPassports;

            foreach (SinglePassportViewModel vaccinePassport in vaccinePassports)
            {
                AddVaccineItem(vaccinePassport, passportItemsViewModel);
            }

            foreach (SinglePassportViewModel testPassport in testPassports)
            {
                AddTestItem(testPassport, passportItemsViewModel);
            }

            foreach (SinglePassportViewModel recoveryPassport in recoveryPassports)
            {
                AddRecoveryItem(recoveryPassport, passportItemsViewModel);
            }
        }

        public ObservableCollection<PassportItemsGroupViewModel> PassportItemsGrouped { get; set; }

        private void AddVaccineItem(SinglePassportViewModel passportViewModel,
            FamilyPassportItemsViewModel passportItemsViewModel)
        {
            PassportItemsGroupViewModel vaccineGroup =
                PassportItemsGrouped.FirstOrDefault(n =>
                    n.Title == PassportItemCellViewModel.PassportItemType.Vaccine.ToString());

            if (vaccineGroup == null)
            {
                vaccineGroup =
                    new PassportItemsGroupViewModel(PassportItemCellViewModel.PassportItemType.Vaccine.ToString());
                PassportItemsGrouped.Add(vaccineGroup);
            }

            vaccineGroup.Add(
                PassportItemCellViewModel.CreateVaccineItemCellViewModel(passportViewModel, passportItemsViewModel));
        }

        private void AddTestItem(SinglePassportViewModel passportViewModel,
            FamilyPassportItemsViewModel passportItemsViewModel)
        {
            PassportItemsGroupViewModel testGroup = PassportItemsGrouped.FirstOrDefault(n =>
                n.Title == PassportItemCellViewModel.PassportItemType.Test.ToString());

            if (testGroup == null)
            {
                testGroup = new PassportItemsGroupViewModel(PassportItemCellViewModel.PassportItemType.Test.ToString());
                PassportItemsGrouped.Add(testGroup);
            }

            testGroup.Add(
                PassportItemCellViewModel.CreateTestItemCellViewModel(passportViewModel, passportItemsViewModel));
        }

        private void AddRecoveryItem(SinglePassportViewModel passportViewModel,
            FamilyPassportItemsViewModel passportItemsViewModel)
        {
            PassportItemsGroupViewModel recoveryGroup = PassportItemsGrouped.FirstOrDefault(n =>
                n.Title == PassportItemCellViewModel.PassportItemType.Recovery.ToString());

            if (recoveryGroup == null)
            {
                recoveryGroup = new PassportItemsGroupViewModel(PassportItemCellViewModel.PassportItemType.Recovery.ToString(), false);
                PassportItemsGrouped.Add(recoveryGroup);
            }

            recoveryGroup.Add(
                PassportItemCellViewModel.CreateRecoveryItemCellViewModel(passportViewModel, passportItemsViewModel));
        }

        public string FullName { get; set; }
        public string Birthdate { get; set; }

    }
}