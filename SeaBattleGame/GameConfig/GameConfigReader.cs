using System.Text.Json;

namespace SeaBattleGame.GameConfig
{
    public enum GameMode
    {
        StandartGameMode = 0,
        LongGameMode = 1,
    }
    public class GameConfigReader
    {
        private static object _lock = new object();

        private Dictionary<GameMode, string> GameModeToConfigPath = new();
        public GameConfigReader()
        {
            GameModeToConfigPath.Add(GameMode.StandartGameMode, "standartGameConfig.json");
        }

        public GameModeConfig ReadConfig(GameMode gameMode)
        {
            var configPath = GameModeToConfigPath[gameMode];

            string text = "";

            lock (_lock)
            {
                text = File.ReadAllText(configPath);
            }

            GameModeConfig? gameConfig = JsonSerializer.Deserialize<GameModeConfig>(text);

            if (gameConfig is null)
            {
                throw new JsonException("Не удалось десериализовать GameConfig.");
            }

            return gameConfig;
        }
    }
}
