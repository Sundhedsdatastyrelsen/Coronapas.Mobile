using System;
using System.Runtime.CompilerServices;
using SkiaSharp;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GyroReactiveTextPageView : ContentView
    {
        public GyroReactiveTextPageView() => InitializeComponent();

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == TextProperty.PropertyName)
            {
                AnimationBackground.Text = Text;
            }
            else if (propertyName == ColorsProperty.PropertyName)
            {
                AnimationBackground.OriginalColorGradient = Colors;
            }
            else if (propertyName == DensityProperty.PropertyName)
            {
                AnimationBackground.Density = Density;
            }
            else if (propertyName == AnimationRateProperty.PropertyName)
            {
                AnimationBackground.AnimationRate = AnimationRate;
            }
        }

        public SKColor[] Colors
        {
            get => (SKColor[])GetValue(ColorsProperty);
            set => SetValue(ColorsProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public int Density
        {
            get => (int)GetValue(DensityProperty);
            set => SetValue(DensityProperty, value);
        }

        public float AnimationRate
        {
            get => (float)GetValue(AnimationRateProperty);
            set => SetValue(AnimationRateProperty, value);
        }

        public Func<SKRect, SKPoint> CenterOfGradient
        {
            get => AnimationBackground.CenterOfGradient;
            set => AnimationBackground.CenterOfGradient = value;
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(GyroReactiveTextPageView),
            string.Empty
        );

        public static readonly BindableProperty ColorsProperty = BindableProperty.Create(
            nameof(Colors),
            typeof(SKColor[]),
            typeof(GyroReactiveTextPageView)
        );

        public static readonly BindableProperty DensityProperty = BindableProperty.Create(
            nameof(Density),
            typeof(int),
            typeof(GyroReactiveTextPageView),
            1
        );

        public static readonly BindableProperty AnimationRateProperty = BindableProperty.Create(
            nameof(AnimationRate),
            typeof(float),
            typeof(GyroReactiveTextPageView),
            default(float)
        );
    }
}