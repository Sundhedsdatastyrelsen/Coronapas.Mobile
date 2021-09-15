using Newtonsoft.Json;
using SSICPAS.Core.Services.Interface;
using SSICPAS.Core.Services.Model.EuDCCModel.Shared;
using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;
using SSICPAS.Services.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SSICPAS.Core.Services.Model.Converter
{
    public class DigitalCovidValueSetTestAndTestManufacturerNameTranslator: IDCCValueSetTranslator
    {
        private DevicesObject _valueSetModel = new DevicesObject();
        private IRatListService _ratListService;

        public DigitalCovidValueSetTestAndTestManufacturerNameTranslator(IRatListService ratListService)
        {
            _ratListService = ratListService;
        }
        public async Task InitValueSetAsync()
        {
            var resourceReader = await _ratListService.GetRatList();
            _valueSetModel = JsonConvert.DeserializeObject<DevicesObject>(resourceReader);
        }


        public object Translate(DCCValueSetEnum key, string code)
        {
            TestDevice result = new TestDevice();

            if (key != DCCValueSetEnum.TestManufacturer) return result;
            Test testDevice = _valueSetModel.DeviceList
                .FirstOrDefault(x =>
                    x.Id == code);
            if (testDevice != null)
            {
                result.TestName = testDevice.TestCommercialName;
                result.ManufacturerName = testDevice.Manufacturer.ManufacturerName;
            }

            return result;
        }

        public string GetDCCCode(DCCValueSetEnum key, object value)
        {
            if (value is TestDevice device)
            {
                return _valueSetModel.DeviceList.FirstOrDefault(x => x.TestCommercialName == device.TestName)?.Id ?? device.ManufacturerName;
            }

            return string.Empty;
        }

        public class DevicesObject
        {
            [JsonProperty("deviceList")] 
            public List<Test> DeviceList { get; set; } = new List<Test>();
        }

        public class Test
        {
            [JsonProperty("id_device")]
            public string Id { get; set; }
            [JsonProperty("commercial_name")]
            public string TestCommercialName { get; set; }
            [JsonProperty("manufacturer")]
            public Manufacturer Manufacturer { get; set; }
        }
        
        public class Manufacturer
        {
            [JsonProperty("name")]
            public string ManufacturerName { get; set; }
            
            [JsonProperty("id_manufacturer")]
            public string ManufacturerId { get; set; }
        }
    }
}