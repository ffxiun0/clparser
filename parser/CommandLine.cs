/*
 * Copyright (c) 2018 ffxiun0@gmail.com
 * https://opensource.org/licenses/MIT
 */
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParser {
    public class CommandLine {
        private string exe;
        private string[] args;

        private CommandLine() {
            exe = "";
            args = new string[0];
        }

        private CommandLine(string exe, IEnumerable<string> args) {
            this.exe = exe;
            this.args = args.ToArray();
        }

        private CommandLine(IEnumerable<string> args) {
            var list = args.ToList();

            if (list.Count > 0) {
                this.exe = list[0];
                this.args = list.GetRange(1, list.Count - 1).ToArray();
            } else {
                this.exe = "";
                this.args = new string[0];
            }
        }

        public string Exe {
            get {
                return exe;
            }
        }

        public string[] Args {
            get {
                return args;
            }
        }

        public static CommandLine Parse(string s) {
            var parser = new CommandLineParser();

            var args = parser.Parse(s);
            if (args == null) return null;

            return new CommandLine(args);
        }

        public override string ToString() {
            return ToString(Exe, Args);
        }

        public static string ToString(string s) {
            return CommandLineEncoder.Encode(s);
        }

        public static string ToString(IEnumerable<string> args) {
            return CommandLineEncoder.Encode(args);
        }

        public static string ToString(string exe, IEnumerable<string> args) {
            return CommandLineEncoder.Encode(Concat(exe, args));
        }

        private static IEnumerable<T> Concat<T>(T element, IEnumerable<T> list) {
            yield return element;

            foreach (var item in list)
                yield return item;
        }
    }
}
