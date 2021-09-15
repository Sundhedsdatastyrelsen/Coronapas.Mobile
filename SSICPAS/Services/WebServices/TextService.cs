using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.WebServices;
using SSICPAS.Data;
using SSICPAS.Models.Exceptions;
using SSICPAS.Services.Interfaces;

namespace SSICPAS.Services.WebServices
{
    public class TextService : ITextService
    {
        private readonly IPreferencesService _preferencesService;
        private readonly ITextRepository _textRepository;
        private readonly ILoggingService _loggingService;
        private readonly INavigationTaskManager _navigationTaskManager;
        private readonly IDateTimeService _dateTimeService;

        private const string ZIP_FILE_NAME = "locales.zip";
        private const Environment.SpecialFolder ZIP_FILE_DIRECTORY = Environment.SpecialFolder.Personal;

        public TextService(IPreferencesService preferencesService,
            ITextRepository textRepository,
            ILoggingService loggingService,
            INavigationTaskManager navigationTaskManager, 
            IDateTimeService dateTimeService)
        {
            _preferencesService = preferencesService;
            _textRepository = textRepository;
            _loggingService = loggingService;
            _navigationTaskManager = navigationTaskManager;
            _dateTimeService = dateTimeService;
        }

        public async Task LoadSavedLocales()
        {
            await LoadEmbeddedCopies();
            SetLocales();
        }

        public async Task LoadRemoteLocales()
        {
            long lastTimeFetchedTexts = _preferencesService.GetUserPreferenceAsLong(PreferencesKeys.LAST_TIME_FETCHED_TEXTS);
            int minTimeBetweenFetches = IoCContainer.Resolve<ISettingsService>().TextFileFetchIntervalInMinutes;
            if ((_dateTimeService.Now - new DateTime(lastTimeFetchedTexts)).TotalMinutes > minTimeBetweenFetches)
            {
                await FetchAndSaveLatestVersionOfLocales();
                SetLocales();
            }
        }

        private void SetLocales()
        {
            string selectedLanguage = _preferencesService.GetUserPreferenceAsString(PreferencesKeys.LANGUAGE_SETTING);
            SetLocale(string.IsNullOrEmpty(selectedLanguage) ? "dk" : selectedLanguage);
        }

        private async Task LoadEmbeddedCopies()
        {
            try
            {
                var assembly = typeof(TextService).GetTypeInfo().Assembly;
                Stream dkStream = assembly.GetManifestResourceStream("SSICPAS.Locales.dk.json");
                Stream enStream = assembly.GetManifestResourceStream("SSICPAS.Locales.en.json");

                string dkText = "";
                string enText = "";

                using (var reader = new StreamReader(dkStream))
                {
                    dkText = await reader.ReadToEndAsync();
                }
                using (var reader = new StreamReader(enStream))
                {
                    enText = await reader.ReadToEndAsync();
                }
                
                File.WriteAllText(Path.Combine(Environment.GetFolderPath(ZIP_FILE_DIRECTORY), "dk.json"), dkText);
                File.WriteAllText(Path.Combine(Environment.GetFolderPath(ZIP_FILE_DIRECTORY), "en.json"), enText);
            }
            catch (Exception e)
            {
                _loggingService.LogException(LogSeverity.ERROR, new TextServiceException("Failed to load embedded locales", e));
            }
        }

        private async Task FetchAndSaveLatestVersionOfLocales()
        {
            string currentVersion = _preferencesService.GetUserPreferenceAsString(PreferencesKeys.CURRENT_TEXT_VERSION);
            Stream result = await FetchZipFileFromServer(
                String.IsNullOrEmpty(currentVersion)
                ? IoCContainer.Resolve<ISettingsService>().EmbeddedTextVersion
                : currentVersion);
            if (result != null && result.Length != 0)
            {
                string path = await SaveZipFile(result);
                if (!string.IsNullOrEmpty(path))
                {
                    string versionNumber = ExtractZipFile(path);
                    _preferencesService.SetUserPreference(PreferencesKeys.CURRENT_TEXT_VERSION, versionNumber);
                }
            }
        }

