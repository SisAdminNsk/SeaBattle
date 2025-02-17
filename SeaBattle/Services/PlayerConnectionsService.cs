﻿using SeaBattleApi.Websockets;
using SeaBattleGame.Map;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace SeaBattleApi.Services
{
    public class PlayerConnectionsService : IPlayerConnectionsService
    {
        private static ConcurrentDictionary<Guid, PlayerConnection> _playerConnections = new();
        public PlayerConnection? TryTakeFirstConnection()
        {
            var firstConnection = _playerConnections.Values
                .OrderBy(pc => pc.ConnectedAt) 
                .FirstOrDefault();

            if (firstConnection != null)
            {
                _playerConnections.TryRemove(firstConnection.Id, out _);
            }
            return firstConnection;
        }

        public PlayerConnection AddNewConnection(WebSocket socket, IGameMap gameMap)
        {
            var playerConnection = new PlayerConnection(socket, gameMap);

            _playerConnections.TryAdd(playerConnection.Id, playerConnection);

            playerConnection.Completion.ContinueWith(t => _playerConnections.TryRemove(playerConnection.Id, out _));

            return playerConnection;
        }
    }
}
