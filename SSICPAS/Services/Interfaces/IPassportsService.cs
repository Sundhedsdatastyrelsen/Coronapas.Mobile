using System.Threading.Tasks;
using SSICPAS.Core.WebServices;
using SSICPAS.ViewModels.Certificates;

namespace SSICPAS.Services.Interfaces
{
    public interface IPassportsService
    {
        Task<ApiResponse<FamilyPassportItemsViewModel>> GetPassports(bool forced = false);
    }
}
