using AiForms.Dialogs.Abstractions;
using SSICPAS.Enums;

namespace SSICPAS.Views.Elements
{
    public partial class CustomToast : ToastView
    {
        public CustomToast()
        {
            InitializeComponent();
            image.Source = SSICPASImage.ToastTick.Image();
        }
    }
}
