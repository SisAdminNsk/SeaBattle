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
        delegate void OnAllShipsDestroyed(IGameMap sender);
        event OnAllShipsDestroyed AllShipsDestroyed;
        Dictionary<GameCell, Ship?> GetCellsToShipMap();
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
