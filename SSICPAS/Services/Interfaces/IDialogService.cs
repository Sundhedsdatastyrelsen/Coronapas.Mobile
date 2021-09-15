using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSICPAS.Enums;
using SSICPAS.ViewModels;
using Xamarin.Forms;

namespace SSICPAS.Services.Interfaces
{
    public interface IDialogService
    {
        Task<bool> ShowAlertAsync(string title, string message, bool isCanceledOnTouchOutside, bool isCancelTheFirstButton,
            StackOrientation buttonStackOrientation, string okButtonText, string cancelButtonText);
        Task<bool> ShowStyleAlertAsync(string title, string message, bool isCanceledOnTouchOutside, bool isCancelTheFirstButton,
            StackOrientation buttonStackOrientation, string okButtonText, string cancelButtonText, DialogStyle style);
        Task ShowPicker(IEnumerable<SelectionControl> itemSource, Action<ISelection> onItemPickedAction, string title, IEnumerable<SelectionControl> underItemSource, Action<ISelection> onUnderItemPickedAction, Action onEUPassportTypeChangeSpecificAction);
        Task<bool> ShowTimerAlertWithoutTouchOutsideAsync(string title, string message, string accept, DialogStyle style, int showForSeconds);
    }
}