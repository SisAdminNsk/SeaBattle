using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using SeaBattleApi.Services;
using SeaBattleApi.Websockets;
using SeaBattle.Contracts;
using Microsoft.Extensions.Caching.Memory;
using SeaBattleApi.Controllers;
using System.Text.Json;
using SeaBattleGame.Map;

namespace SeaBattle.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StartGameController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private readonly IPlayerConnectionsService _playerConnectionService;
        private readonly IGameSessionService _gameSessionService;
        private readonly IStartGameService _startGameService;
        public StartGameController
        (
            ILogger<StartGameController> logger,
            IPlayerConnectionsService playerConnectionsService,
            IGameSessionService gameSessionService,
            IStartGameService startGameService,
            IMemoryCache cache
        )
        {
            _logger = logger;
            _playerConnectionService = playerConnectionsService;
            _gameSessionService = gameSessionService;
            _startGameService = startGameService;
            _cache = cache;
        }

        [AllowAnonymous]
        [HttpGet("GetAllGameConfigs")]
        public async Task<ActionResult> GetAllGameConfigs()
        {
            return Ok(_startGameService.GetAllGameConfigs());
        }

        [AllowAnonymous]
        [HttpPost("ValidateGameMap")]
        public async Task<ActionResult> ValidateGameMap([FromBody] PlayerGameMapRequest playerGameMapRequest)
        {
            var errorOrGameMap = _startGameService.TryParseGameMap(playerGameMapRequest);

            if (errorOrGameMap.IsError)
            {
                var errorResponse = new
                {
                    ErrorCode = "InvalidGameMap",
                    ErrorMessage = "Игровая карта невалидна",
                    Details = new[]
                    {
                        "Ошибка заключается в клиентской части приложения.",
                        "Протейстируйте клиента и убедитесь что карта правильно валидируется."
                    }
                };

                return BadRequest(errorResponse);
            }

            var accessToken = new StartGameAccessToken
            {
                Token = Guid.NewGuid().ToString(),
            };

            var gameMap = errorOrGameMap.Value;

            _cache.Set(accessToken.Token, gameMap, TimeSpan.FromMinutes(1));

            return Ok(accessToken);
        }

        [AllowAnonymous]
        [HttpGet("StartGame")]
        public async Task StartGame([FromQuery] string startGameAccessToken)
        {
            GameMap? playerGameMap = await GetPlayerGameMapOrWriteError(startGameAccessToken);

            if(playerGameMap != null)
            {
                if (HttpContext.WebSockets.IsWebSocketRequest)
                {
                    WebSocket socket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                    var playerConnection = _playerConnectionService.TryTakeFirstConnection();

                    if (playerConnection != null)
                    {
                        var newPlayerConnection = new PlayerConnection(socket);

                        var sessionId = _gameSessionService.TryStartGameSession(playerConnection, newPlayerConnection);

                        await newPlayerConnection.ListenSocket();
                    }
                    else
                    {
                        var newPlayerConnection = _playerConnectionService.AddNewConnection(socket);

                        await newPlayerConnection.ListenSocket();
                    }
                }
                else
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await HttpContext.Response.WriteAsync("Метод поддерживает только websocket подключение.");
                }
            }  
        } 
        
        private async Task<GameMap?> GetPlayerGameMapOrWriteError(string playerAccessToken)
        {
            if (!_cache.TryGetValue(playerAccessToken, out GameMap? playerGameMap))
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                var errorResponse = new
                {
                    ErrorCode = "InvalidStartGameAccessToken",
                    ErrorMessage = "Не удалось получить игровую карту, переданный токен начала игры истек либо неверен"
                };

                var jsonResponse = JsonSerializer.Serialize(errorResponse);

                HttpContext.Response.ContentType = "application/json";

                await HttpContext.Response.WriteAsync(jsonResponse);

                return null;
            }

            return playerGameMap;
        }
    }
}
