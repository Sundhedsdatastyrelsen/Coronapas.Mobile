using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using SSICPAS.Services;
using SSICPAS.Utils;

namespace SSICPAS.Tests.UtilsTests
{
    public class DateUtilsTests
    {
        [SetUp]
        public void Setup(){
            var testStream = new MemoryStream(Encoding.UTF8.GetBytes(@"{""LANG_DATEUTIL"": ""en-GB""}"));
            LocaleService.Current.LoadLocale("en", testStream, false);
        }
        [TestCase("01/08/2021 14:00:00", "Jan 8, 2021")]
        [TestCase("05/29/2021", "May 29, 2021")]
        [TestCase("1/9/2021 05:50:06", "Jan 9, 2021")]
        [TestCase("2021/10/12", "Oct 12, 2021")]
        [TestCase("2021-02-28T05:50:06", "Feb 28, 2021")]
        public void TestFormatDateTimeToDateDK(DateTime date, string expected)
        {
            var actual = DateUtils.LocaleFormatDate(date);

            Assert.AreEqual(expected, actual);
        }

        [TestCase("01/08/2021 14:00:00", "02:00 PM")]
        [TestCase("05/29/2021", "12:00 AM")]
        [TestCase("1/9/2021 05:50:06", "05:50 AM")]
        [TestCase("2021/10/12", "12:00 AM")]
        [TestCase("2021-02-28T05:50:06", "05:50 AM")]
        public void TestFormatDateTimeToTimeDK(DateTime date, string expected)
        {
            var actual = DateUtils.LocaleFormatTime(date);

            Assert.AreEqual(expected, actual);
        }

        [TestCase("01/08/2021 14:00:00", "Jan 8, 2021")]
        [TestCase("05/29/2021", "May 29, 2021")]
        [TestCase("1/9/2021 05:50:06", "Jan 9, 2021")]
        [TestCase("2021/10/12", "Oct 12, 2021")]
        [TestCase("2021-02-28T05:50:06", "Feb 28, 2021")]
        public void TestFormatDateToScannerResultFormat(DateTime date, string expected)
        {
            var actual = date.ToLocaleDateFormat();
            
            Assert.AreEqual(expected, actual);
        }

        [TestCase("01/08/2021 14:00:00", "Jan 8, 2021")]
        [TestCase("05/29/2021", "May 29, 2021")]
        [TestCase("1/9/2021 05:50:06", "Jan 9, 2021")]
        [TestCase("2021/10/12", "Oct 12, 2021")]
        [TestCase("2021-02-28T05:50:06", "Feb 28, 2021")]
        public void TestFormatDatePersonalPage(DateTime date, string expected)
        {
            var actual = date.LocaleFormatDate();

            Assert.AreEqual(expected, actual);
        }

        [TestCase("01/08/2021 14:00:00", "Jan 8, 2021")]
        [TestCase("05/29/2021", "May 29, 2021")]
        [TestCase("1/9/2021 05:50:06", "Jan 9, 2021")]
        [TestCase("2021/10/12", "Oct 12, 2021")]
        [TestCase("2021-02-28T05:50:06", "Feb 28, 2021")]
        public void TestChooseFormat(DateTime date, string expected)
        {
            //The language is english so there will never be a situation where the bool is false.
            bool isEnglish = true;
            var actual = date.ToLocaleDateFormat(isEnglish);

            Assert.AreEqual(expected, actual);
        }

    }
}