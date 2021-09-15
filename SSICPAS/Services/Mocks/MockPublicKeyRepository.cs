using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using SSICPAS.Core.Services.Model;
using SSICPAS.Core.WebServices;
using SSICPAS.Services.Interfaces;

namespace SSICPAS.Services.Mocks
{
    public class MockPublicKeyRepository : IPublicKeyRepository
    {
        public async Task<ApiResponse<List<PublicKeyDto>>> GetPublicKey()
        {
            ApiResponse<List<PublicKeyDto>> result = new ApiResponse<List<PublicKeyDto>>("");
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("SSICPAS.publicKey.pem");
            if (stream == null)
                return result;
            var reader = new StreamReader(stream);
            string publicKey = await reader.ReadToEndAsync();
            result.Data = new List<PublicKeyDto>()
            {
                new PublicKeyDto()
                {
                    Kid = "qweqweqwe",
                    PublicKey = publicKey
                }
            };
            
            return result;
        }
    }
}
