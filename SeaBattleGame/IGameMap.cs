namespace SeaBattleGame
{
    public interface IGameMap
    {
        bool TryAddShip(Ship ship);
        Ship? IsShipOnCell(GameCell gameCell);
        void Hit(GameCell gameCell);
    }
}
