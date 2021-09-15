using System.IO;
using System.Threading.Tasks;

namespace SSICPAS.Services.Interfaces
{
    public interface ITextService
    {
        Task LoadSavedLocales();
        Task LoadRemoteLocales();
        void SetLocale(string isoCode);
    }
}
