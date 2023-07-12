using Microsoft.Identity.Client;
using MinesServer.GameShit;

namespace MinesServer.Server
{
    public class ServerTime
    {
        public delegate void GameAction();
        public Queue<GameAction> gameActions;
        public ServerTime()
        {
            gameActions = new Queue<GameAction>();
        }
        public void AddAction(GameAction action)
        {
            gameActions.Enqueue(action);
        }
        public void Update()
        {
            for (int j = 1; j <= MServer.Instance.players.Count; j++)
            {
                if (MServer.Instance.players.Keys.Contains(j))
                {
                    MServer.Instance.players[j].UpdateMs();
                }
            }
            for (int i = 0; i < gameActions.Count; i++)
            { 
                gameActions.Dequeue()();
            }
        }
    }
}
