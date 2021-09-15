using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model;
using SSICPAS.Core.WebServices;
using SSICPAS.Models;
using SSICPAS.Services.Interfaces;

namespace SSICPAS.Services.DataManagers
{
    public class PublicKeyDataManager: IPublicKeyService
    {
        private ISettingsService _settingsService;
        private IDateTimeService _dateTimeService;
        private INavigationTaskManager _navigationTaskManager;
        private IPublicKeyStorageRepository _publicKeySecureStorageService;
        private IPublicKeyRepository _publicKeyRepository;

        private PublicKeyStorageModel _publicKeyStorageModel { get; set; } = new PublicKeyStorageModel();
        private TimeSpan _periodicFetchingInterval { get; set; }

        public PublicKeyDataManager(
            IPublicKeyRepository publicKeyRepository,
            ISettingsService settingsService,
            INavigationTaskManager navigationTaskManager,
            IDateTimeService dateTimeService, 
            IPublicKeyStorageRepository publicKeySecureStorageService 
            )
        {
            _settingsService = settingsService;
            _dateTimeService = dateTimeService;
            _navigationTaskManager = navigationTaskManager;
            _publicKeySecureStorageService = publicKeySecureStorageService;
            _publicKeyRepository = publicKeyRepository;
            _periodicFetchingInterval = TimeSpan.FromHours(_settingsService.PublicKeyPeriodicFetchingIntervalInHours);
        }

        public async Task FetchPublicKeyFromBackend()
        {
            var publicKeyStorageModel = await _publicKeySecureStorageService.GetPublicKeyFromSecureStorage();
            if (publicKeyStorageModel == null 
                || (!publicKeyStorageModel?.PublicKeys.Any() ?? true ) 
                || publicKeyStorageModel?.LastFetchTimestamp.Add(_periodicFetchingInterval) < _dateTimeService.Now)
            {
                ApiResponse<List<PublicKeyDto>> response = await _publicKeyRepository.GetPublicKey();
                if (response.Data != null && response.IsSuccessfull)
                {
                    if (publicKeyStorageModel == null)
                    {
                        publicKeyStorageModel = new PublicKeyStorageModel();
                    }
                    publicKeyStorageModel.PublicKeys = response.Data;
                    publicKeyStorageModel.LastFetchTimestamp = _dateTimeService.Now;
                    await _publicKeySecureStorageService.SavePublicKeyToSecureStorage(publicKeyStorageModel);
                }

                // Handle ForceUpdate
                if (response.StatusCode == 410)
                {
                    await _navigationTaskManager.HandlerErrors(response, true);
                }
            }
            _publicKeyStorageModel = publicKeyStorageModel ?? new PublicKeyStorageModel();
        }

        public async Task<List<string>> GetPublicKeyByKid(string base64Kid)
        {
            //In case the app has connection issue on startup
            if (!_publicKeyStorageModel.PublicKeys.Any())
            {
                await FetchPublicKeyFromBackend();
            }
            
            var pks = _publicKeyStorageModel.PublicKeys.Where(x => x.Kid == base64Kid).Select(x => x.PublicKey);
            return pks.ToList();
        }
    }
}