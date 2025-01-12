using SeaBattleGame.Game;
using SeaBattleGame.GameConfig;
using SeaBattleGame.Map;
using SeaBattleGame.Player;

namespace SeaBattleApi.Websockets
{
    public class SeaBattleSession : IDisposable
    {
        public Guid Id { get; private set; }
        private IGameSession _gameSession { get; set; }

        private bool _isFinished = false;
        private bool _isDisposed = false;

        public delegate void OnSeaBattleSessionFinished(SeaBattleSession sender);
        public event OnSeaBattleSessionFinished SessionFinished;
        private IPlayerConnection _player1Connection { get; set; }
        private IPlayerConnection _player2Connection { get; set; }

        private readonly ILogger _logger;

        public SeaBattleSession(Guid id, IPlayerConnection player1Connection, IPlayerConnection player2Connection, ILogger logger)
        {
            var gameSessionFactory = new GameSessionFactory();
            var gameConfigReader = new GameConfigReader();

            var gameConfig = gameConfigReader.ReadConfig(GameMode.StandartGameMode);

            // заполненные карты нужно будет передать при подключении и выполнить их валидацию на сервере, пока такая затычка
            var product = gameSessionFactory.CreateGameSession(new GameMap(gameConfig), new GameMap(gameConfig));

            _gameSession = product.GameSession;
            _gameSession.GameSessionFinished += InvokeGameSessionFinished;

            Id = id;

            _player1Connection = player1Connection;
            _player2Connection = player2Connection;

            _player1Connection.GamePlayer = product.GamePlayer1;
            _player2Connection.GamePlayer = product.GamePlayer2;

            _player1Connection.MessageRecived += _player1Connection_MessageRecived;
            _player2Connection.MessageRecived += _player2Connection_MessageRecived;

            _player1Connection.PlayerDisconnected += OnPlayerDisconnected;
            _player2Connection.PlayerDisconnected += OnPlayerDisconnected;

            _logger = logger;

            _gameSession.Start();
        }

        private void OnPlayerDisconnected(IPlayerConnection sender)
        {
            _logger.LogInformation($"Сессия: {Id}, Игрок: {sender.Id}, Отключился.");

            //Console.WriteLine($"Сессия: {Id}, Игрок: {sender.Id}, Отключился.");
        }

        private void _player2Connection_MessageRecived(string message)
        {
            _logger.LogInformation($"Сессия: {Id}, Игрок: {_player2Connection.Id}, Сообщение: {message}");

            //Console.WriteLine($"Сессия: {Id}, Игрок: {_player2Connection.Id}, Сообщение: {message}");
        }

        private void _player1Connection_MessageRecived(string message)
        {
            _logger.LogInformation($"Сессия: {Id}, Игрок: {_player1Connection.Id}, Сообщение: {message}");

            //Console.WriteLine($"Сессия: {Id}, Игрок: {_player1Connection.Id}, Сообщение: {message}");
        }

        public bool IsFinished()
        {
            return _isFinished;
        }

        private void InvokeGameSessionFinished(IGameSession sender, IGamePlayer? winnerPlayer)
        {
            _isFinished = true;

            var message = $"Сессия: {Id} завершена";

            _player1Connection.SendMessage(message);
            _player2Connection.SendMessage(message);

            _logger.LogInformation(message);

            SessionFinished?.Invoke(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _player1Connection.MessageRecived -= _player1Connection_MessageRecived;
                    _player2Connection.MessageRecived -= _player2Connection_MessageRecived;

                    _player1Connection.PlayerDisconnected -= OnPlayerDisconnected;
                    _player2Connection.PlayerDisconnected -= OnPlayerDisconnected;

                    (_player1Connection as IDisposable)?.Dispose();
                    (_player2Connection as IDisposable)?.Dispose();
                    (_gameSession as IDisposable)?.Dispose();
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
