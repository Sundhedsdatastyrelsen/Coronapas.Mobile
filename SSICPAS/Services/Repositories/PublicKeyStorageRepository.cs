using System.Threading.Tasks;
using SSICPAS.Core.Data;
using SSICPAS.Data;
using SSICPAS.Models;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;

namespace SSICPAS.Services.Repositories
{
    public class PublicKeyStorageRepository : IPublicKeyStorageRepository
    {
        private readonly ISecureStorageService<PublicKeyStorageModel> _secureStorageService;

        public PublicKeyStorageRepository(ISecureStorageService<PublicKeyStorageModel> secureStorageService)
        {
            _secureStorageService = secureStorageService;
        }

        public async Task DeletePublicKeyFromSecureStorage()
        {
            await _secureStorageService.TryClear(SecureStorageKeys.PUBLIC_KEY);
        }

        public async Task<PublicKeyStorageModel> GetPublicKeyFromSecureStorage()
        {
            PublicKeyStorageModel publicKeyStorageModel = new PublicKeyStorageModel();
            
            if (await _secureStorageService.TryHasValue(SecureStorageKeys.PUBLIC_KEY))
            {
                publicKeyStorageModel = await _secureStorageService.TryGetSecureStorageAsync(SecureStorageKeys.PUBLIC_KEY);
            }

            return publicKeyStorageModel;
        }

        public async Task SavePublicKeyToSecureStorage(PublicKeyStorageModel publicKeyStorageModel)
        {
            await _secureStorageService.TrySetSecureStorageAsync(SecureStorageKeys.PUBLIC_KEY, publicKeyStorageModel);
        }
    }
}
