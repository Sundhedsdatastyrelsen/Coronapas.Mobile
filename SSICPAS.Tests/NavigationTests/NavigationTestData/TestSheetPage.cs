using SSICPAS.Views;
using Xamarin.Forms;

namespace SSICPAS.Tests.NavigationTests
{
    public class TestSheetPage : ContentSheetPageNoBackButtonOnIOS
    {
        public TestSheetPage()
        {
            BindingContext = new TestSheetPageVM();
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello Sheet page" }
                }
            };
        }
    }
}

