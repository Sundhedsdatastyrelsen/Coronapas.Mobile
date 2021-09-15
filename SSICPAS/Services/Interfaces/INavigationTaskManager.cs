using System.Threading.Tasks;
using SSICPAS.Core.WebServices;

namespace SSICPAS.Services.Interfaces
{
    public interface INavigationTaskManager
    {
        Task ShowSuccessPage(string message);
        Task HandlerErrors(ApiResponse response, bool silently = false);
    }
}
