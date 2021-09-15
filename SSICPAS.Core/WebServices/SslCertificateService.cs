using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Services.Interface;

namespace SSICPAS.Core.WebServices
{
    public class SslCertificateService : ISslCertificateService
    {
        private readonly ILoggingService _loggingService;
        private X509Certificate _trustedCert;

        public SslCertificateService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }
        
        public X509Certificate GetTrustedCertificate()
        {
            return _trustedCert;
        }

        public void SetTrustedCertificate(Stream trustedCertStream)
        {
            if (trustedCertStream == null)
            {
                Debug.WriteLine($"{nameof(SetTrustedCertificate)} failure: given parameter {nameof(trustedCertStream)} was null");
                return;
            }

            try
            {
                var ms = new MemoryStream();
                trustedCertStream.CopyTo(ms);
                _trustedCert = new X509Certificate(ms.ToArray());
            }
            catch (Exception e)
            {
                _loggingService.LogException(LogSeverity.SECURITY_ERROR, e, $"{nameof(RestClient)}.{nameof(SetTrustedCertificate)}: Failed to load trusted certificate");
            }
        }
    }
}