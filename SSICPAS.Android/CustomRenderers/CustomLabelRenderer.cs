using System.ComponentModel;
using Android.Content;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(Label), typeof(A11y.Droid.Renderers.LabelRenderer))]
namespace A11y.Droid.Renderers
{
    public class LabelRenderer : Xamarin.Forms.Platform.Android.LabelRenderer
    {
        public LabelRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            string name = AutomationProperties.GetName(Element);
            string text = Element.Text;
            string helpText = AutomationProperties.GetHelpText(Element);

            if (string.IsNullOrEmpty(name))
            {
                Control.ContentDescription = $"{text}";
            }
            else
            {
                Control.ContentDescription = $"{name}";
            }

            Control.Hint = helpText;
        }
    }
}