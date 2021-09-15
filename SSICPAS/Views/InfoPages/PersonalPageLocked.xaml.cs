using System;
using SSICPAS.ViewModels.InfoPages;
using Xamarin.Forms;

namespace SSICPAS.Views.InfoPages
{
    public partial class PersonalPageLocked : ContentView
    {
        public PersonalPageLocked()
        {
            InitializeComponent();
        }

        void Button_Clicked(object sender, EventArgs e)
        {
            (BindingContext as PersonalPageViewModel).SeeInformationButtonClicked();
        }

        void CheckBox_IsCheckedChanged(object sender, TappedEventArgs e)
        {
            (BindingContext as PersonalPageViewModel).CheckBoxChanged(e.Parameter.Equals(true));
        }

        void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            CheckBox.IsChecked = !CheckBox.IsChecked;
        }
    }
}
