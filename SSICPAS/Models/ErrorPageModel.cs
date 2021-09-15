using SSICPAS.Enums;

namespace SSICPAS.Models
{
    public class ErrorPageModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Image { get; set; }
        public string ButtonTitle { get; set; }
        public ErrorPageType Type { get; set; } = ErrorPageType.Default;
    }
}
