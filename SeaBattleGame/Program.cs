using SeaBattleGame.Game;
using SeaBattleGame.GameConfig;
using SeaBattleGame.Map;
using SeaBattleGame.Player;

namespace SeaBattleGame
{
    internal class Program
    {
        private static IGamePlayer _player1 = new GamePlayer();
        private static IGamePlayer _player2 = new GamePlayer();

        private static IGameMap _player1Map;
        private static IGameMap _player2Map;
        static void Main(string[] args)
        {
            GameConfigReader configReader = new GameConfigReader();

            var gameConfig = configReader.ReadConfig(GameMode.StandartGameMode);

            _player1Map = new GameMap(gameConfig);

            if (!_player1Map.TryAddShipsRandomly(gameConfig.GetShipsFromConfig()).Success)
            {
                throw new Exception("Ошибка при расставлении кораблей.");
            }

            _player2Map = new GameMap(gameConfig);

            if (!_player2Map.TryAddShipsRandomly(gameConfig.GetShipsFromConfig()).Success)
            {
                throw new Exception("Ошибка при расставлении кораблей.");
            }

            IGameSession gameSession = new GameSession
            (
                new GameSessionArgs
                (
                    new PlayerArgs(_player1Map, _player1),
                    new PlayerArgs(_player2Map, _player2)
                )
            );

            bool gameFinished = false;

            gameSession.GameSessionFinished += (sender, winnerPlayerOrNll) => 
            {
                OnGameFinished(sender, winnerPlayerOrNll);
            };

            gameSession.GameSessionTurnTimeHasPassed += OnPlayerTurnTimeHasPassed;

            gameSession.PlayerHit += OnPlayerHit;

            gameSession.Start();

            while (!gameFinished)
            {
                ProcessGame(gameSession);
            }
        }

        private static void OnPlayerHit(IGameSession sender, IGamePlayer player, Game.GameResponses.PlayerHitResponse playerHitResponse)
        {
            Console.Clear();
            ProcessGame(sender);
        }

        private static void ProcessGame(IGameSession gameSession)
        {
            Console.WriteLine();
            Console.WriteLine($"Карта игрока: {_player1.GetId()}");
            _player1Map.PrintGameMap();
            Console.WriteLine();
            Console.WriteLine($"Карта игрока: {_player2.GetId()}");
            _player2Map.PrintGameMap();

            Console.WriteLine($"Ход игрока: {gameSession.GetCurrentTurnPlayer().GetId()}");
            Console.Write("Введите координаты для атаки в формате (x,y): ");

            string input = Console.ReadLine();

            if (!TryParseCoordinates(input, out int x, out int y))
            {
                Console.WriteLine("Неправильный формат координат.");
            }
            else
            {
                var gameCell = new GameCell(x, y);

                if (gameSession.GetCurrentTurnPlayer().GetId() == _player1.GetId())
                {
                    _player1.RequestMakeHit(gameCell);
                }
                else
                {
                    _player2.RequestMakeHit(gameCell);
                }
            }
        }
        private static void OnPlayerTurnTimeHasPassed(IGameSession sender, IGamePlayer player)
        {
            Console.Clear();
            ProcessGame(sender);
        }

        private static void OnGameFinished(IGameSession sender, IGamePlayer? winnerPlayer)
        {
            if(winnerPlayer is null)
            {
                Console.WriteLine("Ничья");
            }
            else
            {
                Console.WriteLine($"Победил игрок: {winnerPlayer.GetId()}");
            }
        }
        private static bool TryParseCoordinates(string input, out int x, out int y)
        {
            x = 0;
            y = 0;

            input = input.Trim();

            if (!input.StartsWith("(") || !input.EndsWith(")"))
            {
                return false;
            }

            input = input[1..^1];

            var parts = input.Split(',');

            if (parts.Length != 2)
            {
                return false;
            }

            return int.TryParse(parts[0].Trim(), out x) && int.TryParse(parts[1].Trim(), out y);
        }
    }
}
