using SSICPAS.Enums;
using SSICPAS.Models;

namespace SSICPAS.Services.Navigation
{
    public static class Errors
    {
        public static ErrorPageModel UnknownError => new ErrorPageModel
        {
            Title = "ERROR_TITLE_UNKNOWN".Translate(),
            Message = "ERROR_SUBTITLE_UNKNOWN".Translate(),
            Image = SSICPASImage.ErrorUnknown.Image(),
            ButtonTitle = "ERROR_SUBTITLE_UNKNOWN_BUTTON".Translate()
        };
        
        public static ErrorPageModel MaintenanceError => new ErrorPageModel
        {
            Title = "ERROR_TITLE_MAINTENANCE".Translate(),
            Message = "ERROR_SUBTITLE_MAINTENANCE".Translate(),
            Image = SSICPASImage.ErrorMaintenance.Image(),
            ButtonTitle = "ERROR_SUBTITLE_MAINTENANCE_BUTTON".Translate()
        };
        
        public static ErrorPageModel InQueueError => new ErrorPageModel
        {
            Title = "ERROR_TITLE_QUEUE".Translate(),
            Message = "ERROR_SUBTITLE_QUEUE".Translate(),
            Image = SSICPASImage.ErrorQueue.Image(),
            ButtonTitle = "ERROR_SUBTITLE_QUEUE_BUTTON".Translate()
        };
        
        public static ErrorPageModel LockError => new ErrorPageModel
        {
            Title = "ERROR_TITLE_NEMID".Translate(),
            Message = "ERROR_SUBTITLE_NEMID".Translate(),
            Image = SSICPASImage.ErrorLock.Image(),
            ButtonTitle = "ERROR_BUTTON_NEMID".Translate(),
            Type = ErrorPageType.NemID
        };
        
        public static ErrorPageModel SessionExpiredError => new ErrorPageModel
        {
            Title = "ERROR_TITLE_SESSION_EXPIRED".Translate(),
            Message = "ERROR_SUBTITLE_SESSION_EXPIRED".Translate(),
            Image = SSICPASImage.ErrorLock.Image(),
            ButtonTitle = "ERROR_BUTTON_SESSION_EXPIRED".Translate(),
            Type = ErrorPageType.NemID
        };
        
        public static ErrorPageModel SessionRenewalFailedError => new ErrorPageModel
        {
            Title = "ERROR_TITLE_SESSION_RENEWAL_FAILED".Translate(),
            Message = "ERROR_SUBTITLE_SESSION_RENEWAL_FAILED".Translate(),
            Image = SSICPASImage.ErrorLock.Image(),
            ButtonTitle = "ERROR_BUTTON_SESSION_RENEWAL_FAILED".Translate(),
            Type = ErrorPageType.NemID
        };
        
        public static ErrorPageModel BadInternetError => new ErrorPageModel
        {
            Title = "ERROR_TITLE_BAD_INTERNET_CONNECTION".Translate(),
            Message = "ERROR_SUBTITLE_BAD_INTERNET_CONNECTION".Translate(),
            Image = SSICPASImage.ErrorUnknown.Image(),
            ButtonTitle = "ERROR_BUTTON_BAD_INTERNET_CONNECTION".Translate()
        };
        
        public static ErrorPageModel NoInternetError => new ErrorPageModel
        {
            Title = "ERROR_TITLE_NO_INTERNET_CONNECTION".Translate(),
            Message = "ERROR_SUBTITLE_NO_INTERNET_CONNECTION".Translate(),
            Image = SSICPASImage.ErrorUnknown.Image(),
            ButtonTitle = "ERROR_BUTTON_NO_INTERNET_CONNECTION".Translate()
        };
        
        public static ErrorPageModel ForceUpdateRequiredError => new ErrorPageModel
        {
            Title = "ERROR_TITLE_FORCE_UPDATE_REQUIRED".Translate(),
            Message = "ERROR_SUBTITLE_FORCE_UPDATE_REQUIRED".Translate(),
            Image = SSICPASImage.ErrorMaintenance.Image(),
            ButtonTitle = "ERROR_BUTTON_FORCE_UPDATE_REQUIRED".Translate(),
            Type = ErrorPageType.ForceUpdate
        };
    }
}
