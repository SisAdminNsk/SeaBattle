using SeaBattleGame.GameConfig;
using SeaBattleGame.Map;

namespace SeaBattleGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameConfigReader configReader = new GameConfigReader();

            var gameConfig = configReader.ReadConfig(GameMode.StandartGameMode);

            var gameMap = new GameMap(gameConfig);
            var ships = gameConfig.GetShipsFromConfig();

            gameMap.TryAddShip(ships[0], new GameCell(0, 0), ShipOrientation.Horizontal);
            gameMap.TryAddShip(ships[1], new GameCell(0, 2), ShipOrientation.Horizontal);
            gameMap.TryAddShip(ships[2], new GameCell(0, 4), ShipOrientation.Horizontal);
            gameMap.TryAddShip(ships[3], new GameCell(0, 6), ShipOrientation.Horizontal);

            gameMap.TryAddShip(ships[4], new GameCell(2, 0), ShipOrientation.Horizontal);
            gameMap.TryAddShip(ships[5], new GameCell(2, 2), ShipOrientation.Horizontal);
            gameMap.TryAddShip(ships[6], new GameCell(2, 4), ShipOrientation.Horizontal);

            gameMap.TryAddShip(ships[7], new GameCell(6, 0), ShipOrientation.Vertical);
            gameMap.TryAddShip(ships[8], new GameCell(6, 4), ShipOrientation.Horizontal);

            gameMap.TryAddShip(ships[9], new GameCell(5, 9), ShipOrientation.Horizontal);

            gameMap.PrintGameMap();

            gameMap.AllShipsDestroyed += (sender) => { Console.WriteLine("Все корабли уничтожены"); };

            gameMap.Hit(new GameCell(6, 0));
            gameMap.Hit(new GameCell(6, 1));
            gameMap.Hit(new GameCell(6, 2));
            gameMap.Hit(new GameCell(7, 1));
            gameMap.Hit(new GameCell(8, 1));

            gameMap.Hit(new GameCell(2, 0));
            gameMap.Hit(new GameCell(3, 0));

            gameMap.Hit(new GameCell(2, 2));
            gameMap.Hit(new GameCell(3, 2));

            gameMap.Hit(new GameCell(2, 4));
            gameMap.Hit(new GameCell(3, 4));

            gameMap.Hit(new GameCell(0, 0));
            gameMap.Hit(new GameCell(0,2));
            gameMap.Hit(new GameCell(0, 4));
            gameMap.Hit(new GameCell(0, 6));

            gameMap.Hit(new GameCell(2, 0));
            gameMap.Hit(new GameCell(3, 0));

            gameMap.Hit(new GameCell(6, 4));
            gameMap.Hit(new GameCell(7, 4));
            gameMap.Hit(new GameCell(8, 4));

            gameMap.Hit(new GameCell(5, 9));
            gameMap.Hit(new GameCell(6, 9));
            gameMap.Hit(new GameCell(7, 9));
            gameMap.Hit(new GameCell(8, 9));

            Console.WriteLine();

            gameMap.PrintGameMap();

        }
    }
}
