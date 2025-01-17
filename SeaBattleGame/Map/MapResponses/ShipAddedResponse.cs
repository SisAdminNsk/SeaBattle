namespace SeaBattleGame.Map.MapResponses
{
    public class ShipAddedResponse
    {
        public bool Success { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public Ship? Ship { get; set; }
        public List<GameCell> ShipLocation { get; set; }
        public GameCell StartCell { get; set; }
        public ShipOrientation ShipOrientation { get; set; }
        public ShipAddedResponse()
        {

        }
        public ShipAddedResponse(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public ShipAddedResponse(Ship ship, bool success, List<GameCell> shipLocation)
        {
            Success = success;
            Ship = ship;
            ShipLocation = shipLocation;
        }
    }
}
