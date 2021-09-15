using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SSICPAS.Data;
using SSICPAS.Services.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SSICPAS.Services
{
    public class GyroscopeService : IGyroscopeService
    {
        List<EventHandler<GyroscopeChangedEventArgs>> _eventHandlers = new List<EventHandler<GyroscopeChangedEventArgs>>();
        bool _active;

        public GyroscopeService()
        {
            MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.GOING_TO_BACKGROUND, StopCapturingSensorData);
            MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.BACK_FROM_BACKGROUND, StartCapturingSensorData);
        }

        public static bool IsGyroEnabled { get; set; } = false;
        
        public void SubscribeGyroscopeReadingUpdatedEvent(EventHandler<GyroscopeChangedEventArgs> eventHandler, Action onNoGyro = null)
        {
            TurnOffOrientation();
            _eventHandlers.Add( eventHandler);
            TurnOnOrientation();
            if (!IsGyroEnabled)
            {
                onNoGyro?.Invoke();
            }
        }

        void StartCapturingSensorData(object sender)
        {
            if (_eventHandlers.Any() && !_active)
            {
                Debug.Print($"Starting gyroscope (Started by {sender.GetType().Name})");
                _eventHandlers.ForEach(x => Gyroscope.ReadingChanged += x);
                _active = true;
            }
        }

        void StopCapturingSensorData(object sender)
        {
            if (_eventHandlers.Any() && _active)
            {
                Debug.Print($"Stopping gyroscope (Started by {sender.GetType().Name})");
                _eventHandlers.ForEach(x => Gyroscope.ReadingChanged -= x);
                _active = false;
            }
        }

        public void ToggleOrientation(SensorSpeed speed = SensorSpeed.Game)
        {
            try
            {
                if (Gyroscope.IsMonitoring)
                    Gyroscope.Stop();
                else
                    Gyroscope.Start(speed);
                IsGyroEnabled = true;
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
                Debug.WriteLine(fnsEx.Message);
                IsGyroEnabled = false;
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                Debug.WriteLine(ex.Message);
                IsGyroEnabled = false;
            }
        }

        public void TurnOnOrientation(SensorSpeed speed = SensorSpeed.Game)
        {
            try
            {
                if (!Gyroscope.IsMonitoring)
                {
                    StartCapturingSensorData(this);
                    Gyroscope.Start(speed);
                    IsGyroEnabled = true;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
                Debug.WriteLine(fnsEx.Message);
                IsGyroEnabled = false;
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                Debug.WriteLine(ex.Message);
                IsGyroEnabled = false;
            }
        }

        public void TurnOffOrientation()
        {
            try
            {
                if (Gyroscope.IsMonitoring)
                {
                    StopCapturingSensorData(this);
                    Gyroscope.Stop();
                    IsGyroEnabled = true;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
                Debug.WriteLine(fnsEx.Message);
                IsGyroEnabled = false;
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                Debug.WriteLine(ex.Message);
                IsGyroEnabled = false;
            }
        }


        public void ToggleGyroscope(SensorSpeed speed = SensorSpeed.Game)
        {
            try
            {
                if (Gyroscope.IsMonitoring)
                    Gyroscope.Stop();
                else
                    Gyroscope.Start(speed);
                IsGyroEnabled = true;
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
                Debug.WriteLine(fnsEx.Message);
                IsGyroEnabled = false;
            }
            catch (Exception ex)
            {
                // Other error has occurred.
                Debug.WriteLine(ex.Message);
                IsGyroEnabled = false;
            }
        }
    }
}
