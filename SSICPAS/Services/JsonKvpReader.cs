using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SSICPAS.Services
{
    public class JsonKvpReader
    {
        public Dictionary<string, string> Read(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var json = streamReader.ReadToEnd();

                return JsonConvert
                    .DeserializeObject<Dictionary<string, string>>(json);
            }
        }
    }
}
