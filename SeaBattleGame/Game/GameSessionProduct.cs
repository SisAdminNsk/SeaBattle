using SeaBattleGame.Player;

namespace SeaBattleGame.Game
{
    public class GameSessionProduct
    {
        public IGameSession GameSession { get; set; }
        public IGamePlayer GamePlayer1 { get; set; }
        public IGamePlayer GamePlayer2 { get; set; }

        public GameSessionProduct(IGameSession gameSession, IGamePlayer gamePlayer1, IGamePlayer gamePlayer2)
        {
            GameSession = gameSession;
            GamePlayer1 = gamePlayer1;
            GamePlayer2 = gamePlayer2;
        }
    }
}
