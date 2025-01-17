using SeaBattleApi.Websockets;
using SeaBattleGame.Map;
using System.Net.WebSockets;

namespace SeaBattleApi.Services
{
    public interface IPlayerConnectionsService
    {
        PlayerConnection AddNewConnection(WebSocket socket, IGameMap gameMap);
        PlayerConnection? TryTakeFirstConnection();
    }
}
