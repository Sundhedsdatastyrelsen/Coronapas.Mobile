using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSICPAS.Core.Services.Interface
{
    public interface IPublicKeyService
    {
        Task FetchPublicKeyFromBackend();
        Task<List<string>> GetPublicKeyByKid(string base64Kid);
    }
}