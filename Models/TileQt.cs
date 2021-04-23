using System;
using System.Collections.Generic;
using System.Text;

namespace SkrabbleLt.Models
{
    public class TileQt : Tile
    {
        public TileQt(int tileId, char letter, int value, int quantity) : base(tileId, letter, value)
        {
            Quantity = quantity;
        }

        public int Quantity { get; set; }
    }
}
