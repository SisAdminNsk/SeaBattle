namespace SeaBattleGame.Map.MapResponses
{
    public class ShipsAddedResponse
    {
        public bool Success { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public List<ShipAddedResponse>? Ships { get; set; } = new();   
    }
}
