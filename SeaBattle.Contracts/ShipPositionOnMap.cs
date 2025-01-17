using SeaBattleGame;
using SeaBattleGame.Map;

namespace SeaBattle.Contracts
{
    public class ShipPositionOnMap
    {
        public Ship ship;
        public GameCell startPosition;
        public ShipOrientation shipOrientation;

        public ShipPositionOnMap(Ship ship, GameCell startPosition, ShipOrientation shipOrientation)
        {
            this.ship = ship;
            this.startPosition = startPosition;
            this.shipOrientation = shipOrientation;
        }
    }
}
