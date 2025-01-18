using SeaBattleGame.GameConfig;

namespace SeaBattle.Contracts
{
    public class PlayerGameMapRequest : BasePlayerRequest
    {
        public GameModeConfig GameModeConfiguration { get; set; }
        public List<ShipPositionOnMap> ShipPositions { get; set; }
        public PlayerGameMapRequest(List<ShipPositionOnMap> shipPositions, GameModeConfig config)
        {
            MessageType = "ValidateGameMap";

            GameModeConfiguration = config;
            ShipPositions = shipPositions;
        }

        public PlayerGameMapRequest()
        {
            MessageType = "ValidateGameMap";
        }
    }
}
