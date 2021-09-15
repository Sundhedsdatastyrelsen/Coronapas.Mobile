using System.Threading.Tasks;
using SSICPAS.ViewModels.Certificates;

namespace SSICPAS.Services.Interfaces
{
    public interface IFamilyPassportStorageRepository
    {
        Task SaveFamilyPassportToSecureStorage(FamilyPassportItemsViewModel content);
        Task<FamilyPassportItemsViewModel> GetFamilyPassportFromSecureStorage();
        Task DeleteFamilyPassportFromSecureStorage();
        Task DeleteOnlyDKPassportFromSecureStorage();
        Task MigrateData();
    }
}