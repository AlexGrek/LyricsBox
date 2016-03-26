using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using LyricsBox;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void LyricStringParser()
        {
            //Some TDD
            var s1 = "[12:03.48]Hello, world!";
            var s1l = new LyricString(s1);
            Assert.AreEqual(s1, s1l.ToString());
            var s2 = "[12:03.48][15:12.97]Bye, world!";
            var s2l = new LyricString(s2);
            Assert.AreEqual(s2, s2l.ToString());
            var s3 = "[12:03][15:12]Bye, world!";
            var s3l = new LyricString(s3);
            Assert.AreEqual("[12:03.00][15:12.00]Bye, world!", s3l.ToString());
        }

        [TestMethod]
        public void GetLyrics()
        {
            var ld = new LyricsDownloader("Northlane", "Weightless");
            var t = ld.GetLyricsAsync();
            
            var result = t.Result;
        }
    }
}
