using MinesServer.GameShit.Buildings;
using MinesServer.Server;

namespace MinesServer.GameShit.Consumables
{
    public static class ShitClass
    {
        public static void C190Shot(int x, int y, Player p)
        {
            int shotx = 0;
            int shoty = 0;
            switch (p.dir)
            {
                case 0:
                    shoty = y + 9;
                    if (!World.W.ValidCoord(0, shoty))
                    {
                        break;
                    }
                    p.SendDFToBots(7, x, shoty, 0, 1);
                    for (; y <= shoty; y++)
                    {
                        var c = World.GetCell(x, y);
                        foreach (var player in World.W.GetPlayersFromPos(x, y))
                        {
                            player.health.Hurt(20 + 60 * player.c190stacks);
                            player.c190stacks++;
                            player.lastc190hit = DateTime.Now;
                        }
                        if (!World.isAlive(c) && World.GetProp(c).is_diggable && World.GetProp(c).is_destructible)
                        {
                            World.DamageCell(x, y, 50);
                        }
                    }
                    break;
                case 1:
                    shotx = x - 9;
                    if (!World.W.ValidCoord(shotx, 0))
                    {
                        break;
                    }
                    p.SendDFToBots(7, shotx, y, 0, 1);
                    for (; x >= shotx; x--)
                    {
                        var c = World.GetCell(x, y);
                        foreach (var player in World.W.GetPlayersFromPos(x, y))
                        {
                            player.health.Hurt(20 + 60 * player.c190stacks);
                            player.c190stacks++;
                            player.lastc190hit = DateTime.Now;
                        }
                        if (!World.isAlive(c) && World.GetProp(c).is_diggable && World.GetProp(c).is_destructible)
                        {
                            World.DamageCell(x, y, 50);
                        }
                    }
                    break;
                case 2:
                    shoty = y - 9;
                    if (!World.W.ValidCoord(0, shoty))
                    {
                        break;
                    }
                    p.SendDFToBots(7, x, shoty, 0, 1);
                    for (; y >= shoty; y--)
                    {
                        var c = World.GetCell(x, y);
                        foreach (var player in World.W.GetPlayersFromPos(x, y))
                        {
                            player.health.Hurt(20 + 60 * player.c190stacks);
                            player.c190stacks++;
                            player.lastc190hit = DateTime.Now;
                        }
                        if (!World.isAlive(c) && World.GetProp(c).is_diggable && World.GetProp(c).is_destructible)
                        {
                            World.DamageCell(x, y, 50);
                        }
                    }
                    break;
                case 3:
                    shotx = x + 9;
                    if (!World.W.ValidCoord(shotx, 0))
                    {
                        break;
                    }
                    p.SendDFToBots(7, shotx, y, 0, 1);
                    for (; x <= shotx; x++)
                    {
                        var c = World.GetCell(x, y);
                        foreach (var player in World.W.GetPlayersFromPos(x, y))
                        {
                            player.health.Hurt(20 + 60 * player.c190stacks);
                            player.c190stacks++;
                            player.lastc190hit = DateTime.Now;
                        }
                        if (!World.isAlive(c) && World.GetProp(c).is_diggable && World.GetProp(c).is_destructible)
                        {
                            World.DamageCell(x, y, 50);
                        }
                    }
                    break;

            }
        }
        public static void Boom(int x, int y, Player player)
        {
            var ch = World.W.GetChunk(x, y);
            ch.SendPack('B', x, y, 0, 0);
            World.W.AsyncAction(10, () =>
            {
                for (int _x = -4; _x <= 4; _x++)
                {
                    for (int _y = -4; _y <= 4; _y++)
                    {
                        if (World.W.ValidCoord(x + _x, y + _y) && System.Numerics.Vector2.Distance(new System.Numerics.Vector2(x, y), new System.Numerics.Vector2(x + _x, y + _y)) <= 3.5f)
                        {
                            foreach (var p in World.W.GetPlayersFromPos(x + _x, y + _y))
                            {
                                p.health.Hurt(40);
                            }
                            var c = World.GetCell(x + _x, y + _y);
                            if (World.GetProp(c).is_destructible && !World.PackPart(x + _x, y + _y))
                            {
                                if (c == 117 && Physics.r.Next(1, 101) > 98)
                                {
                                    World.SetCell(x + _x, y + _y, 118);
                                }
                                else if (c == 118)
                                {
                                    World.SetCell(x + _x, y + _y, 103);
                                }
                                else if (c != 117 && c != 118)
                                {
                                    World.Destroy(x + _x, y + _y, World.destroytype.CellAndRoad);
                                }
                            }
                        }
                    }
                }
                ch.SendDirectedFx(1, x, y, 3, 0, 0);
                ch.ClearPack(x, y);
            });
        }
        public static void Prot(int x, int y, Player player)
        {
            var ch = World.W.GetChunk(x, y);
            ch.SendPack('B', x, y, 0, 1);
            World.W.AsyncAction(10, () =>
            {
                for (int _x = -1; _x <= 1; _x++)
                {
                    for (int _y = -1; _y <= 1; _y++)
                    {
                        if (World.W.ValidCoord(x + _x, y + _y) && System.Numerics.Vector2.Distance(new System.Numerics.Vector2(x, y), new System.Numerics.Vector2(x + _x, y + _y)) <= 3.5f)
                        {
                            foreach (var p in World.W.GetPlayersFromPos(x + _x, y + _y))
                            {
                                p.health.Hurt(50);
                            }
                            var c = World.GetCell(x + _x, y + _y);
                            if (World.GetProp(c).is_destructible && !World.PackPart(x + _x, y + _y))
                            {
                                World.Destroy(x + _x, y + _y, World.destroytype.CellAndRoad);
                            }
                        }
                    }
                }
                ch.SendDirectedFx(1, x, y, 1, 0, 1);
                ch.ClearPack(x, y);
            });
        }
        public static void Raz(int x, int y, Player p)
        {
            var ch = World.W.GetChunk(x, y);
            ch.SendPack('B', x, y, 0, 2);
            World.W.AsyncAction(15, () =>
            {
                using var db = new DataBase();
                for (int _x = -10; _x <= 10; _x++)
                {
                    for (int _y = -10; _y <= 10; _y++)
                    {
                        if (World.W.ValidCoord(x + _x, y + _y) && System.Numerics.Vector2.Distance(new System.Numerics.Vector2(x, y), new System.Numerics.Vector2(x + _x, y + _y)) <= 9.5f)
                        {
                            if (World.ContainsPack(x + _x, y + _y, out var pack) && pack is IDamagable)
                            {
                                var damagable = pack as IDamagable;
                                db.Attach(pack);

                                if (damagable.CanDestroy())
                                {
                                    damagable.Destroy(p);
                                }
                                else
                                {
                                    damagable.Damage(10);
                                }
                            }
                            foreach (var player in World.W.GetPlayersFromPos(x + _x, y + _y))
                            {
                                player.health.Hurt(500);
                            }
                        }
                    }
                }
                db.SaveChanges();
                ch.SendDirectedFx(1, x, y, 9, 0, 2);
                ch.ClearPack(x, y);
            });
        }
    }
}
