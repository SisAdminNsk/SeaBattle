using SeaBattle.Contracts;
using SeaBattleGame.GameConfig;

namespace SeaBattleApi.Services
{
    public interface IStartGameService
    {
        List<GameModeConfig> GetAllGameConfigs();
        bool IsGameMapValid(PlayerGameMapRequest gameMapRequest);
    }
}
