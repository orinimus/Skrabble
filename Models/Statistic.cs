using System.Collections.Generic;
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
        public List<Tile> Tiles { get; set; } //the hand before move
        public string PlayedWord { get; set; }
        public int HPosition { get; set; }
        public int VPosition { get; set; }
        public char Direction { get; set; } //'H' horizontal 'V' vertical
    }
}
