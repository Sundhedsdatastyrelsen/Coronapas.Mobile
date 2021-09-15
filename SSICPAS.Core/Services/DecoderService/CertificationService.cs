using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSICPAS.Core.CustomExceptions;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model.CoseModel;

namespace SSICPAS.Core.Services.DecoderService
{
    public class CertificationService: ICertificationService
    {
        private readonly IPublicKeyService _publicKeyService;
        private readonly ILoggingService _loggingService;

        public CertificationService(ILoggingService loggingService, IPublicKeyService publicKeyService)
        {
            _publicKeyService = publicKeyService;
            _loggingService = loggingService;
        }
        
        public async Task VerifyCoseSign1Object(CoseSign1Object coseSign1Object)
        {
            string kidBase64 = Convert.ToBase64String(coseSign1Object.GetKeyIdentifier());

            List<string> publicKeys = await _publicKeyService.GetPublicKeyByKid(kidBase64);
            if (!publicKeys.Any())
            {
                throw new TokenDecodeException(
                    $"no public key corespondent to provided key identifier found. key identifier: {kidBase64}");
            }

            foreach (string publicKey in publicKeys)
            {
                try
                {
                    byte[] pk = Convert.FromBase64String(publicKey);
                    coseSign1Object.VerifySignature(pk);
                    return;
                }
                catch (Exception e)
                {
                    _loggingService.LogException(LogSeverity.SECURITY_WARNING, e, $"Cannot verify signature");
                    continue;
                }
            }

            throw new TokenDecodeException("Signature verification failed for all attempted keys");
        }
    }
}