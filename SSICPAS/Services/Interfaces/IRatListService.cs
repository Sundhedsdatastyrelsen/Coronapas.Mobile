using System.IO;
using System.Threading.Tasks;

namespace SSICPAS.Services.Interfaces
{
    public interface IRatListService
    {
        Task LoadSavedFiles();
        Task LoadRemoteFiles();
        Task<string> GetRatList();
        Task<string> GetDCCValueSet();
    }
}
