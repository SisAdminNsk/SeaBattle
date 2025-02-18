using System.Net.Http.Json;
using ErrorOr;
using SeaBattleGame.GameConfig;
using SeaBattle.Contracts;

namespace SeaBattleConsoleClient.ServerCommunication
{
    public class SeaBattleHttpClient : ISeaBattleHttpClient
    {
        private readonly HttpClient _httpClient;
        private bool _disposed = false;
        public SeaBattleHttpClient(string serverAddress, string serverPort)
        {
            _httpClient = new HttpClient();

            _httpClient.BaseAddress = new Uri($"http://{serverAddress}:{serverPort}/Game/");
        }

        public async Task<ErrorOr<List<GameModeConfig>>> GetAllConfigsAsync()
        {
            var response = await _httpClient.GetAsync("GetAllGameConfigs");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<GameModeConfig>>();
            }

            return Error.Failure("Ошибка при получении игровых конфигураций: " + response.ReasonPhrase);
        }

        public async Task<ErrorOr<ValidateGameMapResponse>> TryValidateGameMapAsync(PlayerGameMapRequest gameMapRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("ValidateGameMap", gameMapRequest);

            var content = await response.Content.ReadFromJsonAsync<ValidateGameMapResponse>();

            if (response.IsSuccessStatusCode)
            {
                return content;
            }

            return Error.Failure($"Ошибка при валидации игровой карты: {content.ErrorMessage}, {response.ReasonPhrase}");
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _httpClient.Dispose();

                _disposed = true;
            }
        }
    }
}
