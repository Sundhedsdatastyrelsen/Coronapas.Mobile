using SSICPAS.Core.Data;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Data;
using SSICPAS.Models;
using SSICPAS.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SSICPAS.Services
{
    class NotificationService : INotificationService
    {
        private readonly IDialogService _dialogService;
        private readonly IDateTimeService _dateTimeService;
        private readonly ISecureStorageService<UpdateNotificationModel> _updateNotificationService;

        public NotificationService(
            IDialogService dialogService,
            IDateTimeService dateTimeService,
            ISecureStorageService<UpdateNotificationModel> updateNotificationService)
        {
            _dialogService = dialogService;
            _dateTimeService = dateTimeService;
            _updateNotificationService = updateNotificationService;
        }

        public async Task NotificationOnStartUp()
        {
            UpdateNotificationModel pcrReminder =
                await _updateNotificationService.GetSecureStorageAsync(
                    SecureStorageKeys.PCR_TEST_UPDATE_NOTIFICATION_SHOWN_REMINDER);

            if (pcrReminder is { PCRUpdateShown: true })
            {
                return;
            }

            if (new DateTime(2021, 7, 1).Ticks <= _dateTimeService.Now.Ticks &&
                new DateTime(2021, 7, 15).Ticks >= _dateTimeService.Now.Ticks)
            {
                string title = "PCR_TEXT_UPDATE_TITLE".Translate();
                string content = "PCR_TEXT_UPDATE_CONTENT".Translate();
                string accept = "PCR_TEXT_UPDATE_BUTTON_TEXT".Translate();

                await _dialogService.ShowStyleAlertAsync(
                    title, 
                    content,
                    true,
                    true,
                    StackOrientation.Horizontal,
                    accept, 
                    null,
                    Enums.DialogStyle.Notification);

                UpdateNotificationModel updateNotificationModel =
                    new UpdateNotificationModel 
                    { 
                        PCRUpdateShown = true 
                    }; 
                await _updateNotificationService.SetSecureStorageAsync(
                    SecureStorageKeys.PCR_TEST_UPDATE_NOTIFICATION_SHOWN_REMINDER, updateNotificationModel);
            }
        }
    }
}
