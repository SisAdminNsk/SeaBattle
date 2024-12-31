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

        event OnShipHitted? ShipHitted;
        event OnShipDestroyed? ShipDestroyed;
        bool TryAddShip(Ship ship, GameCell startPosition, ShipOrientation shipOrientation);
        bool ChangeShipLocation(Ship ship, GameCell newStartPosition, ShipOrientation newShipOrientation);
        Ship? IsShipOnCell(GameCell gameCell);
        void Hit(GameCell gameCell);
    }
}
