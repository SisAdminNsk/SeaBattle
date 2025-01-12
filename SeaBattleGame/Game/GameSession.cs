using SeaBattleGame.Game.GameResponses;
using SeaBattleGame.GameConfig;
using SeaBattleGame.Map;
using SeaBattleGame.Map.MapResponses;
using SeaBattleGame.Player;
using System.Timers;

namespace SeaBattleGame.Game
{
    public class GameSession : IGameSession, IDisposable
    {
        bool _isDisposed = false;
        bool _isFinished = false;

        public int MaxSessionDurationInMsc { get; private set; } = 20000;
        public int MaxTurnDurationInMsc { get; private set; } = 1000;
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
        public event IGameSession.OnPlayerHit PlayerHit;

        public GameSession(GameSessionArgs gameSessionArgs) 
        {
            _player1Map = gameSessionArgs.Player1Args.GameMap;
            _player2Map = gameSessionArgs.Player2Args.GameMap;

            _player1 = gameSessionArgs.Player1Args.GamePlayer;
            _player2 = gameSessionArgs.Player2Args.GamePlayer;

            _gameTimer = new System.Timers.Timer(MaxSessionDurationInMsc);
            _gameTimer.Elapsed += GameSessionTimeHasPassed;

            _playerTurnTimer = new System.Timers.Timer(MaxTurnDurationInMsc);
            _playerTurnTimer.Interval = MaxTurnDurationInMsc;
            _playerTurnTimer.Elapsed += PlayerTurnTimeHasPassed;

            _player1Map.AllShipsDestroyed += AllShipsDestroyed;
            _player2Map.AllShipsDestroyed += AllShipsDestroyed;
        }

        public bool IsFinished()
        {
            return _isFinished;
        }

        private void AllShipsDestroyed(IGameMap sender)
        {
            if (sender.Equals(_player1Map))
            {
                Stop(_player2);
            }
            else
            {
                Stop(_player1);
            }
        }

        private void PlayerTurnTimeHasPassed(object? sender, ElapsedEventArgs e)
        {
            var oldPlayerTurnId = PlayerIdTurn;

            ChangePlayerTurn();

            _playerTurnTimer.Stop();
            _playerTurnTimer.Start();

            GameSessionTurnTimeHasPassed?.Invoke(this, GetPlayerById(oldPlayerTurnId));
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
            var response = new PlayerHitResponse();

            if(sender.GetId() != PlayerIdTurn)
            {
                response.ErrorMessage = "Сейчас ходит другой игрок.";

                PlayerHit?.Invoke(this, sender, response);

                return;
            }

            var hitResponse = new HitGameMapResponse();

            if (sender.Equals(_player1))
            {
                hitResponse = _player2Map.Hit(gameCell);
            }
            else
            {
                hitResponse = _player1Map.Hit(gameCell);
            }

            response.Success = true;
            response.HitGameMapResponse = hitResponse;
            response.PlayerTurnId = PlayerIdTurn;

            if (!_isDisposed)
            {
                if (hitResponse.HitStatus == HitStatus.Missed)
                {
                    ChangePlayerTurn();

                    _playerTurnTimer.Stop();
                    _playerTurnTimer.Start();
                }

                if (hitResponse.HitStatus == HitStatus.Hitted)
                {
                    _playerTurnTimer.Stop();
                    _playerTurnTimer.Interval = MaxTurnDurationInMsc;
                    _playerTurnTimer.Start();
                }

                PlayerHit?.Invoke(this, sender, response);
            }
        }

        public IGamePlayer GetCurrentTurnPlayer()
        {
            return GetPlayerById(PlayerIdTurn);
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

        public void Stop(IGamePlayer winnerPlayer)
        {
            _isFinished = true;

            Dispose();

            GameSessionFinished?.Invoke(this, winnerPlayer);
        }

        public void Stop()
        {
            _isFinished = true;

            Dispose();

            GameSessionFinished?.Invoke(this, null);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            { 
                _gameTimer.Dispose();
                _playerTurnTimer.Dispose();

                _isDisposed = true;
            }
        }
    }
}
