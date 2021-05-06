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
                //Console.WriteLine($"Player's {player.PlayerName} hand: {string.Join(' ', lettersOnHand)}");
            } //inicijuojam zaidejams pirmasias rankas
            List<BoardCell> gameBoard = new List<BoardCell>();
            foreach (var cell in BoardInitialData.DataSeed)
            {
                gameBoard.Add(new BoardCell(cell.BoardCellId, cell.HPosition, cell.VPosition, cell.SpecialValue, ' '));
            } //suformuojam lentą su tuščiomis reikšmėmis
            //PrintBoard(); //atsispausdinam pradine lentele
            while (true) //čia reikia užkodint kokokiomis salygomis baigiasi zaidimas
            {
                for (int i = 0; i < howManyPlayers; i++)
                {
                    var gameBoardAfterTurn = PlayersMove(roundNumber, playingPlayersId[i], manageScrableDb, playerHands, gameBoard, skipedMoves); //IDpriskyrimas
                    PrintBoard(gameBoardAfterTurn);
                }
                roundNumber += 1; 
            }
        }

        public List<BoardCell> PlayersMove(int roundNUmber, int playerId, IManageScrabbleDb manageScrableDb, List<PlayerHand> playerHands, List<BoardCell> gameBoard, int skipedMoves)
        {
            var playerName = manageScrableDb.GetAllPlayers().FirstOrDefault(p => p.PlayerId == playerId).PlayerName;
            var playersHand = GetPlayerHand(playerId, playerHands);
            Console.WriteLine($"Round nr.{roundNUmber}, player's {playerName} turn and letters on hand: {String.Join(' ',LettersFromHand(playersHand))}");
            string wordToPlay = null;
            int h = 0;
            int v = 0;
            char d = ' ';
            bool correctPositioning = false;
            bool possibleToCreate = false;
            //bool correctCrossing = false;
            while (correctPositioning is false || possibleToCreate is false)//&& correctCrossing is false)
            {
                Console.WriteLine("Enter the word to play");
                wordToPlay = Console.ReadLine();
                Console.WriteLine("Enter Horizontal position");
                int.TryParse(Console.ReadLine(), out h); //reikia tikrinimo, kad butu tarp 1 ir 15
                Console.WriteLine("Enter Vertical position");
                int.TryParse(Console.ReadLine(), out v); //reikia tikrinimo, kad butu tarp 1 ir 15
                d = Direction();
                var chArrayOfWordToPlay = wordToPlay.ToArray();
                correctPositioning = CheckTheWord(chArrayOfWordToPlay, h, v, d, gameBoard, out skipedMoves);
                possibleToCreate = CheckTheTryToCreateTheWord(wordToPlay.ToArray(), h, v, d, gameBoard, playersHand);
                //correctCrossing = CheckTheWordCrossing(wordToPlay.ToArray(), h, v, d, gameBoard, playersHand);
            }            
            return PlaceTheWordOnBoard(wordToPlay.ToArray(), h, v, d, gameBoard, playersHand);
        }

        public bool CheckTheWordCrossing(char[] wordToPlay, int h, int v, char d, List<BoardCell> gameBoard, List<Tile> playersHand)
        {
            if (d.ToString().ToUpper() == "R")
            {
                int warning = 0;
                int i = 0;

                int tempH = wordToPlay.Length + h;
                foreach (var upperCell in gameBoard.Where(vp => vp.HPosition == v)) 
                {
                    if (upperCell.LetterOnBoard != ' ' && i < tempH)
                    {
                        warning += 1;
                    }
                    else
                    {
                        warning = 0;
                    }
                    i += 1;
                }
                int j = 0;
                foreach (var upperCell in gameBoard.Where(vp => vp.HPosition == v + 1)) //tikrinam ar nesusiliecia daugiau nei 1 raide su zodziu apacioje
                {
                    if (upperCell.LetterOnBoard != ' ' && j < tempH)
                    {
                        warning += 1;
                    }
                    else
                    {
                        warning = 0;
                    }
                    j += 1;
                }
                if (warning > 1)
                {
                    Console.WriteLine("It's forbiden to place the word here");
                    return false;
                }
            }
            return false;
        } //kolkas atstavit, einam tiesiai prie zodzio patalpinimo

        public char Direction()
        {
            Console.WriteLine("Enter Word direction Right or Down");
            char d;
            while (true)
            {
                d = (char)Console.ReadKey().Key;
                if (d == (char)ConsoleKey.R)
                {
                    d = 'R';
                    return d;
                }
                else if (d == (char)ConsoleKey.D)
                {
                    d = 'D';
                    return d;
                }
                else
                {
                    Console.WriteLine("Please enter r/R or d/D");
                }
            }
        }

        public List<Tile> GetPlayerHand(int playerId, List<PlayerHand> playerHands)
        {
            List<Tile> hand = new List<Tile>();
            hand = playerHands.FirstOrDefault(p => p.PlayerID == playerId).Hand.ToList();
            return hand;
        }

        public bool CheckTheWord(char[] wordToPlay, int horizontalPosition, int verticalPosition, char direction, List<BoardCell> gameBoard, out int skipedMoves)
        {
            if (wordToPlay.Length != 0)
            {
                //reikia logikos dėl tuščios plytelės (1as variantas patikrinama ar turi tokią ant rankos ir tada leidžia įvesti betkokią raidę 2o dar nesugalvojau :))
                skipedMoves = 0;
                return CheckTheBoardNeighbours(wordToPlay, direction, horizontalPosition, verticalPosition, gameBoard);
            }
            else
            {
                skipedMoves = 1; //reikia padidinimo 
                return true;
            } 
        }

        public bool CheckTheTryToCreateTheWord(char[] wordToPlay, int h, int v, char d, List<BoardCell> gameBoard, List<Tile> playersHand)
        {
            if (d.ToString().ToUpper() == "R")
            {
                List<char> possibilityList = new List<char>();
                int i = 0;
                int j = 0;
                foreach (var cell in gameBoard.Where(vp => vp.HPosition == v)) //sumaišyta vietomis
                {
                    if ((cell.VPosition == h + i) && (i < wordToPlay.Length) && (cell.LetterOnBoard != ' '))
                    {
                        possibilityList.Add(cell.LetterOnBoard);
                        j += 1;                        
                    }
                    i += 1;
                }
                foreach (var letter in LettersFromHand(playersHand))
                {
                    possibilityList.Add(letter);
                }
                Console.WriteLine($"zaidejo galimybes nuo šios pozicijos: {string.Join('|',possibilityList)}");
                foreach (var letter in wordToPlay)
                {
                    if (!possibilityList.Contains(letter))
                    {
                        Console.WriteLine("It's impossible to create such word at this starting position with such board composition");
                        return false;
                    }
                    else
                    {
                        possibilityList.Remove(letter);
                    }
                }
            }
            else
            {
                List<char> possibilityList = new List<char>();
                int i = 0;
                int j = 0;
                foreach (var cell in gameBoard.Where(hp => hp.VPosition == h)) //sumaišyta vietomis
                {
                    if ((cell.HPosition == v + i) && (i < wordToPlay.Length) && (cell.LetterOnBoard != ' '))
                    {
                        possibilityList.Add(cell.LetterOnBoard);
                        j += 1;
                    }
                    i += 1;
                }
                foreach (var letter in LettersFromHand(playersHand))
                {
                    possibilityList.Add(letter);
                }
                Console.WriteLine($"zaidejo galimybes nuo šios pozicijos: {string.Join('|', possibilityList)}");
                foreach (var letter in wordToPlay)
                {
                    if (!possibilityList.Contains(letter))
                    {
                        Console.WriteLine("It's impossible to create such word at this starting position with such board composition");
                        return false;
                    }
                    else
                    {
                        possibilityList.Remove(letter);
                    }
                }
            }
            return true;
        } //OK bet sumaišyta h su v vietomis 

        public List<BoardCell> PlaceTheWordOnBoard(char[] wordToPlay, int h, int v, char d, List<BoardCell> gameBoard, List<Tile> playersHand)
        {
            if (d.ToString().ToUpper() == "R")
            {
                int i = 0;
                foreach (var cell in gameBoard.Where(vp => vp.HPosition == v)) //sumaišyta vietomis
                {
                    if ((cell.VPosition == h+i) && (i < wordToPlay.Length))
                    {
                        cell.LetterOnBoard = wordToPlay[i];
                        i += 1;
                    }                    
                }
            }
            else
            {
                int i = 0;
                foreach (var cell in gameBoard.Where(hp => hp.VPosition == h)) //sumaišyta vietomis
                {
                    if ((cell.HPosition == v + i) && (i < wordToPlay.Length))
                    {
                        cell.LetterOnBoard = wordToPlay[i];
                        i += 1;
                    }
                }
            }
            return gameBoard;
        } //OK bet sumaišyta h su v vietomis       

        public bool CheckTheBoardNeighbours(char[] wordToPlay, char d, int h, int v, List<BoardCell> gameBoard)
        {            
            if (d.ToString().ToUpper() == "R")
            {
                if ((wordToPlay.Length + h) > 15)
                {
                    Console.WriteLine("It's not enough cells to place the word at this starting position");
                    return false;
                }                
                int warning = 0;
                int i = 0;
                int tempH = wordToPlay.Length + h;
                foreach (var upperCell in gameBoard.Where(vp => vp.HPosition == v - 1)) //tikrinam ar nesusiliecia daugiau nei 1 raide su zodziu virsuje
                {
                    if (upperCell.LetterOnBoard != ' ' && i < tempH)
                    {
                        warning += 1;
                        if (warning > 1)
                        {
                            Console.WriteLine("It's forbiden to place the word here");
                            return false;
                        }
                    }
                    else
                    {
                        warning = 0;
                    }
                    i += 1;
                }
                int j = 0;
                foreach (var upperCell in gameBoard.Where(vp => vp.HPosition == v + 1)) //tikrinam ar nesusiliecia daugiau nei 1 raide su zodziu apacioje
                {
                    if (upperCell.LetterOnBoard != ' ' && j < tempH)
                    {
                        warning += 1;
                        if (warning > 1)
                        {
                            Console.WriteLine("It's forbiden to place the word here");
                            return false;
                        }
                    }
                    else
                    {
                        warning = 0;
                    }
                    j += 1;
                }
                
            }
            else
            {
                if ((wordToPlay.Length + v) > 15)
                {
                    Console.WriteLine("It's not enough cells to place the word in this starting position");
                    return false;
                }
                int warning = 0;
                int i = 0;
                int tempV = wordToPlay.Length + v;
                foreach (var upperCell in gameBoard.Where(hp => hp.VPosition == h - 1)) //tikrinam ar nesusiliecia daugiau nei 1 raide su zodziu kaireje
                {
                    if (upperCell.LetterOnBoard != ' ' && i < tempV)
                    {
                        warning += 1;
                        if (warning > 1)
                        {
                            Console.WriteLine("It's forbiden to place the word here");
                            return false;
                        }
                    }
                    else
                    {
                        warning = 0;
                    }
                    i += 1;
                }
                int j = 0;
                foreach (var upperCell in gameBoard.Where(hp => hp.VPosition == h + 1)) //tikrinam ar nesusiliecia daugiau nei 1 raide su zodziu kaireje
                {
                    if (upperCell.LetterOnBoard != ' ' && j < tempV)
                    {
                        warning += 1;
                        if (warning > 1)
                        {
                            Console.WriteLine("It's forbiden to place the word here");
                            return false;
                        }
                    }
                    else
                    {
                        warning = 0;
                    }
                    j += 1;
                }                
            }
            return true;
        }//OK

        public List<char> LettersFromHand(List<Tile> hand)
        {
            List<char> lettersOnHand = new List<char>();
            foreach (var item in hand)
            {
                lettersOnHand.Add(item.Letter);
            }
            return lettersOnHand;
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
                    Console.Write(cell.LetterOnBoard);
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
