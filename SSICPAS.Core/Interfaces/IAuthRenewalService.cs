using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSICPAS.Core.Interfaces
{
    public interface IAuthRenewalService
    {
        Task<IDictionary<string, string>> RenewAccessToken(Dictionary<string, string> queryValues);
    }
}
