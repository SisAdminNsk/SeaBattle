using SeaBattleGame.Game.GameResponses;
using SeaBattleGame.Player;

namespace SeaBattleGame.Game
{
    public interface IGameSession : IDisposable
    {
        public bool IsFinished();

        delegate void OnGameSessionStarted(IGameSession sender, List<IGamePlayer> players);
        delegate void OnPlayerTurnTimeHasPassed(IGameSession sender, IGamePlayer player);
        delegate void OnGameSessionFinished(IGameSession sender, IGamePlayer? winnerPlayer);
        delegate void OnPlayerHit(IGameSession sender, IGamePlayer player, PlayerHitResponse playerHitResponse);

        event OnGameSessionStarted GameSessionStarted;
        event OnGameSessionFinished GameSessionFinished;
        event OnPlayerTurnTimeHasPassed GameSessionTurnTimeHasPassed;
        event OnPlayerHit PlayerHit;
        IGamePlayer GetCurrentTurnPlayer();
        void Start();
        void Stop();
    }
}
