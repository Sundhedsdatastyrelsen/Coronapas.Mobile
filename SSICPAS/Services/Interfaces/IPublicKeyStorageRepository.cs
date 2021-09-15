using System.Threading.Tasks;
using SSICPAS.Models;

namespace SSICPAS.Services.Interfaces
{
    public interface IPublicKeyStorageRepository
    {
        Task SavePublicKeyToSecureStorage(PublicKeyStorageModel publicKeyStorageModel);
        Task<PublicKeyStorageModel> GetPublicKeyFromSecureStorage();
        Task DeletePublicKeyFromSecureStorage();
    }
}
