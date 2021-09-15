using System.Threading.Tasks;
using SSICPAS.ViewModels.Certificates;

namespace SSICPAS.Services.Interfaces
{
    public interface IPassportStorageRepository
    {
        Task SavePassportToSecureStorage(PassportItemsViewModel content);
        Task<PassportItemsViewModel> GetPassportFromSecureStorage();
        Task DeletePassportFromSecureStorage();
    }
}