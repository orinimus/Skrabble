namespace SkrabbleLt.Models
{
    public class BoardCellValue : BoardCell
    {
        public BoardCellValue(int hPosition, int vPosition, string specialValue, Tile letter) : base(hPosition, vPosition, specialValue)
        {
            Letter = letter;
        }

        public Tile Letter { get; set; }
    }
}
