using SSICPAS.Configuration;
using SSICPAS.Core.WebServices;
using SSICPAS.Services.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace SSICPAS.Services.Repositories
{
    public class RatlistRepository : BaseRepository, IRatListRepository
    {
        public async Task<ApiResponse<Stream>> GetRatList(string currentVersion)
        {
            string url = Urls.URL_GET_RATLIST;
            _restClient.RegisterLocalesRequestHeaders(currentVersion);
            ApiResponse<Stream> response = await _restClient.GetFileAsStreamAsync(url);
            _restClient.ClearLocalesRequestHeaders();
            return response;
        }
    }
}
