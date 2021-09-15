using System;
using NUnit.Framework;
using SSICPAS.Configuration;
using SSICPAS.Core.Interfaces;
using SSICPAS.Tests.TestMocks;
using SSICPAS.ViewModels.Menu;

namespace SSICPAS.Tests.ViewModelTests
{
    public class MenuPageViewModelTest: BaseVMTests
    {
        [TestCase("1.0", "12", "api", "1.0 (12) - api")]
        [TestCase("2.0.2.3", "345", "api-test", "2.0.2.3 (345) - api-test")]
        public void VersionNumber(string version, string build, string envDescription, string expected)
        {
            MockSettingsService service = IoCContainer.Resolve<ISettingsService>() as MockSettingsService;
            service.EnvironmentString = envDescription;
            service.Build = build;
            service.Version = version;

            MenuPageViewModel vm = new MenuPageViewModel();
            Assert.AreEqual(expected, vm.VersionNumber);
        }

    }
}
