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
                var tps = 128;
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
        public void UnlimitedUpdate()
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
        public void Update()
        {
            if (!MServer.started)
            {
                return;
            }
            for (int i = 0; i < gameActions.Count; i++)
            {
                /*try
                {*/
                var action = gameActions.Dequeue();
                if (action != null)
                {
                    action();
                }
                /*}
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }*/
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
