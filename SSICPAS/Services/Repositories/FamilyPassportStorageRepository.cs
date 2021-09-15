using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Enums;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Certificates;
using static SSICPAS.Data.SecureStorageKeys;

namespace SSICPAS.Services.Repositories
{
    public static class FamilyExtension
    {
        public static string Child(this string str, int id)
        {
            return $"{str}_CHILD_{id}";
        }

        public static string ParentDK(this string str)
        {
            return $"{str}_PARENT_DK";
        }

        public static string ParentEU(this string str)
        {
            return $"{str}_PARENT_EU";
        }

        public static string AdditionalData(this string str)
        {
            return $"{str}_ADDITIONAL_DATA";
        }
    }

    public class FamilyPassportStorageRepository : IFamilyPassportStorageRepository
    {
        private readonly ISecureStorageService<AdditionalDataViewModel> _additionalDataSecureStorageService;
        private readonly ISecureStorageService<DKPassportsViewModel> _dkDataSecureStorageService;
        private readonly ISecureStorageService<EUPassportsViewModel> _euDataSecureStorageService;

        public FamilyPassportStorageRepository(
            ISecureStorageService<AdditionalDataViewModel> additionalDataSecureStorageService,
            ISecureStorageService<EUPassportsViewModel> euDataSecureStorageService,
            ISecureStorageService<DKPassportsViewModel> dkDataSecureStorageService)
        {
            _additionalDataSecureStorageService = additionalDataSecureStorageService;
            _euDataSecureStorageService = euDataSecureStorageService;
            _dkDataSecureStorageService = dkDataSecureStorageService;
        }

        public async Task SaveFamilyPassportToSecureStorage(FamilyPassportItemsViewModel content)
        {
            // Number of family members may vary so we need to reset the values
            await DeleteFamilyPassportFromSecureStorage();
            
            List<EUPassportsViewModel> familyData = content.FamilyData;
            DKPassportsViewModel dkData = content.DkData;
            AdditionalDataViewModel additionalData = content.AdditionalData;

            await _additionalDataSecureStorageService.TrySetSecureStorageAsync(PASSPORT_DATA.AdditionalData(),
                additionalData);
            await _dkDataSecureStorageService.TrySetSecureStorageAsync(PASSPORT_DATA.ParentDK(), dkData);

            familyData
                .Select((data, i) => new { Model = data, Number = i })
                .ToList()
                .ForEach(async data =>
                    await _euDataSecureStorageService.TrySetSecureStorageAsync(
                        data.Number == 0 ? PASSPORT_DATA.ParentEU() : PASSPORT_DATA.Child(data.Number - 1),
                        data.Model));
        }

        public async Task<FamilyPassportItemsViewModel> GetFamilyPassportFromSecureStorage()
        {
            FamilyPassportItemsViewModel passport = new FamilyPassportItemsViewModel();

            if (await _additionalDataSecureStorageService.TryHasValue(PASSPORT_DATA.AdditionalData()))
            {
                passport.AdditionalData =
                    await _additionalDataSecureStorageService.TryGetSecureStorageAsync(PASSPORT_DATA.AdditionalData());
            }

            if (await _dkDataSecureStorageService.TryHasValue(PASSPORT_DATA.ParentDK()))
            {
                passport.DkData = await _dkDataSecureStorageService.TryGetSecureStorageAsync(PASSPORT_DATA.ParentDK());
            }

            List<EUPassportsViewModel> familyList = new List<EUPassportsViewModel>();
            if (await _euDataSecureStorageService.TryHasValue(PASSPORT_DATA.ParentEU()))
            {
                familyList.Add(await _euDataSecureStorageService.TryGetSecureStorageAsync(PASSPORT_DATA.ParentEU()));
            }

            int counter = 0;
            while (await _euDataSecureStorageService.TryHasValue(PASSPORT_DATA.Child(counter)))
            {
                familyList.Add(await _euDataSecureStorageService.TryGetSecureStorageAsync(PASSPORT_DATA.Child(counter++)));
            }

            passport.FamilyData = familyList;

            return passport.IsAnyPassportAvailable ||
                (!string.IsNullOrEmpty(passport.AdditionalData?.JobId) && passport.AdditionalData?.JobStatus == PassportJobStatus.Inprogress) ?
                passport : null;
        }

        public async Task DeleteFamilyPassportFromSecureStorage()
        {
            if (await _additionalDataSecureStorageService.TryHasValue(PASSPORT_DATA.AdditionalData()))
            {
                await _additionalDataSecureStorageService.TryClear(PASSPORT_DATA.AdditionalData());
            }

            if (await _dkDataSecureStorageService.TryHasValue(PASSPORT_DATA.ParentDK()))
            {
                await _dkDataSecureStorageService.TryClear(PASSPORT_DATA.ParentDK());
            }

            if (await _euDataSecureStorageService.TryHasValue(PASSPORT_DATA.ParentEU()))
            {
                await _euDataSecureStorageService.TryClear(PASSPORT_DATA.ParentEU());
            }

            int counter = 0;
            while (await _euDataSecureStorageService.TryHasValue(PASSPORT_DATA.Child(counter)))
            {
                await _euDataSecureStorageService.TryClear(PASSPORT_DATA.Child(counter++));
            }
        }

        public async Task DeleteOnlyDKPassportFromSecureStorage()
        {
            if (await _dkDataSecureStorageService.TryHasValue(PASSPORT_DATA.ParentDK()))
            {
                await _dkDataSecureStorageService.TryClear(PASSPORT_DATA.ParentDK());
            }
        }

        public async Task MigrateData()
        {
            IPassportStorageRepository passportStorageRepository = IoCContainer.Resolve<IPassportStorageRepository>();
            PassportItemsViewModel oldPassport = await passportStorageRepository.GetPassportFromSecureStorage();
            if (await passportStorageRepository.GetPassportFromSecureStorage() == null)
            {
                return;
            }

            FamilyPassportItemsViewModel newPassport = new FamilyPassportItemsViewModel
            {
                SelectedPassportType = oldPassport.SelectedPassportType,
                SelectedFamilyMember = 0,
                
                DkData = oldPassport.DKPassportsViewModel,
                FamilyData = new List<EUPassportsViewModel>
                {
                    oldPassport.EUPassportsViewModel
                },
                AdditionalData = new AdditionalDataViewModel
                {
                    JobId = oldPassport.JobId,
                    JobStatus = oldPassport.JobStatus,
                    LanguageSelection = oldPassport.LanguageSelection
                }
            };

            await SaveFamilyPassportToSecureStorage(newPassport);
            await passportStorageRepository.DeletePassportFromSecureStorage();
        }
    }
}