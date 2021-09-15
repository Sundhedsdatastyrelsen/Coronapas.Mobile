using System.Collections.ObjectModel;
using System.Globalization;
using SSICPAS.Services;

namespace SSICPAS.ViewModels.Certificates
{
    public class PassportItemsGroupViewModel : ObservableCollection<PassportItemCellViewModel>
    {
        private bool _displayItemCount;
        public string Title { get; set; }
        public string TitleLabelText { get => $"{Title}" + (_displayItemCount ? $" ({Count})" : ""); }

        public string TitleLabelTextAccessibility { get => (_displayItemCount ? $"{Count}" : "") + $" {Title}" + $"{AddEndingToTitleLabelTextAccessibility(Title)}"; } 

        public PassportItemsGroupViewModel(string title, bool displayItemCount = true)
        {
            Title = title;
            _displayItemCount = displayItemCount;
        }
        private string AddEndingToTitleLabelTextAccessibility(string name)
        {
            if (Count < 2)
            {
                return "";
            }

            CultureInfo culture = new CultureInfo("LANG_DATEUTIL".Translate());

            if (culture.Name == "da-DK")
            {
                if (name.ToLower().EndsWith("e") || name.ToLower().EndsWith("t"))
                    return "r";
                return "s";
            }
                                        
            if (name.ToLower().EndsWith("e") || name.ToLower().EndsWith("t"))
                return   "s";
            return  "s";  
        }
    }
}
