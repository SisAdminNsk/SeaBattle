using SeaBattleApi.Websockets;
using System.Collections.Concurrent;

namespace SeaBattleApi.Services
{
    public class GameSessionService : IGameSessionService
    {
        private static ConcurrentDictionary<Guid, Task<SeaBattleSession>> _activeSessions = new();
        public Guid TryStartGameSession(PlayerConnection player1Connection, PlayerConnection player2Connection)
        {
            var sessionId = Guid.NewGuid();

            var seaBattleSession = new SeaBattleSession(sessionId, player1Connection, player2Connection);

            seaBattleSession.SessionFinished += OnSessionFinished;

            var task = Task.Run(async () =>
            {
                while (!seaBattleSession.IsFinished())
                {
                    await Task.Delay(1000);
                }

                return seaBattleSession;
            });

            _activeSessions[sessionId] = task;

            return sessionId;
        }

        private void OnSessionFinished(SeaBattleSession sender)
        {
            RemoveSession(sender.Id);
        }

        private bool RemoveSession(Guid sessionId)
        {
            return _activeSessions.TryRemove(sessionId, out var removedTask);
        }
    }
}
