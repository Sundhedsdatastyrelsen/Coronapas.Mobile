using SSICPAS.ViewModels.Certificates;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PassportItemCellView : ViewCell
    {
        public PassportItemCellView()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if(BindingContext != null)
            {
                HeaderLabel.Text = HeaderLabelText;
                SubHeaderLabel.Text = SubHeaderLabelText;
                DurationLabelFirstSpan.Text = DurationLabelFirstSpanText;
                DurationLabelSecondSpan.Text = DurationLabelSecondSpanText;
                DoseLabel.Text = DoseLabelText;
                IconImage.Source = IconImageSource;
                TapCommand = TapCommand;
                FormatDurationLabel();
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == HeaderLabelTextProperty.PropertyName)
            {
                HeaderLabel.Text = HeaderLabelText;
            }
            else if (propertyName == SubHeaderLabelTextProperty.PropertyName)
            {
                SubHeaderLabel.Text = SubHeaderLabelText;
            }
            else if (propertyName == DurationLabelFirstSpanTextProperty.PropertyName)
            {
                DurationLabelFirstSpan.Text = DurationLabelFirstSpanText;
                FormatDurationLabel();
            }
            else if (propertyName == DurationLabelSecondSpanTextProperty.PropertyName)
            {
                DurationLabelSecondSpan.Text = DurationLabelSecondSpanText;
                FormatDurationLabel();
            }
            else if (propertyName == DoseLabelTextProperty.PropertyName)
            {
                DoseLabel.Text = DoseLabelText;
            }
            else if (propertyName == IconImageSourceProperty.PropertyName)
            {
                IconImage.Source = IconImageSource;
            }
            else if(propertyName == TapCommandProperty.PropertyName)
            {
                TapCommand = TapCommand;
            }
        }

        public static readonly BindableProperty HeaderLabelTextProperty =
            BindableProperty.Create(nameof(HeaderLabelText), typeof(string), typeof(PassportItemCellView), "Header", BindingMode.OneWay);

        public string HeaderLabelText
        {
            get { return (string)GetValue(HeaderLabelTextProperty); }
            set { SetValue(HeaderLabelTextProperty, value); }
        }

        public static readonly BindableProperty SubHeaderLabelTextProperty =
            BindableProperty.Create(nameof(SubHeaderLabelText), typeof(string), typeof(PassportItemCellView), "SubHeader", BindingMode.OneWay);

        public string SubHeaderLabelText
        {
            get { return (string)GetValue(SubHeaderLabelTextProperty); }
            set { SetValue(SubHeaderLabelTextProperty, value); }
        }

        public static readonly BindableProperty DurationLabelFirstSpanTextProperty =
            BindableProperty.Create(nameof(DurationLabelFirstSpanText), typeof(string), typeof(PassportItemCellView), "Duration", BindingMode.OneWay);

        public string DurationLabelFirstSpanText
        {
            get { return (string)GetValue(DurationLabelFirstSpanTextProperty); }
            set { SetValue(DurationLabelFirstSpanTextProperty, value); }
        }

        public static BindableProperty DurationLabelSecondSpanTextProperty =
            BindableProperty.Create(nameof(DurationLabelSecondSpanText), typeof(string), typeof(PassportItemCellView), string.Empty, BindingMode.OneWay);


        public string DurationLabelSecondSpanText
        {
            get { return (string)GetValue(DurationLabelSecondSpanTextProperty); }
            set { SetValue(DurationLabelSecondSpanTextProperty, value); }
        }

        public static readonly BindableProperty DoseLabelTextProperty =
            BindableProperty.Create(nameof(DoseLabelText), typeof(string), typeof(PassportItemCellView), "X of Y", BindingMode.OneWay);

        public string DoseLabelText
        {
            get { return (string)GetValue(DoseLabelTextProperty); }
            set { SetValue(DoseLabelTextProperty, value); }
        }

        public static readonly BindableProperty IconImageSourceProperty =
            BindableProperty.Create(nameof(IconImageSource), typeof(string), typeof(PassportItemCellView), null, BindingMode.OneWay);

        public string IconImageSource
        {
            get { return (string)GetValue(IconImageSourceProperty); }
            set { SetValue(IconImageSourceProperty, value); }
        }

        public static readonly BindableProperty TapCommandProperty =
            BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(PassportItemCellView), null, BindingMode.OneWay);

        public ICommand TapCommand
        {
            get { return (ICommand)GetValue(TapCommandProperty); }
            set { SetValue(TapCommandProperty, value); }
        }

        private void FormatDurationLabel()
        {
            if (BindingContext == null) return;
            if ( ((PassportItemCellViewModel)BindingContext).Type == PassportItemCellViewModel.PassportItemType.Recovery)
            {
                DurationLabelFirstSpan.Style = (Style)Application.Current.Resources["SecondaryContentStyle"];
                DurationLabelSecondSpan.Style = (Style)Application.Current.Resources["SecondaryContentBoldStyle"];
            }
            else
            {
                DurationLabelFirstSpan.Style = (Style)Application.Current.Resources["SecondaryContentBoldStyle"];
                DurationLabelSecondSpan.Style = (Style)Application.Current.Resources["SecondaryContentStyle"];
            }
        }
            
    }
}