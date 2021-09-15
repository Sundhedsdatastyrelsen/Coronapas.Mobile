using System.IO;
using System.Threading.Tasks;
using SSICPAS.Core.WebServices;

namespace SSICPAS.Services.Interfaces
{
    public interface ITextRepository
    {
        Task<ApiResponse<Stream>> GetTexts(string currentVersion);
    }
}
