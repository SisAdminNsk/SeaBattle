using SeaBattleApi.Websockets;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace SeaBattleApi.Services
{
    public class PlayerConnectionsService : IPlayerConnectionsService
    {
        private static ConcurrentQueue<PlayerConnection> _playerConnections = new();
        public PlayerConnection? TryTakeFirstConnection()
        {
            var isSuccess = _playerConnections.TryDequeue(out var playerConnection);

            if (isSuccess)
            {
                return playerConnection;
            }

            return null;
        }
        public void AddNewConnection(WebSocket socket)
        {
            var playerConnection = new PlayerConnection(socket);

            _playerConnections.Enqueue(playerConnection);
        }
    }
}
