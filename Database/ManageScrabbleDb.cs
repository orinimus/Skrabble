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

        public void InsertPlayer(string playerName)
        {
            _context.Players.Add(new Player { PlayerName = playerName });
            _context.SaveChanges();
        }

    }
}
