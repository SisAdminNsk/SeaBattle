using SeaBattleGame.Map.MapResponses;

namespace SeaBattleGame.Player
{
    public interface IGamePlayer
    {
        delegate void OnHit(IGamePlayer sender, GameCell cellToHit);
        event OnHit Hit;
        void SetGameStarted();
        string GetId();
        void RequestMakeHit(GameCell cellToHit);
    }
}
