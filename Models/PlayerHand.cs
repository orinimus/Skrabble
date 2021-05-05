using System;
using System.Collections.Generic;
using System.Text;

namespace SkrabbleLt.Models
{
    public class PlayerHand
    {
        public PlayerHand(int playerID, List<Tile> hand)
        {
            PlayerID = playerID;
            Hand = hand;
        }

        public int PlayerID { get; set; }
        public List<Tile> Hand { get; set; }
    }
}
