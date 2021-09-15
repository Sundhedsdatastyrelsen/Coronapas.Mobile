using SSICPAS.Core.Interfaces;

namespace SSICPAS.Tests.TestMocks
{
    public class MockConnectivityService : IConnectivityService
    {
        bool _hasInternet = true;

        public MockConnectivityService()
        {
        }

        public void SetConnection(bool hasInternet)
        {
            _hasInternet = hasInternet;
        }

        public bool HasInternetConnection()
        {
            return _hasInternet;
        }
    }
}
