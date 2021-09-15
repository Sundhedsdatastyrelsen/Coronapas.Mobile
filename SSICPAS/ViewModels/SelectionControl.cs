using SSICPAS.Enums;
using SSICPAS.Services;

namespace SSICPAS.ViewModels
{
    public class SelectionControl : ISelection
    {
        public int NumberInFamilyList { get; set; }
        public virtual bool IsSelected { get; set; }
        public virtual string Text { get; set; }
        public virtual string AccessibilityText => IsSelected
            ? Text + "SELECTION_CONTROL_SELECTED".Translate()
            : Text;
        public PassportType SelectedPassportType { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
