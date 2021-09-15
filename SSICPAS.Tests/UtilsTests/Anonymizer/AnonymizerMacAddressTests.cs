using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using static SSICPAS.Core.Logging.Anonymizer;

namespace SSICPAS.Tests.Anonymizer
{
    public class AnonymizerMacAddressTests
    {
        private string replacementMacAddress = "xx:xx:xx:xx";
        [Test]
        public void MacAddressShouldBeEmpty()
        {
            string macAddress = "";
            Assert.IsEmpty(ReplaceMacAddress(macAddress));
        }

        [Test]
        public void CorrectMacAddressShouldBeHidden()
        {
            List<string> macAddresses = new List<string>
            {
                "00-22-64-a6-c4-f0",
                "00:22:64:a6:c4:f0",
                "00.22.64.a6.c4.f0",
                "002-264-a6c-4f0",
                "002:264:a6c:4f0",
                "002.264.a6c.4f0"
            };

            foreach (var address in macAddresses)
            {
                Assert.AreEqual(replacementMacAddress, ReplaceMacAddress(address));
            }
        }

        [Test]
        public void IncorrectMacAddressShouldBeShown()
        {
            List<string> macAddresses = new List<string>
            {
                "00-22-64-a6-c4-fg",
                "00:22:64:a6:c4:fg",
                "00.22.64.a6.c4.fg",
                "002-264-a6c-4fg",
                "002:264:a6c:4fg",
                "002.264.a6c.4fg"
            };

            for (var index = 0; index < macAddresses.Count; index++)
            {
                var address = macAddresses[index];
                Assert.AreEqual(address, ReplaceMacAddress(address));
            }
        }

        [Test]
        public void MacAddressChainShouldBeHidden()
        {
            List<string> macAddresses = new List<string>
            {
                "00-22-64-a6-c4-f0",
                "00:22:64:a6:c4:f0",
                "00.22.64.a6.c4.f0",
                "002-264-a6c-4f0",
                "002:264:a6c:4f0",
                "002.264.a6c.4f0"
            };

            Assert.AreEqual(string.Concat(Enumerable.Repeat(replacementMacAddress, 6)), ReplaceMacAddress(macAddresses.Aggregate("", (s, s1) => s + s1)));
        }
    }
}
