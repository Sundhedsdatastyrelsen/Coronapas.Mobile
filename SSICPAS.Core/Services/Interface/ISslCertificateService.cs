using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace SSICPAS.Core.Services.Interface
{
    public interface ISslCertificateService
    {
        X509Certificate GetTrustedCertificate();
        void SetTrustedCertificate(Stream stream);
    }
}