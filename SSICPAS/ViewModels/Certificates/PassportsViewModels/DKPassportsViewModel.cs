using Newtonsoft.Json;

namespace SSICPAS.ViewModels.Certificates
{
    public class DKPassportsViewModel: PassportsViewModel
    {
        public SinglePassportViewModel DanishPassportWithInfo { get; set; }
        public SinglePassportViewModel DanishPassportNoInfo { get; set; }

        [JsonIgnore]
        public override bool AreAllPassportsValid =>
            (DanishPassportNoInfo?.IsValid ?? false) && (DanishPassportWithInfo?.IsValid ?? false);

        [JsonIgnore]
        public override bool IsAnyPassportValid =>
            (DanishPassportNoInfo?.IsValid ?? false) || (DanishPassportWithInfo?.IsValid ?? false);

        [JsonIgnore]
        public override bool ShouldFetchNewPassport =>
            (DanishPassportNoInfo?.ShouldPrefetchNewPassport ?? false)
                || (DanishPassportWithInfo?.ShouldPrefetchNewPassport ?? false);

        [JsonIgnore]
        public override bool IsAnyPassportAvailable =>
            // AND (&&) because we are not allowed to have only one DK passport
            DanishPassportNoInfo != null && DanishPassportWithInfo != null;

        [JsonIgnore]
        public bool AreAllAvailablePassportsValid => 
            (DanishPassportNoInfo?.IsValid ?? true) && (DanishPassportWithInfo?.IsValid ?? true);
    }
}
