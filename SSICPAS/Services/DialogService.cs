using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AiForms.Dialogs;
using SSICPAS.Enums;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels;
using SSICPAS.ViewModels.Custom;
using SSICPAS.Views.Elements;
using Xamarin.Forms;

namespace SSICPAS.Services
{
    public class DialogService : IDialogService
    {
        public async Task<bool> ShowAlertAsync(string title, string message, bool isCanceledOnTouchOutside, bool isCancelTheFirstButton,
            StackOrientation buttonStackOrientation, string okButtonText, string cancelButtonText)
        {
            if (string.IsNullOrEmpty(okButtonText))
            {
                okButtonText = "DIALOG_ACCEPT_BUTTON".Translate();
            }

            CustomDialogViewModel viewmodel = new CustomDialogViewModel(title, message, isCanceledOnTouchOutside, buttonStackOrientation, isCancelTheFirstButton, okButtonText, cancelButtonText);
            return await Dialog.Instance.ShowAsync<CustomDialog>(viewmodel);
        }

        public async Task<bool> ShowStyleAlertAsync(string title, string message, bool isCanceledOnTouchOutside, bool isCancelTheFirstButton,
            StackOrientation buttonStackOrientation, string okButtonText, string cancelButtonText, DialogStyle style)
        {
            if (string.IsNullOrEmpty(okButtonText))
            {
                okButtonText = "DIALOG_ACCEPT_BUTTON".Translate();
            }

            CustomDialogViewModel viewmodel = new CustomDialogViewModel(title, message, isCanceledOnTouchOutside, buttonStackOrientation, isCancelTheFirstButton, okButtonText, cancelButtonText, style);
            return await Dialog.Instance.ShowAsync<CustomDialog>(viewmodel);
        }

        public async Task ShowPicker(
            IEnumerable<SelectionControl> itemSource,
            Action<ISelection> onItemPickedAction,
            string title,
            IEnumerable<SelectionControl> underItemSource,
            Action<ISelection> onUnderItemPickedAction,
            Action onEUPassportTypeChangeSpecificAction)
        {
            await CustomPickerBuilder.Builder()
                .WithItemSource(itemSource)
                .WithOnItemPickedAction(onItemPickedAction)
                .WithTitle(title)
                .WithUnderItemSource(underItemSource)
                .WithOnUnderItemPickedAction(onUnderItemPickedAction)
                .WithEUPassportTypeChangeSpecificAction(onEUPassportTypeChangeSpecificAction)
                .Show();
        }

        public async Task<bool> ShowTimerAlertWithoutTouchOutsideAsync(string title, string message, string accept, DialogStyle style, int showForSeconds)
        {
            if(string.IsNullOrEmpty(accept))
            {
                accept = "DIALOG_ACCEPT_BUTTON".Translate();
            }

            if(CustomTimerDialogViewModel.DialogIsActive())
            {
                CustomTimerDialogViewModel.UpdateActiveDialog(title, message, accept, style, showForSeconds);
                return true;
            }
            else
            {
                CustomTimerDialogViewModel viewModel = new CustomTimerDialogViewModel(title, message, accept, style, showForSeconds);
                return await Dialog.Instance.ShowAsync<CustomTimerDialog>(viewModel);
            }
        }
    }
}