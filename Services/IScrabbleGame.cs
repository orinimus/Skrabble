using SkrabbleLt.Database;
using SkrabbleLt.Models;
using System.Collections.Generic;

namespace SkrabbleLt.Services
{
    public interface IScrabbleGame
    {
        int HowManyPlayers();
        List<Player> PlayersNames(int howManyPlayers, IManageScrabbleDb manageScrableDb);
        void PrintBoard(List<BoardCell> gameBoard);
        void Scrabble(IManageScrabbleDb manageScrableDb);
    }
}