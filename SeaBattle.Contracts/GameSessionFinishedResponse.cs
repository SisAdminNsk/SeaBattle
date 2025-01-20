namespace SeaBattle.Contracts
{
    public class GameSessionFinishedResponse
    {
        public string SessionId { get; set; }
        public string WinnerPlayerId { get; set; }
        public bool IsDraw { get; set; } = false;
        public string Message { get; set; }
        public GameSessionFinishedResponse(string sessionId,string winnerPlayerId, string message) 
        {
            SessionId = sessionId;
            WinnerPlayerId = winnerPlayerId;
            Message = message;
        }

        public GameSessionFinishedResponse()
        {

        }
    }
}
