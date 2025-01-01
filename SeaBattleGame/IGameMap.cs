namespace SeaBattleGame
{
    public enum ShipOrientation
    {
        Vertical = 0, 
        Horizontal = 1
    }
    public interface IGameMap
    {
        delegate void OnShipHitted(Ship? ship, IGameMap sender, GameCell hittedCell);
        delegate void OnShipDestroyed(Ship? ship, IGameMap sender, GameCell lastHittedCell);
        delegate void OnHitMissed(IGameMap sender, GameCell hittedCell);
        delegate void OnAlreadyHitted(IGameMap sender, GameCell hittedCell);
        delegate void OnShipAdded(Ship ship, IGameMap sender);
        delegate void OnShipLocationChanged(Ship ship, IGameMap sender);
        delegate void OnAllShipsDestroyed(List<Ship> destroyedShips, IGameMap sender);

        event OnShipHitted? ShipHitted;
        event OnShipDestroyed? ShipDestroyed;
        event OnHitMissed? HitMissed;
        event OnAlreadyHitted? AreadyHitted;
        event OnShipAdded? ShipAdded;
        event OnShipLocationChanged? ShipLocationChanged;
        event OnAllShipsDestroyed? AllShipsDestroyed;
        bool TryAddShip(Ship ship, GameCell startPosition, ShipOrientation shipOrientation);
        bool ChangeShipLocation(Ship ship, GameCell newStartPosition, ShipOrientation newShipOrientation);
        Ship? IsShipOnCell(GameCell gameCell);
        void Hit(GameCell gameCell);
    }
}
