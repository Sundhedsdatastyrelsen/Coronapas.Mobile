namespace SSICPAS.ViewModels.Certificates
{
    public abstract class PassportsViewModel
    {
        public abstract bool AreAllPassportsValid { get; }
        public abstract bool IsAnyPassportValid { get; }
        public abstract bool ShouldFetchNewPassport { get; }
        public abstract bool IsAnyPassportAvailable { get; }
    }
}
