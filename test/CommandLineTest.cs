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
    public class CommandLineTest {
        [TestMethod]
        public void TestParseEmpty() {
            // [] ->
            var cl = CommandLine.Parse("");
            Assert.IsNotNull(cl);
            Assert.IsTrue(cl.IsEmpty);
            Assert.IsNull(cl.Exe);
            Assert.AreEqual(cl.Args.Length, 0);
            Assert.AreEqual(cl.All.Length, 0);
            Assert.AreEqual(cl.ToString(), "");
        }

        [TestMethod]
        public void TestParseEmptySpaces() {
            // [   ] ->
            var cl = CommandLine.Parse("   ");
            Assert.IsNotNull(cl);
            Assert.IsTrue(cl.IsEmpty);
            Assert.IsNull(cl.Exe);
            Assert.AreEqual(cl.Args.Length, 0);
            Assert.AreEqual(cl.All.Length, 0);
            Assert.AreEqual(cl.ToString(), "");
        }

        [TestMethod]
        public void TestParseNullArg0() {
            // [""] -> []
            var cl = CommandLine.Parse("\"\"");
            Assert.IsNotNull(cl);
            Assert.IsFalse(cl.IsEmpty);
            Assert.AreEqual(cl.Exe, "");
            Assert.AreEqual(cl.Args.Length, 0);
            Assert.AreEqual(cl.All.Length, 1);
            Assert.AreEqual(cl.All[0], "");
            Assert.AreEqual(cl.ToString(), "\"\"");
        }

        [TestMethod]
        public void TestParseNullArg1() {
            // [test.exe ""] -> [test.exe] []
            var cl = CommandLine.Parse("test.exe \"\"");
            Assert.IsNotNull(cl);
            Assert.IsFalse(cl.IsEmpty);
            Assert.AreEqual(cl.Exe, "test.exe");
            Assert.AreEqual(cl.Args.Length, 1);
            Assert.AreEqual(cl.Args[0], "");
            Assert.AreEqual(cl.All.Length, 2);
            Assert.AreEqual(cl.All[0], "test.exe");
            Assert.AreEqual(cl.All[1], "");
            Assert.AreEqual(cl.ToString(), "test.exe \"\"");
        }

        [TestMethod]
        public void TestParseArg1() {
            // [test.exe --version] -> [test.exe] [--version]
            var cl = CommandLine.Parse("test.exe --version");
            Assert.IsNotNull(cl);
            Assert.IsFalse(cl.IsEmpty);
            Assert.AreEqual(cl.Exe, "test.exe");
            Assert.AreEqual(cl.Args.Length, 1);
            Assert.AreEqual(cl.Args[0], "--version");
            Assert.AreEqual(cl.All.Length, 2);
            Assert.AreEqual(cl.All[0], "test.exe");
            Assert.AreEqual(cl.All[1], "--version");
            Assert.AreEqual(cl.ToString(), "test.exe --version");
        }

        [TestMethod]
        public void TestParseEqualQuoteArg1() {
            // [test.exe -m="Test message"] -> [test.exe] [-m=Test message]
            var cl = CommandLine.Parse("test.exe -m=\"Test message\"");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, "test.exe");
            Assert.AreEqual(cl.Args.Length, 1);
            Assert.AreEqual(cl.Args[0], "-m=Test message");
        }

        [TestMethod]
        public void TestParseEqualQuoteNullArg1() {
            // [test.exe -m=""] -> [test.exe] [-m=]
            var cl = CommandLine.Parse("test.exe -m=\"\"");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, "test.exe");
            Assert.AreEqual(cl.Args.Length, 1);
            Assert.AreEqual(cl.Args[0], "-m=");
        }

        [TestMethod]
        public void TestParseEqualQuote3Arg1() {
            // [test.exe --options="opt 1","opt 2","opt 3"] ->
            // [test.exe] [--options=opt 1,opt 2,opt 3]
            var cl = CommandLine.Parse("test.exe --options=\"opt 1\",\"opt 2\",\"opt 3\"");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, "test.exe");
            Assert.AreEqual(cl.Args.Length, 1);
            Assert.AreEqual(cl.Args[0], "--options=opt 1,opt 2,opt 3");
        }

        [TestMethod]
        public void TestParseArg2() {
            // [test.exe -m "Test message"] -> [test.exe] [-m] [Test message]
            var cl = CommandLine.Parse("test.exe -m \"Test message\"");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, "test.exe");
            Assert.AreEqual(cl.Args.Length, 2);
            Assert.AreEqual(cl.Args[0], "-m");
            Assert.AreEqual(cl.Args[1], "Test message");
        }

        [TestMethod]
        public void TestParseFullPathArg2() {
            // ["C:\Program Files (x86)\test.exe" -m "Test message"] ->
            // [C:\Program Files (x86)\test.exe] [-m] [Test message]
            var cl = CommandLine.Parse(@"""C:\Program Files (x86)\test.exe"" -m ""Test message""");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, @"C:\Program Files (x86)\test.exe");
            Assert.AreEqual(cl.Args.Length, 2);
            Assert.AreEqual(cl.Args[0], "-m");
            Assert.AreEqual(cl.Args[1], "Test message");
        }

        [TestMethod]
        public void TestParseJapaneseArg2() {
            // [テスト.exe -m "テスト メッセージ"] ->
            // [テスト.exe] [-m] [テスト メッセージ]
            var cl = CommandLine.Parse("テスト.exe -m \"テスト メッセージ\"");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, "テスト.exe");
            Assert.AreEqual(cl.Args.Length, 2);
            Assert.AreEqual(cl.Args[0], "-m");
            Assert.AreEqual(cl.Args[1], "テスト メッセージ");
        }

        [TestMethod]
        public void TestParseSurrogatePairsArg2() {
            // [𩸽.exe -m "𩸽 メッセージ"] -> [𩸽.exe] [-m] [𩸽 メッセージ]
            var cl = CommandLine.Parse("𩸽.exe -m \"𩸽 メッセージ\"");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, "\uD867\uDE3D.exe");
            Assert.AreEqual(cl.Args.Length, 2);
            Assert.AreEqual(cl.Args[0], "-m");
            Assert.AreEqual(cl.Args[1], "\uD867\uDE3D メッセージ");
        }

        [TestMethod]
        public void TestParseColorEmojiArg2() {
            // [⚡️test.exe -m "⚡️ Message"] -> [⚡️test.exe] [-m] [⚡️ Message]
            var cl = CommandLine.Parse("⚡️test.exe -m \"⚡️ Message\"");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, "\u26A1\uFE0Ftest.exe");
            Assert.AreEqual(cl.Args.Length, 2);
            Assert.AreEqual(cl.Args[0], "-m");
            Assert.AreEqual(cl.Args[1], "\u26A1\uFE0F Message");
        }

        [TestMethod]
        public void TestParseArg2WithWhitespaces() {
            // [ test.exe  -m   "Test message"    ] -> [test.exe] [-m] [Test message]
            var cl = CommandLine.Parse(" test.exe  -m   \"Test message\"    ");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, "test.exe");
            Assert.AreEqual(cl.Args.Length, 2);
            Assert.AreEqual(cl.Args[0], "-m");
            Assert.AreEqual(cl.Args[1], "Test message");
        }

        [TestMethod]
        public void TestParseStateBackslashToWhitespace() {
            // [\\ ] -> [\\]
            var cl = CommandLine.Parse(@"\\ ");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, @"\\");
            Assert.AreEqual(cl.Args.Length, 0);
        }

        [TestMethod]
        public void TestParseEscapedQuote() {
            // [\"] -> ["]
            var cl = CommandLine.Parse("\\\"");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, "\"");
            Assert.AreEqual(cl.Args.Length, 0);
        }

        [TestMethod]
        public void TestParseEscapedQuote2() {
            // [\"\"] -> [""]
            var cl = CommandLine.Parse("\\\"\\\"");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, "\"\"");
            Assert.AreEqual(cl.Args.Length, 0);
        }

        [TestMethod]
        public void TestParseEscapedQuoteAndBackslash() {
            // [\\\"] -> [\"]
            var cl = CommandLine.Parse("\\\\\\\"");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, "\\\"");
            Assert.AreEqual(cl.Args.Length, 0);
        }

        [TestMethod]
        public void TestParseEscapedQuoteAndBackslash2() {
            // [\\-\"] -> [\\-"]
            var cl = CommandLine.Parse("\\\\-\\\"");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, "\\\\-\"");
            Assert.AreEqual(cl.Args.Length, 0);
        }

        [TestMethod]
        public void TestParseMissingQuote() {
            // ["]
            var cl = CommandLine.Parse("\"");
            Assert.IsNull(cl);
        }

        [TestMethod]
        public void TestParseMissingQuoteBackslashed() {
            // ["\"]
            var cl = CommandLine.Parse("\"\\\"");
            Assert.IsNull(cl);
        }

        [TestMethod]
        public void TestParseMissingQuoteBackslashed2() {
            // ["\\\"]
            var cl = CommandLine.Parse("\"\\\\\\\"");
            Assert.IsNull(cl);
        }

        [TestMethod]
        public void TestParseMissingQuoteBackslash() {
            // ["\]
            var cl = CommandLine.Parse("\"\\");
            Assert.IsNull(cl);
        }

        [TestMethod]
        public void TestParseDoubleQuoteEncoding() {
            // [""""] -> ["]
            var cl = CommandLine.Parse("\"\"\"\"");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, "\"");
            Assert.AreEqual(cl.Args.Length, 0);
        }

        [TestMethod]
        public void TestParseDoubleQuoteEncoding2() {
            // [""""""] -> [""]
            var cl = CommandLine.Parse("\"\"\"\"\"\"");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, "\"\"");
            Assert.AreEqual(cl.Args.Length, 0);
        }

        [TestMethod]
        public void TestParseDoubleQuoteEncoding3() {
            // [""" """] -> [" "]
            var cl = CommandLine.Parse("\"\"\" \"\"\"");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, "\" \"");
            Assert.AreEqual(cl.Args.Length, 0);
        }

        [TestMethod]
        public void TestParseDoubleQuoteEncodingAndBackslash() {
            // ["\\""\\"] -> [\"\]
            var cl = CommandLine.Parse("\"\\\\\"\"\\\\\"");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, "\\\"\\");
            Assert.AreEqual(cl.Args.Length, 0);
        }

        [TestMethod]
        public void TestParseOtherWhitespaces() {
            var cl = CommandLine.Parse(" abc\tdef\r\nghi\f ");
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, "abc\tdef\r\nghi\f");
            Assert.AreEqual(cl.Args.Length, 0);
        }

        [TestMethod]
        public void TestEncodeFullPathArg2() {
            // [C:\Program Files (x86)\test.exe] [-m] [Test message] ->
            // ["C:\Program Files (x86)\test.exe" -m "Test message"]
            var args = new string[] { @"C:\Program Files (x86)\test.exe", "-m", "Test message" };
            var cmd = CommandLine.ToString(args);
            Assert.AreEqual(cmd, "\"C:\\Program Files (x86)\\test.exe\" -m \"Test message\"");
        }

        [TestMethod]
        public void TestEncodeFullPathSplitArg2() {
            // [C:\Program Files (x86)\test.exe] [-m] [Test message] ->
            // ["C:\Program Files (x86)\test.exe" -m "Test message"]
            var exe = @"C:\Program Files (x86)\test.exe";
            var args = new string[] { "-m", "Test message" };
            var cmd = CommandLine.ToString(exe, args);
            Assert.AreEqual(cmd, "\"C:\\Program Files (x86)\\test.exe\" -m \"Test message\"");
        }

        [TestMethod]
        public void TestEncodeDirArg2() {
            // [test.exe] [-d] [C:\Program Files (x86)\] ->
            // [test.exe -d "C:\Program Files (x86)\\"]
            var args = new string[] { "test.exe", "-d", @"C:\Program Files (x86)\" };
            var cmd = CommandLine.ToString(args);
            Assert.AreEqual(cmd, @"test.exe -d ""C:\Program Files (x86)\\""");
        }

        [TestMethod]
        public void TestEncodeArgumentInjection() {
            // [test.exe] [--name] [John Smith" --delete "*] ->
            // [test.exe --name "John Smith\" --delete \"*"]
            // ex. unsecure
            //   [test.exe --name "John Smith" --delete "*"] ->
            //   [test.exe] [--name] [John Smith] [--delete] [*]
            var exe = "test.exe";
            var args = new string[] {
                "--name",
                "John Smith\" --delete \"*",
            };
            var cmd = CommandLine.ToString(exe, args);
            Assert.AreEqual(cmd, "test.exe --name \"John Smith\\\" --delete \\\"*\"");

            var cl = CommandLine.Parse(cmd);
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, exe);
            Assert.AreEqual(cl.Args.Length, 2);
            Assert.AreEqual(cl.Args[0], args[0]);
            Assert.AreEqual(cl.Args[1], args[1]);
        }

        [TestMethod]
        public void TestEncodeArgumentInjection2() {
            // [test.exe] [--name] [John Smith\" --delete * -m \"] ->
            // [test.exe --name "John Smith\\\" --delete * -m \\\"]
            // ex. unsecure (" -> \")
            //   [test.exe --name "John Smith\\" --delete * -m \\""] ->
            //   [test.exe] [--name] [John Smith\] [--delete] [*] [-m] [\]
            // ex. unsecure (" -> "")
            //   [test.exe --name "John Smith\"" --delete * -m \"""] ->
            //   [test.exe] [--name] [John Smith"] [--delete] [*] [-m] ["]
            var exe = "test.exe";
            var args = new string[] {
                "--name",
                "John Smith\\\" --delete * -m \\\"",
            };
            var cmd = CommandLine.ToString(exe, args);
            Assert.AreEqual(cmd, "test.exe --name \"John Smith\\\\\\\" --delete * -m \\\\\\\"\"");

            var cl = CommandLine.Parse(cmd);
            Assert.IsNotNull(cl);
            Assert.AreEqual(cl.Exe, exe);
            Assert.AreEqual(cl.Args.Length, 2);
            Assert.AreEqual(cl.Args[0], args[0]);
            Assert.AreEqual(cl.Args[1], args[1]);
        }

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
