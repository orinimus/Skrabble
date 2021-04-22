using SkrabbleLt.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SkrabbleLt.InitialData
{
    public static class RackInitialData
    {
        public static List<Tile> DataSeed => new List<Tile>
        {
            new Tile('A', 1, 12),
            new Tile('Ą', 8, 1),
            new Tile('B', 2, 1),
            new Tile('C', 10, 1),
            new Tile('Č', 8, 1),
            new Tile('D', 2, 3),
            new Tile('E', 1, 5),
            new Tile('Ę', 10, 1),
            new Tile('Ė', 4, 2),
            new Tile('F', 10, 1),
            new Tile('G', 4, 2),
            new Tile('H', 10, 1),
            new Tile('I', 1, 13),
            new Tile('Į', 8, 1),
            new Tile('Y', 5, 1),
            new Tile('J', 4, 2),
            new Tile('K', 1, 4),
            new Tile('L', 2, 3),
            new Tile('M', 2, 3),
            new Tile('N', 1, 5),
            new Tile('O', 1, 6),
            new Tile('P', 3, 3),
            new Tile('R', 1, 5),
            new Tile('S', 1, 8),
            new Tile('Š', 5, 1),
            new Tile('T', 1, 6),
            new Tile('U', 1, 4),
            new Tile('Ū', 8, 1),
            new Tile('Ų', 6, 1),
            new Tile('V', 4, 2),
            new Tile('Z', 10, 1),
            new Tile('Ž', 6, 1),
            new Tile('_', 0, 2)
        };
    }
}
