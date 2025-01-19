namespace SeaBattle.Contracts
{
    public class BasePlayerRequest
    {
        public string MessageType { get; set; }
        public object Request { get; set; }

        public BasePlayerRequest(string type, object request)
        {
            MessageType = type;
            Request = request;
        }
    }
}
