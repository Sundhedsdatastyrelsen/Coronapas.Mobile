using Xamarin.Forms;

namespace SSICPAS.Tests.NavigationTests
{
    public class TestPage2 : ContentPage
    {
        public TestPage2()
        {
            BindingContext = new TestViewModel();
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello Test Page 2" }
                }
            };
        }
    }
}

