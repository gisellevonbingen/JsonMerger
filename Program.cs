using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangFileDiff
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                // Initialze paths
                int argIndex = 0;
                var basePath = args.GetArg(ref argIndex, "Enter base file path").EnsureFileExists();
                var myPath = args.GetArg(ref argIndex, "Enter my file path").EnsureFileExists();
                var resultPath = Path.Combine(Path.GetDirectoryName(myPath), "result.json");
                var logPath = Path.Combine(Path.GetDirectoryName(myPath), "log.txt");

                // Fetch lines
                var baseLines = File.ReadLines(basePath).ToArray();
                var remainPairs = File.ReadLines(myPath).ExtractPairs();
                var resultLines = new List<string>();
                var missingLines = new List<LineInfo>();

                // Process lines
                for (var i = 0; i < baseLines.Length; i++)
                {
                    var baseLine = baseLines[i];
                    var key = baseLine.ExtractKey();
                    string resultLine;

                    if (remainPairs.TryGetValue(key, out var myLine))
                    {
                        resultLine = myLine.Content;
                    }
                    else
                    {
                        resultLine = baseLine;
                        missingLines.Add(new LineInfo() { LineNumber = i, Content = baseLine });
                    }

                    resultLines.Add(resultLine);
                    remainPairs.Remove(key);
                }

                File.WriteAllLines(resultPath, resultLines);

                // Dump log file
                var logs = new List<string>();

                if (missingLines.Count > 0)
                {
                    logs.AddRange(CreateLogSection($"Missing lines in base file : {missingLines.Count}", missingLines.Select(l => l.ToString())));
                }

                if (remainPairs.Count > 0)
                {
                    logs.AddRange(CreateLogSection($"Unmerged lines in my file : {remainPairs.Count}", remainPairs.Values.Select(l => l.ToString())));
                }

                if (logs.Count == 0)
                {
                    File.Delete(logPath);
                    Console.WriteLine("Merged perfectly");
                    Console.WriteLine();
                }
                else
                {
                    foreach (var log in logs)
                    {
                        Console.WriteLine(log);
                    }

                    File.WriteAllLines(logPath, logs);
                }

                // Finishing
                Console.WriteLine($"Merged file is created on");
                Console.WriteLine(resultPath);
                Console.WriteLine();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"File not found: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected exception: {e}");
            }

            Console.WriteLine("Enter to close");
            Console.ReadLine();
        }

        public static IEnumerable<string> CreateLogSection(string title, IEnumerable<string> lines)
        {
            yield return "※※※※※※※※※※※※※※※※※※※※";
            yield return $"※ {title}";
            yield return "※※※※※※※※※※※※※※※※※※※※";
            yield return string.Empty;

            foreach (var line in lines)
            {
                yield return line;
            }

            yield return string.Empty;
            yield return string.Empty;
        }

    }

}
