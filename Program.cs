using SkrabbleLt.Database;
using SkrabbleLt.Services;
using System;

namespace Skrabble
{
    static class Program
    {
        static void Main(string[] args)
        {
            using (var context = new ScrabbleContext())
            {
                IManageScrabbleDb manageScrableDb = new ManageScrabbleDb(context);
                //IScrabbleGame scrabbleGame = new ScrabbleGame(manageScrableDb);
                var game = new ScrabbleGame();
                game.Scrabble(manageScrableDb);
            }
        }
    }
}
