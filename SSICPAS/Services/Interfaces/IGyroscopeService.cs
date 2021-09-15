using System;
using Xamarin.Essentials;

namespace SSICPAS.Services.Interfaces
{
    public interface IGyroscopeService
    {
        void SubscribeGyroscopeReadingUpdatedEvent(EventHandler<GyroscopeChangedEventArgs> eventHandler, Action onNoGyro = null);
        void ToggleOrientation(SensorSpeed speed = SensorSpeed.Game);
        void TurnOnOrientation(SensorSpeed speed = SensorSpeed.Game);
        void TurnOffOrientation();
    }
}
