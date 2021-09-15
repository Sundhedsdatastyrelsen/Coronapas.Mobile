using System;
using SSICPAS.Core.Services.Interface;

namespace SSICPAS.Core.Services
{
    public class DateTimeService: IDateTimeService
    {
        public DateTime Now => DateTime.UtcNow;
    }
}