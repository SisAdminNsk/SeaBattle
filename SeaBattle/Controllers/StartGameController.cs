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
    public class GameController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private readonly IPlayerConnectionsService _playerConnectionService;
        private readonly IGameSessionService _gameSessionService;
        private readonly IStartGameService _startGameService;
        public GameController
        (
            ILogger<GameController> logger,
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

            var response = new ValidateGameMapResponse();

            if (errorOrGameMap.IsError)
            {
                response.ErrorCode = "InvalidGameMap";
                response.ErrorMessage = "Invalid game map request";

                return BadRequest(response);
            }

            var accessToken = new StartGameAccessToken
            {
                Token = Guid.NewGuid().ToString(),
            };

            var gameMap = errorOrGameMap.Value;

            _cache.Set(accessToken.Token, gameMap, TimeSpan.FromMinutes(1));

            response.Success = true;
            response.AccessToken = accessToken.Token;

            return Ok(response);
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
                        var newPlayerConnection = new PlayerConnection(socket, playerGameMap);

                        var sessionId = _gameSessionService.TryStartGameSession(playerConnection, newPlayerConnection);

                        await newPlayerConnection.ListenSocket();
                    }
                    else
                    {
                        var newPlayerConnection = _playerConnectionService.AddNewConnection(socket, playerGameMap);

                        await newPlayerConnection.ListenSocket();
                    }
                }
                else
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await HttpContext.Response.WriteAsync("this endpoint allows only websocket connection.");
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
                    ErrorMessage = "invalid start game token, token is wrong or outdated"
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
