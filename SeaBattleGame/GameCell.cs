namespace SeaBattleGame
{
    public class GameCell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool Hitted { get; set; } = false;
        public GameCell(int x, int y)
        {
            X = x;
            Y = y;
        }
        public GameCell()
        {

        }

        public bool CompareValue(GameCell other)
        {
            return other.X == X && other.Y == Y;
        }
    }
}
