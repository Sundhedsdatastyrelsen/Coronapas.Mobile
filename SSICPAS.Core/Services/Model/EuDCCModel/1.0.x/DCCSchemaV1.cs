using System;
using Newtonsoft.Json;

namespace SSICPAS.Core.Services.Model.EuDCCModel._1._0._x
{
    public class DCCSchemaV1
    {
        [JsonProperty("ver")]
        public string Version { get; set; }

        [JsonProperty("nam")]
        public PersonName PersonName { get; set; }
        
        [JsonProperty("dob")]
        public DateTime DateOfBirth { get; set; }

        [JsonProperty("v")]
        public Vaccination[] Vaccinations { get; set; }
        [JsonProperty("t")]
        public TestResult[] Tests { get; set; }
        [JsonProperty("r")]
        public Recovery[] Recovery { get; set; }
    }
}