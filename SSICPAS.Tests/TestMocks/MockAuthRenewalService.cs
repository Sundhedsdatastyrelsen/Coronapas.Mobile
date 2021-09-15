using System.Collections.Generic;
using System.Threading.Tasks;
using SSICPAS.Core.Interfaces;

namespace SSICPAS.Tests.TestMocks
{
    public class MockAuthRenewalService : IAuthRenewalService
    {
        public MockAuthRenewalService()
        {
        }

        public Task<IDictionary<string, string>> RenewAccessToken(Dictionary<string, string> queryValues)
        {
            return null;
        }
    }
}
