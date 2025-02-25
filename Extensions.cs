using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public static ENUM EnsureEnum<ENUM>(this string name) where ENUM : struct
        {
            if (Enum.TryParse<ENUM>(name, true, out var result))
            {
                return result;
            }
            else
            {
                throw new FormatException($"Invalid input: {name}");
            }

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
                return line.Substring(0, index).ExtractQuote();
            }

        }

        public static string ExtractValue(this string line)
        {
            var index = line.IndexOf(':');

            if (index == -1)
            {
                return line;
            }
            else
            {
                return line.Substring(index + 1).ExtractQuote();
            }

        }

        public static string ExtractQuote(this string line)
        {
            var startIndex = line.IndexOf('\"');
            var endIndex = line.LastIndexOf('\"');
            return line.Substring(startIndex + 1, endIndex - startIndex - 1);
        }

        public static string ReplaceValue(this string line, string newValue)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return line;
            }
            else
            {
                var index = line.IndexOf(':');

                if (index == -1)
                {
                    return line;
                }
                else
                {
                    var prev = line.ExtractValue();
                    var next = newValue.ExtractValue();
                    var newRight = string.IsNullOrEmpty(prev) ? next : line.Substring(index).Replace(prev, next);
                    return line.Substring(0, index) + newRight;
                }

            }

        }

        public static bool IsNullOrWhiteSpaceOrBracket(this string line)
        {
            return string.IsNullOrWhiteSpace(line) || line.Equals("{") || line.Equals("}");
        }

    }

}
