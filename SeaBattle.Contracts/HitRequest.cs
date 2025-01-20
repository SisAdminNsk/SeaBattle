using SeaBattleGame;

namespace SeaBattle.Contracts
{
    public class HitRequest
    {
        public GameCell CellToHit { get; set; }
        public HitRequest(GameCell cellToHit)
        {
            CellToHit = cellToHit;
        }
    }
}
