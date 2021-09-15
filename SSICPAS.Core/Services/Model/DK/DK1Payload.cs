using System;
using Newtonsoft.Json;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model.Converter;

namespace SSICPAS.Core.Services.Model.DK
{
    public class DK1Payload: ITokenPayload
    {
        [JsonProperty("1")]
        public String iss { get; set; }

        [JsonProperty("6")]
        [JsonConverter(typeof(EpochDatetimeConverter))]
        public DateTime? iat { get; set; }

        [JsonProperty("4")]
        [JsonConverter(typeof(EpochDatetimeConverter))]
        public DateTime? exp { get; set; }

        [JsonProperty("7")]
        public string CwtId { get; set; }

        public DateTime? ExpiredDateTime() => exp;
    }
}