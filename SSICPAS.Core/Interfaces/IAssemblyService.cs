using System.Reflection;

namespace SSICPAS.Core.Interfaces
{
    public interface IAssemblyService
    {
        string CertificatesFolderPath { get; }
        Assembly GetSharedFormsAssembly();
    }
}
