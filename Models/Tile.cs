using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SkrabbleLt.Models
{
    public class Tile
    {
        public Tile(char letter, int value)
        {
            Letter = letter;
            Value = value;
        }

        public Tile(int tileId, char letter, int value)
        {
            TileId = tileId;
            Letter = letter;
            Value = value;
        }

        [Key]
        public int TileId { get; set; }
        public char Letter { get; set; }
        public int Value { get; set; }
    }
}
