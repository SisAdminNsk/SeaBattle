using SeaBattle.Contracts;
using SeaBattleGame.GameConfig;
using SeaBattleGame.Map;
using ErrorOr;

namespace SeaBattleApi.Services
{
    public interface IStartGameService
    {
        List<GameModeConfig> GetAllGameConfigs();
        ErrorOr<GameMap> TryParseGameMap(PlayerGameMapRequest gameMapRequest);
    }
}
