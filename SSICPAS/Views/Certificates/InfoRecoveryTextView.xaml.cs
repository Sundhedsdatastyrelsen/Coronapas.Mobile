using System.Runtime.CompilerServices;
using SSICPAS.ViewModels.Certificates;
using Xamarin.Forms;

namespace SSICPAS.Views.Certificates
{
    public partial class InfoRecoveryTextView : ContentView
    {
        public InfoRecoveryTextView()
        {
            InitializeComponent();
            Content.BindingContext = new InfoRecoveryTextViewModel();
        }
        public static readonly BindableProperty PassportViewModelProperty =
            BindableProperty.Create(nameof(PassportViewModel), typeof(SinglePassportViewModel), typeof(InfoVaccineTextView), null,
                BindingMode.OneWay);

        public static readonly BindableProperty ShowCertificateProperty =
            BindableProperty.Create(nameof(ShowCertificateBool), typeof(bool), typeof(InfoVaccineTextView), null,
                BindingMode.OneWay);

        public static readonly BindableProperty ShowHeaderProperty =
            BindableProperty.Create(nameof(ShowHeaderBool), typeof(bool), typeof(InfoVaccineTextView), null,
                BindingMode.OneWay);

        public static readonly BindableProperty ShowTextInEnglishProperty =
            BindableProperty.Create(nameof(ShowHeaderBool), typeof(bool), typeof(InfoVaccineTextView), null,
                BindingMode.OneWay);

        public static readonly BindableProperty OnlyOneEUPassportProperty =
            BindableProperty.Create(nameof(OnlyOneEUPassportBool), typeof(bool), typeof(InfoRecoveryTextView), null,
                BindingMode.OneWay);


        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == PassportViewModelProperty.PropertyName)
            {
                if (PassportViewModel != null)
                {
                    (Content.BindingContext as InfoRecoveryTextViewModel).PassportViewModel = PassportViewModel;
                    (Content.BindingContext as InfoRecoveryTextViewModel).UpdateView();
                }

            }
            if (propertyName == ShowCertificateProperty.PropertyName)
            {
                (Content.BindingContext as InfoRecoveryTextViewModel).ShowCertificate = ShowCertificateBool;
            }
            if (propertyName == ShowHeaderProperty.PropertyName)
            {
                (Content.BindingContext as InfoRecoveryTextViewModel).ShowHeader = ShowHeaderBool;
            }
            if (propertyName == ShowTextInEnglishProperty.PropertyName)
            {
                (Content.BindingContext as InfoRecoveryTextViewModel).ShowTextInEnglish = ShowTextInEnglishBool;
            }
            if (propertyName == OnlyOneEUPassportProperty.PropertyName)
            {
                (Content.BindingContext as InfoRecoveryTextViewModel).OnlyOneEUPassport = OnlyOneEUPassportBool;
            }

        }

        public SinglePassportViewModel PassportViewModel
        {
            get { return (SinglePassportViewModel)GetValue(PassportViewModelProperty); }
            set { SetValue(PassportViewModelProperty, value); }
        }

        public bool ShowCertificateBool
        {
            get { return (bool)GetValue(ShowCertificateProperty); }
            set { SetValue(ShowCertificateProperty, value); }
        }
        public bool ShowHeaderBool
        {
            get { return (bool)GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }
        public bool ShowTextInEnglishBool
        {
            get { return (bool)GetValue(ShowTextInEnglishProperty); }
            set { SetValue(ShowTextInEnglishProperty, value); }
        }
        public bool OnlyOneEUPassportBool
        {
            get { return (bool)GetValue(OnlyOneEUPassportProperty); }
            set { SetValue(OnlyOneEUPassportProperty, value); }
        }
    }
}
