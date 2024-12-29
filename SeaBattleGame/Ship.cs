namespace SeaBattleGame
{
    public class Ship
    {
        private List<GameCell> _body = new();
        public bool Killed { get; set; }
        public Ship(GameCell startCell)
        {
            _body.Add(startCell);
        }
        public Ship(GameCell startCell, GameCell endCell)
        {
            _body.AddRange([startCell, endCell]);
        }
    }
}