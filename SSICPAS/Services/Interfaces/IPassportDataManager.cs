using System;
using System.Threading.Tasks;
using SSICPAS.Enums;
using SSICPAS.ViewModels.Certificates;

namespace SSICPAS.Services.Interfaces
{
    public interface IPassportDataManager
    {
        Task<FamilyPassportItemsViewModel> FetchPassport(bool forced = false);
        void UpdateSelectedPassportPreference(PassportType passportType);
        bool IsContinuouslyFetchingPassport { get; set; }
        Action StopContinuousFetching { get; set; }
        Action StartContinuousFetching { get; set; }
        void Reset();
    }
}