using SeaBattleGame.Map.MapResponses;

namespace SeaBattleGame.Player
{
    public interface IGamePlayer
    {
        delegate void OnHit(IGamePlayer sender, HitGameMapResponse hitGameMapResponse);
        event OnHit Hit;

        void SetGameStarted();
        string GetId();
        HitGameMapResponse MakeHitTurn(GameCell cellToHit);
    }
}
