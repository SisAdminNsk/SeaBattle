using SeaBattleApi.Websockets;
using System.Net.WebSockets;

namespace SeaBattleApi.Services
{
    public interface IGameSessionService
    {
        SeaBattleSession? TryCreateGameSession(PlayerConnection player1Connection, PlayerConnection player2Connection);
    }
}
