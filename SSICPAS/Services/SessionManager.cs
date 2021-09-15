using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SSICPAS.Core.Data;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Data;
using SSICPAS.Models;
using SSICPAS.Services.Interfaces;
using Xamarin.Forms;

namespace SSICPAS.Services
{
    public class SessionManager: ISessionManager
    {
        private DateTime _onSleepDateTime;
        private List<CancellationTokenSource> _cancellationTokenSources = new List<CancellationTokenSource>();
        private INavigationService _navigationService;
        private ISecureStorageService<PinCodeBiometricsModel> _pinCodeService;
        private IDateTimeService _dateTimeService;
        private TimeSpan _sessionDuration;

        public SessionManager(ISettingsService settingsService, INavigationService navigationService, ISecureStorageService<PinCodeBiometricsModel> pinCodeService, IDateTimeService dateTimeService)
        {
            _navigationService = navigationService;
            _pinCodeService = pinCodeService;
            _dateTimeService = dateTimeService;
            _onSleepDateTime = dateTimeService.Now;
            _sessionDuration = TimeSpan.FromMinutes(settingsService.TimeOutMinuteUntilReauthenticate);
        }

        public CancellationToken RegisterCancellationToken()
        {
            CancellationTokenSource newCancellationToken = new CancellationTokenSource();
            _cancellationTokenSources.Add(newCancellationToken);
            return newCancellationToken.Token;
        }

        public EventHandler<SessionTrackingEventArgs> OnSessionTrackEnded { get; set; }

        public async void EndTrackSession()
        {
            // Determine how long the app have been in the background.
            TimeSpan elapsed = _dateTimeService.Now - _onSleepDateTime;

            // If the app have been in the background for too long, then expire the session.
            bool isSessionExpired = elapsed.Minutes >= _sessionDuration.Minutes;
            bool wasSignedIn = await HasPinCodeAsync();

            switch (wasSignedIn, isSessionExpired)
            {
                case (false, _):
                    // Happens in these scenarios:
                    // When the app is moved from background to foreground.
                    // When the nemid auth flow is completed, and the app is moved to foreground.
                    //
                    // The user was not signed in when the app went to sleep,
                    // so there are no handlers to be cancelled.
                    // so there is no need to ask the user for verifying their pincode.
                    return;
                case (true, true):
                    await EndTrackSession_UserWasSignedIn_SessionExpired();
                    return;
                case (true, false):
                    EndTrackSession_UserWasSignedIn_SessionStillValid(elapsed);
                    return;
            }
        }

        private async Task EndTrackSession_UserWasSignedIn_SessionExpired()
        {
            // The app has been in the background for too long, so the session has expired.
            // The user must verify their pincode in order to use the app.
            await _navigationService.OpenVerifyPinCodePageAsync(Device.RuntimePlatform == Device.iOS);
        }

        private void EndTrackSession_UserWasSignedIn_SessionStillValid(TimeSpan elapsed)
        {
            // Run the OnSessionTrackEnded event handlers
            Task.Run(() =>
            {
                OnSessionTrackEnded?.Invoke(this, new SessionTrackingEventArgs(elapsed));
            });
        }

        public async void StartTrackSessionAsync()
        {
            // Begin measuring elapsed time that the app have been in the background.
            // When resumed, the elapsed time is used for determining if the session has expired.
            _onSleepDateTime = _dateTimeService.Now;
            bool isSignedIn = await HasPinCodeAsync();
            if (isSignedIn)
            {
                StartTrackSessionAsync_CancelTokenSources();
            }
        }

        // The user is moving the app to the background.
        // The user is signed in.
        // Cancel active network connections.
        private void StartTrackSessionAsync_CancelTokenSources()
        {
            _cancellationTokenSources.ForEach(x => x.Cancel());
            _cancellationTokenSources.Clear();
        }

        private async Task<bool> HasPinCodeAsync()
        {
            PinCodeBiometricsModel PinCodeModel = await _pinCodeService.GetSecureStorageAsync(SecureStorageKeys.PIN_LOCATION);
            if (PinCodeModel == null)
            {
                return false;
            } else
            {
                return true;
            }
        }
    }

    public class SessionTrackingEventArgs : EventArgs
    {
        public SessionTrackingEventArgs(TimeSpan elapsedTime)
        {
            ElapsedTime = elapsedTime;
        }

        public TimeSpan ElapsedTime;
    }
}
