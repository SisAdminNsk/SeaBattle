namespace SeaBattleGame
{
    public class Ship
    {
        public string Id { get; private set; }
        public bool Killed { get; set; } = false;
        public int Size { get; private set; }
        public Ship(int size, string id)
        {
            Size = size;

            Id = id;

        }
    }
}