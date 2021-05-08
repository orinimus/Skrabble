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

        public PlayerHand(int playerID, List<Tile> hand, int points) : this(playerID, hand)
        {
            Points = points;
        }

        public int PlayerID { get; set; }
        public List<Tile> Hand { get; set; }
        public int Points { get; set; }
    }
}
