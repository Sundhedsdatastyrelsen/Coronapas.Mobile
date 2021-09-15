using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using SSICPAS.Core.Services.Enum;
using SSICPAS.Core.Services.Model;
using SSICPAS.Core.Services.Model.DK;
using SSICPAS.Services;
using SSICPAS.Utils;
using SSICPAS.ViewModels.Base;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.QrScannerViewModels
{
    public class ImagerResultViewModel : BaseViewModel
    {
        private const double TimerInterval = 1000;
        private static readonly string _closesInText = "POPUP_CLOSES_IN".Translate();
        private static readonly string _secondsText = "POPUP_CLOSES_IN_2".Translate();
        private double _msRemaining;

        public Timer Timer = new Timer();
        
        public bool IsResultValid => ResultEnum == TokenValidateResult.Valid;
        public bool IsResultInValid => ResultEnum == TokenValidateResult.Invalid;
        public bool IsResultExpired => ResultEnum == TokenValidateResult.Expired;
        public TokenValidateResult ResultEnum => TokenValidateResultModel.ValidationResult;
        public TokenValidateResultModel TokenValidateResultModel { get; set; } = new TokenValidateResultModel();
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public string RepeatedText => string.Concat(Enumerable.Repeat(BannerText.PadLeft(15), 10));
        public string SecondsRemainingText => $"{_closesInText} {Math.Truncate(_msRemaining / 1000)} {_secondsText}";
        public bool IsInfoAvailable { get; set; }
        public bool IsVisibleSeparator => IsInfoAvailable && IsResultValid;

        public string ResultTitle
        {
            get
            {
                switch (ResultEnum)
                {
                    case(TokenValidateResult.Valid):
                        return "VALID_RESULT".Translate();
                    case(TokenValidateResult.Invalid):
                        return "SCANNER_ERROR_INVALID_TITLE".Translate();
                    case TokenValidateResult.Expired:
                        return "SCANNER_ERROR_EXPIRED_TITLE".Translate();
                    default:
                        return string.Empty;
                }
            }
        }

        public string BannerText
        {
            get
            {
                switch (ResultEnum)
                {
                    case (TokenValidateResult.Valid):
                        return "VALID_RESULT_BANNER_TEXT".Translate();
                    case (TokenValidateResult.Invalid):
                        return  "SCANNER_ERROR_INVALID_BANNER".Translate();
                    case TokenValidateResult.Expired:
                        return "SCANNER_ERROR_EXPIRED_BANNER".Translate();
                    default:
                        return string.Empty;
                }
            }
        }
        
        public Color BannerColor {
            get
            {
                switch (ResultEnum)
                {
                    case (TokenValidateResult.Valid):
                        return SSICPASColor.SuccessBorderColor.Color();
                    case (TokenValidateResult.Invalid):
                        return SSICPASColor.InvalidBorderColor.Color();
                    case TokenValidateResult.Expired:
                        return SSICPASColor.ExpiredBorderColor.Color();
                    default:
                        return Color.Transparent;
                }
            }
        }

        public void OnAttachTimer()
        {
            _msRemaining = IsResultValid ?  _settingsService.ScannerSuccessShownDurationMs: _settingsService.ScannerInvalidShownDurationMs;
            Timer.Interval = TimerInterval;
            Timer.Elapsed += TimerOnElapsed;
            Timer.Enabled = true;
        }

        public void OnDetachTimer()
        {
            Timer.Elapsed -= TimerOnElapsed;
            Timer.Enabled = false;
        }
        
        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (_msRemaining <= 0 && Timer.Enabled)
            {
                OnDetachTimer();
                CloseResultPage();
            }
            else
            {
                _msRemaining -= Timer.Interval;
                OnPropertyChanged(nameof(SecondsRemainingText));
            }
        }

        private void CloseResultPage()
        {
            Device.BeginInvokeOnMainThread(async () => await _navigationService.PopPage());
        }

        public override Task InitializeAsync(object navigationData)
        {

            if (!(navigationData is TokenValidateResultModel tokenValidateResultModel))
                return base.InitializeAsync(navigationData);

            TokenValidateResultModel = tokenValidateResultModel;

            switch (tokenValidateResultModel.DecodedModel)
            {
                case DK1Payload dk1:
                    IsInfoAvailable = false;
                    break;
                case DK2Payload dk2:
                    IsInfoAvailable = true;
                    DateOfBirth = dk2.DateOfBirth.ToLocaleDateFormat();
                    Name = dk2.LegalName;
                    break;
                case Core.Services.Model.EuDCCModel._1._0._x.DCCPayload cwt:
                    IsInfoAvailable = true;
                    DateOfBirth = cwt.DCCPayloadData.DCC.DateOfBirth.ToLocaleDateFormat();
                    Name = cwt.DCCPayloadData.DCC.PersonName.FullNameTransliteratedReversedWithComma;
                    break;
                case Core.Services.Model.EuDCCModel._1._3._0.DCCPayload cwt2:
                    IsInfoAvailable = true;
                    DateOfBirth = cwt2.DCCPayloadData.DCC.DateOfBirth;
                    Name = cwt2.DCCPayloadData.DCC.PersonName.FullNameTransliteratedReversedWithComma;
                    break;
            }

            OnPropertyChanged(nameof(IsInfoAvailable));
            OnPropertyChanged(nameof(TokenValidateResultModel));
            OnPropertyChanged(nameof(ResultEnum));
            OnPropertyChanged(nameof(IsResultValid));
            OnPropertyChanged(nameof(IsResultInValid));
            OnPropertyChanged(nameof(IsResultExpired));
            OnPropertyChanged(nameof(ResultTitle));
            OnPropertyChanged(nameof(BannerText));
            OnPropertyChanged(nameof(RepeatedText));
            OnPropertyChanged(nameof(DateOfBirth));
            OnPropertyChanged(nameof(Name));

            return base.InitializeAsync(navigationData);
        }
    }
}