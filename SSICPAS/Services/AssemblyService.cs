using System.Reflection;
using SSICPAS.Core.Interfaces;

namespace SSICPAS.Services
{
    public class AssemblyService : IAssemblyService
    {
        public string CertificatesFolderPath => "SSICPAS.Certs";

        public Assembly GetSharedFormsAssembly()
        {
            return typeof(App).GetTypeInfo().Assembly;
        }
    }
}
