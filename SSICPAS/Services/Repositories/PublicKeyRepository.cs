using System.Collections.Generic;
using System.Threading.Tasks;
using SSICPAS.Configuration;
using SSICPAS.Core.Services.Model;
using SSICPAS.Core.WebServices;
using SSICPAS.Services.Interfaces;

namespace SSICPAS.Services.Repositories
{
    public class PublicKeyRepository : BaseRepository, IPublicKeyRepository
    {
        public async Task<ApiResponse<List<PublicKeyDto>>> GetPublicKey()
        {
            string url = Urls.URL_GET_PUBLIC_KEY;
            return await _restClient.Get<List<PublicKeyDto>>(url, false);
        }
    }
}
