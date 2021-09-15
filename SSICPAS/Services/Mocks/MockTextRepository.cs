using System.IO;
using System.Threading.Tasks;
using SSICPAS.Core.WebServices;
using SSICPAS.Services.Interfaces;

namespace SSICPAS.Services.Repositories
{
    public class MockTextRepository : ITextRepository
    {
        public async Task<ApiResponse<Stream>> GetTexts(string currentVersion)
        {
            return await Task.FromResult(new ApiResponse<Stream>(null, 200));
        }
    }
}
