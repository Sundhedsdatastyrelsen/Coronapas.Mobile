using System;
using SSICPAS.Enums;
using SSICPAS.ViewModels;
using Xamarin.Forms;

namespace SSICPAS.Views.Elements
{
    public partial class CustomIconPickerLayout : ContentView
    {
        private readonly SelectionControl _item;
        public CustomIconPickerLayout(ISelection item)
        {
            _item = (SelectionControl)item;
            InitializeComponent();
            if (_item.SelectedPassportType == PassportType.UNIVERSAL_EU)
            {
                SelectedIcon.Source = 
                    _item.IsSelected ?
                        SSICPASImage.PassportEUIconSelected.Image() :
                        SSICPASImage.PassportEUIcon.Image();
            }
            else
            {
                SelectedIcon.Source =
                    _item.IsSelected ?
                        SSICPASImage.PassportDKIconSelected.Image() :
                        SSICPASImage.PassportDKIcon.Image();
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
