using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Plugin.SimpleAudioPlayer;
using SSICPAS.Core.Data;
using SSICPAS.Data;
using SSICPAS.Services.Interfaces;
using Xamarin.Essentials;

namespace SSICPAS.Services
{
    public class DeviceFeedbackService : IDeviceFeedbackService
    {
        private readonly IPreferencesService _preferencesService;
        
        public DeviceFeedbackService(IPreferencesService preferencesService)
        {
            _preferencesService = preferencesService;
        }
        
        public void Vibrate()
        {
            try
            {
                if (!_preferencesService.GetUserPreferenceAsBoolean(PreferencesKeys.SCANNER_VIBRATION_SETTING)) return;
                
                Vibration.Vibrate();
            }
            catch (FeatureNotSupportedException ex)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public void Vibrate(double durationMs)
        {
            try
            {
                if (!_preferencesService.GetUserPreferenceAsBoolean(PreferencesKeys.SCANNER_VIBRATION_SETTING)) return;
                
                Vibration.Vibrate(durationMs);
            }
            catch (FeatureNotSupportedException ex)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public void PlaySound(string fileNameWithExtension)
        {
            try
            {
                if (!_preferencesService.GetUserPreferenceAsBoolean(PreferencesKeys.SCANNER_SOUND_SETTING)) return;
                
                Stream audioStream = typeof(App).GetTypeInfo().Assembly
                    .GetManifestResourceStream($"SSICPAS.Resources.Sounds.{fileNameWithExtension}");
                var player = CrossSimpleAudioPlayer.Current;

                player.Load(audioStream);
                player.Play();
            }
            catch (NullReferenceException nre)
            {
                Debug.WriteLine($"Could not find audio file: {fileNameWithExtension} at SSICPAS.Resources.Sounds.{fileNameWithExtension}");
                Debug.WriteLine(nre.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public void PerformHapticFeedback(HapticFeedbackType type)
        {
            try
            {
                HapticFeedback.Perform(type);
            }
            catch (FeatureNotSupportedException ex)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}