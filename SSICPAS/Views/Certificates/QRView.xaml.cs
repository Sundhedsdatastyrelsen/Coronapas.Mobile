using System.Runtime.CompilerServices;
using System.Windows.Input;
using Lottie.Forms;
using SkiaSharp;
using SSICPAS.Configuration;
using SSICPAS.Core.Interfaces;
using SSICPAS.Views.Elements;
using Xamarin.Forms;

namespace SSICPAS.Views.Certificates
{
    public partial class QRView : ContentView
    {
        public QRView()
        {
            InitializeComponent();
            BottomImage.CenterOfGradient = (SKRect rect) =>
            {
                return new SKPoint(rect.Right + rect.Width, rect.Top - rect.Width);
            };
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            Device.BeginInvokeOnMainThread(() =>
            {
                if (propertyName == BadgeIconAnimationSourceProperty.PropertyName)
                {
                    BadgeIconAnimation.Animation = BadgeIconAnimationSource;
                }
                if (propertyName == IsBadgeIconAnimationPlayingProperty.PropertyName)
                {
                    BadgeIconAnimation.IsPlaying = IsBadgeIconAnimationPlaying;
                }
                if (propertyName == BadgeIconAnimationProgressProperty.PropertyName)
                {
                    BadgeIconAnimation.Progress = BadgeIconAnimationProgress;
                }
                if (propertyName == BadgeTextProperty.PropertyName)
                {
                    BadgeLabel.Text = BadgeText;
                }
                if (propertyName == BadgeTextColorProperty.PropertyName)
                {
                    BadgeLabel.TextColor = BadgeTextColor;
                }
                if (propertyName == BadgeInfoIconSourceProperty.PropertyName)
                {
                    BadgeInfoButton.Source = BadgeInfoIconSource;
                }
                if (propertyName == BadgeInfoShownProperty.PropertyName)
                {
                    BadgeInfoButton.IsVisible = BadgeInfoShown;
                }
                if (propertyName == ColorsProperty.PropertyName)
                {
                    BottomImage.Colors = Colors;
                }
                if (propertyName == BarcodeValueProperty.PropertyName)
                {
                    QRImage.Content = BarcodeValue;
                }
                if (propertyName == QrCodeInfoAccessibilityTextProperty.PropertyName)
                {
                    AutomationProperties.SetHelpText(BadgeInfoButton, QrCodeInfoAccessibilityText);
                }
                if (propertyName == BadgeInfoButtonCommandProperty.PropertyName)
                {
                    BadgeInfoButton.Command = BadgeInfoButtonCommand;
                }
            });
        }

        public string BadgeIconAnimationSource
        {
            get => (string)GetValue(BadgeIconAnimationSourceProperty);
            set => SetValue(BadgeIconAnimationSourceProperty, value);
        }

        public float BadgeIconAnimationProgress
        {
            get => (float)GetValue(BadgeIconAnimationProgressProperty);
            set => SetValue(BadgeIconAnimationProgressProperty, value);
        }

        public bool IsBadgeIconAnimationPlaying
        {
            get => (bool)GetValue(IsBadgeIconAnimationPlayingProperty);
            set => SetValue(IsBadgeIconAnimationPlayingProperty, value);
        }

        public ImageSource BadgeInvalidIconSource
        {
            get => (ImageSource)GetValue(BadgeInvalidIconSourceProperty);
            set => SetValue(BadgeInvalidIconSourceProperty, value);
        }

        public string BadgeText
        {
            get => (string)GetValue(BadgeTextProperty);
            set => SetValue(BadgeTextProperty, value);
        }

        public string QrCodeInfoAccessibilityText
        {
            get => (string)GetValue(QrCodeInfoAccessibilityTextProperty);
            set => SetValue(QrCodeInfoAccessibilityTextProperty, value);
        }

