using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SSICPAS.Core.Data;
using SSICPAS.Models;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.Tests.TestMocks;
using SSICPAS.Tests.ViewModelTests;
using SSICPAS.ViewModels.Menu;
using Xamarin.Forms;

namespace SSICPAS.Tests.ViewModels
{
    public class SettingsPageViewModelTests : BaseVMTests
    {
        private readonly Mock<IDialogService> dialogService = new Mock<IDialogService>();
        private readonly ITextService textService = new Mock<ITextService>().Object;
        private SettingsPageViewModel viewModel;

        [SetUp]
        public void Setup()
        {
            viewModel = new SettingsPageViewModel(
                new Mock<IUserService>().Object,
                new Mock<ISecureStorageService<PinCodeBiometricsModel>>().Object,
                dialogService.Object,
                textService,
                new MockPreferencesService());
            LocaleService.Current.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            dialogService.Reset();
        }

        [Test]
        public void DialogNotDisplayed_Language_Not_Changed()
        {
            dialogService.Verify(d => d.ShowAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<StackOrientation>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void DialogNotDisplayed_Setting_Same_Language()
        {
            textService.SetLocale("dk");
            viewModel.DanishSelected = true;
            dialogService.Verify(d => d.ShowAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<StackOrientation>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void DialogDisplayed_Change_Language()
        {
            textService.SetLocale("dk");
            viewModel.DanishSelected = false;
            dialogService.Verify(d => d.ShowAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<StackOrientation>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void DeclineDialog_DanishSelected_Negated()
        {
            textService.SetLocale("dk");
            bool initialValue = viewModel.DanishSelected = true;
            dialogService.Setup(d => d.ShowAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<StackOrientation>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(false));
            viewModel.DanishSelected = !initialValue;
            Assert.AreEqual(initialValue, viewModel.DanishSelected);
        }
    }
}
