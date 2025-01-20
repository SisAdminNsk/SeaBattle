using SeaBattleGame.GameConfig;

namespace SeaBattleConsoleClient
{
    public class Menu
    {
        private Game _currentGame;
        private GameModeConfig _gameMode;

        public Menu(GameModeConfig gameMode)
        {
            _gameMode = gameMode;
        }

        public async Task ShowMenuAsync()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== Меню ===");
                Console.WriteLine("1. Начать игру");
                Console.WriteLine("2. Выйти из игры");
                Console.Write("Выберите пункт: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await StartGameAsync();
                        break;

                    case "2":
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Некорректный выбор. Пожалуйста, выберите 1 или 2.");
                        break;
                }
            }
        }

        private async Task StartGameAsync()
        {
            if (_currentGame != null)
            {
                Console.WriteLine("Игра уже запущена. Пожалуйста, завершите текущую игру перед началом новой.");
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
                return;
            }

            _currentGame = new Game(_gameMode);

            _currentGame.CriticalErrorOccured += OnGameCriticalErrorOccured;
            _currentGame.GameFinished += OnGameFinished;

            Console.Clear();

            await _currentGame.StartGameAsync();
        }

        private void OnGameCriticalErrorOccured(string errorMessage)
        {
            Console.Clear();
            Console.WriteLine(errorMessage);
            Thread.Sleep(2000);
        }

        private void OnGameFinished(Game game)
        {
            Console.WriteLine("Игра завершена. Возвращаемся в меню...");
            Thread.Sleep(3000);
            _currentGame = null;
        }
    }
}
