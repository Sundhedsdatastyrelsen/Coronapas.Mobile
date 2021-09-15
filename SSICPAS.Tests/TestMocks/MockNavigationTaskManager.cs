using System.Threading.Tasks;
using SSICPAS.Core.WebServices;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;

namespace SSICPAS.Tests.NavigationTests
{
    public class MockNavigationTaskManager : INavigationTaskManager
    {
        public MockNavigationTaskManager()
        {
        }

        public Task HandlerErrors(ApiResponse response, bool silently = false)
        {
            return Task.FromResult(new object());
        }

        public Task ShowSuccessPage(string message)
        {
            return Task.FromResult(new object());
        }
    }
}
