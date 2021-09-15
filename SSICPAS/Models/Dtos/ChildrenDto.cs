using System.Collections.Generic;
using Newtonsoft.Json;

namespace SSICPAS.Models.Dtos
{
    public class ChildrenDto: IPassportDto
    {
        [JsonProperty("dk-info")]
        public string DkInfo { get; set; }

        [JsonProperty("custody-key")]
        public string CustodyKey { get; set; }
        
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
    }
}