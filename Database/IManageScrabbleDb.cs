using SkrabbleLt.Models;
using System.Collections.Generic;

namespace SkrabbleLt.Database
{
    public interface IManageScrabbleDb
    {
        List<BoardCell> GetAllBoardCells();
        List<Player> GetAllPlayers();
        void InsertPlayer(string playerName);
    }
}