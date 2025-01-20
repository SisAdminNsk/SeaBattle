using SeaBattle.Contracts;
using SeaBattleConsoleClient.ServerCommunication;
using SeaBattleGame.Game.Responses;
using SeaBattleGame.GameConfig;
using SeaBattleGame.Map;
using SeaBattleGame;
using System.Net.WebSockets;


namespace SeaBattleConsoleClient
{
    class Game
    {
        private IGameMap _gameMap;
        private OponnentMap _oponnentMap;
        private IPlayerConnection _playerConnection;

        private GameModeConfig _gameMode;

        private string _gamePlayerId;
        private bool isYourTurn = false;

        public delegate void OnCriticalErrorOccured(string errorMessage);
        public event OnCriticalErrorOccured CriticalErrorOccured;

        public delegate void OnGameFinished(Game game);
        public event OnGameFinished GameFinished;

        private bool _gameRunning = false;

        public Game(GameModeConfig gameMode)
        {
            _gameMode = gameMode;
            _gameMap = new GameMap(gameMode);
            _oponnentMap = new OponnentMap(gameMode.GameMapSize);
        }

        public async Task StartGameAsync()
        {
            var playerGameMapRequest = ArrangeShipsOnMap();

            var startGameAccessToken = await GetStartGameAcessTokenAsync(playerGameMapRequest);

            if (startGameAccessToken == null)
            {
                CriticalErrorOccured?.Invoke("Произошла непридвиденная ошибка, не удалось получить токен начала игры.");
            }
            else
            {
                var uri = new Uri($"ws://localhost:5024/Game/StartGame?startGameAccessToken={Uri.EscapeDataString(startGameAccessToken)}");

                using (var socket = new ClientWebSocket())
                {
                    await socket.ConnectAsync(uri, CancellationToken.None);

                    _playerConnection = new PlayerConnection(socket);

                    _playerConnection.MessageRecived += OnMessageRecived;
                    _playerConnection.ServerCloseConnection += OnServerCloseConnection;

                    _gameRunning = true;

                    Task.Run(() => ProcessInputAsync());

                    WaitOpponent();

                    while (_gameRunning)
                    {
                        await _playerConnection.ReceiveMessagesAsync(socket);
                    }
                }
            }
        }

        private void WaitOpponent()
        {
            Console.Clear();
            _gameMap.PrintGameMap();
            Console.WriteLine("Ожидаем подключения второго игрока...");
        }

        private void OnServerCloseConnection(string message)
        {
            _gameRunning = false;

            GameFinished?.Invoke(this);
        }

        private async Task MakeHit(GameCell cellToHit)
        {
            var hitRequest = new HitRequest(cellToHit);

            await _playerConnection.SendMessageAsync(new BasePlayerRequest("HitRequest", hitRequest));
        }

        private void OnSessionStarted(GameSessionStartedResponse startedResponse)
        {
            _gamePlayerId = startedResponse.YourId;

            ChangeTurn(startedResponse.PlayerTurnId);

            Console.Clear();
            Console.WriteLine("Ваша карта:");
            _gameMap.PrintGameMap();
            Console.WriteLine("\nКарта противника:");
            _oponnentMap.Print();
            Console.WriteLine();

            Console.WriteLine("Игра началась!");

            if (isYourTurn)
            {
                Console.WriteLine("Сейчас твой ход, введи клетку (x,y) для выстрела по карте");
            }
            else
            {
                Console.WriteLine("Сейчас ход противника");
            }
        }

        private void OnPlayerWin(PlayerWinResponse playerWinResponse)
        {
            _gameRunning = false;

            Console.Clear();
            Console.WriteLine("Ты победил!");
            Thread.Sleep(2000);
        }
        private async Task OnPlayerTurnChanged(PlayerTurnChangedResponse playerTurnChangedResponse)
        {
            ChangeTurn(playerTurnChangedResponse.CurrentTurnPlayerId);

            Console.Clear();
            Console.WriteLine("Ваша карта:");
            _gameMap.PrintGameMap();
            Console.WriteLine("\nКарта противника:");
            _oponnentMap.Print();
            Console.WriteLine();

            if (isYourTurn)
            {
                Console.WriteLine("Сейчас твой ход, введи клетку (x,y) для выстрела по карте");
            }
            else
            {
                Console.WriteLine("Сейчас ход противника");
            }
        }

