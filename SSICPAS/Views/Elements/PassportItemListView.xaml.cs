using SkiaSharp;
using SSICPAS.ViewModels.Certificates;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PassportItemListView : ContentView
    {
        public PassportItemListView()
        {
            InitializeComponent();

            TopImage.CenterOfGradient = (SKRect rect) =>
            {
                return new SKPoint(rect.Right + rect.Width, rect.Top - rect.Width);
            };
        }
    }
}