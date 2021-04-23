﻿using System.ComponentModel.DataAnnotations;

namespace SkrabbleLt.Models
{
    public class BoardCell
    {
        public BoardCell(int boardCellId, int hPosition, int vPosition, string specialValue)
        {
            BoardCellId = boardCellId;
            HPosition = hPosition;
            VPosition = vPosition;
            SpecialValue = specialValue;
        }

        [Key]
        public int BoardCellId { get; set; }
        public int HPosition { get; set; }
        public int VPosition { get; set; }
        public string SpecialValue { get; set; }
    }
}
