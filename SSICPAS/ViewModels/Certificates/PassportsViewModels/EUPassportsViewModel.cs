using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

#nullable enable

namespace SSICPAS.ViewModels.Certificates
{
    public class EUPassportsViewModel : PassportsViewModel
    {
        public EUPassportsViewModel()
        {
            EuTestPassports = new List<SinglePassportViewModel>();
            EuVaccinePassports = new List<SinglePassportViewModel>();
            EuRecoveryPassports = new List<SinglePassportViewModel>();
        }

        // It is optional because in old versions we may not get it from server
        public SinglePassportViewModel? DkInfo { get; set; }
        public List<SinglePassportViewModel> EuTestPassports { get; set; }
        public List<SinglePassportViewModel> EuVaccinePassports { get; set; }
        public List<SinglePassportViewModel> EuRecoveryPassports { get; set; }
        public string CustodyKey { get; set; } = "parent";

        [JsonIgnore]
        public override bool AreAllPassportsValid =>
            (DkInfo?.IsValid ?? false) &&
            (EuVaccinePassports?.TrueForAll(singlePassport => singlePassport.IsValid) ?? false) &&
            (EuTestPassports?.TrueForAll(singlePassport => singlePassport.IsValid) ?? false) &&
            (EuRecoveryPassports?.TrueForAll(singlePassport => singlePassport.IsValid) ?? false);

        [JsonIgnore]
        public override bool IsAnyPassportValid =>
            (DkInfo?.IsValid ?? false) ||
            (EuVaccinePassports?.Any(singlePassport => singlePassport.IsValid) ?? false) ||
            (EuTestPassports?.Any(singlePassport => singlePassport.IsValid) ?? false) ||
            (EuRecoveryPassports?.Any(singlePassport => singlePassport.IsValid) ?? false);

        [JsonIgnore]
        public override bool ShouldFetchNewPassport =>
            (DkInfo?.ShouldPrefetchNewPassport ?? false) ||
            (EuVaccinePassports?.Any(singlePassport => singlePassport.ShouldPrefetchNewPassport) ?? false) ||
            (EuTestPassports?.Any(singlePassport => singlePassport.ShouldPrefetchNewPassport) ?? false) ||
            (EuRecoveryPassports?.Any(singlePassport => singlePassport.ShouldPrefetchNewPassport) ?? false);

        [JsonIgnore]
        public override bool IsAnyPassportAvailable =>
            IsAnyVaccinePassportAvailable || IsAnyTestPassportAvailable || IsAnyRecoveryPassportAvailable;

        [JsonIgnore] public bool IsAnyVaccinePassportAvailable => EuVaccinePassports.Any();

        [JsonIgnore] public bool IsAnyTestPassportAvailable => EuTestPassports.Any();

        [JsonIgnore] public bool IsAnyRecoveryPassportAvailable => EuRecoveryPassports.Any();

        [JsonIgnore]
        public bool IsMoreThanOneEuPassportAvailable =>
            EuVaccinePassports.Count +
            EuTestPassports.Count +
            EuRecoveryPassports.Count > 1;

    }
}