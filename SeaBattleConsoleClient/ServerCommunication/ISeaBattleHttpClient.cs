using ErrorOr;
using SeaBattle.Contracts;
using SeaBattleGame.GameConfig;

namespace SeaBattleConsoleClient.ServerCommunication
{
    interface ISeaBattleHttpClient : IDisposable
    {
        Task<ErrorOr<List<GameModeConfig>>> GetAllConfigsAsync();
        Task<ErrorOr<ValidateGameMapResponse>> TryValidateGameMapAsync(PlayerGameMapRequest gameMapRequest);
    }
}
