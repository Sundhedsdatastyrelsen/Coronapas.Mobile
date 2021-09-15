using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.WebServices;
using SSICPAS.Data;
using SSICPAS.Models.Dtos;
using SSICPAS.Services.Interfaces;

namespace SSICPAS.Services.Repositories
{
    public class PassportsRepository : BaseRepository, IPassportsRepository
    {
        public async Task<ApiResponse<GetPassportDto>> GetPassports(string jobId = null)
        {
            if (!string.IsNullOrEmpty(jobId))
            {
                _restClient.SetAdditionalHeader("JobId", jobId);
            }

            string url = Urls.URL_GET_PASSPORTS;
            ApiResponse<GetPassportDto> response = await _restClient.Get<GetPassportDto>(url);
            IoCContainer.Resolve<IPreferencesService>().SetUserPreference(PreferencesKeys.LATEST_PASSPORT_CALL_TO_BACKEND_TIMESTAMP, IoCContainer.Resolve<IDateTimeService>().Now);

            if (!string.IsNullOrEmpty(jobId))
            {
                _restClient.ClearAdditionalHeader("JobId");
            }

            return response;
        }
    }
}
