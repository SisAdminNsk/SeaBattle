using SeaBattle.Contracts;
using System.Net.WebSockets;

namespace SeaBattleConsoleClient.ServerCommunication
{
    public interface IPlayerConnection
    {
        Task SendMessageAsync<T>(T request) where T : BasePlayerRequest;
        Task CloseConnectionAsync();
        Task ReceiveMessagesAsync(ClientWebSocket client);

        delegate void OnMessageRecived(BasePlayerResponse message);
        event OnMessageRecived MessageRecived;

        delegate void OnServerCloseConnection(string message);
        event OnServerCloseConnection ServerCloseConnection;
    }
}
