using SeaBattle.Contracts;
using SeaBattleGame.Player;

namespace SeaBattleApi.Websockets
{
    public class PlayerMessageHandler : IPlayerMessageHandler
    {
        private IGamePlayer _gamePlayer;
        public PlayerMessageHandler(IGamePlayer gamePlayer)
        {
            _gamePlayer = gamePlayer;
        }

        public void Handle<T>(T message) where T : BasePlayerRequest
        {
            if (message.MessageType == "HitRequest")
            {
                var hitRequest = message.GetRequest<HitRequest>();

                _gamePlayer.RequestMakeHit(hitRequest.CellToHit);
            }
        }
    }
}
