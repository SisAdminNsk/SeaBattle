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
        ShipsAddedResponse TryAddShipsRandomly(List<Ship> ships);
        ShipAddedResponse TryAddShip(Ship ship, GameCell startPosition, ShipOrientation shipOrientation);
        ShipLocationChangedResponse TryChangeShipLocation(Ship ship, GameCell newStartPosition, ShipOrientation newShipOrientation);
        Ship? IsShipOnCell(GameCell gameCell);
        HitGameMapResponse Hit(GameCell gameCell);

        void Clear();
        bool IsShipDestroyed(Ship ship);
        List<GameCell> GetShipLocation(Ship ship);
    }
}
