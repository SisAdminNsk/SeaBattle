using SeaBattleGame.Player;

namespace SeaBattleGame.Game
{
    public interface IGameSession
    {
        delegate void OnGameSessionStarted(IGameSession sender, List<IGamePlayer> players);
        delegate void OnPlayerTurnTimeHasPassed(IGameSession sender, IGamePlayer player);
        delegate void OnGameSessionFinished(IGameSession sender, IGamePlayer? winnerPlayer);

        event OnGameSessionStarted GameSessionStarted;
        event OnGameSessionFinished GameSessionFinished;
        event OnPlayerTurnTimeHasPassed GameSessionTurnTimeHasPassed;
        void Start();
        void Stop();
    }
}
