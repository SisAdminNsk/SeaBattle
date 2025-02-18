using SeaBattleConsoleClient.ServerCommunication;
using SeaBattleGame.GameConfig;

namespace SeaBattleConsoleClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Введите ip адресс сервера");
            var serverAddress = Console.ReadLine();
            Console.WriteLine("Введите порт сервера");
            var serverPort = Console.ReadLine();

            var startGameConfig = await GetGameConfuration(serverAddress, serverPort);

            if (startGameConfig == null)
            {
                Console.WriteLine("Возникла ошибка при получении конфигураций для запуска игры с сервера.");
                return;
            }

            Menu menu = new Menu(startGameConfig, serverAddress, serverPort);

            await menu.ShowMenuAsync();
        }
        private static async Task<GameModeConfig?> GetGameConfuration(string serverAddress, string serverPort)
        {
            List<GameModeConfig> gameConfigurations = new();

            using (ISeaBattleHttpClient httpClient = new SeaBattleHttpClient(serverAddress, serverPort))
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
