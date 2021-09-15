using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;

namespace SSICPAS.Core.Services.Model.Converter
{
    public static class DigitalCovidValueSetTranslatorFactory
    {
        public static IDCCValueSetTranslator DccValueSetTranslator { get; set; } 
        public static IDCCValueSetTranslator DccValueSetTestDevicesTranslator { get; set; }
    }
}