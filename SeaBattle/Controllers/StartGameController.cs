using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeaBattleApi.Services;
using SeaBattleApi.Websockets;
using System.Net.WebSockets;

namespace SeaBattle.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StartGameController : ControllerBase
    {
        private readonly ILogger<StartGameController> _logger;
        private readonly IPlayerConnectionsService _playerConnectionService;
        public StartGameController(ILogger<StartGameController> logger, IPlayerConnectionsService playerConnectionsService)
        {
            _logger = logger;
            _playerConnectionService = playerConnectionsService;
        }

        [AllowAnonymous]
        [HttpGet(Name = "StartGameController")]
        public async Task<IActionResult> StartGame()
        { 
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                WebSocket newConnection = await HttpContext.WebSockets.AcceptWebSocketAsync();

                PlayerConnection? playerConnection = _playerConnectionService.TryTakeFirstConnection();

                if(playerConnection != null)
                {
                    // ��������� �������� IGameSession ������ IGameSession ��������� ������� �������� ������������
                    //  � ���������� ����������� ���������� � SesssionEventListener
                }
                else
                {
                    _playerConnectionService.AddNewConnection(newConnection);
                }

                // ������� �������������� ������ ���� ��� ������� 
                return Ok();

            }

            return BadRequest("����� ������������ ������ websocket �����������.");
        }
    }
}
