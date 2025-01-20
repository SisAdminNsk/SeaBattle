using SeaBattleConsoleClient.ServerCommunication;
using SeaBattleGame.GameConfig;

namespace SeaBattleConsoleClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var startGameConfig = await GetGameConfuration();

            if (startGameConfig == null)
            {
                Console.WriteLine("Возникла ошибка при получении конфигураций для запуска игры с сервера.");
                return;
            }

            Menu menu = new Menu(startGameConfig);

            await menu.ShowMenuAsync();
        }
        private static async Task<GameModeConfig?> GetGameConfuration()
        {
            List<GameModeConfig> gameConfigurations = new();

            using (ISeaBattleHttpClient httpClient = new SeaBattleHttpClient())
            {
                var erorrOrGameConfigurations = await httpClient.GetAllConfigsAsync();

                if (erorrOrGameConfigurations.IsError)
                {
                    Console.Clear();
                    Console.WriteLine("Возникла ошибка при получении игровых конфигурацицй с сервера");
                    Console.WriteLine(erorrOrGameConfigurations.FirstError);

                    return null;
                }
                else
                {
                    gameConfigurations = erorrOrGameConfigurations.Value;
                }
            }

            var standartConfiguration = gameConfigurations.OrderBy(x => x.GameMapSize).FirstOrDefault();

            return standartConfiguration;
        }
    }
}
