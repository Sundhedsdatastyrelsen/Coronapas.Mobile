using SSICPAS.Services.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace SSICPAS.Tests.TestMocks
{
    public class MockRatListService : IRatListService
    {
        public MockRatListService() { }
        public Task<string> GetDCCValueSet() => Task.Run(() => string.Empty);
        public Task<string> GetRatList() => Task.Run(() => string.Empty);
        public Task LoadRemoteFiles() => Task.CompletedTask;
        public Task LoadSavedFiles() => Task.CompletedTask;
    }
}
