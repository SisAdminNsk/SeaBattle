namespace SeaBattleGame
{
    public class Ship
    {
        public bool Killed { get; set; } = false;
        public int Size { get; private set; }
        public Ship(int size) 
        {
            Size = size;
        }
    }
}