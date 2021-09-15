using System.IO;
using System.Threading.Tasks;
using SSICPAS.Configuration;
using SSICPAS.Core.WebServices;
using SSICPAS.Services.Interfaces;

namespace SSICPAS.Services.Repositories
{
    public class TextRepository : BaseRepository, ITextRepository
    {
        public async Task<ApiResponse<Stream>> GetTexts(string currentVersion)
        {
            string url = Urls.URL_GET_TEXTS;
            _restClient.RegisterLocalesRequestHeaders(currentVersion);
            ApiResponse<Stream> response = await _restClient.GetFileAsStreamAsync(url);
            _restClient.ClearLocalesRequestHeaders();
            return response;
        }
    }
}
