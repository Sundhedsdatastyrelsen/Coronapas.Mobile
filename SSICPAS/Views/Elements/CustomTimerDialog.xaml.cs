using AiForms.Dialogs.Abstractions;
using SSICPAS.ViewModels.Custom;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

namespace SSICPAS.Views.Elements
{
    public partial class CustomTimerDialog : DialogView
    {
        public CustomTimerDialog()
        {
            InitializeComponent();
            IsCanceledOnTouchOutside = false;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext is CustomTimerDialogViewModel viewModel)
            {
                IsCanceledOnTouchOutside = viewModel.IsCanceledOnTouchOutside;
            }
        }

        void Dismiss(System.Object sender, System.EventArgs e)
        {
            ((CustomTimerDialogViewModel)BindingContext).Dismiss();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == OkButtonTextProperty.PropertyName)
            {
                OkButtonLabel.Text = OkButtonText;
            }
            else if(propertyName == OkButtonStyleProperty.PropertyName)
            {
                OkButton.Style = OkButtonStyle;
            }
            else if(propertyName == OkButtonEnabledProperty.PropertyName)
            {
                OkButton.IsEnabled = OkButtonEnabled;
            }
            else if(propertyName == HeaderProperty.PropertyName)
            {
                HeaderLabel.Text = Title;
            }
            else if(propertyName == BodyProperty.PropertyName)
            {
                BodyLabel.Text = Body;
            }
        }

        public static readonly BindableProperty OkButtonTextProperty =
            BindableProperty.Create(nameof(OkButtonText), typeof(string), typeof(Label), null,
                BindingMode.OneWay);

        public string OkButtonText
        {
            get { return (string)GetValue(OkButtonTextProperty); }
            set { SetValue(OkButtonTextProperty, value); }
        }

        public static readonly BindableProperty OkButtonStyleProperty =
            BindableProperty.Create(nameof(OkButtonStyle), typeof(Style), typeof(Label), null, BindingMode.OneWay);

        public Style OkButtonStyle
        {
            get { return (Style)GetValue(OkButtonStyleProperty); }
            set { SetValue(OkButtonStyleProperty, value); }
        }

        public static readonly BindableProperty OkButtonEnabledProperty =
            BindableProperty.Create(nameof(OkButtonEnabled), typeof(bool), typeof(Label), true, BindingMode.OneWay);

        public bool OkButtonEnabled
        {
            get { return (bool)GetValue(OkButtonEnabledProperty); }
            set { SetValue(OkButtonEnabledProperty, value); }
        }

        public static readonly BindableProperty HeaderProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(Label), null, BindingMode.OneWay);

        public string Title
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly BindableProperty BodyProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(Label), null, BindingMode.OneWay);

        public string Body
        {
            get { return (string)GetValue(BodyProperty); }
            set { SetValue(BodyProperty, value); }
        }

    }
}