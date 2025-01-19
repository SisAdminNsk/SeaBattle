using SeaBattleGame.GameConfig;

namespace SeaBattle.Contracts
{
    public class PlayerGameMapRequest
    {
        public GameModeConfig GameModeConfiguration { get; set; }
        public List<ShipPositionOnMap> ShipPositions { get; set; }
        public PlayerGameMapRequest(List<ShipPositionOnMap> shipPositions, GameModeConfig config)
        {
            GameModeConfiguration = config;
            ShipPositions = shipPositions;
        }

        public PlayerGameMapRequest()
        {
            
        }
    }
}
