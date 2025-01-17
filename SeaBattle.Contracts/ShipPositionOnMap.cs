using SeaBattleGame;
using SeaBattleGame.Map;

namespace SeaBattle.Contracts
{
    public class ShipPositionOnMap
    {
        public Ship Ship { get; set; }
        public GameCell StartPosition { get; set; }
        public ShipOrientation ShipOrientation { get; set; }

        public ShipPositionOnMap(Ship ship, GameCell startPosition, ShipOrientation shipOrientation)
        {
            Ship = ship;
            StartPosition = startPosition;
            ShipOrientation = shipOrientation;
        }
    }
}
