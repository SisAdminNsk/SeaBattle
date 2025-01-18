namespace SeaBattle.Contracts
{
    public class GameSessionStartedResponse : BasePlayerResponse
    {
        public string SessionId { get; set; }
        public string YourId { get; set; }
        public string OpponentId { get; set; }
        public string PlayerTurnId { get; set; }

        public GameSessionStartedResponse(string sessionId, string yourId, string opponentId, string playerTurnId) : base("SessionStarted")
        {
            YourId = yourId;
            SessionId = sessionId;
            OpponentId = opponentId;
            PlayerTurnId = playerTurnId;
        }
        public GameSessionStartedResponse() : base("SessionStarted")
        {

        }
    }
}
