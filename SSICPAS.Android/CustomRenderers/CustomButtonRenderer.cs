using Android.Content;
using Android.Views;
using SSICPAS.Controls;
using SSICPAS.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Button), typeof(CustomButtonRenderer), new[] { typeof(CustomRenderVisual) })]
namespace SSICPAS.Droid
{
    public class CustomButtonRenderer : ButtonRenderer
    {
        private bool TouchInsideControl(int x, int y) => (x <= Control.Right) && (x >= Control.Left) && (y <= Control.Bottom) && (y >= Control.Top);

        public CustomButtonRenderer(Context context) : base(context) => AutoPackage = false;
        
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            Control?.SetPadding(0, 0, 0, 0);
            if (Control != null && e.NewElement != null)
                Control.Touch += OnTouch;
        }

        private void OnTouch(object sender, TouchEventArgs args)
        {
            IButtonController buttonController = Element as IButtonController;

            if (buttonController is null)
                return;
            else if (!TouchInsideControl((int)args.Event.GetX(), (int)args.Event.GetY()))
                buttonController.SendReleased();
            else if (args.Event.Action == MotionEventActions.Down)
                buttonController.SendPressed();
            else if (args.Event.Action == MotionEventActions.Up)
            {
                buttonController.SendReleased();
                buttonController.SendClicked();
            }
        }
    }
}