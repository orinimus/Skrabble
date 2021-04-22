namespace SkrabbleLt.Models
{
    public class BoardCell
    {
        public BoardCell(int hPosition, int vPosition, string specialValue)
        {
            HPosition = hPosition;
            VPosition = vPosition;
            SpecialValue = specialValue;
        }

        public int HPosition { get; set; }
        public int VPosition { get; set; }
        public string SpecialValue { get; set; }
    }
}
