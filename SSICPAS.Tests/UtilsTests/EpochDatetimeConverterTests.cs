using System;
using Newtonsoft.Json;
using NUnit.Framework;
using SSICPAS.Core.Services.Model.Converter;

namespace SSICPAS.Tests.UtilsTests
{
    public class EpochDatetimeConverterTests
    {
        [TestCase("{\"dt\":\"2021-06-02T09:16:48Z\"}")]
        [TestCase("{\"dt\":\"2021-02-20T12:34:56Z\"}")]
        [TestCase("{\"dt\":\"2021-06-21T08:00:00Z\"}")]
        [TestCase("{\"dt\":\"2021-07-01T09:13:41.514Z\"}")]
        [TestCase("{\"dt\":\"2021-06-03T06:34:56+00:00\"}")]
        [TestCase("{\"dt\":\"2021-06-24T10:30:00+01:00\"}")]
        public void ConversionOfDifferentDateTimeFormats_ConversionSuccesful(string json)
        {
            SimpleDateTimeJson date = JsonConvert.DeserializeObject<SimpleDateTimeJson>(json);
            Assert.NotNull(date);
            Assert.True(date.date is DateTime);
        }

        [TestCase("{\"dt\":\"Corrupted DateTime format\"}")]
        public void ConversionOfDifferentDateTimeFormats_ConversionFailsForCorruptedData(string json)
        {
            var exception = Assert.Throws<FormatException>(() => JsonConvert.DeserializeObject<SimpleDateTimeJson>(json));
            Assert.That(exception.Message, Is.EqualTo("Input string was not in a correct format."));
        }

        private class SimpleDateTimeJson
        {
            [JsonProperty("dt")]
            [JsonConverter(typeof(EpochDatetimeConverter))]
            public DateTime? date { get; set; }
        }
    }
}
