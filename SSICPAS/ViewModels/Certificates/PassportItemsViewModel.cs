using Newtonsoft.Json;
using SSICPAS.Enums;

namespace SSICPAS.ViewModels.Certificates
{
    public class PassportItemsViewModel
    {
        public LanguageSelection LanguageSelection { get; set; }

        public DKPassportsViewModel DKPassportsViewModel { get; set; }
        public EUPassportsViewModel MyPageViewModel { get; set; }
        public EUPassportsViewModel EUPassportsViewModel { get; set; }
        public PassportJobStatus JobStatus { get; set; } = PassportJobStatus.Inprogress;
        public string JobId { get; set; }
        
        [JsonIgnore]
        public PassportType SelectedPassportType { get; set; } = PassportType.DK_LIMITED;
    }
}