        public void SetLocale(string isoCode)
        {
            string versionNumberOfLastFetchedTextFile = _preferencesService.GetUserPreferenceAsString(PreferencesKeys.CURRENT_TEXT_VERSION);
            FileStream localeFile;

            if (IsLastFetchedVersionNewerThanEmbeddedVersion())
            {
                try
                {
                    localeFile = File.OpenRead(Path.Combine(Environment.GetFolderPath(ZIP_FILE_DIRECTORY), $"{isoCode}_{versionNumberOfLastFetchedTextFile}.json"));
                    LocaleService.Current.LoadLocale(isoCode, localeFile, false);
                }
                catch (Exception e)
                {
                    localeFile = File.OpenRead(Path.Combine(Environment.GetFolderPath(ZIP_FILE_DIRECTORY), $"{isoCode}.json"));
                    LocaleService.Current.LoadLocale(isoCode, localeFile, true);

                    _loggingService.LogException(LogSeverity.WARNING, new TextServiceException("Failed to load fetched locale file. Loaded embedded copy.", e));
                }
            }
            else
            {
                localeFile = File.OpenRead(Path.Combine(Environment.GetFolderPath(ZIP_FILE_DIRECTORY), $"{isoCode}.json"));
                LocaleService.Current.LoadLocale(isoCode, localeFile, true);
            }
        }

        private bool IsLastFetchedVersionNewerThanEmbeddedVersion()
        {
            string stringVersionNumberOfLastFetchedTextFile = _preferencesService.GetUserPreferenceAsString(PreferencesKeys.CURRENT_TEXT_VERSION);
            string stringVersionNumberOfEmbeddedTextFile = IoCContainer.Resolve<ISettingsService>().EmbeddedTextVersion;

            if (string.IsNullOrEmpty(stringVersionNumberOfLastFetchedTextFile))
            {
                stringVersionNumberOfLastFetchedTextFile = "0.0";
            }

            Version versionNumberOfLastFetchedTextFile = new Version(stringVersionNumberOfLastFetchedTextFile);
            Version versionNumberOfEmbeddedTextFile = new Version(stringVersionNumberOfEmbeddedTextFile);

            return versionNumberOfLastFetchedTextFile.CompareTo(versionNumberOfEmbeddedTextFile) > 0;
        }

        private string ExtractZipFile(string path)
        {
            try
            {
                using var zipArchive = ZipFile.OpenRead(path);
                string versionNumber = "";
                string versionNumberRegexPattern = @"(?![\\_\.])[\d\.\\_]+(?i)(?=.json)";
                foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries)
                {
                    // valuesets.csv will still be fetched in the zip file for backward compatability
                    // for the users who do not update the app but we do not need it anymore, so don't save it.
                    if (zipArchiveEntry.FullName.StartsWith("valuesets")) continue;

                    zipArchiveEntry.ExtractToFile(Path.Combine(Environment.GetFolderPath(ZIP_FILE_DIRECTORY), zipArchiveEntry.FullName), true);
                    var matches = Regex.Matches(zipArchiveEntry.FullName, versionNumberRegexPattern);
                    if (matches.Count != 0)
                    {
                        List<Match> matchesList = new List<Match>();
                        foreach (Match match in matches)
                        {
                            matchesList.Add(match);
                        }
                        versionNumber = string.Join(".", matchesList.Select(x => x.Value));
                    }
                }
                return versionNumber;
            }
            catch (Exception e)
            {
                _loggingService.LogException(LogSeverity.WARNING, new TextServiceException("Failed to extract text zip file.", e));
                return "";
            }
        }

        private async Task<string> SaveZipFile(Stream zipStream)
        {
            try
            {
                byte[] bytes;
                using var memoryStream = new MemoryStream();
                await zipStream.CopyToAsync(memoryStream);
                bytes = memoryStream.ToArray();
                var path = Path.Combine(Environment.GetFolderPath(ZIP_FILE_DIRECTORY), ZIP_FILE_NAME);
                File.WriteAllBytes(path, bytes);
                return path;
            }
            catch (Exception e)
            {
                _loggingService.LogException(LogSeverity.WARNING, new TextServiceException("Failed to save text zip file.", e));
                return "";
            }
        }

        private async Task<Stream> FetchZipFileFromServer(string currentVersion)
        {
            ApiResponse<Stream> response = await _textRepository.GetTexts(currentVersion);

            if (response.StatusCode == 200 || response.StatusCode == 204)
            {
                _preferencesService.SetUserPreference(PreferencesKeys.LAST_TIME_FETCHED_TEXTS, _dateTimeService.Now.Ticks);
            }

            if (response.StatusCode == 204)
            {
                return Stream.Null;
            }

            await _navigationTaskManager.HandlerErrors(response, true);
            return response.Data;
        }
    }
}
