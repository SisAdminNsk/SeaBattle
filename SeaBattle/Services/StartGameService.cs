using ErrorOr;
using SeaBattle.Contracts;
using SeaBattleGame.GameConfig;
using SeaBattleGame.Map;

namespace SeaBattleApi.Services
{
    public class StartGameService : IStartGameService
    {
        public List<GameModeConfig> GetAllGameConfigs()
        {
            GameConfigReader reader = new GameConfigReader();

            var configs = new List<GameModeConfig>();

            configs.Add(reader.ReadConfig(GameMode.StandartGameMode));
            configs.Add(reader.ReadConfig(GameMode.LongGameMode));

            return configs;
        }

        public ErrorOr<GameMap> TryParseGameMap(PlayerGameMapRequest gameMapRequest)
        {
            var gameMap = new GameMap(gameMapRequest.GameModeConfiguration);

            foreach (var shipPosition in gameMapRequest.ShipPositions)
            {
                var ship = shipPosition.Ship;
                var startPosition = shipPosition.StartPosition;
                var shipOrientation = shipPosition.ShipOrientation;

                var shipAddedResponse = gameMap.TryAddShip(ship, startPosition, shipOrientation);

                if (!shipAddedResponse.Success)
                {
                    return Error.Failure("ParseGameMapFailure", shipAddedResponse.ErrorMessage);
                }
            }

            return gameMap;
        }
    }
}
