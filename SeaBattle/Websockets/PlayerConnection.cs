using SeaBattleApi.Websockets.PlayerRequests;
using SeaBattleGame.Player;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace SeaBattleApi.Websockets
{
    public class PlayerConnection : IPlayerConnection
    {
        public event IPlayerConnection.OnMessageRecived MessageRecived;
        public event IPlayerConnection.OnPlayerDisconnected PlayerDisconnected;

        private bool _disconnected = false;
        private DateTime _connectedAt;
        private Guid _id;

        private WebSocket _socket;

        private TaskCompletionSource<PlayerConnection> _completionSource = new();
        public Task<PlayerConnection> Completion => _completionSource.Task;
        public IGamePlayer? GamePlayer { get; set; }

        public Guid Id => _id;

        public DateTime ConnectedAt => _connectedAt;

        public PlayerConnection(WebSocket socket)
        {
            _socket = socket;

            _id = Guid.NewGuid();
            _connectedAt = DateTime.Now;
        }

        public async Task ListenSocket()
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (_socket.State == WebSocketState.Open)
                {
                    if(IsPlayingGameSession()){

                        var result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await CloseConnection();
                            break;
                        }
                        else if (result.MessageType == WebSocketMessageType.Text)
                        {
                            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                            BasePlayerRequest? playerRequest = null;

                            try
                            {
                                playerRequest = JsonSerializer.Deserialize<BasePlayerRequest>(message);
                            }
                            catch (JsonException ex)
                            {
                                Console.Error.WriteLine($"Ошибка десериализации запроса от игрока {_id}, ошибка: {ex.Message}");
                            }

                            if (playerRequest != null)
                            {
                                MessageRecived?.Invoke(playerRequest);
                            }
                        }
                    }          
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                await CloseConnection();
                _completionSource.SetResult(this);

                PlayerDisconnected?.Invoke(this);
            }
        }
        public async Task SendMessage<T>(T message)
        {
            var arraySegment = _socket.ToArraySegment(message);

            await _socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
        public async Task CloseConnection()
        {
            if (_socket.State == WebSocketState.Open)
            {
                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the server", CancellationToken.None);

                _disconnected = true;
            }
        }

        public bool IsPlayingGameSession()
        {
            if(GamePlayer is null)
            {
                return false;
            }

            return true;
        }

        public bool IsDisconnected()
        {
            return _disconnected;
        }
        public void Dispose()
        {
            if (!_disconnected)
            {
                CloseConnection().Wait();
            }
        }
    }
}
