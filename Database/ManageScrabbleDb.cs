using SkrabbleLt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkrabbleLt.Database
{
    public class ManageScrabbleDb : IManageScrabbleDb
    {
        private readonly ScrabbleContext _context;

        public ManageScrabbleDb(ScrabbleContext context)
        {
            _context = context;
            context.Database.EnsureCreated();
        }

        public List<BoardCell> GetAllBoardCells()
        {
            return _context.BoardCells.ToList();
        }

        public List<Player> GetAllPlayers()
        {
            return _context.Players.ToList();
        }

        public int GetLastGameId()
        {
            return (_context.Games.Count() + 1);
        }

        public void InsertPlayer(string playerName)
        {
            _context.Players.Add(new Player { PlayerName = playerName });
            _context.SaveChanges();
        }

        public void InsertStatistic(int playerId, int gameId, List<Tile> tilesOnHand, string playedWord, int horizontalPosition, int verticalPosition, char direction)
        {
            _context.Statistics.Add(new Statistic { PlayerId = playerId, GameId = gameId, Tiles = tilesOnHand, PlayedWord = playedWord, HPosition = horizontalPosition, VPosition = verticalPosition, Direction = direction});
            _context.SaveChanges();
        }

        public void InsertGame(List<Player> gamePlayers, DateTime gameDate)
        {
            _context.Games.Add(new Game { GamePlayers = gamePlayers, GameDate = gameDate});
            _context.SaveChanges();
        }

    }
}
