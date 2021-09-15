using System;
namespace SSICPAS.Core.Data
{
    public interface IPreferencesService
    {
        void SetUserPreference(string key, string value);
        void SetUserPreference(string key, bool value);
        void SetUserPreference(string key, int value);
        void SetUserPreference(string key, long value);
        void SetUserPreference(string key, DateTime value);
        bool GetUserPreferenceAsBoolean(string key);
        string GetUserPreferenceAsString(string key);
        int GetUserPreferenceAsInt(string key);
        long GetUserPreferenceAsLong(string key);
        DateTime GetUserPreferenceAsDateTime(string key);
        void ClearUserPreference(string key);
        void ClearAllUserPreferences();
    }
}
