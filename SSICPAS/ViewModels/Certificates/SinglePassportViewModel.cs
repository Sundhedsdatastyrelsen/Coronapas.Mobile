using System;
using Newtonsoft.Json;
using SSICPAS.Configuration;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Models;

namespace SSICPAS.ViewModels.Certificates
{
    public class SinglePassportViewModel
    {
        private const int PassportPrefetchIntervalInMinute = 30;
        
        [JsonIgnore]
        public string FullName => string.IsNullOrEmpty(PassportData.FullName)
            ? $"{PassportData.FirstName ?? ""} {PassportData.LastName ?? ""}" 
            : PassportData.FullName;

        [JsonIgnore]
        public string BirthDate => PassportData.DateOfBirth;

        [JsonIgnore]
        public string QRToken => PassportData.SecureToken;
        public PassportData PassportData { get; set; }

        [JsonIgnore]
        public bool IsValid => PassportData.CertificateValidTo > IoCContainer.Resolve<IDateTimeService>().Now;

        [JsonIgnore]
        public bool ShouldPrefetchNewPassport => IsValid && PassportData.CertificateValidTo <
            IoCContainer.Resolve<IDateTimeService>().Now.Add(TimeSpan.FromMinutes(PassportPrefetchIntervalInMinute));

        public SinglePassportViewModel()
        {
            PassportData = new PassportData();
        }
    }
}
