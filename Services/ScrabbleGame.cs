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
            int roundNumber = 1; //! 
            int skipedMoves = 0; //!
            var howManyPlayers = HowManyPlayers(); //gaunam kiek zaideju
            var players = PlayersNames(howManyPlayers, manageScrableDb); //gaunam zaideju vardus
            List<Tile> bag = ShuffleBag(); //gaunam pradini sumaisyta maiseli
            List<PlayerHand> playersHands = new List<PlayerHand>();
            List<int> playingPlayersId = new List<int>(); //žaidžiančių žaidėjų ID
            foreach (var player in players) 
            {                
                var generatedNewPlayerHand = GenerateNewHand(bag);
                playersHands.Add(new PlayerHand(player.PlayerId, generatedNewPlayerHand));
                bag.RemoveRange(0, 7);
                playingPlayersId.Add(player.PlayerId);
                List<char> lettersOnHand = new List<char>(); //patikrinimui
                foreach (var item in generatedNewPlayerHand) //patikrinimui
                {
                    lettersOnHand.Add(item.Letter);
                }
                Console.WriteLine($"Player's {player.PlayerName} hand: {string.Join(' ', lettersOnHand)}"); //patikrinimui
            } //inicijuojam zaidejams pirmasias rankas
            List<BoardCell> gameBoard = new List<BoardCell>();
            List<Tile> listOfUniqueTiles = new List<Tile>();  
            foreach (var cell in BoardInitialData.DataSeed)
            {
                gameBoard.Add(new BoardCell(cell.BoardCellId, cell.HPosition, cell.VPosition, cell.SpecialValue, ' '));
            } //suformuojam lentą su tuščiomis reikšmėmis
            foreach (var tile in TileInitialData.DataSeed)
            {
                listOfUniqueTiles.Add(new Tile(tile.TileId, tile.Letter, tile.Value));
            }
            //PrintBoard(); //atsispausdinam pradine lentele
            while (true) //čia reikia užkodint kokokiomis salygomis baigiasi zaidimas
            {
                for (int i = 0; i < howManyPlayers; i++)
                {
                    var gameBoardAfterTurn = PlayersMove(roundNumber, playingPlayersId[i], manageScrableDb, playersHands, gameBoard, skipedMoves, roundNumber, bag, listOfUniqueTiles); //IDpriskyrimas
                    PrintBoard(gameBoardAfterTurn);
                }
                roundNumber += 1; 
            }
        } 

        public List<Tile> AddToHand(List<Tile> bag, int usedLetters, List<Tile> leftOnHand)
        {
            List<Tile> newHand = new List<Tile>();
            if (bag.Count < usedLetters)
            {
                foreach (var item in bag)
                {
                    newHand.Add(item);                    
                }
            }
            else
            {
                for (int i = 0; i < usedLetters; i++)
                {
                    newHand.Add(bag[i]);
                }
            }
            foreach (var item in leftOnHand) //pridedam kas liko po suzaidimo
            {
                newHand.Add(item);
            }
            return newHand;
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
        }  // OK How many players

        public List<Player> PlayersNames(int howManyPlayers, IManageScrabbleDb manageScrableDb)
        {
            List<Player> players = new List<Player>();
            for (int i = 0; i < howManyPlayers; i++)
            {
                players.Add(PlayerName(i, manageScrableDb));
            }
            return players;
        } //OK getting players names

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

        public List<BoardCell> PlayersMove(int roundNUmber, int playerId, IManageScrabbleDb manageScrableDb, List<PlayerHand> playersHands, List<BoardCell> gameBoard, int skipedMoves, int roundNumber, List<Tile> bag, List<Tile> listOfUniqueTiles)
        {
            var playerName = manageScrableDb.GetAllPlayers().FirstOrDefault(p => p.PlayerId == playerId).PlayerName;
            var playerHand = GetPlayerHand(playerId, playersHands);
            Console.WriteLine($"Round nr.{roundNUmber}, player's {playerName} turn and letters on hand: {String.Join(' ',LettersFromHand(playerHand))}");
            string wordToPlay = null;
            int h = 0;
            int v = 0;
            char d = ' ';
            int usedLetters = 0;
            bool correctPositioning = false;
            bool possibleToCreate = false;
            List<Tile> helpingLettersOnBoard = new List<Tile>();
            List<Tile> tilesOfWordToPlay = new List<Tile>();
            while (correctPositioning is false || possibleToCreate is false)
            {
                Console.WriteLine("Enter the word to play");
                wordToPlay = Console.ReadLine().ToUpper();
                Console.WriteLine("Enter Horizontal position");
                int.TryParse(Console.ReadLine(), out h); //reikia tikrinimo, kad butu tarp 1 ir 15
                Console.WriteLine("Enter Vertical position");
                int.TryParse(Console.ReadLine(), out v); //reikia tikrinimo, kad butu tarp 1 ir 15
                d = Direction();
                var chArrayOfWordToPlay = wordToPlay.ToArray();
                correctPositioning = CheckTheWord(chArrayOfWordToPlay, h, v, d, gameBoard, out skipedMoves);
                helpingLettersOnBoard = CheckIsItPossibleToCreateTheWord(wordToPlay.ToArray(), h, v, d, gameBoard, playerHand, roundNumber, listOfUniqueTiles, out usedLetters); //patikrina ir grazina kokios buvo susikryziuojancios raides
                if (helpingLettersOnBoard.Count > 0 || roundNumber == 1) //cia gali buti konfiuzas
                {
                    possibleToCreate = true;
                }
                else
                {
                    possibleToCreate = false;
                }                
            }
            foreach (var playingLetter in wordToPlay)
            {
                tilesOfWordToPlay.Add(listOfUniqueTiles.FirstOrDefault(l => l.Letter == playingLetter));
            }
            var leftTilesFromHand = LeftOnHand(playerHand, helpingLettersOnBoard, tilesOfWordToPlay);
            playerHand = AddToHand(bag, usedLetters, leftTilesFromHand);
            foreach (var player in playersHands)
            {
                if(player.PlayerID == playerId)
                {
                    player.Hand = playerHand;
                }
            }
            return PlaceTheWordOnBoard(wordToPlay.ToArray(), h, v, d, gameBoard, playerHand);
        }

        public List<Tile> LeftOnHand(List<Tile> playerHand, List<Tile> helpingLettersOnBoard, List<Tile> tilesOfWordToPlay)
        {
            List<Tile> leftOnHand = new List<Tile>();
            Tile tempas; 
            leftOnHand = playerHand;
            foreach (var item in leftOnHand)
            {
                Console.Write("Po pradinio priskyrimo");
                Console.Write($"{item.Letter} ");
                Console.WriteLine();
            }
            foreach (var letterOnBoard in helpingLettersOnBoard)
            {
                leftOnHand.Add(letterOnBoard);
            }

            foreach (var item in leftOnHand)
            {
                Console.Write("Po helpuku priskyrimo");
                Console.Write($"{item.Letter} ");
                Console.WriteLine();
            }

            foreach (var playedTile in tilesOfWordToPlay)
            {
                tempas = leftOnHand.FirstOrDefault(i => i.TileId == playedTile.TileId);
                leftOnHand.Remove(tempas);
            }

            foreach (var item in leftOnHand)
            {
                Console.Write("Po removinimo");
                Console.Write($"{item.Letter} ");
                Console.WriteLine();
            }

            return leftOnHand;
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
            var hand = playerHands.FirstOrDefault(p => p.PlayerID == playerId).Hand.ToList();
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

        public List<Tile> CheckIsItPossibleToCreateTheWord(char[] wordToPlay, int h, int v, char d, List<BoardCell> gameBoard, List<Tile> playerHand, int roundNumber, List<Tile> listOfUniqueTiles, out int usedLetters)
        {
            List<Tile> playedTiles = new List<Tile>();
            List<Tile> possibilityList = new List<Tile>();
            List<Tile> helpingLettersOnBoard = new List<Tile>();
            int numberOfLettersHelpingHand = 0;
            List<Tile> tilesOfWordToPlay = new List<Tile>();
            if (d.ToString().ToUpper() == "R") //right
            {
                int i = 0;
                int j = 0;
                foreach (var cell in gameBoard.Where(vp => vp.HPosition == v)) //sumaišyta vietomis
                {
                    if ((cell.VPosition == h + i) && (i < wordToPlay.Length) && (cell.LetterOnBoard != ' ')) //tikrinam ar ten kur prasideda zodis yra  bent viena beskertanti raide su norimu patalpnti zodziu
                    {
                        numberOfLettersHelpingHand += 1;
                        helpingLettersOnBoard.Add(listOfUniqueTiles.FirstOrDefault(l => l.Letter == cell.LetterOnBoard));
                        possibilityList.Add(listOfUniqueTiles.FirstOrDefault(l => l.Letter == cell.LetterOnBoard)); //rasta raide prisidedam prie galimybiu listo
                        j += 1;                        
                    }
                    i += 1;
                }
                foreach (var letter in LettersFromHand(playerHand))
                {
                    possibilityList.Add(listOfUniqueTiles.FirstOrDefault(l => l.Letter == letter)); //pridedam prie listo zaidejo turimas raides
                }                
                if (possibilityList.Count == playerHand.Count && roundNumber > 1)
                {
                    Console.WriteLine("It's impossible to create such word, because it's not crossing other word on board");
                    usedLetters = 0;
                    return helpingLettersOnBoard;
                }
                
                foreach (var playingLetter in wordToPlay)
                {
                    tilesOfWordToPlay.Add(listOfUniqueTiles.FirstOrDefault(l => l.Letter == playingLetter));
                }
                foreach (var tile in tilesOfWordToPlay)
                {
                    if (possibilityList.Contains(tile))
                    {
                        possibilityList.Remove(tile);
                    }
                    else
                    {
                        Console.WriteLine("It's impossible to create such word with player's hand and board composition at this starting position");
                        usedLetters = 0;
                        return playedTiles; //netikslus
                    }
                }
            }
            else //down
            {
                int i = 0;
                int j = 0;
                foreach (var cell in gameBoard.Where(hp => hp.VPosition == h)) //sumaišyta vietomis
                {
                    if ((cell.HPosition == v + i) && (i < wordToPlay.Length) && (cell.LetterOnBoard != ' '))
                    {
                        numberOfLettersHelpingHand += 1;
                        helpingLettersOnBoard.Add(listOfUniqueTiles.FirstOrDefault(l => l.Letter == cell.LetterOnBoard));
                        possibilityList.Add(listOfUniqueTiles.FirstOrDefault(l => l.Letter == cell.LetterOnBoard));
                        j += 1;
                    }
                    i += 1;
                }
                foreach (var letter in LettersFromHand(playerHand))
                {
                    possibilityList.Add(listOfUniqueTiles.FirstOrDefault(l => l.Letter == letter));
                }
                if (possibilityList.Count == playerHand.Count && roundNumber > 1)
                {
                    Console.WriteLine("It's impossible to create such word, because it's not crossing other word on board");
                    usedLetters = 0;
                    return helpingLettersOnBoard;
                }

                foreach (var playingLetter in wordToPlay)
                {
                    tilesOfWordToPlay.Add(listOfUniqueTiles.FirstOrDefault(l => l.Letter == playingLetter));
                }
                foreach (var tile in tilesOfWordToPlay)
                {
                    if (possibilityList.Contains(tile))
                    {
                        possibilityList.Remove(tile);
                    }
                    else
                    {
                        Console.WriteLine("It's impossible to create such word with player's hand and board composition at this starting position");
                        usedLetters = 0;
                        return playedTiles; //netikslus
                    }
                }
            }
            usedLetters = wordToPlay.Length - numberOfLettersHelpingHand;

            return helpingLettersOnBoard;
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
    }
}
