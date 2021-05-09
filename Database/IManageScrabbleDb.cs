using SkrabbleLt.Models;
using System;
using System.Collections.Generic;

namespace SkrabbleLt.Database
{
    public interface IManageScrabbleDb
    {
        List<BoardCell> GetAllBoardCells();
        List<Player> GetAllPlayers();
        int GetLastGameId();
        void InsertGame(List<Player> gamePlayers, DateTime gameDate);
        void InsertPlayer(string playerName);
        void InsertStatistic(int playerId, int gameId, List<Tile> tilesOnHand, string playedWord, int horizontalPosition, int verticalPosition, char direction);
    }
}