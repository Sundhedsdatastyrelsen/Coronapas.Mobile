using System.Collections.Generic;
using System.Threading.Tasks;
using SSICPAS.Core.Services.Model;
using SSICPAS.Core.WebServices;

namespace SSICPAS.Services.Interfaces
{
    public interface IPublicKeyRepository
    {
        public Task<ApiResponse<List<PublicKeyDto>>> GetPublicKey();
    }
}
