using System.Text.Json;

namespace SeaBattle.Contracts
{
    public class BasePlayerRequest
    {
        public string MessageType { get; set; }
        public object Request { get; set; }

        public BasePlayerRequest(string messageType, object request)
        {
            MessageType = messageType;
            Request = request;
        }
        public T GetRequest<T>()
        {
            return JsonSerializer.Deserialize<T>(Request.ToString());
        }
    }
}
