using SeaBattleGame.Map.MapResponses;

namespace SeaBattleGame.Game.Responses
{
    public class PlayerHitResponse
    {
        public bool Success { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public string PlayerTurnId { get; set; }
        public HitGameMapResponse HitGameMapResponse { get; set; }

        public PlayerHitResponse(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public PlayerHitResponse() { }
    }
}
