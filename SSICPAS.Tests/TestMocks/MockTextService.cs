using System.IO;
using System.Threading.Tasks;
using SSICPAS.Services.Interfaces;

namespace SSICPAS.Tests.TestMocks
{
    public class MockTextService : ITextService
    {
        public MockTextService()
        {
        }

        public Stream GetDCCValueSet()
        {
            return null;
        }

        public Task LoadRemoteLocales()
        {
            return Task.CompletedTask;
        }

        public Task LoadSavedLocales()
        {
            return Task.CompletedTask;
        }

        public void SetLocale(string isoCode)
        {
        }
    }
}
