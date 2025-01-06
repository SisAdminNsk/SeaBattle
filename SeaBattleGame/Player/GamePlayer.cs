using SeaBattleGame.Map;

namespace SeaBattleGame.Player
{
    public class GamePlayer : IGamePlayer
    {
        private string _id;
        private bool _gameStarted = false;

        public event IGamePlayer.OnHit Hit;
        public void SetGameStarted()
        {
            _gameStarted = true;
        }
        public void RequestMakeHit(GameCell cellToHit)
        {
            if (_gameStarted)
            {
                Hit?.Invoke(this, cellToHit);
            }
            else
            {
                throw new Exception("Неправильное использование метода, нельзя произвести выстрел до того как игра началась");
            }

        }

        public string GetId()
        {
            return _id;
        }

        public GamePlayer()
        {
            _id = Guid.NewGuid().ToString();
        }
    }
}
