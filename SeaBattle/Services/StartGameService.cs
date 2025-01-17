using SeaBattle.Contracts;
using SeaBattleGame.GameConfig;

namespace SeaBattleApi.Services
{
    public class StartGameService : IStartGameService
    {
        public List<GameModeConfig> GetAllGameConfigs()
        {
            GameConfigReader reader = new GameConfigReader();

            //reader.ReadConfig(GameMode.LongGameMode);

            var configs = new List<GameModeConfig>();

            configs.Add(reader.ReadConfig(GameMode.StandartGameMode));

            return configs;
        }

        public bool IsGameMapValid(PlayerGameMapRequest gameMapRequest)
        {
            throw new NotImplementedException();
        }
    }
}
