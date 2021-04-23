using SkrabbleLt.Database;
using SkrabbleLt.InitialData;
using SkrabbleLt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkrabbleLt.Services
{
    public class ScrabbleGame : IScrabbleGame
    {
        private readonly IManageScrabbleDb _manageScrabbleDb;

        public ScrabbleGame(IManageScrabbleDb manageScrabbleDb)
        {
            _manageScrabbleDb = manageScrabbleDb;
        }

        public ScrabbleGame()
        {
        }
        
        public void Scrabble(IManageScrabbleDb manageScrableDb)
        {
            var howManyPlayers = HowManyPlayers();
            PlayersNames(howManyPlayers, manageScrableDb);
            PrintBoard();
        }

        public void PrintBoard()
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

        public int HowManyPlayers()
        {
            Console.Write("Please enter how many players will be playing (min 2, max 4): ");
            Console.WriteLine();
            int x = 0;
            int numberOfPlayers = 0;
            while (x != 1)
            {

                char howManyPlayers = Console.ReadKey().KeyChar;
                if (int.TryParse(howManyPlayers.ToString(), out numberOfPlayers))
                {
                    if (numberOfPlayers < 2 || numberOfPlayers > 4)
                    {
                        Console.WriteLine($"Please enter min 2 or max 4 players");
                    }
                    else x = 1;
                }
                else Console.WriteLine($" - invalid entry, please enter 2, 3 or 4");
            }
            return numberOfPlayers;
        }
           
        public List<Player> PlayersNames (int howManyPlayers, IManageScrabbleDb manageScrableDb)
        {
            List<Player> playersNames = new List<Player>();
            for (int i = 0; i < howManyPlayers; i++)
            {
                Console.WriteLine();
                Console.Write($"Enter player {i + 1} name: ");
                string playerName = Console.ReadLine();
                if ((manageScrableDb.GetAllPlayers().Exists(p => p.PlayerName == playerName))) //Ar tikrai veiks, gal galima papraščiau?
                {
                    playersNames.Add(manageScrableDb.GetAllPlayers().FirstOrDefault(p => p.PlayerName == playerName));
                    Console.WriteLine($"Welcom {playerName} to the Scrabble again!");
                }
                else
                {
                    manageScrableDb.InsertPlayer(playerName);
                    playersNames.Add(manageScrableDb.GetAllPlayers().FirstOrDefault(p => p.PlayerName == playerName));
                    Console.WriteLine($"Welcom {playerName} to the Scrabble!");
                }
            }
            return playersNames;
        }

        public List<Tile> ShuffleBag()
        {
            List<Tile> bag = new List<Tile>();

            foreach (var tileQt in TileQtInitialData.DataSeed)
            {
                for (int i = 0; i < tileQt.Quantity; i++)
                {
                    bag.Add(new Tile(tileQt.Letter, tileQt.Value));
                }
            }
            bag = bag.OrderBy(i => Guid.NewGuid()).ToList(); //shuffle the list of all tiles
            return bag;
        }
    }
}
