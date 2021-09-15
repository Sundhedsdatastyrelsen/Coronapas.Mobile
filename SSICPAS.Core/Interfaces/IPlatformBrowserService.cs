using System.Threading.Tasks;

namespace SSICPAS.Core.Interfaces
{
    public interface IPlatformBrowserService
    {
        public Task CloseBrowser(bool animated);
    }
}