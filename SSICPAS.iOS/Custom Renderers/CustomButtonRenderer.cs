using SSICPAS.Controls;
using SSICPAS.iOS;
using SSICPAS.Utils;
using SSICPAS.Views.Elements;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Button), typeof(CustomButtonRenderer), new[] { typeof(CustomRenderVisual) })]
namespace SSICPAS.iOS
{
    public class CustomButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            if (Control != null && e.NewElement != null)
            {
                Control.TouchDown += Control_TouchDown;
                Control.TouchUpInside += Control_TouchUpInside;
                Control.TouchDragOutside += Control_TouchDragOutside;
            }
        }

        private void Control_TouchDragOutside(object sender, System.EventArgs e) => (Element as IButtonController)?.SendReleased();

        private void Control_TouchUpInside(object sender, System.EventArgs e)
        {
            IButtonController buttonController = Element as IButtonController;

            if (buttonController is null)
                return;

            buttonController.SendReleased();
            // In case the button is in a CustomDialog hierarchy, we don't trigger the clicked event, as it conflicts with the IDialog framework resulting in an iOS crash
            if (!RenderVisualUtils.IsInHierarchy<CustomDialog>(Element))
                buttonController.SendClicked();
        }

        private void Control_TouchDown(object sender, System.EventArgs args)
        {
            IButtonController buttonController = Element as IButtonController;

            if (buttonController is null)
                return;
            else if(!Control.TouchInside)
                buttonController.SendReleased();
            else if(Control.TouchInside)
                buttonController.SendPressed();
        }
    }
}