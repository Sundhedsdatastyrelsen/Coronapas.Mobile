using SSICPAS.ViewModels;

namespace SSICPAS.Services.Scanner
{
    public class ScannerModel: ISelection
    {
        public string Name { get; set; }
        public bool ConnectionState { get; set; }
        public string Id { get; set; }
        
        public string Text => Name;
        public bool IsSelected { get; set; }
        public bool IsMainCamera { get; set; }
        public string AccessibilityText => Name;

        public ScannerModel(string id, string name, bool connectionState )
        {
            Name = name;
            Id = id;
            ConnectionState = connectionState;
        }
    }
}