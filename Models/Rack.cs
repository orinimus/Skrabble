using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SkrabbleLt.Models
{
    public class Rack
    {
        [Key]
        public int RackId { get; set; }
        public List<Tile> Tiles { get; set; }
    }
}
