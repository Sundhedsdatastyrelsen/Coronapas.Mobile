using System;
using SSICPAS.ViewModels.Base;
using static SSICPAS.Views.Elements.PinCodeView;

namespace SSICPAS.ViewModels.Custom
{
    public class CustomPincodeBulletsViewModel: BaseViewModel
    {
        public Action BeginShake;

        public PinCodeStateEnum FirstCode { get; set; }
        public PinCodeStateEnum SecondCode { get; set; }
        public PinCodeStateEnum ThirdCode { get; set; }
        public PinCodeStateEnum FourthCode { get; set; }

        public void UpdateBullets(PinCodeViewStatusEnum state)
        {
            switch (state)
            {
                case PinCodeViewStatusEnum.NoPin:
                    FirstCode = PinCodeStateEnum.Active;
                    SecondCode = PinCodeStateEnum.Inactive;
                    ThirdCode = PinCodeStateEnum.Inactive;
                    FourthCode = PinCodeStateEnum.Inactive;
                    break;
                case PinCodeViewStatusEnum.FirstPin:
                    FirstCode = PinCodeStateEnum.Entered;
                    SecondCode = PinCodeStateEnum.Active;
                    ThirdCode = PinCodeStateEnum.Inactive;
                    FourthCode = PinCodeStateEnum.Inactive;
                    break;
                case PinCodeViewStatusEnum.SecondPin:
                    FirstCode = PinCodeStateEnum.Entered;
                    SecondCode = PinCodeStateEnum.Entered;
                    ThirdCode = PinCodeStateEnum.Active;
                    FourthCode = PinCodeStateEnum.Inactive;
                    break;
                case PinCodeViewStatusEnum.ThirdPin:
                    FirstCode = PinCodeStateEnum.Entered;
                    SecondCode = PinCodeStateEnum.Entered;
                    ThirdCode = PinCodeStateEnum.Entered;
                    FourthCode = PinCodeStateEnum.Active;
                    break;
                case PinCodeViewStatusEnum.FourthPin:
                    FirstCode = PinCodeStateEnum.Entered;
                    SecondCode = PinCodeStateEnum.Entered;
                    ThirdCode = PinCodeStateEnum.Entered;
                    FourthCode = PinCodeStateEnum.Entered;
                    break;
                case PinCodeViewStatusEnum.Error:
                    BeginShake?.Invoke();
                    FirstCode = PinCodeStateEnum.Error;
                    SecondCode = PinCodeStateEnum.Error;
                    ThirdCode = PinCodeStateEnum.Error;
                    FourthCode = PinCodeStateEnum.Error;
                    break;
            }

            RaisePropertyChanged(() => FirstCode);
            RaisePropertyChanged(() => SecondCode);
            RaisePropertyChanged(() => ThirdCode);
            RaisePropertyChanged(() => FourthCode);
        }

        public enum PinCodeViewStatusEnum
        {
            NoPin,
            FirstPin,
            SecondPin,
            ThirdPin,
            FourthPin,
            Error
        }
    }
}
