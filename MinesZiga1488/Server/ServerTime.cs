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
                var player = MServer.GetPlayer(j);
                if (player != null)
                {
                    player.UpdateMs();
                    if (player.player != null)
                    {
                        player.player.Update();
                    }
                }
            }
            for (int i = 0; i < gameActions.Count; i++)
            {
                try
                {
                    gameActions.Dequeue()();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
