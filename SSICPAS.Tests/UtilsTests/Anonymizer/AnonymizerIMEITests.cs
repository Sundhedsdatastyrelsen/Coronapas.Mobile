using NUnit.Framework;
using static SSICPAS.Core.Logging.Anonymizer;

namespace SSICPAS.Tests.Anonymizer
{
    public class AnonymizerIMEITests
    {
        private string replacementImei = "xxxxxxxxxxxxxxx";

        [Test]
        public void ImeiShouldBeEmpty()
        {
            string imeiNumber = "";
            Assert.IsEmpty(ReplaceIMEI(imeiNumber));
        }

        [Test]
        public void CorrectImeiShouldBeHidden()
        {
            string imeiNumber = "490154203237518";
            Assert.AreEqual(replacementImei, ReplaceIMEI(imeiNumber));
        }

        [Test]
        public void CorrectImeiChainShouldBeHidden()
        {
            string imeiNumber = "490154203237518";
            Assert.AreEqual($"{replacementImei}{replacementImei}", ReplaceIMEI($"{imeiNumber}{imeiNumber}"));
            Assert.AreEqual($"{replacementImei} {replacementImei}", ReplaceIMEI($"{imeiNumber} {imeiNumber}"));
        }

        [Test]
        public void IncorrectImeiShouldBeShown()
        {
            string imeiNumber = "990000862471854";
            Assert.AreEqual(imeiNumber, ReplaceIMEI(imeiNumber));
        }

        [Test]
        public void IncorrectImeiChainShouldBeShown()
        {
            string correctImeiNumber = "490154203237518";
            string incorrectImeiNumber = "990000862471854";
            Assert.AreEqual($"{incorrectImeiNumber}{incorrectImeiNumber}", ReplaceIMEI($"{incorrectImeiNumber}{incorrectImeiNumber}"));
            Assert.AreEqual($"{incorrectImeiNumber} {incorrectImeiNumber}", ReplaceIMEI($"{incorrectImeiNumber} {incorrectImeiNumber}"));

            Assert.AreEqual($"{replacementImei}{incorrectImeiNumber}", ReplaceIMEI($"{correctImeiNumber}{incorrectImeiNumber}"));
            Assert.AreEqual($"{replacementImei} {incorrectImeiNumber}", ReplaceIMEI($"{correctImeiNumber} {incorrectImeiNumber}"));

            Assert.AreEqual($"{incorrectImeiNumber}{replacementImei}", ReplaceIMEI($"{incorrectImeiNumber}{correctImeiNumber}"));
            Assert.AreEqual($"{incorrectImeiNumber} {replacementImei}", ReplaceIMEI($"{incorrectImeiNumber} {correctImeiNumber}"));
        }

        [Test]
        public void CorrectImeiInStringShouldBeHidden()
        {
            string correctImeiNumber = "490154203237518";
            string testString1 = "Random test string";
            string testString2 = "Another random test string.";
            Assert.AreEqual($"{testString1}{replacementImei}{testString2}", ReplaceIMEI($"{testString1}{correctImeiNumber}{testString2}"));
            Assert.AreEqual($"{testString1} {replacementImei} {testString2}", ReplaceIMEI($"{testString1} {correctImeiNumber} {testString2}"));
        }
    }
}
