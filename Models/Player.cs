using System.ComponentModel.DataAnnotations;

namespace SkrabbleLt.Models
{
    public class Player
    {
        [Key]
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
    }
}
