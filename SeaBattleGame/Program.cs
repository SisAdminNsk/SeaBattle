using SeaBattleGame.GameConfig;
using SeaBattleGame.Map;
using SeaBattleGame.Player;

namespace SeaBattleGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameConfigReader configReader = new GameConfigReader();

            var gameConfig = configReader.ReadConfig(GameMode.StandartGameMode);

            var gameMap1 = new GameMap(gameConfig);

            var response1 = gameMap1.TryAddShipsRandomly(gameConfig.GetShipsFromConfig());

            if (!response1.Success)
            {
                throw new Exception("Неверная генерация");
            }

            var gameMap2 = new GameMap(gameConfig);

            var response2 = gameMap2.TryAddShipsRandomly(gameConfig.GetShipsFromConfig());

            if (!response2.Success)
            {
                throw new Exception("Неверная генерация");
            }

            GamePlayer player1 = new GamePlayer(gameMap2);
            GamePlayer player2 = new GamePlayer(gameMap1);

            gameMap1.PrintGameMap();
            Console.WriteLine();
            gameMap2.PrintGameMap();   
        }
    }
}
