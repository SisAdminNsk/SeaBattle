using SeaBattleGame.Map;

namespace SeaBattleGame.Game
{
    public interface IGameSessionFactory
    {
        GameSessionProduct CreateGameSession(IGameMap player1GameMap, IGameMap player2GameMap);
    }
}
