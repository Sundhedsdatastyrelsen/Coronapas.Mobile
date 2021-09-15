using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Logging;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.WebServices;
using SSICPAS.Data;
using SSICPAS.Models.Exceptions;
using SSICPAS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SSICPAS.Services.WebServices
{
    public class RatListService : IRatListService
    {
        private static string _loadedValueSetJson = string.Empty;
        private static string _loadedRATJson = string.Empty;

        private readonly IRatListRepository _ratListRepository;
        private readonly IPreferencesService _preferencesService;
        private readonly INavigationTaskManager _navigationTaskManager;
        private readonly IDateTimeService _dateTimeService;
        private readonly ILoggingService _loggingService;

        private const string ZIP_FILE_NAME = "ratlist.zip";
        private const Environment.SpecialFolder ZIP_FILE_DIRECTORY = Environment.SpecialFolder.Personal;

        public RatListService(IRatListRepository ratListRepository,
            IPreferencesService preferencesService,
            INavigationTaskManager navigationTaskManager,
            IDateTimeService dateTimeService,
            ILoggingService loggingService)
        {
            _ratListRepository = ratListRepository;
            _preferencesService = preferencesService;
            _navigationTaskManager = navigationTaskManager;
            _dateTimeService = dateTimeService;
            _loggingService = loggingService;
        }

        public async Task LoadSavedFiles()
        {
            await LoadEmbeddedCopies();
            await SetLatestRATAndValueSet();
        }

        public async Task LoadEmbeddedCopies()
        {
            try
            {
                var assembly = typeof(RatListService).GetTypeInfo().Assembly;

                Stream ratlistStream = assembly.GetManifestResourceStream("SSICPAS.Valuesets.ratlist.json"); 
                Stream valuesetsStream = assembly.GetManifestResourceStream("SSICPAS.Valuesets.valueset.json");

                _loadedValueSetJson = string.Empty;
                _loadedRATJson = string.Empty;

                using (var reader = new StreamReader(valuesetsStream))
                {
                    _loadedValueSetJson = await reader.ReadToEndAsync();
                }

                using (var reader = new StreamReader(ratlistStream))
                {
                    _loadedRATJson = await reader.ReadToEndAsync();
                }

                File.WriteAllText(Path.Combine(Environment.GetFolderPath(ZIP_FILE_DIRECTORY), "valueset.json"), _loadedValueSetJson);
                File.WriteAllText(Path.Combine(Environment.GetFolderPath(ZIP_FILE_DIRECTORY), "ratlist.json"), _loadedRATJson);
            }
            catch (Exception e)
            {
                _loggingService.LogException(LogSeverity.ERROR, new TextServiceException("Failed to load embedded locales", e));
            }
        }

        public async Task LoadRemoteFiles()
        {
            long lastTimeFetchedRatList = _preferencesService.GetUserPreferenceAsLong(PreferencesKeys.LAST_TIME_FETCHED_RATLIST);
            int hoursTimeBetweenFetches = IoCContainer.Resolve<ISettingsService>().RATValueSetsFilesFetchIntervalInHours;
            if ((_dateTimeService.Now - new DateTime(lastTimeFetchedRatList)).TotalHours > hoursTimeBetweenFetches)
            {
                await FetchAndSaveLatestVersionOfZip();
                await SetLatestRATAndValueSet();
            }
        }

        public async Task<string> GetRatList()
        {
            if (!string.IsNullOrEmpty(_loadedRATJson))
            {
                return _loadedRATJson;
            }
            await SetLatestRATAndValueSet();
            return _loadedValueSetJson;
        }

        public async Task<string> GetDCCValueSet()
        {
            if (!string.IsNullOrEmpty(_loadedValueSetJson))
            {
                return _loadedValueSetJson;
            }
            await SetLatestRATAndValueSet();
            return _loadedValueSetJson;
        }

        private bool IsLastFetchedVersionNewerThanEmbeddedVersion()
        {
            string stringVersionNumberOfLastFetchedRatList = _preferencesService.GetUserPreferenceAsString(PreferencesKeys.CURRENT_RATLIST_VERSION);
            string stringVersionNumberOfEmbeddedRatList = IoCContainer.Resolve<ISettingsService>().EmbeddedRATValueSetsFilesVersion;

            if (string.IsNullOrEmpty(stringVersionNumberOfLastFetchedRatList))
            {
                stringVersionNumberOfLastFetchedRatList = "0.0";
            }

            Version versionNumberOfLastFetchedRatList = new Version(stringVersionNumberOfLastFetchedRatList);
            Version versionNumberOfEmbeddedRatList = new Version(stringVersionNumberOfEmbeddedRatList);

            return versionNumberOfLastFetchedRatList.CompareTo(versionNumberOfEmbeddedRatList) > 0;
        }

        private async Task<Stream> FetchZipFileFromServer(string currentVersion)
        {
            ApiResponse<Stream> response = await _ratListRepository.GetRatList(currentVersion);

            if (response.StatusCode == 200 || response.StatusCode == 204)
            {
                _preferencesService.SetUserPreference(PreferencesKeys.LAST_TIME_FETCHED_RATLIST, _dateTimeService.Now.Ticks);
            }

            if (response.StatusCode == 204)
            {
                return Stream.Null;
            }

            await _navigationTaskManager.HandlerErrors(response, true);
            return response.Data;
        }

        private async Task FetchAndSaveLatestVersionOfZip()
        {
            string currentVersion = _preferencesService.GetUserPreferenceAsString(PreferencesKeys.CURRENT_RATLIST_VERSION);
            Stream result = await FetchZipFileFromServer(
                string.IsNullOrEmpty(currentVersion)
                ? IoCContainer.Resolve<ISettingsService>().EmbeddedRATValueSetsFilesVersion
                : currentVersion);
            if (result != null && result.Length != 0)
            {
                string path = await SaveZipFile(result);
                if (!string.IsNullOrEmpty(path))
                {
                    string versionNumber = ExtractZipFile(path);
                    _preferencesService.SetUserPreference(PreferencesKeys.CURRENT_RATLIST_VERSION, versionNumber);
                }
            }
        }

        private async Task SetLatestRATAndValueSet()
        {
            string versionNumberOfLastFetchedTextFile = _preferencesService.GetUserPreferenceAsString(PreferencesKeys.CURRENT_RATLIST_VERSION);

            FileStream ratListLocaleFile;
            FileStream valuesetLocaleFile;

            if (IsLastFetchedVersionNewerThanEmbeddedVersion())
            {
                try
                {
                    ratListLocaleFile = File.OpenRead(Path.Combine(Environment.GetFolderPath(ZIP_FILE_DIRECTORY), $"ratlist_{versionNumberOfLastFetchedTextFile}.json"));
                    valuesetLocaleFile = File.OpenRead(Path.Combine(Environment.GetFolderPath(ZIP_FILE_DIRECTORY), $"valueset_{versionNumberOfLastFetchedTextFile}.json"));

                }
                catch (Exception e)
                {
                    ratListLocaleFile = File.OpenRead(Path.Combine(Environment.GetFolderPath(ZIP_FILE_DIRECTORY), "ratlist.json"));
                    valuesetLocaleFile = File.OpenRead(Path.Combine(Environment.GetFolderPath(ZIP_FILE_DIRECTORY), "valueset.json"));
                    _loggingService.LogException(LogSeverity.WARNING, new TextServiceException("Failed to load fetched ratlist file. Using embedded copy.", e));
                }

                using (var streamReader = new StreamReader(ratListLocaleFile))
                {
                    _loadedRATJson = await streamReader.ReadToEndAsync();
                }

                using (var streamReader = new StreamReader(valuesetLocaleFile))
                {
                    _loadedValueSetJson = await streamReader.ReadToEndAsync();
                }
            }           
        }

        private string ExtractZipFile(string path)
        {
            try
            {
                using var zipArchive = ZipFile.OpenRead(path);
                string versionNumber = string.Empty;
                string versionNumberRegexPattern = @"(?![\\_\.])[\d\.\\_]+(?i)(?=.json)";
                foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries)
                {
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
                return string.Empty;
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
                return string.Empty;
            }
        }
    }
}
