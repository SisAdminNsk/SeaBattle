using System.Diagnostics.Contracts;

namespace SeaBattle.Contracts
{
    public class BasePlayerResponse
    {
        public string MessageType { get; set; }
        public BasePlayerResponse(string messageType)
        {
            MessageType = messageType;
        }
    }
}
