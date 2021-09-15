﻿using System.Threading.Tasks;
using SSICPAS.ViewModels.Base;

namespace SSICPAS.ViewModels
{
    public class SuccessViewModel : BaseViewModel
    {
        private string _successMessage;
        public string SuccessMessage
        {
            get
            {
                return _successMessage;
            }
            set
            {
                _successMessage = value;
                RaisePropertyChanged(() => SuccessMessage);
            }
        }

        public void SetSuccessMessage(string message)
        {
            SuccessMessage = message;
        }

        public override Task InitializeAsync(object navigationData)
        {
            var message = navigationData as string;
            SetSuccessMessage(message);
            return base.InitializeAsync(navigationData);
        }
    }
}
