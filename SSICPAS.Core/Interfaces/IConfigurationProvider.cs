using System.IO;

namespace SSICPAS.Core.Interfaces
{
    public interface IConfigurationProvider
    {
        Stream GetConfiguration();
        string GetEnvironment();
    }
}
