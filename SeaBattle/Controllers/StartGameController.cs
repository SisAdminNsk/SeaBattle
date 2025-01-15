using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using SeaBattleApi.Services;
using SeaBattleApi.Websockets;

namespace SeaBattle.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StartGameController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IPlayerConnectionsService _playerConnectionService;
        private readonly IGameSessionService _gameSessionService;  
        public StartGameController
        (
            ILogger<StartGameController> logger,
            IPlayerConnectionsService playerConnectionsService,
            IGameSessionService gameSessionService
        )
        {
            _logger = logger;
            _playerConnectionService = playerConnectionsService;
            _gameSessionService = gameSessionService;
        }

        [AllowAnonymous]
        [HttpGet(Name = "StartGameController")]
        public async Task StartGame()
        { 
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                WebSocket socket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                var playerConnection = _playerConnectionService.TryTakeFirstConnection();

                if(playerConnection != null)
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
}
