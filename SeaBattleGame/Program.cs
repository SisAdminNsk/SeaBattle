using SeaBattleGame.Map;

namespace SeaBattleGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ships = GetShips();

            int gameMapSize = 10;
            var gameMap = new GameMap(gameMapSize);

            //gameMap.TryAddShip(ships[0], new GameCell(0, 0), ShipOrientation.Horizontal);
            //gameMap.TryAddShip(ships[1], new GameCell(0, 2), ShipOrientation.Horizontal);
            //gameMap.TryAddShip(ships[2], new GameCell(0, 4), ShipOrientation.Horizontal);
            //gameMap.TryAddShip(ships[3], new GameCell(0, 6), ShipOrientation.Horizontal);

            //gameMap.TryAddShip(ships[4], new GameCell(2, 0), ShipOrientation.Horizontal);
            //gameMap.TryAddShip(ships[5], new GameCell(2, 2), ShipOrientation.Horizontal);
            //gameMap.TryAddShip(ships[6], new GameCell(2, 4), ShipOrientation.Horizontal);

            //gameMap.TryAddShip(ships[7], new GameCell(6, 0), ShipOrientation.Vertical);
            //gameMap.TryAddShip(ships[8], new GameCell(6, 4), ShipOrientation.Horizontal);

            //gameMap.TryAddShip(ships[9], new GameCell(4, 9), ShipOrientation.Horizontal);

            //gameMap.TryChangeShipLocation(ships[9], new GameCell(4, 6), ShipOrientation.Horizontal);

            //gameMap.TryChangeShipLocation(ships[0], new GameCell(9, 0), ShipOrientation.Horizontal);

            //var response = gameMap.Hit(new GameCell(0, 1));

            //var response1 = gameMap.Hit(new GameCell(9, 0));
            //var response2 = gameMap.Hit(new GameCell(9, 0));

            //var response3 = gameMap.Hit(new GameCell(0, 2));

            //var response4 = gameMap.Hit(new GameCell(2, 2));
            //var response5 = gameMap.Hit(new GameCell(3, 2));

            //var response6 = gameMap.Hit(new GameCell(4, 6));
            //var response7 = gameMap.Hit(new GameCell(5, 6));
            //var response8 = gameMap.Hit(new GameCell(6, 6));
            //var response9 = gameMap.Hit(new GameCell(7, 6));

            for(int i=0; i < 50; i++)
            {
                var response = gameMap.TryAddShipsRandomly(ships);

                while (!response.Success)
                {
                    gameMap.Clear();
                    response = gameMap.TryAddShipsRandomly(ships);
                }

                gameMap.PrintGameMap();
                gameMap.Clear();

                Console.WriteLine(response.Success);
            }


            //while (true)
            //{
            //    bool success = gameMap.TryAddShipsRandomly(ships);

            //    if (!success)
            //    {
            //        gameMap.Clear();
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}

            //gameMap.PrintGameMap();

            //int notValidAttempts = 0;

            //for (int i = 0; i < 50; i++)
            //{
            //    gameMap.Clear();
            //    bool success = gameMap.TryAddShipsRandomly(ships);
            //    gameMap.PrintGameMap();

            //    if (success)
            //    {
            //        Console.WriteLine("Валидно.");
            //    }
            //    else
            //    {
            //        Console.WriteLine("НЕВАЛИНО.");
            //        notValidAttempts++;
            //    }

            //    Console.WriteLine();
            //}

            //Console.WriteLine($"Невалидных попыток: {notValidAttempts}");
        }

        private static List<Ship> GetShips()
        {
            return new List<Ship>
            {
                new Ship(1),
                new Ship(1),
                new Ship(1),
                new Ship(1),

                new Ship(2),
                new Ship(2),
                new Ship(2),

                new Ship(3),
                new Ship(3),

                new Ship(4)
            };
        }
    }
}
