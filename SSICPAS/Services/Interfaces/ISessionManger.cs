using System;
using System.Threading;

namespace SSICPAS.Services.Interfaces
{
    public interface ISessionManager
    {
        void EndTrackSession();
        void StartTrackSessionAsync();
        EventHandler<SessionTrackingEventArgs> OnSessionTrackEnded { get; set; }
        CancellationToken RegisterCancellationToken();
    }
}