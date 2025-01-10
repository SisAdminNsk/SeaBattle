using SeaBattleGame.Player;
using System.Net.WebSockets;
using System.Text;

namespace SeaBattleApi.Websockets
{
    public class PlayerConnection
    {
        public delegate void OnMessageRecived(string message);
        public event OnMessageRecived MessageRecived;
        public DateTime ConnectedAt { get; private set; }
        public Guid Id { get; private set; }

        private WebSocket _socket;

        private TaskCompletionSource<PlayerConnection> _completionSource = new();
        public Task<PlayerConnection> Completion => _completionSource.Task;
        public IGamePlayer? GamePlayer { get; set; }

        public PlayerConnection(WebSocket socket)
        {
            _socket = socket;

            Id = Guid.NewGuid();

            ConnectedAt = DateTime.Now;
        }

        public async Task ListenSocket()
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (_socket.State == WebSocketState.Open)
                {
                    var result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await CloseConnection();
                        break;
                    }
                    else if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                        MessageRecived?.Invoke(message);
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
            }
        }
        private async Task CloseConnection()
        {
            if (_socket.State == WebSocketState.Open)
            {
                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the server", CancellationToken.None);
            }
        }

        public async Task SendMessage<T>(T message)
        {
            var arraySegment = _socket.ToArraySegment(message);

            await _socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
