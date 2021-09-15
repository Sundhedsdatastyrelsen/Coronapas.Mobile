using System;
using Newtonsoft.Json;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model.Converter;

namespace SSICPAS.Core.Services.Model.EuDCCModel._1._0._x
{
    public class DCCPayload: ITokenPayload
    {
        [JsonProperty("1")]
        public String Issuer { get; set; }

        [JsonProperty("6")]
        [JsonConverter(typeof(EpochDatetimeConverter))]
        public DateTime? IssueAt { get; set; }

        [JsonProperty("4")]
        [JsonConverter(typeof(EpochDatetimeConverter))]
        public DateTime? ExpirationTime { get; set; }

        [JsonProperty("-260")]
        public HCertModel DCCPayloadData { get; set; }

        public DateTime? ExpiredDateTime() => ExpirationTime;
    }
    
    public class HCertModel
    {
        [JsonProperty("1")]
        public DCCSchemaV1 DCC;

    }
}