using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    internal sealed class GyroReactiveBackgroundView : GyroReactiveBase
    {
        private static readonly SKPaint _paint;

        static GyroReactiveBackgroundView() => _paint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.DarkOrchid,
            StrokeCap = SKStrokeCap.Round
        };

        public GyroReactiveBackgroundView() => PaintSurface += OnCanvasViewPaintSurface;

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

            canvas.DrawRect(0, 0, info.Width, info.Height, _paint);
        }
    }
}