        public string BarcodeValue
        {
            get => (string)GetValue(BarcodeValueProperty);
            set => SetValue(BarcodeValueProperty, value);
        }

        public Color BadgeTextColor
        {
            get => (Color)GetValue(BadgeTextColorProperty);
            set => SetValue(BadgeTextColorProperty, value);
        }

        public bool BadgeInfoShown
        {
            get => (bool)GetValue(BadgeInfoShownProperty);
            set => SetValue(BadgeInfoShownProperty, value);
        }

        public ImageSource BadgeInfoIconSource
        {
            get => (ImageSource)GetValue(BadgeInfoIconSourceProperty);
            set => SetValue(BadgeInfoIconSourceProperty, value);
        }

        public SKColor[] Colors
        {
            get => (SKColor[])GetValue(ColorsProperty);
            set => SetValue(ColorsProperty, value);
        }

        public Command BadgeInfoButtonCommand
        {
            get => (Command)GetValue(BadgeInfoButtonCommandProperty);
            set => SetValue(BadgeInfoButtonCommandProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly BindableProperty ColorsProperty = BindableProperty.Create(
            nameof(Colors),
            typeof(SKColor[]),
            typeof(GyroReactiveView)
        );

        public static readonly BindableProperty BadgeIconAnimationSourceProperty = BindableProperty.Create(
            nameof(BadgeIconAnimationSource),
            typeof(string),
            typeof(AnimationView),
            string.Empty,
            BindingMode.OneWay
        );

        public static readonly BindableProperty BadgeIconAnimationProgressProperty = BindableProperty.Create(
            nameof(BadgeIconAnimationProgress),
            typeof(float),
            typeof(QRView),
            default(float)
        );

        public static readonly BindableProperty IsBadgeIconAnimationPlayingProperty = BindableProperty.Create(
            nameof(IsBadgeIconAnimationPlaying),
            typeof(bool),
            typeof(QRView),
            default(bool)
        );

        public static readonly BindableProperty BadgeInvalidIconSourceProperty = BindableProperty.Create(
            nameof(BadgeInvalidIconSource),
            typeof(ImageSource),
            typeof(Image),
            default(ImageSource),
            BindingMode.OneWay
        );

        public static readonly BindableProperty BarcodeValueProperty = BindableProperty.Create(
            nameof(BarcodeValue),
            typeof(string),
            typeof(QRView),
            string.Empty,
            BindingMode.OneWay
        );
        public static readonly BindableProperty BadgeTextProperty = BindableProperty.Create(
            nameof(BadgeText),
            typeof(string),
            typeof(Label),
            string.Empty,
            BindingMode.OneWay
        );
        public static readonly BindableProperty QrCodeInfoAccessibilityTextProperty = BindableProperty.Create(
            nameof(QrCodeInfoAccessibilityText),
            typeof(string),
            typeof(ImageButton),
            string.Empty,
            BindingMode.OneWay
        );

        public static readonly BindableProperty BadgeInfoButtonCommandProperty = BindableProperty.Create(
            nameof(BadgeInfoButtonCommand),
            typeof(Command),
            typeof(ImageButton),
            null,
            BindingMode.OneWay);

        public static readonly BindableProperty BadgeTextColorProperty = BindableProperty.Create(
            nameof(BadgeTextColor),
            typeof(Color),
            typeof(QRView),
            Color.White
        );
        public static readonly BindableProperty BadgeInfoShownProperty = BindableProperty.Create(
            nameof(BadgeInfoShown),
            typeof(bool),
            typeof(QRView),
            true,
            BindingMode.OneWay
        );
        public static readonly BindableProperty BadgeInfoIconSourceProperty = BindableProperty.Create(
            nameof(BadgeInfoIconSource),
            typeof(ImageSource),
            typeof(Image),
            default(ImageSource),
            BindingMode.OneWay);

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            nameof(Command),
            typeof(ICommand),
            typeof(QRView),
            null);
    }
}
