using SSICPAS.Enums;

namespace SSICPAS.ViewModels.Certificates
{
    public class AdditionalDataViewModel
    {
        public LanguageSelection LanguageSelection { get; set; }
        public PassportJobStatus JobStatus { get; set; } = PassportJobStatus.Inprogress;
        public string JobId { get; set; }
    }
}