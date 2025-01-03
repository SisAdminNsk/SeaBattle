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
        public GameCell HittedCell { get; set; }
        public HitStatus HitStatus { get; set; }
        public Ship? HittedShip { get; set; }
        public HitGameMapResponse()
        {

        }

        public HitGameMapResponse(GameCell hittedCell, HitStatus hitStatus)
        {
            HittedCell = hittedCell;
            HitStatus = hitStatus;
        }
        public HitGameMapResponse(GameCell hittedCell, HitStatus hitStatus, Ship? hittedShip)
        {
            HittedCell = hittedCell;
            HitStatus = hitStatus;
            HittedShip = hittedShip;
        }
    }

}
