using System;
using System.Collections.Generic;
using System.Text;

namespace SkrabbleLt.Models
{
    public class Tile
    {
        public Tile(char letter, int value, int quantity)
        {
            Letter = letter;
            Value = value;
            Quantity = quantity;
        }

        public char Letter { get; set; }
        public int Value { get; set; }
        public int Quantity { get; set; }
    }
}
