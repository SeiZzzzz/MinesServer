using MinesServer.GameShit.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    p.SendDFToBots(7, x, shoty, 0, 1);
                    for (; y <= shoty; y++)
                    {
                        var c = World.GetCell(x, y);
                        if (!World.isAlive(c) && World.GetProp(c).is_diggable)
                        {
                            World.DamageCell(x, y, 50);
                        }
                    }
                    break;
                case 1:
                    shotx = x - 9;
                    p.SendDFToBots(7, shotx, y, 0, 1);
                    for (; x >= shotx; x--)
                    {
                        var c = World.GetCell(x, y);
                        if (!World.isAlive(c) && World.GetProp(c).is_diggable)
                        {
                            World.DamageCell(x, y, 50);
                        }
                    }
                    break;
                case 2:
                    shoty = y - 9;
                    p.SendDFToBots(7, x, shoty, 0, 1);
                    for (; y >= shoty; y--)
                    {
                        var c = World.GetCell(x, y);
                        if (!World.isAlive(c) && World.GetProp(c).is_diggable)
                        {
                            World.DamageCell(x, y, 50);
                        }
                    }
                    break;
                case 3:
                    shotx = x + 9;
                    p.SendDFToBots(7, shotx, y, 0, 1);
                    for (; x <= shotx; x++)
                    {
                        var c = World.GetCell(x, y);
                        if (!World.isAlive(c) && World.GetProp(c).is_diggable)
                        {
                            World.DamageCell(x, y, 50);
                        }
                    }
                    break;

            }
        }
        public static void Boom(int x, int y)
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
                                World.Destroy(x + _x, y + _y, World.destroytype.CellAndRoad);
                            }
                        }
                    }
                }
                ch.SendDirectedFx(1, x, y, 3, 0, 0);
                ch.ClearPack(x, y);
            });
        }
        public static void Raz(int x, int y,Player p)
        {
            var ch = World.W.GetChunk(x, y);
            ch.SendPack('B', x, y, 0, 2);
            World.W.AsyncAction(15, () =>
            {
                for (int _x = -10; _x <= 10; _x++)
                {
                    for (int _y = -10; _y <= 10; _y++)
                    {
                        if (World.W.ValidCoord(x + _x, y + _y) && System.Numerics.Vector2.Distance(new System.Numerics.Vector2(x, y), new System.Numerics.Vector2(x + _x, y + _y)) <= 9.5f)
                        {
                            if (World.ContainsPack(x + _x, y + _y, out var pack) && pack is IDamagable)
                            {
                                var damagable = pack as IDamagable;
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
                ch.SendDirectedFx(1, x, y, 9, 0, 2);
                ch.ClearPack(x, y);
            });
        }
    }
}
