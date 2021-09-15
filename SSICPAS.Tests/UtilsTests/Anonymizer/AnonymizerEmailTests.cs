using NUnit.Framework;
using static SSICPAS.Core.Logging.Anonymizer;

namespace SSICPAS.Tests.Anonymizer
{
    public class AnonymizerEmailTests
    {
        [Test]
        public void EmailShouldBeEmpty()
        {
            string testEmail = "";
            Assert.AreEqual("", ReplaceEmailAddress(testEmail));
        }

        [Test]
        public void EmailShouldBeHidden()
        {
            string testEmail = "test@gmail.com";
            Assert.AreEqual($"****@*****.com", ReplaceEmailAddress(testEmail));
        }

        [Test]
        public void EmailTwoPartsShouldBeHidden()
        {
            string testEmail = "test@gmail.co.uk";
            Assert.AreEqual($"****@*****.co.uk", ReplaceEmailAddress(testEmail));
        }

        [Test]
        public void ShortEmailShouldBeHidden()
        {
            string testEmail = "a@a.a";
            Assert.AreEqual($"*@*.a", ReplaceEmailAddress(testEmail));
        }

        [Test]
        public void LongEmailShouldBeHidden()
        {
            string prefix = "very-long-and-complicate-email_address";
            string postfix =
                "and-here-we-have-long-and_complicated_domain_name.with_dots.and_basically.few_of.them.com";
            string mail = $"{prefix}@{postfix}";
            Assert.AreEqual($"{new string('*', prefix.Length)}@{new string('*', postfix.IndexOf('.'))}{postfix.Substring(postfix.IndexOf('.'))}", ReplaceEmailAddress(mail));
        }

        [Test]
        public void DanishEmailShouldBeHidden()
        {
            string testEmail = "test@denmark.gov.dk";
            Assert.AreEqual($"****@*******.gov.dk", ReplaceEmailAddress(testEmail));
        }

        [Test]
        public void EmailChainShouldBeHidden()
        {
            string testEmail1 = "test@test.com";
            string testEmail2 = "test@test.com";
            Assert.AreEqual($"****@************@****.com", ReplaceEmailAddress($"{testEmail1}{testEmail2}"));
            Assert.AreEqual($"****@****.com ****@****.com", ReplaceEmailAddress($"{testEmail1} {testEmail2}"));
        }

        [Test]
        public void EmailInStringShouldBeHidden()
        {
            string testString = "My email is:";
            string testEmail1 = "test@test.com";
            Assert.AreEqual($"{testString} ****@****.com", ReplaceEmailAddress($"{testString} {testEmail1}"));
        }
    }
}