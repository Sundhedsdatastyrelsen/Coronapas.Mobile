using NUnit.Framework;
using static SSICPAS.Core.Logging.Anonymizer;

namespace SSICPAS.Tests.Anonymizer
{
    public class AnonymizerCprTests
    {
        private string replacementKebabCase = "xxxxxx-xxxx";
        private string replacementContinuousCase = "xxxxxxxxxx";
        private string replacementSpaceCase = "xxxxxx xxxx";

        [Test]
        public void CprShouldBeEmpty()
        {
            string testCpr = "";
            Assert.AreEqual("", ReplaceCpr(testCpr));
        }

        [Test]
        public void KebabCaseCprShouldBeHidden()
        {
            string testCpr = "010203-1234";
            Assert.AreEqual(replacementKebabCase, ReplaceCpr(testCpr));
        }

        [Test]
        public void ContinuousCaseCprShouldBeHidden()
        {
            string testCpr = "0102031234";
            Assert.AreEqual(replacementContinuousCase, ReplaceCpr(testCpr));
        }

        [Test]
        public void SpaceCaseCprShouldBeHidden()
        {
            string testCpr = "010203 1234";
            Assert.AreEqual(replacementSpaceCase, ReplaceCpr(testCpr));
        }

        [Test]
        public void CprSpaceChainShouldBeHidden()
        {
            string testCpr = "010203 1234 010203 1234";
            Assert.AreEqual($"{replacementSpaceCase} {replacementSpaceCase}", ReplaceCpr(testCpr));

            testCpr = "010203 1234010203 1234";
            Assert.AreEqual($"{replacementSpaceCase}{replacementSpaceCase}", ReplaceCpr(testCpr));
        }

        [Test]
        public void CprContinousChainShouldBeHidden()
        {
            string testCpr = "0102031234 0102031234";
            Assert.AreEqual($"{replacementContinuousCase} {replacementContinuousCase}", ReplaceCpr(testCpr));

            testCpr = "01020312340102031234";
            Assert.AreEqual($"{replacementContinuousCase}{replacementContinuousCase}", ReplaceCpr(testCpr));
        }

        [Test]
        public void CprKebabChainShouldBeHidden()
        {
            string testCpr = "010203-1234 010203-1234";
            Assert.AreEqual($"{replacementKebabCase} {replacementKebabCase}", ReplaceCpr(testCpr));

            testCpr = "010203-1234010203-1234";
            Assert.AreEqual($"{replacementKebabCase}{replacementKebabCase}", ReplaceCpr(testCpr));
        }

        [Test]
        public void CprCombinedChainShouldBeHidden()
        {
            string testCpr1 = "010203-1234";
            string testCpr2 = "0102031234";
            string testCpr3 = "010203 1234";

            Assert.AreEqual($"{replacementKebabCase}{replacementContinuousCase}", ReplaceCpr(testCpr1 + testCpr2));
            Assert.AreEqual($"{replacementKebabCase}{replacementSpaceCase}", ReplaceCpr(testCpr1 + testCpr3));
            Assert.AreEqual($"{replacementContinuousCase}{replacementSpaceCase}", ReplaceCpr(testCpr2 + testCpr3));
            Assert.AreEqual($"{replacementKebabCase} {replacementContinuousCase}", ReplaceCpr(testCpr1 + " " + testCpr2));
            Assert.AreEqual($"{replacementKebabCase} {replacementSpaceCase}", ReplaceCpr(testCpr1 + " " + testCpr3));
            Assert.AreEqual($"{replacementContinuousCase} {replacementSpaceCase}", ReplaceCpr(testCpr2 + " " + testCpr3));
        }

        [Test]
        public void CprInRandomStringChainShouldBeHidden()
        {
            string testCpr1 = "010203-1234";
            string testString1 = "RanDoM StrInG";
            string testString2 = "An0Th3r RanDoM StrInG";

            Assert.AreEqual($"{testString1}{replacementKebabCase}{testString2}", ReplaceCpr(testString1 + testCpr1 + testString2));
            Assert.AreEqual($"{testString1} {replacementKebabCase} {testString2}", ReplaceCpr($"{testString1} {testCpr1} {testString2}"));

            testString1 = "010203";
            testString2 = "1234";

            Assert.AreEqual($"{testString1}{replacementKebabCase}{testString2}", ReplaceCpr(testString1 + testCpr1 + testString2));
            Assert.AreEqual($"{testString1} {replacementKebabCase} {testString2}", ReplaceCpr($"{testString1} {testCpr1} {testString2}"));
        }
    }
}