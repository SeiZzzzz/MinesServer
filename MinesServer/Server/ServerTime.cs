using MinesServer.GameShit;


namespace MinesServer.Server
{
    public class ServerTime
    {
        public delegate void GameAction();
        public Queue<(GameAction action,Player initiator)> gameActions;
        public ServerTime()
        {
            gameActions = new Queue<(GameAction,Player)>();
        }
        public void AddAction(GameAction action,Player p)
        {
            gameActions.Enqueue((action,p));
        }

        public void Start()
        {
            Task.Run(() =>
            {
                var tps = 20;
                var lasttick = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                while (true)
                {
                    UnlimitedUpdate();
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
        async void UnlimitedUpdate()
        {
            for (int i = 0; i < DataBase.activeplayers.Count; i++)
            {
                using var dbas = new DataBase();
                if (DataBase.activeplayers.Count > i)
                {
                    var player = DataBase.GetPlayer(DataBase.activeplayers.ElementAt(i).Id);
                    player?.UnlimitedUpdate();
                }
            }
        }
        async void Update()
        {
            if (!MServer.started)
            {
                return;
            }
            for (int i = 0; i < gameActions.Count; i++)
            {
                var item = gameActions.Dequeue();
                try
                {
                if (item.action != null)
                {
                     item.action();
                }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{item.initiator.name}[{item.initiator.Id}] caused {ex}");
                }
            }
            for (int i = 0; i < DataBase.activeplayers.Count; i++)
            {
                using var dbas = new DataBase();
                if (DataBase.activeplayers.Count > i)
                {
                    var player = DataBase.GetPlayer(DataBase.activeplayers.ElementAt(i).Id);
                    player?.Update();
                }
            }
            for (int x = 0; x < World.ChunksW; x++)
            {
                for (int y = 0; y < World.ChunksH; y++)
                {
                    World.W.chunks[x, y].Update();
                }
            }
            World.W.cells.Commit();
            World.W.road.Commit();
            World.W.durability.Commit();
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
