using System;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Core.Interfaces;
using SSICPAS.Services.Interfaces;
using SSICPAS.Tests.TestMocks;

namespace SSICPAS.Tests.ViewModelTests
{
    public class BaseVMTests
    {
        public BaseVMTests()
        {
            IoCContainer.RegisterInterface<ISettingsService, MockSettingsService>();
            IoCContainer.RegisterInterface<INavigationService, MockNavigationService>();
            IoCContainer.RegisterInterface<IPreferencesService, MockPreferencesService>();
        }
    }
}
