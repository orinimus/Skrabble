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

        public List<Player> PlayersNames(int howManyPlayers, IManageScrabbleDb manageScrableDb)
        {
            List<Player> players = new List<Player>();
            for (int i = 0; i < howManyPlayers; i++)
            {
                players.Add(PlayerName(i, manageScrableDb));
            }
            return players;
        }

        public void Scrabble(IManageScrabbleDb manageScrableDb)
        {
            int roundNumber = 1; //! 
            int skipedMoves = 0; //!
            var howManyPlayers = HowManyPlayers(); //gaunam kiek zaideju
            var players = PlayersNames(howManyPlayers, manageScrableDb); //gaunam zaideju vardus
            List<Tile> bag = ShuffleBag(); //gaunam pradini sumaisyta maiseli
            List<PlayerHand> playerHands = new List<PlayerHand>();
            List<int> playingPlayersId = new List<int>(); //žaidžiančių žaidėjų ID
            foreach (var player in players) 
            {                
                var generatedNewPlayerHand = GenerateNewHand(bag);
                playerHands.Add(new PlayerHand(player.PlayerId, generatedNewPlayerHand));
                bag.RemoveRange(0, 7);
                playingPlayersId.Add(player.PlayerId);
                List<char> lettersOnHand = new List<char>();
                foreach (var item in generatedNewPlayerHand)
                {
                    lettersOnHand.Add(item.Letter);
                }
                Console.WriteLine($"Player's {player.PlayerName} hand: {string.Join(' ', lettersOnHand)}");
            } //inicijuojam zaidejams pirmasias rankas
            List<BoardCell> gameBoard = new List<BoardCell>();
            foreach (var cell in BoardInitialData.DataSeed)
            {
                gameBoard.Add(new BoardCell(cell.BoardCellId, cell.HPosition, cell.VPosition, cell.SpecialValue, ' '));
            }


            //PrintBoard(); //atsispausdinam pradine lentele
            for (int i = 0; i < howManyPlayers; i++)
            {
                PlayersMove(roundNumber, playingPlayersId[i], manageScrableDb, playerHands, gameBoard); //IDpriskyrimas
            }
            Console.WriteLine();

        }

        public List<Tile> GetPlayerHand(int playerId, List<PlayerHand> playerHands)
        {
            List<Tile> hand = new List<Tile>();
            hand = playerHands.FirstOrDefault(p => p.PlayerID == playerId).Hand.ToList();
            return hand;
        }

        public void PlayersMove(int roundNUmber, int playerId, IManageScrabbleDb manageScrableDb, List<PlayerHand> playerHands, List<BoardCell> gameBoard)
        {
            var playerName = manageScrableDb.GetAllPlayers().FirstOrDefault(p => p.PlayerId == playerId).PlayerName;
            PrintBoard(gameBoard);
            Console.WriteLine($"Round nr.{roundNUmber}, player's {playerName} turn and letters on hand: {LettersFromHand(GetPlayerHand(playerId, playerHands))}");
            Console.WriteLine("Enter the word to play");
            var wordToPlay = Console.ReadLine().ToArray();
            Console.WriteLine("Enter Horizontal position"); 
            int.TryParse(Console.ReadLine(), out int horizontalPosition); //reikia tikrinimo, kad butu tarp 1 ir 15
            Console.WriteLine("Enter Vertical position");
            int.TryParse(Console.ReadLine(), out int verticalPosition); //reikia tikrinimo, kad butu tarp 1 ir 15
            Console.WriteLine("Enter Word direction Right or Down");
            char direction = '0';
            while (true)
            {
                direction = (char)Console.ReadKey().Key;
                if (direction == (char)ConsoleKey.R)
                {
                    //kazkokia logika
                    return;
                }
                else if (direction == (char)ConsoleKey.D)
                {
                    //kazkokia logika
                    return;
                }
                else
                {
                    Console.WriteLine("Please enter r/R or d/D");
                }
            }
            CheckTheWord(wordToPlay, horizontalPosition, verticalPosition, direction, gameBoard);
        }

        public void CheckTheWord(char[] wordToPlay, int horizontalPosition, int verticalPosition, char direction, List<BoardCell> gameBoard)
        {
            if (wordToPlay.Length != 0)
            {
                //reikia logikos dėl tuščios plytelės (1as variantas patikrinama ar turi tokią ant rankos ir tada leidžia įvesti betkokią raidę 2o dar nesugalvojau :))
                if (direction.ToString().ToUpper() == "R")
                {
                    CheckTheBoard(direction, horizontalPosition, verticalPosition, gameBoard);
                }
            }
        }

        /*
        public List<BoardCell> PlaceTheWordOnBoard(char[] wordToPlay, int h, int v, char d, List<BoardCell> gameBoard)
        {
            if (d.ToString().ToUpper() == "R")
            {
                int i = 0;
                foreach (var cell in gameBoard.Where(v => v.VPosition == 1))
                {
                    if (cell.HPosition == v)
                    {
                    }
                    i += 1;
                }
            }
            for (int i = 0; i < wordToPlay.Length; i++)
            {

            }


        }
        */

        public void CheckTheBoard(char d, int h, int v, List<BoardCell> gameBoard)
        {
            List<BoardCell> actualBoardCells = new List<BoardCell>();
            List<char> charList = new List<char>();
            foreach (var cell in gameBoard)
            {
                if (cell.HPosition == h)
                {
                    actualBoardCells.Add(cell);
                }
            }
            foreach (var actualCell in actualBoardCells)
            {
                charList.Add(actualCell.LetterOnBoard);
            }

            /*
            foreach (var cell in gameBoard)
            {
                if (cell.HPosition == h & cell.VPosition == v)
                {
                    if (cell.LetterOnBoard == '!')
                    {
                        Console.WriteLine("It's forbidden to start word at this position");
                    }
                    else if (cell.LetterOnBoard == ' ')
                    {
                        Console.WriteLine("It's allowed to start here" ); //Temp u=ra6as
                    }
                }
            }
            */
        }
        public string LettersFromHand(List<Tile> hand)
        {
            List<char> lettersOnHand = new List<char>();
            foreach (var item in hand)
            {
                lettersOnHand.Add(item.Letter);
            }
            return $"{string.Join(' ', lettersOnHand)}";
        }

        public List<Tile> GenerateNewHand(List<Tile> bag) //OK
        {
            List<Tile> newHand = new List<Tile>();
            for (int i = 0; i < 7; i++)
            {
                newHand.Add(bag[i]);
            }
            return newHand;
        }

        public void PrintBoard(List<BoardCell> gameBoard)
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
                foreach (var cell in gameBoard.Where(h => h.HPosition == i))
                {
                    Console.Write("|");
                    //if celėje yra raidė turi spausdinti raidę, esle "specialValue"
                    Console.Write(cell.SpecialValue);
                }
                Console.WriteLine("|");
                Console.WriteLine("------------------------------------------------------------------");
            }
        } //Printing the board

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
        } //Checking how many players

        public Player PlayerName(int i, IManageScrabbleDb manageScrableDb)
        {
            Console.WriteLine();
            Console.Write($"Enter player {i + 1} name: ");//reikia padaryt kad įvestų bent vieną raidę
            string playerName = Console.ReadLine();
            if ((manageScrableDb.GetAllPlayers().Exists(p => p.PlayerName == playerName))) //Ar tikrai veiks, gal galima papraščiau?
            {
                Console.WriteLine($"Welcom {playerName} to the Scrabble again!");
                
            }
            else
            {
                manageScrableDb.InsertPlayer(playerName);
                Console.WriteLine($"Welcom {playerName} to the Scrabble!");
            }
            return (manageScrableDb.GetAllPlayers().FirstOrDefault(p => p.PlayerName == playerName));
        } //OK Entering the names of players

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
        } // OK Bag of shuffled tiles (all)
    }
}
