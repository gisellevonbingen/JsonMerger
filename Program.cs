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
                var mode = args.GetArg(ref argIndex, $"Enter program mode ({string.Join(", ", Enum.GetNames(typeof(Mode)))})").EnsureEnum<Mode>();
                var basePath = args.GetArg(ref argIndex, "Enter base file path").EnsureFileExists();
                var myPath = args.GetArg(ref argIndex, "Enter my file path").EnsureFileExists();
                var resultPath = Path.Combine(Path.GetDirectoryName(myPath), "merge_result.json");
                var logPath = Path.Combine(Path.GetDirectoryName(myPath), $"{mode}_log.txt".ToLower());

                // Prepare log messages
                var logs = new List<string>();
                var sameFileText = string.Empty;

                // Fetch lines
                var baseLines = File.ReadLines(basePath).ToArray();
                var remainPairs = File.ReadLines(myPath).ExtractPairs();
                var resultLines = new List<string>();

                // Process lines
                if (mode == Mode.Diff)
                {
                    var changedLines = new List<Tuple<LineInfo, LineInfo>>();
                    var removedLines = new List<LineInfo>();

                    for (var i = 0; i < baseLines.Length; i++)
                    {
                        var baseLine = new LineInfo() { LineNumber = i, Content = baseLines[i] };
                        var key = baseLine.Content.ExtractKey();

                        if (remainPairs.TryGetValue(key, out var myLine))
                        {
                            if (!baseLine.Content.ExtractValue().Equals(myLine.Content.ExtractValue()))
                            {
                                changedLines.Add(Tuple.Create(baseLine, myLine));
                            }

                            remainPairs.Remove(key);
                        }
                        else
                        {
                            removedLines.Add(baseLine);
                        }

                    }

                    sameFileText = "File is same";

                    if (changedLines.Count > 0)
                    {
                        logs.AddRange(CreateLogSection($"Changed lines: {changedLines.Count} (base -> my)",
                            changedLines.Select(t => $"{t.Item1.Content.ExtractKey()}\n    {t.Item1.ToStringWithOnlyValue()}\n    {t.Item2.ToStringWithOnlyValue()}")));
                    }

                    if (removedLines.Count > 0)
                    {
                        logs.AddRange(CreateLogSection($"Removed lines in base file: {removedLines.Count}", removedLines.Select(l => $"{l}")));
                    }

                    if (remainPairs.Count > 0)
                    {
                        logs.AddRange(CreateLogSection($"Added lines in my file: {remainPairs.Count}", remainPairs.Values.Select(l => $"{l}")));
                    }

                }
                else if (mode == Mode.Merge)
                {
                    var missingLines = new List<LineInfo>();
                    var sameLines = new List<LineInfo>();

                    for (var i = 0; i < baseLines.Length; i++)
                    {
                        var baseLine = new LineInfo() { LineNumber = i, Content = baseLines[i] };
                        var key = baseLine.Content.ExtractKey();

                        if (remainPairs.TryGetValue(key, out var myLine))
                        {
                            resultLines.Add(baseLine.Content.ReplaceValue(myLine.Content));

                            if (baseLine.EqualsKeyAndValue(myLine.Content) && !myLine.Content.IsNullOrWhiteSpaceOrBracket())
                            {
                                sameLines.Add(myLine);
                            }

                        }
                        else
                        {
                            resultLines.Add(baseLine.Content);
                            missingLines.Add(baseLine);
                        }

                        remainPairs.Remove(key);
                    }

                    File.WriteAllLines(resultPath, resultLines);

                    if (missingLines.Count > 0)
                    {
                        logs.AddRange(CreateLogSection($"Missing lines in base file: {missingLines.Count}", missingLines.Select(l => l.ToString())));
                    }

                    if (sameLines.Count > 0)
                    {
                        logs.AddRange(CreateLogSection($"Same lines in my file: {sameLines.Count}", sameLines.Select(l => l.ToString())));
                    }

                    if (remainPairs.Count > 0)
                    {
                        logs.AddRange(CreateLogSection($"Unmerged lines in my file: {remainPairs.Count}", remainPairs.Values.Select(l => l.ToString())));
                    }

                    sameFileText = "All line changed";

                    Console.WriteLine($"Merged file is created on");
                    Console.WriteLine(resultPath);
                    Console.WriteLine();
                }

                // Dump log messages
                if (logs.Count == 0)
                {
                    File.Delete(logPath);
                    Console.WriteLine(sameFileText);
                    Console.WriteLine();
                }
                else
                {
                    foreach (var log in logs)
                    {
                        Console.WriteLine(log);
                    }

                    File.WriteAllLines(logPath, logs);

                    Console.WriteLine($"Log file is created on");
                    Console.WriteLine(logPath);
                    Console.WriteLine();
                }

            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"File not found: {e.Message}");
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
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
