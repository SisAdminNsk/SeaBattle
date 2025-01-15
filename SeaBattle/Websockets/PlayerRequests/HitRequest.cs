using SeaBattleGame;

namespace SeaBattleApi.Websockets.PlayerRequests
{
    public class HitRequest : BasePlayerRequest
    {
        public GameCell CellToHit { get; set; }
    }
}
