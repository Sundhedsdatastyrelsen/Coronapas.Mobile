using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SSICPAS.Enums;

namespace SSICPAS.Models.Dtos
{
    public class GetPassportDto: IPassportDto
    {
        public GetPassportDto()
        {
        }

        [JsonProperty("dk-min")]
        public string Dkmin { get; set; }

        [JsonProperty("dk-max")]
        public string Dkmax { get; set; }

        [JsonProperty("euro-lab")]
        public List<string> EuroTest { get; set; }
        
        [JsonProperty("euro-vaccine")]
        public List<string> EuroVaccine { get; set; }
        
        [JsonProperty("euro-recovery")]
        public List<string> EuroRecovery { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PassportJobStatus Status { get; set; }

        [JsonProperty("dk-info")]
        public string DkInfo { get; set; }
        
        [JsonProperty("custody-key")]
        public string CustodyKey { get; set; }
        
        [JsonProperty("children-passports")]
        public List<ChildrenDto> Children { get; set; }
            
        public string JobId { get; set; }
    }
}
