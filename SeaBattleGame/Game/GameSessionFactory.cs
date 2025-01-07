using SeaBattleGame.Map;
using SeaBattleGame.Player;

namespace SeaBattleGame.Game
{
    public class GameSessionFactory : IGameSessionFactory
    {
        public GameSessionProduct CreateGameSession(IGameMap player1GameMap, IGameMap player2GameMap)
        {
            IGamePlayer player1 = new GamePlayer();
            IGamePlayer player2 = new GamePlayer();

            IGameSession gameSession = new GameSession
            (
                new GameSessionArgs
                (
                    new PlayerArgs(player1GameMap, player1),
                    new PlayerArgs(player2GameMap, player2)
                )
            );

            return new GameSessionProduct(gameSession, player1, player2);
        }
    }
}
