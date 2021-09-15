using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.WebServices;
using SSICPAS.Models.Dtos;
using SSICPAS.Services.Interfaces;
using SSICPAS.ViewModels.Certificates;

namespace SSICPAS.Services.WebServices
{
    public class PassportsService : IPassportsService
    {
        private readonly IPassportsRepository _repository;
        private readonly IFamilyPassportStorageRepository _passportStorageRepository;
        private readonly ITokenProcessorService _tokenProcessorService;
        private readonly ILoggingService _loggingService;
        private readonly INavigationTaskManager _navigationTaskManager;

        public PassportsService(
            IPassportsRepository repository,
            IFamilyPassportStorageRepository passportStorageRepository,
            ITokenProcessorService tokenProcessorService,
            ILoggingService loggingService,
            INavigationTaskManager navigationTaskManager)
        {
            _repository = repository;
            _passportStorageRepository = passportStorageRepository;
            _tokenProcessorService = tokenProcessorService;
            _loggingService = loggingService;
            _navigationTaskManager = navigationTaskManager;
        }

        private async Task<ApiResponse<GetPassportDto>> GetPassportFromServer(string jobId = null)
        {
            return await _repository.GetPassports(jobId);
        }

        public async Task<ApiResponse<FamilyPassportItemsViewModel>> GetPassports(bool forced = false)
        {
            FamilyPassportItemsViewModel savedPassport = await _passportStorageRepository.GetFamilyPassportFromSecureStorage();
            if (!forced)
            {
                if (savedPassport != null
                    && savedPassport.AdditionalData.JobId == null
                    && savedPassport.IsAnyPassportAvailable
                    && savedPassport.IsDKPassportValid
                    && !savedPassport.ShouldPrefetchNewPassport)
                {
                    Debug.Print("Found passport in secure storage. Returning persisted passports");
                    return new ApiResponse<FamilyPassportItemsViewModel>(savedPassport, 200);
                }
            }

            if (savedPassport == null)
            {
                Debug.Print($"{nameof(PassportsService)}.{nameof(GetPassports)}: " +
                    $"No passports in secure storage.");
            }
            else if (savedPassport.HasJobIDInProgress)
            {
                Debug.Print($"{nameof(PassportsService)}.{nameof(GetPassports)}: " +
                    $"JobID in progress, do not clean secure storage.");
            }
            else if (forced && !savedPassport.IsAnyPassportValid)
            {
                Debug.Print($"{nameof(PassportsService)}.{nameof(GetPassports)}: " +
                    $"Force update of passport. No valid passports are present. Clearing all passports from secure storage");
                await _passportStorageRepository.DeleteFamilyPassportFromSecureStorage();
            }
            else if (forced && !savedPassport.IsDKPassportValid)
            {
                Debug.Print($"{nameof(PassportsService)}.{nameof(GetPassports)}: " +
                    $"Force update of passport. No valid DK passports are present. Clearing only DK passports from secure storage");
                await _passportStorageRepository.DeleteOnlyDKPassportFromSecureStorage();
            }

            FamilyPassportItemsViewModel passportFromServer = new FamilyPassportItemsViewModel();

            Debug.Print("Making Http call to server to fetch new passports"
                + (savedPassport?.AdditionalData.JobId != null ? $" with jobid {savedPassport?.AdditionalData.JobId}" : " with no jobid"));

            ApiResponse<GetPassportDto> response = await GetPassportFromServer(savedPassport?.AdditionalData.JobId);

            if (response?.Data != null && response.IsSuccessfull)
            {
                try
                {
                    await passportFromServer.ProcessPassportToken(response.Data);
                }
                catch (Exception e)
                {
                    //if any issue happened with the process defaults to the user not having a passport
                    _loggingService.LogException(LogSeverity.ERROR, e, "Failed to process passport token from backend");
                }

                if ((string.IsNullOrEmpty(passportFromServer.AdditionalData.JobId) && passportFromServer.IsAnyPassportAvailable) // new passport available
                    || savedPassport == null                           // no old passport
                    || !savedPassport.IsAnyPassportValid               // no valid passport left
                    )
                {
                    savedPassport = passportFromServer; 
                    await _passportStorageRepository.SaveFamilyPassportToSecureStorage(savedPassport);
                }
            }
            else
            {
                if (response?.IsSuccessfull != true)
                {
                    switch (response.ErrorType)
                    {
                        case ServiceErrorType.BadInternetConnection:
                        case ServiceErrorType.NoInternetConnection:
                            await _navigationTaskManager.HandlerErrors(response, false);
                            break;
                        default:
                            await _navigationTaskManager.HandlerErrors(response, true);
                            break;
                    }
                }
            }

            return new ApiResponse<FamilyPassportItemsViewModel>(response, savedPassport ?? passportFromServer);
        }
    }
}
