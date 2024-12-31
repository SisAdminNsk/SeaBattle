namespace SeaBattleGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameMap map = new GameMap(8);

            Ship ship1 = new Ship(3);
            Ship ship2 = new Ship(4);

            bool a = map.TryAddShip(ship1, new GameCell(0, 0), ShipOrientation.Horizontal);

            bool b = map.TryAddShip(ship2, new GameCell(3, 0), ShipOrientation.Vertical);

        }
    }
}
