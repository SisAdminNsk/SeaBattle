using SeaBattleApi.Websockets.PlayerRequests;
using SeaBattleGame.Player;

namespace SeaBattleApi.Websockets
{
    public interface IPlayerConnection : IDisposable
    {
        bool IsDisconnected();
        DateTime ConnectedAt { get; }
        Guid Id { get; }
        IGamePlayer? GamePlayer { get; set; }
        delegate void OnMessageRecived(BasePlayerRequest message);
        event OnMessageRecived MessageRecived;
        delegate void OnPlayerDisconnected(IPlayerConnection sender);
        event OnPlayerDisconnected PlayerDisconnected;
        Task CloseConnection();
        Task ListenSocket();
        Task SendMessage<T>(T message);
    }
}
