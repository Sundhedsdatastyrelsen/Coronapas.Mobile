using System.Threading.Tasks;

namespace SSICPAS.Services.Interfaces
{
    public interface IUserService
    {
        Task CleanupAfterFreshInstallAsync();
        Task InitializeAsync();
        Task UserLogoutAsync(bool showWarning = true, bool shouldNavigateToLanding = true);
    }
}