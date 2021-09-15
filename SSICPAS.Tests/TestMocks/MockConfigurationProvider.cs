using System.IO;
using SSICPAS.Core.Interfaces;

namespace SSICPAS.Tests.TestMocks
{
    public class MockConfigurationProvider : IConfigurationProvider
    {
        public MockConfigurationProvider()
        {
        }

        public Stream GetConfiguration()
        {
            return this.GetType().Assembly.GetManifestResourceStream("SSICPAS.Tests.appsettings.Unittests.json");
        }

        public string GetEnvironment()
        {
            return "Unittests";
        }
    }
}
