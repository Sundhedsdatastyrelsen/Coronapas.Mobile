using SSICPAS.Core.Interfaces;
using System.Linq;
using Xamarin.Essentials;

namespace SSICPAS.Services
{
    public class ConnectivityService : IConnectivityService
    {
        public bool HasInternetConnection()
        {
            if (Connectivity.ConnectionProfiles.Any(x => x is ConnectionProfile.WiFi || x is ConnectionProfile.Cellular))
            {
                // Active Wi-Fi or Cellular connection.
                switch (Connectivity.NetworkAccess)
                {
                    case NetworkAccess.Internet:
                        return true;
                    case NetworkAccess.ConstrainedInternet:
                    case NetworkAccess.Unknown:
                    case NetworkAccess.None:
                    case NetworkAccess.Local:
                    default:
                        break;
                }
            }
            return false;
        }
    }
}
