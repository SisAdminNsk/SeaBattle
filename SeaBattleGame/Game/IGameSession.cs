using SeaBattleGame.Player;

using SeaBattleGame.Game.Responses;
using SeaBattleGame.Map.MapResponses;

namespace SeaBattleGame.Game
{
    public interface IGameSession : IDisposable
    {
        public bool IsFinished();

        delegate Task OnGameSessionStarted(IGameSession sender, List<IGamePlayer> players, IGamePlayer playerTurn);
        delegate Task OnPlayerTurnTimeHasPassed(IGameSession sender, IGamePlayer player);
        delegate Task OnGameSessionFinished(IGameSession sender, IGamePlayer? winnerPlayer, HitGameMapResponse? hitGameMapResponse);
        delegate Task OnPlayerHit(IGameSession sender, IGamePlayer player, PlayerHitResponse playerHitResponse);

        event OnGameSessionStarted GameSessionStarted;
        event OnGameSessionFinished GameSessionFinished;
        event OnPlayerTurnTimeHasPassed GameSessionTurnTimeHasPassed;
        event OnPlayerHit PlayerHit;
        IGamePlayer GetCurrentTurnPlayer();
        void Start();
        void Stop();
    }
}
