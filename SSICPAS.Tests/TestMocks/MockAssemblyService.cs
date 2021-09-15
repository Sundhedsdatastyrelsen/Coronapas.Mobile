using System.Reflection;
using SSICPAS.Core.Interfaces;

namespace SSICPAS.Tests.TestMocks
{
    public class MockAssemblyService : IAssemblyService
    {
        public MockAssemblyService()
        {
        }

        public string CertificatesFolderPath => "";

        public Assembly GetSharedFormsAssembly()
        {
            return null;
        }
    }
}
