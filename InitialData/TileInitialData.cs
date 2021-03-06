using SkrabbleLt.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SkrabbleLt.InitialData
{
    public static class TileInitialData
    {
        public static List<Tile> DataSeed => new List<Tile>
        {
            new Tile(1, 'A', 1),
            new Tile(2, 'Ą', 8),
            new Tile(3, 'B', 2),
            new Tile(4, 'C', 10),
            new Tile(5, 'Č', 8),
            new Tile(6, 'D', 2),
            new Tile(7, 'E', 1),
            new Tile(8, 'Ę', 10),
            new Tile(9, 'Ė', 4),
            new Tile(10, 'F', 10),
            new Tile(11, 'G', 4),
            new Tile(12, 'H', 10),
            new Tile(13, 'I', 1),
            new Tile(14, 'Į', 8),
            new Tile(15, 'Y', 5),
            new Tile(16, 'J', 4),
            new Tile(17, 'K', 1),
            new Tile(18, 'L', 2),
            new Tile(19, 'M', 2),
            new Tile(20, 'N', 1),
            new Tile(21, 'O', 1),
            new Tile(22, 'P', 3),
            new Tile(23, 'R', 1),
            new Tile(24, 'S', 1),
            new Tile(25, 'Š', 5),
            new Tile(26, 'T', 1),
            new Tile(27, 'U', 1),
            new Tile(28, 'Ū', 8),
            new Tile(29, 'Ų', 6),
            new Tile(30, 'V', 4),
            new Tile(31, 'Z', 10),
            new Tile(32, 'Ž', 6),
            new Tile(33, '_', 0)
        };
    }
}
