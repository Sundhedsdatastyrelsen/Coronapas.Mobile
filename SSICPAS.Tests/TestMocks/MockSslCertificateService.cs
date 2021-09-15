using System.IO;
using System.Security.Cryptography.X509Certificates;
using SSICPAS.Core.Services.Interface;

namespace SSICPAS.Tests.TestMocks
{
    public class MockSslCertificateService : ISslCertificateService
    {
        public MockSslCertificateService()
        {
        }

        public X509Certificate GetTrustedCertificate()
        {
            return null;
        }

        public void SetTrustedCertificate(Stream stream)
        {
        }
    }
}
