using System;
using SSICPAS.Enums;
using SSICPAS.Services.Interfaces;

namespace SSICPAS.Services
{
    public class OrientationService: IOrientationService 
    {
        public Action<SupportedOrientation> OnChangeSupportedOrientation { get; set; }

        public void SetSupportedOrientation(SupportedOrientation orientation)
        {
            OnChangeSupportedOrientation?.Invoke(orientation);
        }
    }
}