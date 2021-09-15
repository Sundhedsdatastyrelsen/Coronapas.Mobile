using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSICPAS.Services.Interfaces;
using SSICPAS.Enums;
using SSICPAS.ViewModels;
using Xamarin.Forms;

namespace SSICPAS.Tests.TestMocks
{
    public class MockDialogService : IDialogService
    {
        public MockDialogService()
        {
        }

        public Task<bool> ShowAlertAsync(string title, string message, bool isCanceledOnTouchOutside, bool isCancelTheFirstButton, StackOrientation buttonStackOrientation, string okButtonText, string cancelButtonText)
        {
            return Task.FromResult(true);
        }

        public Task<bool> ShowStyleAlertAsync(string title, string message, bool isCanceledOnTouchOutside, bool isCancelTheFirstButton, StackOrientation buttonStackOrientation, string okButtonText, string cancelButtonText, DialogStyle style)
        {
            return Task.FromResult(true);
        }

        public Task<bool> ShowAlertWithoutTouchOutsideAsync(string title, string message, string accept, string cancel)
        {
            return Task.FromResult(true);
        }

        public Task ShowPicker(IEnumerable<SelectionControl> itemSource, Action<ISelection> onItemPickedAction, string title, IEnumerable<SelectionControl> underItemSource, Action<ISelection> onUnderItemPickedAction, Action onEUPassportTypeChangeSpecificAction)
        {
            return Task.FromResult(true);
        }

        public Task<bool> ShowTimerAlertWithoutTouchOutsideAsync(string title, string message, string accept, DialogStyle style, int showForSeconds)
        {
            return Task.FromResult(true);
        }
    }
}
