namespace SeaBattleGame.Map.MapResponses
{
    public enum HitStatus
    {
        Hitted = 0,
        Missed = 1,
        AlreadyHitted = 2
    }
    public class HitGameMapResponse
    {
        public bool Success { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public GameCell HittedCell { get; set; }
        public HitStatus HitStatus { get; set; }
        public Ship? HittedShip { get; set; }

        public HitGameMapResponse()
        {

        }
        public HitGameMapResponse(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public HitGameMapResponse(GameCell gameCell)
        {
            HittedCell = gameCell;
            HittedCell.Hitted = true;
        }
        public HitGameMapResponse(GameCell hittedCell, HitStatus hitStatus)
        {
            HittedCell = hittedCell;
            HittedCell.Hitted = true;
            HitStatus = hitStatus;
        }
        public HitGameMapResponse(GameCell hittedCell, HitStatus hitStatus, Ship? hittedShip)
        {
            HittedCell = hittedCell;
            HittedCell.Hitted = true;
            HitStatus = hitStatus;
            HittedShip = hittedShip;
        }
    }

}
