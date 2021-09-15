using SSICPAS.Configuration;
using SSICPAS.Core.WebServices;

namespace SSICPAS.Services.Repositories
{
    public class BaseRepository
    {
        protected IRestClient _restClient = IoCContainer.Resolve<IRestClient>();
    }
}