        private void ChangeTurn(string currentTurnPlayerId)
        {
            if (currentTurnPlayerId == _gamePlayerId)
            {
                isYourTurn = true;
            }
            else
            {
                isYourTurn = false;
            }
        }

        private void OnPlayerHitResponse(PlayerHitResponse playerHitResponse)
        {
            if (playerHitResponse.Success)
            {
                ChangeTurn(playerHitResponse.PlayerTurnId);

                Console.Clear();
                Console.WriteLine("Ваша карта:");
                _gameMap.PrintGameMap();
                Console.WriteLine("\nКарта противника:");
                _oponnentMap.Hit(playerHitResponse);
                _oponnentMap.Print();
                Console.WriteLine();

                if (isYourTurn)
                {
                    Console.WriteLine("Сейчас твой ход, введи клетку (x,y) для выстрела по карте");
                }
                else
                {
                    Console.WriteLine("Сейчас ход противника");
                }
            }
        }

        private void OnOpponentHitResponse(PlayerHitResponse playerHitResponse)
        {
            if (playerHitResponse.Success)
            {
                ChangeTurn(playerHitResponse.PlayerTurnId);

                Console.Clear();
                Console.WriteLine("Ваша карта:");
                _gameMap.Hit(playerHitResponse.HitGameMapResponse.HittedCell);
                _gameMap.PrintGameMap();
                Console.WriteLine("\nКарта противника:");
                _oponnentMap.Print();

                Console.WriteLine();

                if (isYourTurn)
                {
                    Console.WriteLine("Сейчас твой ход, введи клетку (x,y) для выстрела по карте");
                }
                else
                {
                    Console.WriteLine("Сейчас ход противника");
                }
            }
        }

        private void OnMessageRecived(BasePlayerResponse message)
        {
            if (message.MessageType == "SessionStarted")
            {
                var gameSessionResponse = message.GetResponse<GameSessionStartedResponse>();

                OnSessionStarted(gameSessionResponse);
            }

            if (message.MessageType == "PlayerWin")
            {
                var playerWinResponse = message.GetResponse<PlayerWinResponse>();

                OnPlayerWin(playerWinResponse);
            }

            if (message.MessageType == "PlayerTurnChanged")
            {
                var playerTurnChanged = message.GetResponse<PlayerTurnChangedResponse>();

                OnPlayerTurnChanged(playerTurnChanged);
            }

            if (message.MessageType == "PlayerHit")
            {
                var playerHitResponse = message.GetResponse<PlayerHitResponse>();

                OnPlayerHitResponse(playerHitResponse);
            }

            if (message.MessageType == "OpponentHit")
            {
                var playerHitResponse = message.GetResponse<PlayerHitResponse>();

                OnOpponentHitResponse(playerHitResponse);
            }
        }

