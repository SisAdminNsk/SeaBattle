using SeaBattleGame.Game;
using SeaBattleGame.GameConfig;
using SeaBattleGame.Map;
using SeaBattleGame.Player;
using System.Net.WebSockets;

namespace SeaBattleApi.Websockets
{
    public class SeaBattleSession
    {
        public Guid Id { get; private set; }
        private IGameSession _gameSession { get; set; }

        private bool _isFinished = false;

        public delegate void OnSeaBattleSessionFinished(SeaBattleSession sender);
        public event OnSeaBattleSessionFinished SessionFinished;
        private PlayerConnection _player1Connection { get; set; }
        private PlayerConnection _player2Connection { get; set; }

        public SeaBattleSession(Guid id, PlayerConnection player1Connection, PlayerConnection player2Connection)
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

            _gameSession.Start();
        }

        public bool IsFinished()
        {
            return _isFinished;
        }

        private void InvokeGameSessionFinished(IGameSession sender, IGamePlayer? winnerPlayer)
        {
            _isFinished = true;

            SessionFinished?.Invoke(this);
        }

        
    }
}
