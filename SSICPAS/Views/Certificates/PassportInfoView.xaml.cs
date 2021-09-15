using System.Runtime.CompilerServices;
using SSICPAS.Enums;
using SSICPAS.ViewModels.Certificates;
using Xamarin.Forms;

namespace SSICPAS.Views.Certificates
{
    public partial class PassportInfoView : ContentView
    {
        public PassportInfoView()
        {
            InitializeComponent();
            Content.BindingContext = PassportInfoViewModel.CreatePassportInfoViewModel();
        }

        public static readonly BindableProperty PassportItemsViewModelProperty =
            BindableProperty.Create(nameof(FamilyPassportItemsViewModel), typeof(FamilyPassportItemsViewModel), typeof(PassportInfoView), null,
                BindingMode.OneWay);

        public static readonly BindableProperty EuPassportTypeProperty =
            BindableProperty.Create(nameof(EuPassportType), typeof(EuPassportType), typeof(PassportInfoView), null,
                BindingMode.OneWay);

        public static readonly BindableProperty SelectedPassportProperty =
            BindableProperty.Create(nameof(SelectedPassport), typeof(SinglePassportViewModel), typeof(PassportInfoView), null,
                BindingMode.OneWay);

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == EuPassportTypeProperty.PropertyName)
            {
                (Content.BindingContext as PassportInfoViewModel).EuPassportType = EuPassportType;
                (Content.BindingContext as PassportInfoViewModel).UpdateView();
            }
            if (propertyName == PassportItemsViewModelProperty.PropertyName)
            {
                if (PassportItemsViewModel != null)
                {
                    (Content.BindingContext as PassportInfoViewModel).PassportItemsViewModel = PassportItemsViewModel;
                    (Content.BindingContext as PassportInfoViewModel).UpdateView();
                }
                
            }
            if (propertyName == SelectedPassportProperty.PropertyName)
            {
                (Content.BindingContext as PassportInfoViewModel).SelectedPassport = SelectedPassport;
                (Content.BindingContext as PassportInfoViewModel).UpdateView();
            }
            
        }

        public FamilyPassportItemsViewModel PassportItemsViewModel
        {
            get { return (FamilyPassportItemsViewModel)GetValue(PassportItemsViewModelProperty); }
            set { SetValue(PassportItemsViewModelProperty, value); }
        }

        public EuPassportType EuPassportType
        {
            get { return (EuPassportType)GetValue(EuPassportTypeProperty); }
            set { SetValue(EuPassportTypeProperty, value); }
        }

        public SinglePassportViewModel SelectedPassport
        {
            get { return (SinglePassportViewModel)GetValue(SelectedPassportProperty); }
            set { SetValue(SelectedPassportProperty, value); }
        }


    }
}
