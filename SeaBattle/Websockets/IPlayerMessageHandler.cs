using SeaBattle.Contracts;

namespace SeaBattleApi.Websockets
{
    public interface IPlayerMessageHandler
    {
        void Handle<T>(T message) where T : BasePlayerRequest;
    }
}
