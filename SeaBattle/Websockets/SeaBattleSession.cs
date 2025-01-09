using SeaBattleGame.Game;

namespace SeaBattleApi.Websockets
{
    public class SeaBattleSession
    {
        private IGameSession _gameSession { get; set; }
        public PlayerConnection Player1Connection { get; set; }
        public PlayerConnection Player2Connection { get; set; }
        public SeaBattleSession(PlayerConnection player1Connection, PlayerConnection player2Connection)
        {
            Player1Connection = player1Connection;
            Player2Connection = player2Connection;
        }
    }
}
