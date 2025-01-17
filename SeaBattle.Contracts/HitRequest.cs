using SeaBattleGame;

namespace SeaBattle.Contracts
{
    public class HitRequest : BasePlayerRequest
    {
        public GameCell CellToHit { get; set; }

        public HitRequest(GameCell cellToHit)
        {
            MessageType = "Hit";
            CellToHit = cellToHit;
        }
    }
}
