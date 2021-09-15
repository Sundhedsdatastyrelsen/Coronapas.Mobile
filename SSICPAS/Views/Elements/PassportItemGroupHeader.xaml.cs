using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PassportItemGroupHeader : ViewCell
    {
        public PassportItemGroupHeader()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                TitleLabel.Text = $"{TitleLabelText}";
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == TitleLabelTextProperty.PropertyName)
            {
                TitleLabel.Text = $"{TitleLabelText}";
            }
        }

        public static readonly BindableProperty TitleLabelTextProperty =
            BindableProperty.Create(nameof(TitleLabelText), typeof(string), typeof(PassportItemGroupHeader), "Group Title", BindingMode.OneWay);

        public string TitleLabelText
        {
            get { return (string)GetValue(TitleLabelTextProperty); }
            set { SetValue(TitleLabelTextProperty, value); }
        }
    }
}