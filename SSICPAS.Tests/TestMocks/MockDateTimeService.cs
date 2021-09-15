using System;
using SSICPAS.Core.Services.Interface;

namespace SSICPAS.Tests.TestMocks
{
    public class MockDateTimeService : IDateTimeService
    {
        public DateTime Now { get; set; } = DateTime.UtcNow;
    }
}