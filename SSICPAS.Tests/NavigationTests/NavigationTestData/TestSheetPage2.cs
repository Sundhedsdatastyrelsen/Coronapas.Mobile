using SSICPAS.Views;
using Xamarin.Forms;

namespace SSICPAS.Tests.NavigationTests
{
    public class TestSheetPage2 : ContentSheetPageNoBackButtonOnIOS
    {
        public TestSheetPage2()
        {
            BindingContext = new TestSheetPageVM();
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello Sheet page 2" }
                }
            };
        }
    }
}

