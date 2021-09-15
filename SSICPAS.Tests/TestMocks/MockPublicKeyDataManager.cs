using System.Collections.Generic;
using System.Threading.Tasks;
using SSICPAS.Core.Services.Interface;

namespace SSICPAS.Tests.TestMocks
{
    public class MockPublicKeyDataManager: IPublicKeyService
    {
        public Task FetchPublicKeyFromBackend()
        {
            return Task.CompletedTask;
        }

        public Task<List<string>> GetPublicKeyByKid(string base64Kid)
        {
            return (Task<List<string>>) Task.CompletedTask;
        }
    }
}