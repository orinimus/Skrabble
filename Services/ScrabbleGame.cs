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
        static readonly List<int> skipedMoves = new List<int>();

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
            int roundNumber = 1;
            int gameId = manageScrableDb.GetLastGameId();
            var howManyPlayers = HowManyPlayers(); //getting the number of players
            var players = PlayersNames(howManyPlayers, manageScrableDb); //getting the names of players
            List<Tile> bag = ShuffleBag(); //getting full shuffled bag with tiles  
            List<PlayerHand> playersHands = new List<PlayerHand>();
            List<int> playingPlayersId = new List<int>(); // the list for playing players
            foreach (var player in players) 
            {                
                var generatedNewPlayerHand = GenerateNewHand(bag);
                playersHands.Add(new PlayerHand(player.PlayerId, generatedNewPlayerHand, 0));
                bag.RemoveRange(0, 7);
                playingPlayersId.Add(player.PlayerId);
            } //initiation hands and the starting points for players
            List<BoardCell> gameBoard = new List<BoardCell>();            
            foreach (var cell in BoardInitialData.DataSeed)
            {
                gameBoard.Add(new BoardCell(cell.BoardCellId, cell.HPosition, cell.VPosition, cell.SpecialValue, '.'));
            } //initiation of the board //'.'
            List<Tile> listOfUniqueTiles = new List<Tile>();
            foreach (var tile in TileInitialData.DataSeed)  
            {
                listOfUniqueTiles.Add(new Tile(tile.TileId, tile.Letter, tile.Value));
            } //initiation of all possible tiles and their values
            PrintBoard(gameBoard, listOfUniqueTiles);
            while (skipedMoves.Count < 2) //the case when the game is over
            {
                for (int i = 0; i < howManyPlayers; i++)
                {
                    var gameBoardAfterTurn = PlayersMove(roundNumber, playingPlayersId[i], manageScrableDb, playersHands, gameBoard, roundNumber, bag, listOfUniqueTiles, i, gameId); //IDpriskyrimas
                    PrintBoard(gameBoardAfterTurn, listOfUniqueTiles);
                }
                roundNumber += 1; 
            }

            int maxPoints = playersHands.Max(x => x.Points);
            var winner = playersHands.FirstOrDefault(y => y.Points == maxPoints).PlayerID;
            Console.WriteLine($"The winner of this game is {players.FirstOrDefault(p => p.PlayerId == winner)}");

            manageScrableDb.InsertGame(players, DateTime.Now);
        } 
        
        public List<Tile> GenerateNewHand(List<Tile> bag) //OK generating new hands in the begining of game 
        {
            List<Tile> newHand = new List<Tile>();
            for (int i = 0; i < 7; i++)
            {
                newHand.Add(bag[i]);
            }
            return newHand;
        } 

        public int HowManyPlayers() // OK How many players
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

        public List<Player> PlayersNames(int howManyPlayers, IManageScrabbleDb manageScrableDb) //OK getting players names
        {
            List<Player> players = new List<Player>();
            for (int i = 0; i < howManyPlayers; i++)
            {
                players.Add(PlayerName(i, manageScrableDb));
            }
            return players;
        } 

        public List<Tile> ShuffleBag() // OK Bag of shuffled tiles (all) 
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

        public List<BoardCell> PlayersMove(int roundNUmber, int playerId, IManageScrabbleDb manageScrableDb, List<PlayerHand> playersHands, List<BoardCell> gameBoard, int roundNumber, List<Tile> bag, List<Tile> listOfUniqueTiles, int playerNumber, int gameId)
        {
            var playerName = manageScrableDb.GetAllPlayers().FirstOrDefault(p => p.PlayerId == playerId).PlayerName;
            var playerHand = GetPlayerHand(playerId, playersHands);
            Console.WriteLine($"Round nr.{roundNUmber}, player's {playerName} turn and letters on hand: {String.Join(' ',LettersFromHand(playerHand))}");
            string wordToPlay = null;
            int horizontalPosition = 0;
            int verticalPosition = 0;
            char direction = ' ';
            var points = playersHands.FirstOrDefault(p => p.PlayerID == playerId).Points;
            int turnPoints = 0;            
            bool correctPositioning = false;
            bool possibleToCreate = false;
            List<Tile> playedTiles = new List<Tile>();
            while (correctPositioning is false || possibleToCreate is false)
            {
                Console.WriteLine("Enter the word to play");
                wordToPlay = Console.ReadLine().ToUpper();
                if (wordToPlay.Length == 0)
                {
                    skipedMoves.Add(1);
                    return gameBoard;
                }
                else
                {
                    skipedMoves.Clear();
                }
                Console.WriteLine("Enter Horizontal position");
                int.TryParse(Console.ReadLine(), out horizontalPosition);
                Console.WriteLine("Enter Vertical position");
                int.TryParse(Console.ReadLine(), out verticalPosition);
                direction = Direction();
                var chArrayOfWordToPlay = wordToPlay.ToArray();
                correctPositioning = CheckTheWord(chArrayOfWordToPlay, horizontalPosition, verticalPosition, direction, gameBoard); //checking if it's possible to corectly place the word on board
                playedTiles = CheckIsItPossibleToCreateTheWord(wordToPlay.ToArray(), horizontalPosition, verticalPosition, direction, gameBoard, playerHand, roundNumber, listOfUniqueTiles); // cheking if there was any and returning it if the word was crossing other words on board
                if (playedTiles.Count > 0 || (roundNumber == 1 && playerNumber == 0)) //if it's first word on board it's not need to cross other word
                {
                    possibleToCreate = true;
                }
                else
                {
                    possibleToCreate = false;
                }
            }

            turnPoints = PointsOfPlacedWord(playedTiles, gameBoard, listOfUniqueTiles, wordToPlay.ToArray(), horizontalPosition, verticalPosition, direction); //points after turn
            Console.WriteLine($"Player {playerName} received {turnPoints} points");
            points += turnPoints;
            Console.WriteLine($"Player {playerName} has total {points} points");
            playersHands.FirstOrDefault(p => p.PlayerID == playerId).Points = points;
            Console.WriteLine();
            manageScrableDb.InsertStatistic(playerId, gameId, playerHand, wordToPlay, horizontalPosition, verticalPosition, direction); //inserting statistic to DB

            var newPlayerHand = AddToHand(bag, playedTiles, playerHand);

            foreach (var player in playersHands)
            {
                if(player.PlayerID == playerId)
                {
                    player.Hand = newPlayerHand;
                }
            }

            return PlaceTheWordOnBoard(wordToPlay.ToArray(), horizontalPosition, verticalPosition, direction, gameBoard);
        }

        public int PointsOfPlacedWord(List<Tile> playedTiles, List<BoardCell> gameBoard, List<Tile> listOfUniqueTiles, char[] wordToPlay, int horizontalPosition, int verticalPosition, char direction)
        {
            int turnPoints = 0;
            int multiplyer = 1;
            int i = 0;
            if (direction.ToString().ToUpper() == "R") //right
            {
                foreach (var cell in gameBoard.Where(vp => vp.HPosition == verticalPosition).Skip(horizontalPosition - 1).Take(wordToPlay.Length))
                {           
                    if (cell.SpecialValue == "3xW")
                    {
                        multiplyer *= 3;
                        turnPoints += listOfUniqueTiles.FirstOrDefault(l => l.Letter == wordToPlay[i]).Value;
                        cell.SpecialValue = "   ";
                    }
                    else if (cell.SpecialValue == "2xW")
                    {
                        multiplyer *= 2;
                        turnPoints += listOfUniqueTiles.FirstOrDefault(l => l.Letter == wordToPlay[i]).Value;
                        cell.SpecialValue = "   ";
                    }
                    else if (cell.SpecialValue == "2xL")
                    {
                        turnPoints += (listOfUniqueTiles.FirstOrDefault(l => l.Letter == wordToPlay[i]).Value) * 2;
                        cell.SpecialValue = "   ";
                    }
                    else if (cell.SpecialValue == "3xL")
                    {
                        turnPoints += (listOfUniqueTiles.FirstOrDefault(l => l.Letter == wordToPlay[i]).Value) * 3;
                        cell.SpecialValue = "   ";
                    }
                    else 
                    {
                        turnPoints += listOfUniqueTiles.FirstOrDefault(l => l.Letter == wordToPlay[i]).Value;
                    }
                    i += 1;
                }                
            }
            else
            {
                foreach (var cell in gameBoard.Where(hp => hp.VPosition == horizontalPosition).Skip(verticalPosition - 1).Take(wordToPlay.Length))
                {                    
                    if (cell.SpecialValue == "3xW")
                    {
                        multiplyer *= 3;
                        cell.SpecialValue = "   ";
                    }
                    else if (cell.SpecialValue == "2xW")
                    {
                        multiplyer *= 2;
                        cell.SpecialValue = "   ";
                    }
                    else if (cell.SpecialValue == "2xL")
                    {
                        turnPoints += listOfUniqueTiles.FirstOrDefault(l => l.Letter == wordToPlay[i]).Value * 2;
                        cell.SpecialValue = "   ";
                    }
                    else if (cell.SpecialValue == "3xL")
                    {
                        turnPoints += listOfUniqueTiles.FirstOrDefault(l => l.Letter == wordToPlay[i]).Value * 3;
                        cell.SpecialValue = "   ";
                    }
                    else
                    {
                        turnPoints += listOfUniqueTiles.FirstOrDefault(l => l.Letter == wordToPlay[i]).Value;
                    }
                    i += 1;
                }
            }

            turnPoints *= multiplyer;
            return turnPoints;
        }

        public List<Tile> TilesOfWordToPlay(char[] wordToPlay, List<Tile> listOfUniqueTiles)
        {
            List<Tile> tilesOfWordToPlay = new List<Tile>();
            foreach (var letterOfWord in wordToPlay)
            {
                tilesOfWordToPlay.Add(listOfUniqueTiles.FirstOrDefault(l => l.Letter == letterOfWord));
            }
            return tilesOfWordToPlay;
        } //word conversion to Tile list

        public List<Tile> AddToHand(List<Tile> bag, List<Tile> playedTiles, List<Tile> playerHand) //Adding Tiles from bag after word was played
        {
            var newHand = playerHand;

            foreach (var tile in playedTiles) //remove from hand the played tiles 
            {
                newHand.Remove(playerHand.FirstOrDefault(x => x.Letter == tile.Letter));
            }

            if (bag.Count < playedTiles.Count) //if bag is almost empty
            {
                foreach (var item in bag)
                {
                    newHand.Add(item);                    
                }
            }
            else
            {
                for (int i = 0; i < playedTiles.Count; i++) //adding new Tiles from bag
                {
                    newHand.Add(bag[i]);
                }
            }
            return newHand;
        }

        public List<Tile> LeftOnHand(List<Tile> playerHand, List<Tile> helpingLettersOnBoard, List<Tile> tilesOfWordToPlay)
        {
            var leftOnHand = playerHand;
            foreach (var letterOnBoard in helpingLettersOnBoard)
            {
                leftOnHand.Add(letterOnBoard);
            }
            foreach (var playedTile in tilesOfWordToPlay)
            {
                leftOnHand.Remove(leftOnHand.FirstOrDefault(i => i.Letter == playedTile.Letter));
            }
            return leftOnHand;
        }

        public char Direction() //OK for r/d entry 
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

        public bool CheckTheWord(char[] wordToPlay, int horizontalPosition, int verticalPosition, char direction, List<BoardCell> gameBoard) //if the word is empty it meens skiping the turn
        {
            if (wordToPlay.Length != 0)
            {
                //skipedMoves.Clear();
                return CheckTheBoardNeighbours(wordToPlay, direction, horizontalPosition, verticalPosition, gameBoard);
            }
            else
            {
                //skipedMoves.Add(1); 
                return true;
            } 
        } //reikia padidinimo

        public List<Tile> CheckIsItPossibleToCreateTheWord(char[] wordToPlay, int horizontalPosition, int verticalPosition, char direction, List<BoardCell> gameBoard, List<Tile> playerHand, int roundNumber, List<Tile> listOfUniqueTiles)
        {
            List<Tile> playedTiles = new List<Tile>();
            List<Tile> possibilityList = new List<Tile>();
            List<Tile> helpingLettersOnBoard = new List<Tile>();
            var tilesOfWordToPlay = TilesOfWordToPlay(wordToPlay, listOfUniqueTiles);

            if (direction.ToString().ToUpper() == "R") //right
            {
                foreach (var cell in gameBoard.Where(vp => vp.HPosition == verticalPosition).Skip(horizontalPosition - 1).Take(wordToPlay.Length))
                {
                    if(cell.LetterOnBoard != '.')
                    {
                        helpingLettersOnBoard.Add(listOfUniqueTiles.FirstOrDefault(l => l.Letter == cell.LetterOnBoard)); //the crossing letter 
                        possibilityList.Add(listOfUniqueTiles.FirstOrDefault(l => l.Letter == cell.LetterOnBoard)); //filling the list of all letters from hand and wich are crossing
                    }
                }
                foreach (var letter in LettersFromHand(playerHand)) // filling the list of all letters from hand and which are crossing
                {
                    possibilityList.Add(listOfUniqueTiles.FirstOrDefault(l => l.Letter == letter));                    
                }

                if (possibilityList.Count == playerHand.Count && roundNumber > 1) // possiblilityList must be longer than hand, that meens that it's crossing other word and meeting the main rule of the Game
                {
                    Console.WriteLine("It's impossible to create such word, because it's not crossing other word on board");
                    return playedTiles;
                }               

                foreach (var tile in tilesOfWordToPlay) //checking if the player with his letters on hand and with crossing letters can create the word
                {
                    if (possibilityList.Contains(tile))
                    {
                        possibilityList.Remove(possibilityList.FirstOrDefault(i => i.Letter == tile.Letter));                    
                    }
                    else
                    {
                        Console.WriteLine("It's impossible to create such word with player's hand and board composition at this starting position");
                        return playedTiles;  //returning empty list to confirm that it'was impossible to play the word
                    }
                }
            }
            else //down
            {
                foreach (var cell in gameBoard.Where(hp => hp.VPosition == horizontalPosition).Skip(verticalPosition - 1).Take(wordToPlay.Length))
                {
                    if (cell.LetterOnBoard != '.')
                    {
                        helpingLettersOnBoard.Add(listOfUniqueTiles.FirstOrDefault(l => l.Letter == cell.LetterOnBoard)); //the crossing letter 
                        possibilityList.Add(listOfUniqueTiles.FirstOrDefault(l => l.Letter == cell.LetterOnBoard)); //filling the list of all letters from hand and wich are crossing
                    }
                }

                foreach (var letter in LettersFromHand(playerHand)) // filling the list of all letters from hand and which are crossing
                {
                    possibilityList.Add(listOfUniqueTiles.FirstOrDefault(l => l.Letter == letter)); 
                }

                if (possibilityList.Count == playerHand.Count && roundNumber > 1)  // possiblilityList must be longer than hand, that meens that it's crossing other word and meeting the main rule of the Game
                {
                    Console.WriteLine("It's impossible to create such word, because it's not crossing other word on board");
                    return playedTiles;
                }
                foreach (var tile in tilesOfWordToPlay) //checking if the player with his letters on hand and with crossing letters can create the word
                {
                    if (possibilityList.Contains(tile))
                    {
                        possibilityList.Remove(tile);
                    }
                    else
                    {
                        Console.WriteLine("It's impossible to create such word with player's hand and board composition at this starting position");
                        return playedTiles; //returning empty list to confirm that it'was impossible to play the word
                    }
                }
            }
            playedTiles = PlayedTilesWitoutHelpingTiles(tilesOfWordToPlay, helpingLettersOnBoard);
            return playedTiles; //returning the list of played Tiles from hand
            
        } //OK patikrina ar susikerta ir grazina su kokiomis raidemis

        public List<Tile> PlayedTilesWitoutHelpingTiles(List<Tile> tilesOfWordToPlay, List<Tile> helpingLettersOnBoard)
        {
            var playedTiles = tilesOfWordToPlay;
            foreach (var tile in helpingLettersOnBoard)
            {
                playedTiles.Remove(playedTiles.FirstOrDefault(t => t.Letter == tile.Letter));
            }
            return playedTiles;
        }

        public List<BoardCell> PlaceTheWordOnBoard(char[] wordToPlay, int horizontalPosition, int verticalPosition, char direction, List<BoardCell> gameBoard) //OK placing word in gameBoard 
        {
            if (direction.ToString().ToUpper() == "R")
            {
                int i = 0;
                foreach (var cell in gameBoard.Where(vp => vp.HPosition == verticalPosition)) 
                {
                    if ((cell.VPosition == horizontalPosition + i) && (i < wordToPlay.Length))
                    {
                        cell.LetterOnBoard = wordToPlay[i];
                        i += 1;
                    }                    
                }
            }
            else
            {
                int i = 0;
                foreach (var cell in gameBoard.Where(hp => hp.VPosition == horizontalPosition)) 
                {
                    if ((cell.HPosition == verticalPosition + i) && (i < wordToPlay.Length))
                    {
                        cell.LetterOnBoard = wordToPlay[i];
                        i += 1;
                    }
                }
            }
            return gameBoard;
        }     

        public bool CheckTheBoardNeighbours(char[] wordToPlay, char d, int h, int v, List<BoardCell> gameBoard) //OK checking neighbouring columns and rows alows to place the word  
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
                foreach (var upperCell in gameBoard.Where(vp => vp.HPosition == v - 1)) //checking if there no more than 1 touching cell with letter in upper row
                {
                    if (upperCell.LetterOnBoard != '.' && i < tempH)
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
                foreach (var upperCell in gameBoard.Where(vp => vp.HPosition == v + 1)) //checking if there no more than 1 touching cell with letter in bottom row
                {
                    if (upperCell.LetterOnBoard != '.' && j < tempH)
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
                foreach (var upperCell in gameBoard.Where(hp => hp.VPosition == h - 1)) //checking if there no more than 1 touching cell with letter in left column
                {
                    if (upperCell.LetterOnBoard != '.' && i < tempV)
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
                foreach (var upperCell in gameBoard.Where(hp => hp.VPosition == h + 1)) //checking if there no more than 1 touching cell with letter in right column
                {
                    if (upperCell.LetterOnBoard != '.' && j < tempV)
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
        }

        public List<char> LettersFromHand(List<Tile> hand)
        {
            List<char> lettersOnHand = new List<char>();
            foreach (var item in hand)
            {
                lettersOnHand.Add(item.Letter);
            }
            return lettersOnHand;
        }

        public void PrintBoard(List<BoardCell> gameBoard, List<Tile> listOfUniqueTiles)  
        {
            List<char> unicTileLetters = new List<char>();
            foreach (var Tile in listOfUniqueTiles)
            {
                unicTileLetters.Add(Tile.Letter);
            }

            Console.WriteLine("     -------------------------------------------------------------------------------------------");
            Console.WriteLine("     |  0  |  1  |  2  |  3  |  4  |  5  |  6  |  7  |  8  |  9  |  10 |  11 |  12 |  13 |  14 |");
            Console.WriteLine("------------------------------------------------------------------------------------------------");
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
                    Console.Write("| ");
                    if (unicTileLetters.Contains(cell.LetterOnBoard))
                    {
                        Console.Write(" ");
                        Console.Write(cell.LetterOnBoard);
                        Console.Write("  ");
                    }
                    else
                    {
                        Console.Write(cell.SpecialValue);
                        Console.Write(" ");
                    }
                }
                Console.WriteLine("|");
                Console.WriteLine("------------------------------------------------------------------------------------------------");
            }
        } //Printing the board

        public Player PlayerName(int i, IManageScrabbleDb manageScrableDb) //OK Entering the names of players 
        {
            Console.WriteLine();
            string playerName = "";
            int x = 0;
            while (x == 0)
            {
                Console.Write($"Enter player {i + 1} name: ");
                playerName = Console.ReadLine();
                if (playerName.Length < 1)
                {
                    Console.WriteLine("You havn't entered correct name");
                }
                else
                {
                    x = 1;
                }
                
            }
            if ((manageScrableDb.GetAllPlayers().Exists(p => p.PlayerName == playerName))) 
            {
                Console.WriteLine($"Welcome {playerName} to the Scrabble again!");                
            }
            else
            {
                manageScrableDb.InsertPlayer(playerName);
                Console.WriteLine($"Welcome {playerName} to the Scrabble!");
            }
            return (manageScrableDb.GetAllPlayers().FirstOrDefault(p => p.PlayerName == playerName));
        }        
    }
}
