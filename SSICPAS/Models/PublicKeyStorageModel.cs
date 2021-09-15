using System;
using System.Collections.Generic;
using SSICPAS.Core.Services.Model;

namespace SSICPAS.Models
{
    public class PublicKeyStorageModel
    {
        public List<PublicKeyDto> PublicKeys { get; set; } = new List<PublicKeyDto>();
        public DateTime LastFetchTimestamp { get; set; }
    }
}