using SeaBattleApi.Websockets;

namespace SeaBattleApi.Services
{
    public class GameSessionService : IGameSessionService
    {
        public SeaBattleSession? TryCreateGameSession(PlayerConnection player1Connection, PlayerConnection player2Connection)
        {
            return new SeaBattleSession
            (
                player1Connection,
                player2Connection
            );
        }
    }
}
