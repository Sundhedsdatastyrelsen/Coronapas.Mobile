using System;
using SSICPAS.Configuration;
using SSICPAS.Core.Interfaces;
using SSICPAS.Core.Services.Model;
using SSICPAS.Data;
using SSICPAS.Enums;
using SSICPAS.Services.Interfaces;
using SSICPAS.Utils;
using SSICPAS.ViewModels.QrScannerViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSICPAS.Views.ScannerPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImagerSuccessResultPage : ContentPage, IScanResultView, IExtraOrientationSupport
    {
        public ImagerSuccessResultPage(TokenValidateResultModel model)
        {
            InitializeComponent();
            ImagerResultViewModel viewModel = new ImagerResultViewModel();
            viewModel.InitializeAsync(model);
            BindingContext = viewModel;

            ProgressBar.ProgressTo(1.0,
                Convert.ToUInt32(IoCContainer.Resolve<ISettingsService>().ScannerSuccessShownDurationMs), Easing.Linear);
        }

        protected override void OnAppearing()
        {
            IoCContainer.Resolve<INavigationService>().SetStatusBar(SSICPASColor.NavigationHeaderBackgroundColor.Color(), Color.Black);
            ((ImagerResultViewModel)BindingContext).OnAttachTimer();

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            ((ImagerResultViewModel)BindingContext).OnDetachTimer();
            base.OnDisappearing();
        }

        public SupportedOrientation SupportedOrientation => SupportedOrientation.SensorPortrait;
    }
}