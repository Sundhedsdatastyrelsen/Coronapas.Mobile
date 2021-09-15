using System.Threading.Tasks;
using SSICPAS.Core.WebServices;
using SSICPAS.Models.Dtos;

namespace SSICPAS.Services.Interfaces
{
    public interface IPassportsRepository
    {
        Task<ApiResponse<GetPassportDto>> GetPassports(string jobId = null);
    }
}
