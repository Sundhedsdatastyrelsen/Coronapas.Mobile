using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using SSICPAS.Services;
using SSICPAS.Utils;

namespace SSICPAS.Tests.UtilsTests
{
    public class DateUtilsTestsDanish
    {
        [SetUp]
        public void Setup()
        {
            var testStream = new MemoryStream(Encoding.UTF8.GetBytes(@"{""LANG_DATEUTIL"": ""da-DK""}"));
            LocaleService.Current.LoadLocale("dk", testStream, false);
        }
        [TestCase("01/08/2021 14:00:00", "8. jan 2021")]
        [TestCase("05/29/2021", "29. maj 2021")]
        [TestCase("1/9/2021 05:50:06", "9. jan 2021")]
        [TestCase("2021/10/12", "12. okt 2021")]
        [TestCase("2021-02-28T05:50:06", "28. feb 2021")]
        [TestCase("2020-02-29T05:50:06", "29. feb 2020")]
        public void TestFormatDateTimeToDateDK(DateTime date, string expected)
        {
            var actual = DateUtils.LocaleFormatDate(date);

            Assert.AreEqual(expected, actual);
        }

        [TestCase("01/08/2021 14:00:00", "14.00 ")]
        [TestCase("05/29/2021", "00.00 ")]
        [TestCase("1/9/2021 05:50:06", "05.50 ")]
        [TestCase("2021/10/12", "00.00 ")]
        [TestCase("2021-02-28T05:50:06", "05.50 ")]
        public void TestFormatDateTimeToTimeDK(DateTime date, string expected)
        {
            var actual = DateUtils.LocaleFormatTime(date);

            Assert.AreEqual(expected, actual);
        }

        [TestCase("01/08/2021 14:00:00", "8. jan 2021")]
        [TestCase("05/29/2021", "29. maj 2021")]
        [TestCase("1/9/2021 05:50:06", "9. jan 2021")]
        [TestCase("2021/10/12", "12. okt 2021")]
        [TestCase("2021-02-28T05:50:06", "28. feb 2021")]
        public void TestFormatDateToScannerResultFormat(DateTime date, string expected)
        {
            var actual = date.ToLocaleDateFormat();

            Assert.AreEqual(expected, actual);
        }

        [TestCase("01/08/2021 14:00:00", "8. jan 2021")]
        [TestCase("05/29/2021", "29. maj 2021")]
        [TestCase("1/9/2021 05:50:06", "9. jan 2021")]
        [TestCase("2021/10/12", "12. okt 2021")]
        [TestCase("2021-02-28T05:50:06", "28. feb 2021")]
        public void TestFormatDatePersonalPage(DateTime date, string expected)
        {
            var actual = date.LocaleFormatDate();

            Assert.AreEqual(expected, actual);
        }

        [TestCase("01/08/2021 14:00:00", "Jan 8, 2021", true)]
        [TestCase("05/29/2021", "May 29, 2021", true)]
        [TestCase("1/9/2021 05:50:06", "Jan 9, 2021", true)]
        [TestCase("2021/10/12", "Oct 12, 2021", true)]
        [TestCase("2021-02-28T05:50:06", "Feb 28, 2021", true)]
        [TestCase("01/08/2021 14:00:00", "8. jan 2021", false)]
        [TestCase("05/29/2021", "29. maj 2021", false)]
        [TestCase("1/9/2021 05:50:06", "9. jan 2021", false)]
        [TestCase("2021/10/12", "12. okt 2021", false)]
        [TestCase("2021-02-28T05:50:06", "28. feb 2021", false)]
        public void TestChooseFormat(DateTime date, string expected, bool isEnglish)
        {
            var actual = date.ToLocaleDateFormat(isEnglish);

            Assert.AreEqual(expected, actual);
        }

    }
}
