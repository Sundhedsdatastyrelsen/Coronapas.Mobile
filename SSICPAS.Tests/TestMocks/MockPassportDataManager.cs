using System;
using System.Threading.Tasks;
using SSICPAS.Enums;
using SSICPAS.Services.Interfaces;
using SSICPAS.ViewModels.Certificates;

namespace SSICPAS.Tests.TestMocks
{
    public class MockPassportDataManager: IPassportDataManager
    {
        public bool IsContinuouslyFetchingPassport { get; set; }
        public Action StopContinuousFetching { get; set; }
        public Action StartContinuousFetching { get; set; }

        public Task<FamilyPassportItemsViewModel> FetchPassport(bool forced = false)
        {
            return Task.FromResult(new FamilyPassportItemsViewModel());
        }

        public void Reset()
        {
        }

        public void UpdateSelectedPassportPreference(PassportType passportType)
        {
        }
    }
}
