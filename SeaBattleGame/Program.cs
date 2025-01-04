using SeaBattleGame.Game;
using SeaBattleGame.Map;
using SeaBattleGame.Player;

namespace SeaBattleGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ships = GetShips();

            int gameMapSize = 10;
            var gameMap1 = new GameMap(gameMapSize);

            gameMap1.TryAddShipsRandomly(ships);

            var gameMap2 = new GameMap(gameMapSize);

            var ships2 = GetShips();
            gameMap2.TryAddShipsRandomly(ships2);

            GamePlayer player1 = new GamePlayer(gameMap2);
            GamePlayer player2 = new GamePlayer(gameMap1);

            GameSession gameSession = new GameSession(player1, player2);

            //gameMap1.PrintGameMap();

            Console.WriteLine();

            //gameMap2.PrintGameMap();

            //for(int i = 0; i < 4000; i++)
            //{
            //    var map = new GameMap(gameMapSize);

            //    var response = map.TryAddShipsRandomly(ships);
                
            //    if (!response.Success)
            //    {
            //        throw new Exception($"Неверная генерация, номер на котором сломалась: {i}");
            //    }
            //}

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
