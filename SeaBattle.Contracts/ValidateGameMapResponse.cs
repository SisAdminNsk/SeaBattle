namespace SeaBattle.Contracts
{
    public class ValidateGameMapResponse
    {
        public bool Success { get; set; } = false;
        public string? AccessToken { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
