using System.Diagnostics.Contracts;
using System.Text.Json;

namespace SeaBattle.Contracts
{
    public class BasePlayerResponse
    {
        public string MessageType { get; set; }
        public object Response { get; set; }
        public BasePlayerResponse(string messageType, object response)
        {
            MessageType = messageType;
            Response = response;
        }
        public T GetResponse<T>()
        {
            return JsonSerializer.Deserialize<T>(Response.ToString());
        }
    }
}
