using Moq;
using NUnit.Framework;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;
using SSICPAS.Services.Interfaces;
using SSICPAS.ViewModels;
using Xamarin.Forms;

namespace SSICPAS.Tests.ViewModelTests
{
    public class LandingViewModelTests : BaseVMTests
    {
        private readonly Mock<IDialogService> dialogService = new Mock<IDialogService>();
        private readonly Mock<ITextService> textService = new Mock<ITextService>();
        private readonly Mock<IScannerFactory> scannerService = new Mock<IScannerFactory>();
        private readonly Mock<IPreferencesService> preferencesService = new Mock<IPreferencesService>();
        private readonly Mock<INotificationService> notificationService = new Mock<INotificationService>();

        private LandingViewModel viewModel;

        [SetUp]
        public void Setup()
        {
            IoCContainer.RegisterInstance<IDialogService>(dialogService.Object);
            IoCContainer.RegisterInstance<ITextService>(textService.Object);
            IoCContainer.RegisterInstance<IScannerFactory>(scannerService.Object);
            IoCContainer.RegisterInstance<IPreferencesService>(preferencesService.Object);
            IoCContainer.RegisterInstance<INotificationService>(notificationService.Object);
            viewModel = new LandingViewModel();
        }

        [TearDown]
        public void Teardown()
        {
            dialogService.Reset();
        }

        [Test]
        public void DialogNotDisplayed_Language_Not_Changed()
        {
            dialogService.Verify(d => d.ShowAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<StackOrientation>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            dialogService.Reset();
        }

        [Test]
        public void DialogDisplayed_Change_Language()
        {
            textService.Object.SetLocale("dk");
            viewModel.ChangeLanguageCommand.Execute(null);
            dialogService.Verify(d => d.ShowAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<StackOrientation>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }
    }
}
