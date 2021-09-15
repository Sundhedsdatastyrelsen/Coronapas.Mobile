using SkiaSharp;
using SkiaSharp.Views.Forms;
using SSICPAS.Configuration;
using SSICPAS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;

namespace SSICPAS.Views.Elements
{
    public abstract class GyroReactiveBase : SKCanvasView
    {
        private const float OptimalAngleForRation = 65; 
        private float _angle { get; set; }
        private float _heightWidthRatio { get; set; }

        protected static IGyroscopeService GyroscopeService { get; }
        public virtual Func<SKRect, SKPoint> CenterOfGradient { get; set; }

        private float _animationRate;
        public virtual float AnimationRate
        {
            get => _animationRate;
            set
            {
                _animationRate = value;
                InvalidateSurface();
            }
        }

        public virtual SKColor[] OriginalColorGradient
        {
            get => _originalColorGradient;
            set
            {
                _originalColorGradient = value;
                UpdateCurrentExtendedColorGradient();
                InvalidateSurface();
            }
        }

        public virtual int Density
        {
            get => _density;
            set
            {
                _density = value;
                UpdateCurrentExtendedColorGradient();
                InvalidateSurface();
            }
        }

        public virtual float Angle
        {
            get => _angle;
            set
            {
                _angle = value;
                _heightWidthRatio = (float)Math.Tan((Math.PI / 180) * _angle);
            }
        }

        protected float AnimationAngle { get; set; }

        private float _previousAngle;
        private SKColor[] _currentExtendedColorGradient;
        private SKColor[] _originalColorGradient;
        private int _density;

        static GyroReactiveBase() => GyroscopeService = IoCContainer.Resolve<IGyroscopeService>();

        protected GyroReactiveBase() {
            OriginalColorGradient = new SKColor[]
            {
                new SKColor(192, 168, 218),
                new SKColor(223, 180, 191),
                new SKColor(218, 190, 179),
                new SKColor(227, 210, 169),
                new SKColor(210, 222, 173),
                new SKColor(190, 227, 180),
                new SKColor(131, 216, 216),
            };
            Angle = OptimalAngleForRation;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            GyroscopeService.SubscribeGyroscopeReadingUpdatedEvent(OrientationSensor_ReadingChanged);
            base.OnSizeAllocated(width, height);
        }

        protected SKPoint GetCenterOfGradient(SKImageInfo info)
        {
            float calculatedWidth = info.Width * 3;
            float calculatedHeight = _heightWidthRatio * calculatedWidth;

            return new SKPoint(info.Rect.Right + calculatedWidth, info.Rect.Bottom - calculatedHeight);
        }

        protected abstract void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args);

        private void UpdateCurrentExtendedColorGradient() => _currentExtendedColorGradient = CreateExtendedColorGradient();

        protected SKColor[] GetCurrentExtendedColorGradient() => _currentExtendedColorGradient;

        private SKColor[] CreateExtendedColorGradient()
        {
            var mainGradient = OriginalColorGradient.ToList();
            var reverseGradient = OriginalColorGradient.ToList();

            //remove top and bottom color since it will be dublicate when doing addrange
            reverseGradient.RemoveAt(0);
            reverseGradient.RemoveAt(reverseGradient.Count - 1);
            reverseGradient.Reverse();

            mainGradient.AddRange(reverseGradient);

            var extendedGradient = new List<SKColor>();

            for (int densityCount = 0; densityCount < Density; densityCount++)
            {
                extendedGradient.AddRange(mainGradient);
            }

            // return to the first color in the list for a smooth transition
            extendedGradient.Add(mainGradient.First());

            return extendedGradient.ToArray();
        }

        private void OrientationSensor_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            var data = e.Reading.AngularVelocity;
            _previousAngle = AnimationAngle;
            var currentAngle = AnimationRate * (data.X + data.Y + data.Z);
            AnimationAngle = _previousAngle + currentAngle;
            InvalidateSurface();
        }
    }
}
