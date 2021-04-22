using SkrabbleLt.InitialData;
using SkrabbleLt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkrabbleLt.Services
{
    public class ScrabbleGame
    {
        public static void PrintBoard()
        {
            Console.WriteLine("     -------------------------------------------------------------");
            Console.WriteLine("     | 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10| 11| 12| 13| 14|");

            Console.WriteLine("------------------------------------------------------------------");
            for (int i = 0; i < 15; i++)
            {
                if (i < 10)
                {
                    Console.Write($"| {i}  ");
                }
                else
                {
                    Console.Write($"| {i} ");
                }
                foreach (var cell in BoardInitialData.DataSeed.Where(h => h.HPosition == i))
                {
                    Console.Write("|");
                    Console.Write(cell.SpecialValue);
                }
                Console.WriteLine("|");
                Console.WriteLine("------------------------------------------------------------------");
            }
        }

    }
}
