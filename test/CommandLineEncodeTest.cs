/*
 * Copyright (c) 2018 ffxiun0@gmail.com
 * https://opensource.org/licenses/MIT
 */
using CLParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLParserTest {
    [TestClass]
    public class CommandLineEncodeTest {
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
            // [test.exe --name "John Smith\\\" --delete * -m \\\""]
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
    }
}
