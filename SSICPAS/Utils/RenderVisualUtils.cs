using Xamarin.Forms;

namespace SSICPAS.Utils
{
    public static class RenderVisualUtils
    {
        public static bool IsInHierarchy<T>(Element element) where T : VisualElement
        {
            if (element is T)
                return true;
            else if (element is null)
                return false;
            else
                return IsInHierarchy<T>(element?.Parent);
        }
    }
}
