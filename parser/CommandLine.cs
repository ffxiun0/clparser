﻿/*
 * Copyright (c) 2018 ffxiun0@gmail.com
 * https://opensource.org/licenses/MIT
 */
using System.Collections.Generic;
using System.Linq;

namespace CLParser {
    public class CommandLine {
        private string[] all;
        private string exe;
        private string[] args;

        private CommandLine(IEnumerable<string> args) {
            var list = args.ToList();

            this.all = list.ToArray();

            if (list.Count > 0) {
                this.exe = list[0];
                this.args = list.GetRange(1, list.Count - 1).ToArray();
            } else {
                this.exe = null;
                this.args = new string[0];
            }
        }

        public string[] All { get => all; }
        public string Exe { get => exe; }
        public string[] Args { get => args; }
        public bool IsEmpty { get => all.Length == 0; }

        public static CommandLine Parse(string s) {
            var parser = new CommandLineParser();

            var args = parser.Parse(s);
            if (args == null) return null;

            return new CommandLine(args);
        }

        public override string ToString() {
            return CommandLineEncoder.Encode(all);
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
