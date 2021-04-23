using SkrabbleLt.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkrabbleLt.InitialData
{
    public static class TileQtInitialData
    {
        public static List<TileQt> DataSeed => new List<TileQt>
        {
            new TileQt(1, 'A', 1, 12),
            new TileQt(2, 'Ą', 8, 1),
            new TileQt(3, 'B', 2, 1),
            new TileQt(4, 'C', 10, 1),
            new TileQt(5, 'Č', 8, 1),
            new TileQt(6, 'D', 2, 3),
            new TileQt(7, 'E', 1, 5),
            new TileQt(8, 'Ę', 10, 1),
            new TileQt(9, 'Ė', 4, 2),
            new TileQt(10, 'F', 10, 1),
            new TileQt(11, 'G', 4, 2),
            new TileQt(12, 'H', 10, 1),
            new TileQt(13, 'I', 1, 13),
            new TileQt(14, 'Į', 8, 1),
            new TileQt(15, 'Y', 5, 1),
            new TileQt(16, 'J', 4, 2),
            new TileQt(17, 'K', 1, 4),
            new TileQt(18, 'L', 2, 3),
            new TileQt(19, 'M', 2, 3),
            new TileQt(20, 'N', 1, 5),
            new TileQt(21, 'O', 1, 6),
            new TileQt(22, 'P', 3, 3),
            new TileQt(23, 'R', 1, 5),
            new TileQt(24, 'S', 1, 8),
            new TileQt(25, 'Š', 5, 1),
            new TileQt(26, 'T', 1, 6),
            new TileQt(27, 'U', 1, 4),
            new TileQt(28, 'Ū', 8, 1),
            new TileQt(29, 'Ų', 6, 1),
            new TileQt(30, 'V', 4, 2),
            new TileQt(31, 'Z', 10, 1),
            new TileQt(32, 'Ž', 6, 1),
            new TileQt(33, '_', 0, 2)
        };
    }
}
