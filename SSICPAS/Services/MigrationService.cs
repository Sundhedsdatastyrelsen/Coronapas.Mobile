using System.Diagnostics;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Data;
using SSICPAS.Services.Interfaces;

namespace SSICPAS.Services
{
    public class MigrationService : IMigrationService
    {
        public const int CurrentMigrationVersion = 1; // Manually increment if migration is needed.
        private IPreferencesService _preferencesService;

        public MigrationService(IPreferencesService preferencesService)
        {
            _preferencesService = preferencesService;
        }

        public void Migrate()
        {
            int LastVersionMigratedTo = _preferencesService.GetUserPreferenceAsInt(PreferencesKeys.MIGRATION_COUNT);

            Debug.Print($"{nameof(MigrationService)}.{nameof(Migrate)}: Current expected version of models: {CurrentMigrationVersion}, " +
                $"Previous version of the models: {LastVersionMigratedTo}");

            while (LastVersionMigratedTo < CurrentMigrationVersion)
            {
                Debug.Print($"{nameof(MigrationService)}.{nameof(Migrate)}: " +
                    $"Migrating from {LastVersionMigratedTo} to {LastVersionMigratedTo + 1}");

                DoTheMigrationToVersion(LastVersionMigratedTo + 1);
                _preferencesService.SetUserPreference(PreferencesKeys.MIGRATION_COUNT, ++LastVersionMigratedTo);

                Debug.Print($"{nameof(MigrationService)}.{nameof(Migrate)}: " +
                    $"Migration to version {LastVersionMigratedTo} is finished");
            }
        }

        private void DoTheMigrationToVersion(int versionToMigrateTo)
        {
            switch (versionToMigrateTo)
            {
                case 1:
                    MigrateToVersion1();
                    break;
                default:
                    break;
            }
        }

        //Add migration code to run below. Use migrateToVersionX signature.
        private void MigrateToVersion1()
        {
            IoCContainer.Resolve<IFamilyPassportStorageRepository>().MigrateData();
        }
    }    
}
