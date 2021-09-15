using System;
using Newtonsoft.Json;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model.Converter;

namespace SSICPAS.Core.Services.Model.EuDCCModel
{
    public class DefaultCWTPayload: ITokenPayload
    {
        [JsonProperty("1")]
        public String iss { get; set; }

        [JsonProperty("6")]
        [JsonConverter(typeof(EpochDatetimeConverter))]
        public DateTime? iat { get; set; }

        [JsonProperty("4")]
        [JsonConverter(typeof(EpochDatetimeConverter))]
        public DateTime? exp { get; set; }
        
        [JsonProperty("-260")]
        public HCertModel DCCPayloadData { get; set; }
        public DateTime? ExpiredDateTime() => exp;
    }
    
    public class HCertModel
    {
        [JsonProperty("1")]
        public DefaultDccSchema euHcertV1Schema;
    }

    public class DefaultDccSchema
    {
        [JsonProperty("ver")]
        public string Version { get; set; }
    }
}