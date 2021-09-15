using System;
using System.IO;
using System.Threading.Tasks;
using SSICPAS.Core.WebServices;

namespace SSICPAS.Services.Interfaces
{
    public interface IRatListRepository
    {
        Task<ApiResponse<Stream>> GetRatList(string currentVersion);
    }
}
