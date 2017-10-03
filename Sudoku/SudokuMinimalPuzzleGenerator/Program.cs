using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalSudokuGen
{
    class Program
    {
        static Stopwatch timer = Stopwatch.StartNew();

        static string appTime ()
        {
            return timer.Elapsed.TotalSeconds.ToString("0").PadLeft(5, ' ') + "s";
        }

        static void Main(string[] args)
        {
            var sourceFile = args.Length > 0 ? args[0] : @"sudoku_1000.csv";
            var outputPath = @"minimal_sudokus.csv";
            Console.WriteLine("{0}: Loading sudokus from {1}...", appTime(), sourceFile);
            var lines = File.ReadAllLines(sourceFile).Select(a => a.Split(',')[0]);
            var csv = from line in lines
                      select (line.Split(',')[0]);
            Console.WriteLine("{0}: Done.", appTime());
            int puzzleCounter = 0;
            foreach (var sudoku in csv)
            {
                puzzleCounter++;
                Puzzle original = new Puzzle();
                original.pullFromLongString(sudoku);
                //var minimal = p.findMinimalPuzzle(); // minimal with fewest possible givens (computational explosion)
                var minimal = original.findBadMinimalPuzzle(); // minimal, but a minimal with fewer givens might exist
                Console.WriteLine("{0}: sudoku  #{1}: {2} ({3} givens)", appTime(), puzzleCounter, original.toLongString(), original.givensCount());
                if (minimal != null)
                {
                    Console.WriteLine("{0}: minimal #{1}: {2} ({3} givens)", appTime(), puzzleCounter, minimal.toLongString(), minimal.givensCount());
                    var isWrittenInOutputFile = false;
                    while (!isWrittenInOutputFile)
                    {
                        try
                        {
                            using (StreamWriter sw = File.AppendText(outputPath))
                            {
                                var newLine = string.Format("{0}", minimal.toLongString());
                                sw.WriteLine(newLine);
                                isWrittenInOutputFile = true;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("{0}: derp ({1})", appTime(), e.Message);
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("{0}: minimal #{1}: no solution found", appTime(), puzzleCounter);
                }
            }
            Console.WriteLine("{0}: Done", appTime());

            Console.ReadLine();
        }

        static void findAndPrintMinimal(Puzzle p)
        {
            p = p.findMinimalPuzzle();

            if (p != null)
            {
                Console.WriteLine("The puzzle with the fewest starting numbers whose solution is unique and matches the solution of the input puzzle is: \n");
                p.print();
            }
        }
    }
}