        private async Task<string?> GetStartGameAcessTokenAsync(PlayerGameMapRequest playerGameMapRequest)
        {
            using (ISeaBattleHttpClient httpClient = new SeaBattleHttpClient())
            {
                Console.WriteLine("Выполняется проверка карты...");
                var errorOrValidateGameMapResponse = await httpClient.TryValidateGameMapAsync(playerGameMapRequest);

                if (errorOrValidateGameMapResponse.IsError || !errorOrValidateGameMapResponse.Value.Success)
                {
                    Console.Clear();
                    Console.WriteLine("Ошбика валидации карты, возможно проблема на стороне сервера," +
                        "на всякий случай попробуйте выполнить расстановку еще раз");
                    Console.WriteLine(errorOrValidateGameMapResponse.FirstError.ToString());
                    Thread.Sleep(2000);
                }

                var startGameAccessToken = errorOrValidateGameMapResponse.Value.AccessToken;

                return startGameAccessToken;
            }
        }
        private PlayerGameMapRequest ArrangeShipsOnMap()
        {
            bool isAllShipsOnMap = false;

            PlayerGameMapRequest? playerGameMapRequest = null;

            while (!isAllShipsOnMap)
            {
                Console.WriteLine("Выберите способ расстановки кораблей (введите 'manual' для ручного или 'auto' для случайного):");
                string setupChoice = Console.ReadLine().Trim().ToLower();

                if (setupChoice == "manual")
                {
                    playerGameMapRequest = ManualSetup(_gameMode, _gameMap);
                }
                else if (setupChoice == "auto")
                {
                    playerGameMapRequest = AutomaticSetup(_gameMode, _gameMap);
                }
                else
                {
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    Thread.Sleep(2000);
                    Console.Clear();
                }

                if (playerGameMapRequest != null)
                {
                    isAllShipsOnMap = true;
                }
            }

            return playerGameMapRequest;
        }
        private PlayerGameMapRequest? ManualSetup(GameModeConfig gameConfig, IGameMap gameMap)
        {
            List<ShipPositionOnMap> shipPositionOnMap = new();

            foreach (var configShip in gameConfig.Ships)
            {
                for (int i = 0; i < configShip.Count; i++)
                {
                    bool placed = false;

                    while (!placed)
                    {
                        Console.Clear();
                        gameMap.PrintGameMap();

                        Console.WriteLine($"Введите координаты для расстановки {configShip.Name} (формат: строка столбец направление [h/v]):");

                        var inputData = Console.ReadLine().Split();

                        int row, col;
                        char direction;

                        ShipOrientation shipOrientation = ShipOrientation.Vertical;

                        if (!int.TryParse(inputData[0], out row))
                        {
                            Console.WriteLine("Ошибка: неверный ввод для строки. Пожалуйста, введите целое число.");
                        }

                        if (!int.TryParse(inputData[1], out col))
                        {
                            Console.WriteLine("Ошибка: неверный ввод для столбца. Пожалуйста, введите целое число.");
                        }

                        if (inputData.Length > 2 && char.TryParse(inputData[2].ToString(), out direction))
                        {
                            if (direction == 'h')
                            {
                                shipOrientation = ShipOrientation.Horizontal;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: неверный ввод для направления. Пожалуйста, введите один символ.");
                        }

                        var startCell = new GameCell(col, row);
                        var ship = new Ship(configShip.Size);

                        var addResponse = gameMap.TryAddShip(ship, startCell, shipOrientation);

                        if (addResponse.Success)
                        {
                            placed = true;

                            shipPositionOnMap.Add(new ShipPositionOnMap(ship, startCell, shipOrientation));
                        }
                        else
                        {
                            Console.WriteLine($"Невозможно установить корабль размера {configShip.Size} в заданную позицию");
                            Thread.Sleep(2000);
                        }
                    }
                }
            }

            PlayerGameMapRequest playerGameMapRequest = new(shipPositionOnMap, gameConfig);

            return playerGameMapRequest;
        }

        private PlayerGameMapRequest? AutomaticSetup(GameModeConfig gameConfig, IGameMap gameMap)
        {
            List<Ship> ships = new();

            foreach (var confShip in gameConfig.Ships)
            {
                for (int i = 0; i < confShip.Count; i++)
                {
                    ships.Add(new Ship(confShip.Size));
                }
            }

            gameMap.PrintGameMap();

            var shipsAddedResponse = gameMap.TryAddShipsRandomly(ships);

            if (!shipsAddedResponse.Success)
            {
                Console.WriteLine("Возникла непредвиденная ошибка при автоматическом добавлении, попробуйте еще раз");
                Thread.Sleep(2000);

                return null;
            }

            List<ShipPositionOnMap> shipPositionOnMap = new();

            shipsAddedResponse.Ships.ForEach(shipAddedResponse => shipPositionOnMap.Add
            (
                new ShipPositionOnMap
                (
                    shipAddedResponse.Ship,
                    shipAddedResponse.StartCell,
                    shipAddedResponse.ShipOrientation
                )
            ));

            PlayerGameMapRequest playerGameMapRequest = new(shipPositionOnMap, gameConfig);

            return playerGameMapRequest;
        }
        private bool TryParseCoordinates(string input, out int x, out int y)
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

        private async Task ProcessInputAsync()
        {
            while (_gameRunning)
            {
                string input = Console.ReadLine();

                if (isYourTurn)
                {
                    if (!TryParseCoordinates(input, out int x, out int y))
                    {
                        Console.WriteLine("Неправильный формат координат.");
                    }
                    else
                    {
                        var gameCell = new GameCell(x, y);
                        await MakeHit(gameCell);
                    }
                }
                else
                {
                    Console.WriteLine("Сейчас ход противника");
                }
            }
        }
    }
}
