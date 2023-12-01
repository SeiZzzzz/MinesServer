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

        public void Start()
        {
            Task.Run(() =>
            {
                var tps = 64;
                var lasttick = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                while (true)
                {
                    int ticksToProcess = (int)((DateTimeOffset.Now.ToUnixTimeMilliseconds() - lasttick) / 1000f * tps);
                    if (ticksToProcess > 0)
                    {
                        if (ticksToProcess > 1)
                        {
                            Console.WriteLine("overload");
                        }
                        while (ticksToProcess-- > 0) Update();
                        lasttick = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    }
                }
            });
        }
        public void Update()
        {
            if (World.W.map == null || !MServer.started)
            {
                return;
            }
            for (int j = 1; j <= MServer.Instance.players.Count; j++)
            {
                var player = MServer.GetPlayer(j);
                if (player != null)
                {
                    player?.connection?.UpdateMs();
                    player?.Update();
                }
            }
            for (int i = 0; i < gameActions.Count; i++)
            {
                try
                {
                    var action = gameActions.Dequeue();
                    if (action != null)
                    {
                        action();
                    }
               }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            for (int x = 0; x < World.W.chunksCountW; x++)
            {
                for (int y = 0; y < World.W.chunksCountH; y++)
                {
                    World.W.chunks[x, y].Update();
                }
            }
            World.Update();
            using var db = new DataBase();
            foreach (var order in db.orders)
            {
                order.CheckReady();
            }
            db.SaveChanges();
        }
    }
}
