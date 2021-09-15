using System;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace SSICPAS.Views.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class PinCodeView : SKCanvasView
    {
        public static readonly BindableProperty StateProperty = BindableProperty.Create(
            "State",
            typeof(PinCodeStateEnum), typeof(PinCodeView),
            PinCodeStateEnum.Inactive,
            BindingMode.OneWay,
            propertyChanged: OnStateChanged
        );
        
        private static void OnStateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var pinCodeView = bindable as PinCodeView;
            pinCodeView.InvalidateSurface();
        }
        
        public PinCodeView()
        {
            PaintSurface += OnCanvasViewPaintSurface;
        }
        
        public PinCodeStateEnum State
        {
            get => (PinCodeStateEnum)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();
            if (State == PinCodeStateEnum.Entered)
            {
                DrawEnteredPinCode(canvas, info);
            }
            else
            {
                DrawUnEnteredPinCode(canvas, info);
            }
        }
        
        private void DrawEnteredPinCode(SKCanvas canvas, SKImageInfo info)
        {
            var paint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                Color = GetStateColor(State),
                IsAntialias = true
            };
            canvas.DrawCircle(info.Width / 2, info.Height / 2, 20, paint);
        }
        
        private void DrawUnEnteredPinCode(SKCanvas canvas, SKImageInfo info)
        {
            float borderWidth = 10f;
            float radius = Math.Min(info.Width, info.Height) * 0.5f - borderWidth;
            var borderPaint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                Color = GetStateColor(State),
                StrokeWidth = borderWidth,
                IsAntialias = true

            };
            var innerPaint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.White,
                IsAntialias = true
            };
            canvas.DrawCircle(info.Width / 2, info.Height / 2, radius, borderPaint);
            canvas.DrawCircle(info.Width / 2, info.Height / 2, radius, innerPaint);
        }
        
        private SKColor GetStateColor(PinCodeStateEnum stateEnum)
        {
            switch (stateEnum)
            {
                case PinCodeStateEnum.Entered:
                    return new SKColor(0, 70, 49);
                case PinCodeStateEnum.Active:
                    return new SKColor(0, 70, 49);
                case PinCodeStateEnum.Error:
                    return new SKColor(212, 23, 0);
                case PinCodeStateEnum.Inactive:
                default:
                    return new SKColor(189, 189, 189);
            }
        }

        public enum PinCodeStateEnum
        {
            Inactive = 0,
            Active = 1,
            Entered = 2,
            Error = 3
        }
    }
}