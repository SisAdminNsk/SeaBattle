using SeaBattleApi.Websockets;
using System.Net.WebSockets;

namespace SeaBattleApi.Services
{
    public interface IPlayerConnectionsService
    {
        void AddNewConnection(WebSocket socket);
        PlayerConnection? TryTakeFirstConnection();
    }
}
