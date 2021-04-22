﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SkrabbleLt.Models
{
    public class Statistic
    {
        [Key]
        public int StatisticId { get; set; }
        public int PlayerId { get; set; }
        public int GameId { get; set; }
        public List<Tile> Tiles { get; set; }
        public string PlayedWord { get; set; }
        public BoardCell Position { get; set; } //position of the words first letter
        public char Direction { get; set; } //'H' horizontal 'V' vertical
    }
}
