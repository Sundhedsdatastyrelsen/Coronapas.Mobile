using SSICPAS.Enums;
using SSICPAS.Services;

namespace SSICPAS.ViewModels.Certificates
{
    public class PassportTypePickerViewModel :SelectionControl
    {
        public PassportTypePickerViewModel(PassportType type)
        {
            Type = type;
        }

        public PassportType Type { get; }

        public override string Text
        {
            get
            {
                if (Type == PassportType.UNIVERSAL_EU)
                {
                    return "PASSPORT_TYPE_INTERNATIONAL_EU_TEXT".Translate();
                }

                return "PASSPORT_TYPE_DANISH_TEXT".Translate();
            }
        }
    }
}
