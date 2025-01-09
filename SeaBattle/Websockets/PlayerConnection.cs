using System.Net.WebSockets;

namespace SeaBattleApi.Websockets
{
    public class PlayerConnection
    {
        public DateTime ConnectedAt { get; set; }
        public Guid Id { get; private set; }
        public WebSocket Socket { get; set; }

        public PlayerConnection(WebSocket socket)
        {
            Socket = socket;

            Id = Guid.NewGuid();

            ConnectedAt = DateTime.Now;
        }
    }
}
