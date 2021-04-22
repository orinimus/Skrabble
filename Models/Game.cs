using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SkrabbleLt.Models
{
    public class Game
    {
        [Key]
        public int GameId { get; set; }
        public List<Player> GamePlayers { get; set; }
        public DateTime GameDate { get; set; }

    }
}
