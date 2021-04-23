using SkrabbleLt.Database;
using SkrabbleLt.Models;
using System.Collections.Generic;

namespace SkrabbleLt.Services
{
    public interface IScrabbleGame
    {
        int HowManyPlayers();
        List<Player> PlayersNames(int howManyPlayers, IManageScrabbleDb manageScrableDb);
        void PrintBoard();
        void Scrabble(IManageScrabbleDb manageScrableDb);
    }
}