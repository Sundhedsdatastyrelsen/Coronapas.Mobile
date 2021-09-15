using NUnit.Framework;
using static SSICPAS.Core.Logging.Anonymizer;

namespace SSICPAS.Tests.Anonymizer
{
    public class AnonymizerPhoneNumberTests
    {
        private string replacementShort = "xxxxxxxx";
        private string replacementFourSpaced = "xx-xx-xx-xx";
        private string replacementTwoSpaced = "xxxx-xxxx";
        private string replacementDirectional = "+xx";

        [Test]
        public void PhoneNumberShouldBeEmpty()
        {
            string testPhoneNumberEmpty = "";
            Assert.AreEqual(testPhoneNumberEmpty, ReplacePhoneNumber(testPhoneNumberEmpty));
        }

        [Test]
        public void PhoneNumberShouldBeHidden()
        {
            string directional = "";
            string testPhoneNumber1 = $"{directional}11111111";
            string testPhoneNumber2 = $"{directional}11 11 11 11";
            string testPhoneNumber3 = $"{directional}1111 1111";
            string testPhoneNumber4 = $"{directional}11-11-11-11";
            string testPhoneNumber5 = $"{directional}1111-1111";

            Assert.AreEqual($"{replacementShort}", ReplacePhoneNumber(testPhoneNumber1));
            Assert.AreEqual($"{replacementFourSpaced}", ReplacePhoneNumber(testPhoneNumber2));
            Assert.AreEqual($"{replacementTwoSpaced}", ReplacePhoneNumber(testPhoneNumber3));
            Assert.AreEqual($"{replacementFourSpaced}", ReplacePhoneNumber(testPhoneNumber4));
            Assert.AreEqual($"{replacementTwoSpaced}", ReplacePhoneNumber(testPhoneNumber5));

            directional = "+45";
            testPhoneNumber1 = $"{directional}11111111";
            testPhoneNumber2 = $"{directional}11 11 11 11";
            testPhoneNumber3 = $"{directional}1111 1111";
            testPhoneNumber4 = $"{directional}11-11-11-11";
            testPhoneNumber5 = $"{directional}1111-1111";

            Assert.AreEqual($"{replacementDirectional}{replacementShort}", ReplacePhoneNumber(testPhoneNumber1));
            Assert.AreEqual($"{replacementDirectional}{replacementFourSpaced}", ReplacePhoneNumber(testPhoneNumber2));
            Assert.AreEqual($"{replacementDirectional}{replacementTwoSpaced}", ReplacePhoneNumber(testPhoneNumber3));
            Assert.AreEqual($"{replacementDirectional}{replacementFourSpaced}", ReplacePhoneNumber(testPhoneNumber4));
            Assert.AreEqual($"{replacementDirectional}{replacementTwoSpaced}", ReplacePhoneNumber(testPhoneNumber5));

            directional = "0045";
            testPhoneNumber1 = $"{directional}11111111";
            testPhoneNumber2 = $"{directional}11 11 11 11";
            testPhoneNumber3 = $"{directional}1111 1111";
            testPhoneNumber4 = $"{directional}11-11-11-11";
            testPhoneNumber5 = $"{directional}1111-1111";

            Assert.AreEqual($"{replacementDirectional}{replacementShort}", ReplacePhoneNumber(testPhoneNumber1));
            Assert.AreEqual($"{replacementDirectional}{replacementFourSpaced}", ReplacePhoneNumber(testPhoneNumber2));
            Assert.AreEqual($"{replacementDirectional}{replacementTwoSpaced}", ReplacePhoneNumber(testPhoneNumber3));
            Assert.AreEqual($"{replacementDirectional}{replacementFourSpaced}", ReplacePhoneNumber(testPhoneNumber4));
            Assert.AreEqual($"{replacementDirectional}{replacementTwoSpaced}", ReplacePhoneNumber(testPhoneNumber5));

            directional = "+45";
            testPhoneNumber1 = $"{directional} 11111111";
            testPhoneNumber2 = $"{directional} 11 11 11 11";
            testPhoneNumber3 = $"{directional} 1111 1111";
            testPhoneNumber4 = $"{directional} 11-11-11-11";
            testPhoneNumber5 = $"{directional} 1111-1111";

            Assert.AreEqual($"{replacementDirectional} {replacementShort}", ReplacePhoneNumber(testPhoneNumber1));
            Assert.AreEqual($"{replacementDirectional} {replacementFourSpaced}", ReplacePhoneNumber(testPhoneNumber2));
            Assert.AreEqual($"{replacementDirectional} {replacementTwoSpaced}", ReplacePhoneNumber(testPhoneNumber3));
            Assert.AreEqual($"{replacementDirectional} {replacementFourSpaced}", ReplacePhoneNumber(testPhoneNumber4));
            Assert.AreEqual($"{replacementDirectional} {replacementTwoSpaced}", ReplacePhoneNumber(testPhoneNumber5));

            directional = "0045";
            testPhoneNumber1 = $"{directional} 11111111";
            testPhoneNumber2 = $"{directional} 11 11 11 11";
            testPhoneNumber3 = $"{directional} 1111 1111";
            testPhoneNumber4 = $"{directional} 11-11-11-11";
            testPhoneNumber5 = $"{directional} 1111-1111";

            Assert.AreEqual($"{replacementDirectional} {replacementShort}", ReplacePhoneNumber(testPhoneNumber1));
            Assert.AreEqual($"{replacementDirectional} {replacementFourSpaced}", ReplacePhoneNumber(testPhoneNumber2));
            Assert.AreEqual($"{replacementDirectional} {replacementTwoSpaced}", ReplacePhoneNumber(testPhoneNumber3));
            Assert.AreEqual($"{replacementDirectional} {replacementFourSpaced}", ReplacePhoneNumber(testPhoneNumber4));
            Assert.AreEqual($"{replacementDirectional} {replacementTwoSpaced}", ReplacePhoneNumber(testPhoneNumber5));
        }

        [Test]
        public void PhoneNumberInStringChainShouldBeHidden()
        {
            string testString1 = "RanDoM StrInG";
            string testString2 = "An0Th3r RanDoM StrInG";
            string directional = "+45";
            string phoneNumber = $"{directional} 11-11-11-11";

            Assert.AreEqual($"{testString1}{replacementDirectional} {replacementFourSpaced}{testString2}", ReplacePhoneNumber($"{testString1}{phoneNumber}{testString2}"));
            Assert.AreEqual($"{testString1} {replacementDirectional} {replacementFourSpaced} {testString2}", ReplacePhoneNumber($"{testString1} {phoneNumber} {testString2}"));
        }
    }
}