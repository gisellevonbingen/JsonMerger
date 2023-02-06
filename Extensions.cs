using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangFileDiff
{
    public static class Extensions
    {
        public static string GetArg(this string[] args, ref int argIndex, string prompt)
        {
            if (argIndex < args.Length)
            {
                return args[argIndex++];
            }
            else
            {
                Console.WriteLine(prompt);
                Console.Write(">");
                var arg = Console.ReadLine();
                Console.WriteLine();

                return arg;
            }

        }

        public static string EnsureFileExists(this string path)
        {
            path = path.AsPath();

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            return path;
        }

        public static string AsPath(this string path)
        {
            if (path.StartsWith("\"") && path.EndsWith("\""))
            {
                return path.Substring(1, path.Length - 2);
            }
            else
            {
                return path;
            }

        }

        public static Dictionary<string, LineInfo> ExtractPairs(this IEnumerable<string> lines)
        {
            var map = new Dictionary<string, LineInfo>();
            var array = lines.ToArray();

            for (var i = 0; i < array.Length; i++)
            {
                var line = array[i];
                var key = line.ExtractKey();
                map[key] = new LineInfo() { LineNumber = i, Content = line };
            }

            return map;
        }

        public static string ExtractKey(this string line)
        {
            var index = line.IndexOf(':');

            if (index == -1)
            {
                return line;
            }
            else
            {
                return line.Substring(0, index);
            }

        }

    }

}
