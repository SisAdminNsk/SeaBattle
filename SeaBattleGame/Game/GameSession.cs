using SeaBattleGame.GameConfig;
using SeaBattleGame.Map;
using SeaBattleGame.Player;
using System.Timers;

namespace SeaBattleGame.Game
{
    public class GameSession : IGameSession
    {
        public int MaxSessionDurationInSeconds { get; private set; } = 600;
        public int MaxTurnDurationInSeconds { get; private set; } = 15;
        public string PlayerIdTurn { get; private set; }

        private System.Timers.Timer _gameTimer;
        private System.Timers.Timer _playerTurnTimer;

        private GameModeConfig _gameModeConfig;

        private IGameMap _player1Map;
        private IGameMap _player2Map;

        private readonly IGamePlayer _player1;
        private readonly IGamePlayer _player2;

        public event IGameSession.OnGameSessionStarted GameSessionStarted;
        public event IGameSession.OnGameSessionFinished GameSessionFinished;
        public event IGameSession.OnPlayerTurnTimeHasPassed GameSessionTurnTimeHasPassed;

        public GameSession(GameSessionArgs gameSessionArgs) 
        {
            _player1Map = gameSessionArgs.Player1Args.GameMap;
            _player2Map = gameSessionArgs.Player2Args.GameMap;

            _player1 = gameSessionArgs.Player1Args.GamePlayer;
            _player2 = gameSessionArgs.Player2Args.GamePlayer;

            _gameTimer = new System.Timers.Timer(MaxSessionDurationInSeconds);
            _gameTimer.Elapsed += GameSessionTimeHasPassed;

            _playerTurnTimer = new System.Timers.Timer(MaxTurnDurationInSeconds);
            _playerTurnTimer.AutoReset = true;
            _playerTurnTimer.Elapsed += PlayerTurnTimeHasPassed;          
        }
        private void PlayerTurnTimeHasPassed(object? sender, ElapsedEventArgs e)
        {
            GameSessionTurnTimeHasPassed.Invoke(this, GetPlayerById(PlayerIdTurn));

            ChangePlayerTurn();
        }

        private IGamePlayer GetPlayerById(string id)
        {
            if(_player1.GetId() == id)
            {
                return _player1;
            }

            return _player2;
        }

        private void ChangePlayerTurn()
        {
            if (PlayerIdTurn == _player1.GetId())
            {
                PlayerIdTurn = _player2.GetId();
            }
            else
            {
                PlayerIdTurn = _player1.GetId();
            }
        }

        private void GameSessionTimeHasPassed(object? sender, ElapsedEventArgs e)
        {
            Stop();
        }

        private void OnPlayerHit(IGamePlayer sender, GameCell gameCell)
        {
            ChangePlayerTurn();

            // произвести выстерл по карте чужого игрока != sender


            _playerTurnTimer.Stop();
            _playerTurnTimer.Start();
        }

        public void Start()
        {
            _player1.SetGameStarted();
            _player2.SetGameStarted();

            _player1.Hit += OnPlayerHit;
            _player2.Hit += OnPlayerHit;

            PlayerIdTurn = _player1.GetId();

            _gameTimer.Start();
            _playerTurnTimer.Start();

            GameSessionStarted?.Invoke(this, new List<IGamePlayer> { _player1, _player2 });
        }

        public void Stop()
        {
            GameSessionFinished?.Invoke(this, null);
        }
    }
}
