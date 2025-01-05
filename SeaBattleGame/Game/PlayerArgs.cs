using SeaBattleGame.Map;
using SeaBattleGame.Player;

namespace SeaBattleGame.Game
{
    public class PlayerArgs
    {
        public IGameMap GameMap { get; set; }
        public IGamePlayer GamePlayer { get; set; }
        public PlayerArgs(IGameMap gameMap, IGamePlayer gamePlayer)
        {
            GameMap = gameMap;
            GamePlayer = gamePlayer;
        }
    }
}
