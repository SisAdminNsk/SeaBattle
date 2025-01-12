using SeaBattleApi.Websockets;

namespace SeaBattleApi.Services
{
    public interface IGameSessionService
    {
        Guid TryStartGameSession(PlayerConnection player1Connection, PlayerConnection player2Connection, ILogger logger);
    }
}
