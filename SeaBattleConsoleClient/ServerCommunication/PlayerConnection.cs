using SeaBattle.Contracts;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using static SeaBattleConsoleClient.ServerCommunication.IPlayerConnection;

namespace SeaBattleConsoleClient.ServerCommunication
{
    public class PlayerConnection : IPlayerConnection
    {
        private ClientWebSocket _socket;

        public event OnMessageRecived MessageRecived;
        public event OnServerCloseConnection ServerCloseConnection;

        public async Task CloseConnectionAsync()
        {
            await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Закрытие соединения", CancellationToken.None);
        }

        public async Task ReceiveMessagesAsync(ClientWebSocket client)
        {
            var receiveBuffer = new ArraySegment<byte>(new byte[1024]);

            while (client.State == WebSocketState.Open)
            {
                try
                {
                    var result = await client.ReceiveAsync(receiveBuffer, CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Закрытие соединения", CancellationToken.None);

                        ServerCloseConnection?.Invoke("Соединение закрыто сервером.");

                        Console.WriteLine("Соединение закрыто сервером.");
                    }
                    else
                    {
                        var receivedMessage = Encoding.UTF8.GetString(receiveBuffer.Array, 0, result.Count);

                        var response = TryDeserializeServerResponse(receivedMessage);

                        MessageRecived?.Invoke(response);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при получении сообщения: {ex.Message}");
                    break;
                }
            }
        }
        private BasePlayerResponse? TryDeserializeServerResponse(string response)
        {
            BasePlayerResponse? playerResponse = null;

            try
            {
                playerResponse = JsonSerializer.Deserialize<BasePlayerResponse>(response);
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine($"Ошибка десериализации запроса от сервера, ошибка: {ex.Message}");

                throw new JsonException(ex.Message);
            }

            return playerResponse;
        }

        public async Task SendMessageAsync<T>(T request) where T : BasePlayerRequest
        {
            var jsonData = JsonSerializer.Serialize(request);
            var byteArray = Encoding.UTF8.GetBytes(jsonData);

            var arraySegment = new ArraySegment<byte>(byteArray);

            await _socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public PlayerConnection(ClientWebSocket socket)
        {
            _socket = socket;
        }
    }
}
