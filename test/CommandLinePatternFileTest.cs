/*
 * Copyright (c) 2018 ffxiun0@gmail.com
 * https://opensource.org/licenses/MIT
 */
using CLParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace CLParserTest {
    [TestClass]
    public class CommandLinePatternFileTest {
        [TestMethod]
        public void TestEncodeFromPatternFile() {
            var tp = TestPattern.Load(@"..\..\pattern.xml");
            Assert.IsTrue(tp.PatternList.Length > 0);

            foreach (var p in tp.PatternList) {
                var encoded = CommandLine.ToString(p.Input);

                var m = string.Format("■Input[{0}] Output[{1}] Encoded[{2}]",
                    p.Input, p.Output, encoded);
                Trace.WriteLine(m);

                Assert.AreEqual(encoded, p.Output);
            }
        }

        [TestMethod]
        public void TestParseFromPatternFile() {
            var tp = TestPattern.Load(@"..\..\pattern.xml");
            Assert.IsTrue(tp.PatternList.Length > 0);

            foreach (var p in tp.PatternList) {
                var cl = CommandLine.Parse(p.Output);
                Assert.IsNotNull(cl);

                Assert.IsFalse(cl.IsEmpty);
                var decoded = cl.All[0];

                var m = string.Format("■Input[{0}] Output[{1}] Decoded[{2}]",
                    p.Output, p.Input, decoded);
                Trace.WriteLine(m);

                Assert.AreEqual(decoded, p.Input);
            }
        }
    }

    public class TestPattern {
        public Pattern[] PatternList { get; set; }

        public static TestPattern Load(string path) {
            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(path);

            var xnr = new XmlNodeReader(doc.DocumentElement);
            var xs = new XmlSerializer(typeof(TestPattern));
            var testPattern = (TestPattern)xs.Deserialize(xnr);

            return testPattern;
        }
    }

    public class Pattern {
        public string Input { get; set; }
        public string Output { get; set; }
    }
}
