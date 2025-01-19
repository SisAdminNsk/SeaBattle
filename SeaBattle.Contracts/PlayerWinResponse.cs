namespace SeaBattle.Contracts
{
    public class PlayerWinResponse
    {
        public string? WinnerPlayerId { get; set; } 

        public PlayerWinResponse(string? winnerPlayerId)
        {
            WinnerPlayerId = winnerPlayerId;
        }   
    }
}
