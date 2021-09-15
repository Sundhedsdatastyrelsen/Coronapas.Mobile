using Xamarin.Forms;

namespace SSICPAS.Tests.NavigationTests
{
    public class TestPage1 : ContentPage
    {
        public TestPage1()
        {
            BindingContext = new TestViewModel();
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello Test Page 1" }
                }
            };
        }
    }
}

