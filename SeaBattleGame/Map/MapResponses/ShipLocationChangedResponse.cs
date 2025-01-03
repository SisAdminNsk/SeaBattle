namespace SeaBattleGame.Map.MapResponses
{
    public class ShipLocationChangedResponse
    {
        public bool Success { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public Ship? Ship { get; set; }
        public List<GameCell> OldLocation { get; set; }
        public List<GameCell> NewLocation { get; set; }

        public ShipLocationChangedResponse(
            bool success, string? errorMessage, Ship ship, List<GameCell> newLocation, List<GameCell> oldLocation)
        {
            Success = success;
            ErrorMessage = errorMessage;
            Ship = ship;
            NewLocation = newLocation;
            OldLocation = oldLocation;
        }

        public ShipLocationChangedResponse()
        {

        }
    }
}
