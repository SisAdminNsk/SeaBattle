using SeaBattleGame.GameConfig;

namespace SeaBattle.Tests
{
    public class BaseGameMapTests
    {
        public GameModeConfig Config { get; init; }

        public BaseGameMapTests(GameMode gameMode)
        {
            var configReader = new GameConfigReader();

            Config = configReader.ReadConfig(gameMode);
        }
    }
}
