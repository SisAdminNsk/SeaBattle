using SeaBattleGame.Map.MapResponses;

namespace SeaBattleGame.Map
{
    public enum ShipOrientation
    {
        Vertical = 0,
        Horizontal = 1
    }
    public interface IGameMap
    {
        bool TryAddShip(Ship ship, GameCell startPosition, ShipOrientation shipOrientation);
        bool TryChangeShipLocation(Ship ship, GameCell newStartPosition, ShipOrientation newShipOrientation);
        Ship? IsShipOnCell(GameCell gameCell);
        HitGameMapResponse Hit(GameCell gameCell);
    }
}
