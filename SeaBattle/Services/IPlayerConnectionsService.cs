using SeaBattleApi.Websockets;
using System.Net.WebSockets;

namespace SeaBattleApi.Services
{
    public interface IPlayerConnectionsService
    {
        PlayerConnection AddNewConnection(WebSocket socket);
        PlayerConnection? TryTakeFirstConnection();
    }
}
