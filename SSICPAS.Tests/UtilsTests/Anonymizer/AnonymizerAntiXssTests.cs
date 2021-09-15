
using NUnit.Framework;
using static SSICPAS.Core.Logging.Anonymizer;

namespace SSICPAS.Tests.Anonymizer
{
    public class AnonymizerAntiXssTests
    {
        [Test]
        public void MaliciousCodeShouldBeEncoded()
        {
            var script = "<SCRIPT SRC=http://xss.rocks/xss.js></SCRIPT>";

            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("", RedactText(script));

            script =
                "javascript:/*--></title></style></textarea></script></xmp><svg/onload='+/\"/+/onmouseover=1/+/[*/[]/+alert(1)//'>";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("javascript:/*--&amp;gt;", RedactText(script));

            script = "<IMG SRC=\"javascript:alert('XSS');\">";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("&lt;img src=&quot;&quot;&gt;", RedactText(script));

            script = "<IMG SRC=javascript:alert('XSS')>";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("&lt;img src=&quot;&quot;&gt;", RedactText(script));

            script = "<IMG SRC=JaVaScRiPt:alert('XSS')>";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("&lt;img src=&quot;&quot;&gt;", RedactText(script));

            script = "<IMG SRC=javascript:alert(&quot;XSS&quot;)>";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("&lt;img src=&quot;&quot;&gt;", RedactText(script));

            script = "<IMG SRC=`javascript:alert(\"RSnake says, 'XSS'\")`>";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("&lt;img src=&quot;&quot;&gt;", RedactText(script));

            script = "\\<a onmouseover=\"alert(document.cookie)\"\\>xxs link\\</a\\>";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("\\&lt;a&gt;xxs link\\&lt;/a&gt;", RedactText(script));

            script = "\\<a onmouseover=alert(document.cookie)\\>xxs link\\</a\\>";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("\\&lt;a&gt;xxs link\\&lt;/a&gt;", RedactText(script));

            script = "<IMG \"\"\"><SCRIPT>alert(\"XSS\")</SCRIPT>\"\\>";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("&lt;img&gt;&amp;quot;\\&amp;gt;", RedactText(script));

            script = "<IMG SRC=javascript:alert(String.fromCharCode(88,83,83))>";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("&lt;img src=&quot;&quot;&gt;", RedactText(script));

            script = "<IMG SRC= onmouseover=\"alert('xxs')\">";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("&lt;img src=&quot;onmouseover=&amp;quot;alert(&#39;xxs&#39;)&amp;quot;&quot;&gt;", RedactText(script));

            script = "<IMG SRC=/ onerror=\"alert(String.fromCharCode(88,83,83))\"></img>";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("&lt;img src=&quot;/&quot;&gt;", RedactText(script));

            script = "<IMG SRC=&#106;&#97;&#118;&#97;&#115;&#99;&#114;&#105;&#112;&#116;&#58;&#97;&#108;&#101;&#114;&#116;&#40;&#39;&#88;&#83;&#83;&#39;&#41;>";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("&lt;img src=&quot;&quot;&gt;", RedactText(script));

            script = "<IMG SRC=&#x6A&#x61&#x76&#x61&#x73&#x63&#x72&#x69&#x70&#x74&#x3A&#x61&#x6C&#x65&#x72&#x74&#x28&#x27&#x58&#x53&#x53&#x27&#x29>";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("&lt;img src=&quot;&quot;&gt;", RedactText(script));

            script = "<IMG SRC=\"jav&#x09;ascript:alert('XSS');\">";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("&lt;img src=&quot;&quot;&gt;", RedactText(script));

            script = "<IMG SRC=\" &#14;  javascript:alert('XSS');\">";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("&lt;img src=&quot;&quot;&gt;", RedactText(script));

            script = "<BODY onload!#$%&()*~+-_.,:;?@[/|\\]^`=alert(\"XSS\")>";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("", RedactText(script));

            script = "<<SCRIPT>alert(\"XSS\");//\\<</SCRIPT>";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("&amp;lt;", RedactText(script));

            script = "<SCRIPT SRC=http://xss.rocks/xss.js?< B >";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("", RedactText(script));

            script = "<SCRIPT SRC=//xss.rocks/.j>";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("", RedactText(script));

            script = "<IMG SRC=\"`<javascript:alert>`('XSS')\"";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("&amp;lt;IMG SRC=&amp;quot;``(&#39;XSS&#39;)&amp;quot;", RedactText(script));

            script = "<iframe src=http://xss.rocks/scriptlet.html <";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("&amp;lt;iframe src=http://xss.rocks/scriptlet.html &amp;lt;", RedactText(script));

            script = "\\\";alert('XSS');//";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("\\&amp;quot;;alert(&#39;XSS&#39;);//", RedactText(script));

            script = "<BR SIZE=\"&{alert('XSS')}\">";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("&lt;br size=&quot;&amp;amp;{alert(&#39;XSS&#39;)}&quot;&gt;&#13;&#10;", RedactText(script));

            script = "<STYLE>BODY{-moz-binding:url(\"http://xss.rocks/xssmoz.xml#xss\")}</STYLE>";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("", RedactText(script));

            script = "exp/*<A STYLE='no\\xss:noxss(\"*//*\");\r\nxss:ex/*XSS*//*/*/pression(alert(\"XSS\"))'>";
            Assert.AreNotEqual(script, RedactText(script));
            Assert.AreEqual("exp/*&lt;a&gt;&lt;/a&gt;", RedactText(script));
        }
    }
}
