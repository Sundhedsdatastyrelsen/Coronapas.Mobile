using Xamarin.Forms;

namespace SSICPAS.Utils
{
    public static class AnimationUtils
    {
        public static async void ShakeAnimation(this VisualElement element)
        {
            uint timeout = 50;

            await element.TranslateTo(-15, 0, timeout);

            await element.TranslateTo(15, 0, timeout);

            await element.TranslateTo(-10, 0, timeout);

            await element.TranslateTo(10, 0, timeout);

            await element.TranslateTo(-5, 0, timeout);

            await element.TranslateTo(5, 0, timeout);

            element.TranslationX = 0;
        }
    }
}
