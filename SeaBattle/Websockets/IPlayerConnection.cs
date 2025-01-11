using SeaBattleGame.Player;

namespace SeaBattleApi.Websockets
{
    public interface IPlayerConnection
    {
        DateTime ConnectedAt { get; }
        Guid Id { get; }
        IGamePlayer? GamePlayer { get; set; }
        public delegate void OnMessageRecived(string message);
        public event OnMessageRecived MessageRecived;
        public delegate void OnPlayerDisconnected(IPlayerConnection sender);
        public event OnPlayerDisconnected PlayerDisconnected;
        Task ListenSocket();
        Task SendMessage<T>(T message);
    }
}
