using SeaBattleGame;
using SeaBattleGame.Map.MapResponses;

namespace SeaBattle.Contracts
{
    public class GameSessionFinishedResponse
    {
        public string SessionId { get; set; }
        public string WinnerPlayerId { get; set; }
        public bool IsDraw { get; set; } = false;
        public string Message { get; set; }
        public HitGameMapResponse? HitGameMapResponse { get; set; }
        public GameSessionFinishedResponse(string sessionId,string winnerPlayerId, string message, HitGameMapResponse? hitGameMapResponse) 
        {
            SessionId = sessionId;
            WinnerPlayerId = winnerPlayerId;
            Message = message;
            HitGameMapResponse = hitGameMapResponse;
        }

        public GameSessionFinishedResponse()
        {

        }
    }
}
