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

        //public bool CompareValue(GameCell other)
        //{
        //    return other.X == X && other.Y == Y;
        //}

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            GameCell other = (GameCell)obj;
            return other.X == X && other.Y == Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

    }
}
