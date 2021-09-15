using System;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using SSICPAS.Utils;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    internal sealed class GyroReactiveTextView : GyroReactiveBase
    {
        private string _text = "";
        public string Text {
            get => _text;
            set
            {
                _text = value;
                InvalidateSurface();
            }
        }

        private static readonly SKPaint _paint;
        private SKRect _textBounds;

        static GyroReactiveTextView() => _paint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.DarkOrchid,
            StrokeCap = SKStrokeCap.Round,
        };

        public GyroReactiveTextView()
        { 
            PaintSurface += OnCanvasViewPaintSurface;
            _textBounds = new SKRect();   
        }

        protected override void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();
            //We are creating a sweep gradient with a center point to the left side
            SKPoint centerOfGradientWheel = GetCenterOfGradient(info);

            var radiant = (float)(AnimationAngle * (Math.PI / 360));
            _paint.Shader = SKShader.CreateSweepGradient(
                centerOfGradientWheel,
                GetCurrentExtendedColorGradient(),
                null,
                SKMatrix.CreateRotation(radiant, centerOfGradientWheel.X, centerOfGradientWheel.Y));
            _paint.IsAntialias = true;
            _paint.Style = SKPaintStyle.StrokeAndFill;
            _paint.StrokeWidth = 3;

            // Adjust TextSize property so text is 80% of screen width or 95% text height base on which will fit the frame
            float heightBaseTextSize = 0.95f * info.Height;
            float textWidth = _paint.MeasureText(_text);
            float widthBaseTextSize = 0.8f * info.Width * _paint.TextSize / textWidth;
            _paint.TextSize = Math.Min( heightBaseTextSize, widthBaseTextSize);

            // Set font to monospace
            _paint.Typeface = FontUtils.GetMonospaceSKTypeface();

            // Find the text bounds
            _paint.MeasureText(Text, ref _textBounds);
            // Calculate offsets to center the text on the screen
            float xText = info.Width / 2 - _textBounds.MidX;
            float yText = info.Height / 2 - _textBounds.MidY;
            // Draw text
            canvas.DrawText(Text, xText, yText, _paint);
        }
    }
}
