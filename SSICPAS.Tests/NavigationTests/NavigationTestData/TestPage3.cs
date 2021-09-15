using Xamarin.Forms;

namespace SSICPAS.Tests.NavigationTests
{
    public class TestPage3 : ContentPage
    {
        public TestPage3()
        {
            BindingContext = new TestViewModel();
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello Test Page 3" }
                }
            };
        }
    }
}

