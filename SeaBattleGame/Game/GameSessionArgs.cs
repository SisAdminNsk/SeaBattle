using SeaBattleGame.GameConfig;

namespace SeaBattleGame.Game
{
    public class GameSessionArgs
    {
        public PlayerArgs Player1Args { get; set; }
        public PlayerArgs Player2Args { get; set; }
        public GameSessionArgs(PlayerArgs player1Args, PlayerArgs player2Args)
        {
            Player1Args = player1Args;
            Player2Args = player2Args;
        }
    }
}
