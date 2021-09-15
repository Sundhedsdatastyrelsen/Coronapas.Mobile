using System;
using SSICPAS.Data;
using SSICPAS.Enums;
using Xamarin.Forms;
using SSICPAS.Configuration;
using SSICPAS.Core.Services.Interface;

namespace SSICPAS.ViewModels.Custom
{
    public class CustomTimerDialogViewModel : CustomDialogViewModel
    {
        public CustomTimerDialogViewModel(string title, string body, string okButtonText = null, DialogStyle style = DialogStyle.Info, int showForSeconds = 0, bool dismiss = true)
            : base(title, body, false, StackOrientation.Horizontal, true, okButtonText, null, style)
        {
            _dateTimeService = IoCContainer.Resolve<IDateTimeService>();
            _activeDialog = this;

            _showForSeconds = showForSeconds;
            SecondsRemaining = _showForSeconds;
            ButtonLabel = okButtonText;
            _dismissDialog = dismiss;

            if(SecondsRemaining > 0)
            {
                OkButtonText = TimeRemainingString;
                OkButtonEnabled = false;
                StartTimer();
            }
            else
            {
                OkButtonEnabled = true;
            }

            MessagingCenter.Subscribe<object>(this, MessagingCenterKeys.BACK_FROM_BACKGROUND, OnReturnFromBackground);
        }

        public static CustomTimerDialogViewModel UpdateActiveDialog(string title, string body, string okButtonText = null, DialogStyle style = DialogStyle.Info, int showForSeconds = 0, bool dismiss = true)
        {
            if(_activeDialog == null)
            {
                return new CustomTimerDialogViewModel(title, body, okButtonText, style, showForSeconds, dismiss);
            }
            else
            {
                _activeDialog.Title = title;
                _activeDialog.RaisePropertyChanged(() => _activeDialog.Title);
                _activeDialog.Body = body;
                _activeDialog.RaisePropertyChanged(() => _activeDialog.Body);
                _activeDialog._showForSeconds = showForSeconds;
                _activeDialog.SecondsRemaining = showForSeconds;
                _activeDialog.ButtonLabel = okButtonText;
                _activeDialog._dismissDialog = dismiss;

                if(_activeDialog.SecondsRemaining > 0)
                {
                    _activeDialog.UpdateButtonText();
                    if(!_activeDialog._hasActiveTimer)
                    {
                        _activeDialog.StartTimer();
                    }
                    else
                    {
                        _activeDialog._timerStartedDateTime = _activeDialog._dateTimeService.Now;
                    }
                }
                else
                {
                    _activeDialog.OkButtonEnabled = true;
                }
            }
            return _activeDialog;
        }

        private readonly IDateTimeService _dateTimeService;
        private static CustomTimerDialogViewModel _activeDialog;
        private int _showForSeconds;
        private bool _okButtonEnabled;
        private bool _dismissDialog;
        private DateTime _timerStartedDateTime;
        private bool _hasActiveTimer = false;

        public bool OkButtonEnabled
        {
            get { return _okButtonEnabled; }
            protected set
            {
                _okButtonEnabled = value;
                RaisePropertyChanged(() => OkButtonEnabled);
            }
        }

        public int SecondsRemaining { get; protected set; }
        public string ButtonLabel { get; protected set; }
        public string TimeRemainingString
        {
            get
            {
                int seconds = SecondsRemaining % 60;
                int minutes = SecondsRemaining / 60;
                return $"{minutes}:{seconds.ToString("00")}";
            }
        }

        protected void StartTimer()
        {
            _timerStartedDateTime = _dateTimeService.Now;
            _hasActiveTimer = true;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                return UpdateButtonText();
            }
            );
        }

        protected bool UpdateButtonText()
        {
            SecondsRemaining--;
            bool restartTimer;

            if(SecondsRemaining > 0)
            {
                OkButtonText = TimeRemainingString;
                restartTimer = true;
            }
            else
            {
                OkButtonText = ButtonLabel;
                restartTimer = false;
            }

            OkButtonEnabled = !restartTimer;
            RaisePropertyChanged(() => OkButtonText);

            if(!restartTimer && _dismissDialog)
            {
                Dismiss();
            }

            return restartTimer;
        }

        public void OnReturnFromBackground(object sender)
        {
            DateTime nowUtc = _dateTimeService.Now;
            int secondsElapsed = (int)(nowUtc - _timerStartedDateTime).TotalSeconds;

            SecondsRemaining = secondsElapsed > _showForSeconds ? 0 : _showForSeconds - secondsElapsed;
        }

        public void Dismiss()
        {
            _activeDialog = null;
            MessagingCenter.Send<object>(this, MessagingCenterKeys.SCREENSHOT_TAKEN_TIMER_ELAPSED);
            Complete();
        }

        public static bool DialogIsActive()
        {
            return _activeDialog != null;
        }
    }
}
