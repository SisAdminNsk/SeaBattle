
namespace SeaBattle.Contracts
{
    public class PlayerTurnChangedResponse
    {
        public string CurrentTurnPlayerId { get; set; }

        public PlayerTurnChangedResponse(string currentTurnPlayerId) 
        {
            CurrentTurnPlayerId = currentTurnPlayerId;
        }
    }
}
