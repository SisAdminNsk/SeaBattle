namespace SeaBattleGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameMap map = new GameMap(10);

            Ship ship1 = new Ship(3);
            Ship ship2 = new Ship(4);
            Ship ship3 = new Ship(3);

            map.TryAddShip(ship1, new GameCell(1, 0), ShipOrientation.Horizontal);
            map.TryAddShip(ship2, new GameCell(4, 4), ShipOrientation.Horizontal);

            map.TryAddShip(ship3, new GameCell(4, 7), ShipOrientation.Vertical);

            map.ChangeShipLocation(ship2, new GameCell(8, 6), ShipOrientation.Vertical);
            map.ChangeShipLocation(ship3, new GameCell(4, 7), ShipOrientation.Horizontal);

            map.PrintGameMap();
        }
    }
}
