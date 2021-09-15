using System.Threading.Tasks;
using SSICPAS.Core.Data;
using SSICPAS.Data;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Certificates;

namespace SSICPAS.Services.Repositories
{
    public class PassportStorageRepository : IPassportStorageRepository
    {
        private readonly ISecureStorageService<PassportItemsViewModel> _secureStorageService;

        public PassportStorageRepository(ISecureStorageService<PassportItemsViewModel> secureStorageService)
        {
            _secureStorageService = secureStorageService;
        }
        
        public async Task SavePassportToSecureStorage(PassportItemsViewModel content)
        {
            await _secureStorageService.TrySetSecureStorageAsync(SecureStorageKeys.PASSPORT_DATA, content);
        }

        public async Task<PassportItemsViewModel> GetPassportFromSecureStorage()
        {
            if (await _secureStorageService.TryHasValue(SecureStorageKeys.PASSPORT_DATA))
            {
                return await _secureStorageService.TryGetSecureStorageAsync(SecureStorageKeys.PASSPORT_DATA);
            }

            return null;
        }

        public async Task DeletePassportFromSecureStorage()
        {
            await _secureStorageService.TryClear(SecureStorageKeys.PASSPORT_DATA);
        }
    }
}