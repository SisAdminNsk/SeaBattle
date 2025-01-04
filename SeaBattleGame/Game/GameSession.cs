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

        private IGameMap _player1Map;
        private IGameMap _player2Map;

        private readonly IGamePlayer _player1;
        private readonly IGamePlayer _player2;

        public event IGameSession.OnGameSessionStarted GameSessionStarted;
        public event IGameSession.OnGameSessionFinished GameSessionFinished;
        public event IGameSession.OnPlayerTurnTimeHasPassed GameSessionTurnTimeHasPassed;
        public GameSession(IGamePlayer player1, IGamePlayer player2) 
        {
            _player1Map = new GameMap(10);
            _player2Map = new GameMap(10);

            _player1 = player1;
            _player2 = player2;

            _player1.SetGameStarted();
            _player2.SetGameStarted();

            _player1.Hit += OnPlayerHit;
            _player2.Hit += OnPlayerHit;

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

        private void OnPlayerHit(IGamePlayer sender, Map.MapResponses.HitGameMapResponse hitGameMapResponse)
        {
            ChangePlayerTurn();

            if(hitGameMapResponse.HittedShip != null && hitGameMapResponse.HittedShip.Killed)
            {
                
            }


            _playerTurnTimer.Stop();
            _playerTurnTimer.Start();
        }

        public void Start()
        {
            PlayerIdTurn = _player1.GetId();

            GameSessionStarted?.Invoke(this, new List<IGamePlayer> { _player1, _player2 });
        }

        public void Stop()
        {
            GameSessionFinished?.Invoke(this, null);
        }
    }
}
