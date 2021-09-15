using SSICPAS.ViewModels.Onboarding;
using System;
using Xamarin.Forms;

namespace SSICPAS.Views.Onboarding
{
    public class OnboardingPageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OnboardingInfoTemplate { get; set; }
        public DataTemplate OnboardingInfo2Template { get; set; }
        public DataTemplate OnboardingInfo3Template { get; set; }
        public DataTemplate OnboardingInfo4Template { get; set; }
        public DataTemplate OnboardingInfo5Template { get; set; }


        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var type = item.GetType();
            if (type == typeof(OnboardingInfoViewModel)) return OnboardingInfoTemplate;
            if (type == typeof(OnboardingInfo2ViewModel)) return OnboardingInfo2Template;
            if (type == typeof(OnboardingInfo3ViewModel)) return OnboardingInfo3Template;
            if (type == typeof(OnboardingInfo4ViewModel)) return OnboardingInfo4Template;
            if (type == typeof(OnboardingInfo5ViewModel)) return OnboardingInfo5Template;
            throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
}
