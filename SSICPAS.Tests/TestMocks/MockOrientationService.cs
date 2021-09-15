using System;
using SSICPAS.Enums;
using SSICPAS.Services.Interfaces;

namespace SSICPAS.Tests.TestMocks
{
    public class MockOrientationService : IOrientationService
    {
        public Action<SupportedOrientation> OnChangeSupportedOrientation { get; set; }

        public void SetSupportedOrientation(SupportedOrientation orientation)
        {
        }
    }
}
