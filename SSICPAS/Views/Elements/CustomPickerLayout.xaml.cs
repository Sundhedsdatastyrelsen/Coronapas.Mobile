using System;
using SSICPAS.Enums;
using SSICPAS.ViewModels;
using Xamarin.Forms;

namespace SSICPAS.Views.Elements
{
    public partial class CustomPickerLayout : ContentView
    {
        ISelection _item;
        public CustomPickerLayout(ISelection item, bool IsDKSelected, bool shouldMarkItemSelected = false)
        {
            _item = item;
            InitializeComponent();
            if (IsDKSelected)
            {
                Label.Text = String.Empty;
                AutomationProperties.SetIsInAccessibleTree(Label,false);
                AutomationProperties.SetIsInAccessibleTree(AccessibilityButton, false);
                return;
            }
            if (_item.IsSelected && !shouldMarkItemSelected)
            {
                SelectedIcon.Source = SSICPASImage.PassportSelected.Image();
            }
            else
            {
                SelectedIcon.Source = SSICPASImage.PassportNotSelected.Image();
            }
            Label.Text = _item.Text;
            Label.FontFamily = _item.IsSelected ? "IBMPlexSansSemiBold" : "IBMPlexSansRegular";
            AccessibilityButton.Command = new Command(() =>
            {
                OnItemSelected?.Invoke(_item);
            });
            AutomationProperties.SetName(AccessibilityButton, _item.AccessibilityText);
        }
        public Action<ISelection> OnItemSelected;
    }
}
