using SeaBattleGame.Map;
using SeaBattleGame.Map.MapResponses;

namespace SeaBattleGame.Player
{
    public class GamePlayer : IGamePlayer
    {
        private string _id;
        private bool _gameStarted = false;

        public event IGamePlayer.OnHit Hit;
        public IGameMap GameMap { get; private set; }
        public void SetGameStarted()
        {
            _gameStarted = true;
        }
        public HitGameMapResponse MakeHitTurn(GameCell cellToHit)
        {
            if (_gameStarted)
            {
                var hitResponse = GameMap.Hit(cellToHit);

                // я хочу выстрелить сессия - обработай мой запрос 

                Hit?.Invoke(this, hitResponse);

                return hitResponse;
            }

            throw new Exception("Неправильное использование метода, нельзя произвести выстрел до того как игра началась");
        }

        public string GetId()
        {
            return _id;
        }
        public GamePlayer(IGameMap gameMap)
        {
            _id = Guid.NewGuid().ToString();

            GameMap = gameMap;
        }
    }
}
