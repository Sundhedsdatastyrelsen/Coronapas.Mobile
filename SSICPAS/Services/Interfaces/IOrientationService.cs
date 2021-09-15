using System;
using SSICPAS.Enums;

namespace SSICPAS.Services.Interfaces
{
    public interface IOrientationService
    {
        Action<SupportedOrientation> OnChangeSupportedOrientation { get; set; }
        void SetSupportedOrientation(SupportedOrientation orientation);
    }
}