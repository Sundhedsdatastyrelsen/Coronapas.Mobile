using NUnit.Framework;
using SSICPAS.Utils;
using Xamarin.Forms;

namespace SSICPAS.Tests.UtilsTests
{
    public class SSICPASColorTest : BaseUITests
    {
        [TestCase(SSICPASColor.SSITitleTextColor, "#1c1999")]
        [TestCase(SSICPASColor.SSILightTextColor, "#77819A")]
        [TestCase(SSICPASColor.SSIContentTextColor, "#47526F")]
        [TestCase(SSICPASColor.SSIBaseTextColor, "#24215f")]
        public void TestValidFullName(SSICPASColor actualColor, string resourceValue)
        {
            //when calling .Color on the enum
            Color actual = actualColor.Color();
            //then it will fetch the right color from the resource dict
            Assert.AreEqual(Color.FromHex(resourceValue), actual);
        }
    }
}