﻿using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.Generator;
using MinesServer.Server;
using System.Reflection.Metadata.Ecma335;

namespace MinesServer.GameShit
{
    public class World
    {
        public string name { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }
        public int chunksCountW { get { return width / 32; } }
        public int chunksCountH { get { return height / 32; } }
        public readonly Map map;
        public Chunk[,] chunks;
        public static World W;
        public Gen gen;
        public World(string name, int width, int height)
        {
            W = this;
            this.width = width;
            this.height = height;
            this.name = name;
            map = new Map(width, height);
            gen = new Gen(width, height);
            var x = DateTime.Now;
            chunks = new Chunk[chunksCountW, chunksCountH];
            CreateChunks();
            if (!map.MapExists)
            {
                Console.WriteLine($"Creating World Preset{width} x {height}({chunksCountW} x {chunksCountH} chunks)");
                Console.WriteLine("EmptyMapGeneration");
                x = DateTime.Now;
                //CreateEmptyMap(114);
                gen.StartGeneration();
                Console.WriteLine("Generation End");
                Console.WriteLine($"{DateTime.Now - x} loaded");
                x = DateTime.Now;
            }
            Console.WriteLine("Creating chunkmesh");
            x = DateTime.Now;
            Console.WriteLine($"{DateTime.Now - x} loaded");
            Console.WriteLine("LoadConfirmed");
            Console.WriteLine("Started");
            gen.GenerateSpawn(4);
        }
        public void CreateChunks()
        {
            for (int chx = 0; chx < chunksCountW; chx++)
            {
                for (int chy = 0; chy < chunksCountH; chy++)
                {
                    chunks[chx, chy] = new Chunk((chx, chy));
                }
            }
        }
        public void DestroyByBoom()
        {
            //idk
        }
        public void Destroy(int x, int y)
        {
            //todo + static
            if (!ValidCoord(x, y))
            {
                return;
            }/*
            var c = map.GetRoad(x, y);
            if (c != 0)
            {
                //map.SetCell(x, y, c);
            }
            else
            {
                //map.SetCell(x, y, 32);
            }*/
        }
        public static float GetDurability(int x,int y)
        {
            //todo
            return 1f;
        }
        public static void SetDurability(int x, int y, float d)
        { 
            //todo 
        }
            public void CreateEmptyMap(byte cell)
        {
            int cells = 0;
            var j = DateTime.Now;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells += 1;
                    SetCell(x, y, cell);
                }
                if (DateTime.Now - j > TimeSpan.FromSeconds(2))
                {
                    Console.Write($"\r{cells}/{width * height}");
                    j = DateTime.Now;
                }
            }
            Console.Write($"\r{cells}/{width * height}");
            Console.WriteLine("");
        }
        public static Cell GetProp(byte type)
        {
            return CellsSerializer.cells[type];
        }
        //todo + static
        public void SetCell(int x, int y, byte cell)
        {
            if (!ValidCoord(x, y))
            {
                return;
            }
            //map.SetCell(x, y, cell);
            UpdateChunkByCoords(x, y);
        }
        public async void AsyncAction(int secdelay, Action act)
        {
            await Task.Run(delegate ()
            {
                System.Threading.Thread.Sleep(secdelay * 100);
                act();
            });
        }
        public Stack<Player> GetPlayersFromPos(int x,int y)
        {
            var st = new Stack<Player>();
            foreach(var id in GetChunk(x,y).bots.Keys)
            {
                var p = MServer.GetPlayer(id);
                if (p == null)
                {
                    continue;
                }
                    if (p.player.x == x && p.player.y == y)
                    {
                        st.Push(p.player);
                    }
                
            }
            return st;
        }
        public static void Boom(int x,int y)
        {
            var ch = W.GetChunk(x, y);
            ch.SendPack('B', x, y, 0, 0);
            W.AsyncAction(7, () =>
            {
                for (int _x = -4; _x < 4; _x++)
                {
                    for (int _y = -4; _y < 4; _y++)
                    {
                        if (W.ValidCoord(x + _x, y + _y) && System.Numerics.Vector2.Distance(new System.Numerics.Vector2(x, y),new System.Numerics.Vector2(x + _x, y + _y)) <= 3.5f)
                        {
                            foreach (var p in W.GetPlayersFromPos(x + _x, y + _y))
                            {
                                p.health.Hurt(40);
                            }
                                var c = GetCell(x + _x, y + _y);
                            if (GetProp(c).is_destructible)
                            {
                                    W.Destroy(x + _x, y + _y);
                            }
                        }
                    }
                }
                ch.SendDirectedFx(1, x, y, 3, 0, 0);
                ch.ClearPack(x, y);
            });
        }
        //todo + static
        public static byte GetCell(int x, int y)
        {
            if (!W.ValidCoord(x, y))
            {
                return 0;
            }
            return 32;
        }
        public bool ContainsPack(int x, int y, out Pack p)
        {
            var chpos = GetChunkPosByCoords(x, y);
            var ch = chunks[chpos.Item1, chpos.Item2];
            p = ch.GetPackAt((x - ch.pos.Item1 * 32), (y - ch.pos.Item2 * 32))!;
            if (p == null)
            {
                return false;
            }
            return true;
        }
        public static bool isAlive(byte cell)
        {
            return ((CellType)cell) switch
            {
                CellType.AliveBlue or CellType.AliveCyan or CellType.AliveRed or CellType.AliveNigger or CellType.AliveViol or CellType.AliveWhite or CellType.AliveRainbow => true,
                _ => false
            };
        }
        public static bool isCry(byte cell)
        {
            return ((CellType)cell) switch
            {
                CellType.XGreen or CellType.Green => true,
                CellType.XBlue or CellType.Blue => true,
                CellType.XRed or CellType.Red => true,
                CellType.XViolet or CellType.Violet => true,
                CellType.White => true,
                CellType.XCyan or CellType.Cyan => true,
                _ => false
            };
        }
        public bool ValidCoord(int x, int y) => (x >= 0 && y >= 0) && (x < width && y < height);
        private (int, int) GetChunkPosByCoords(int x, int y) => ((int)Math.Floor((float)x / 32), (int)Math.Floor((float)y / 32));
        public void UpdateChunkByCoords(int x, int y)
        {
            var ch = GetChunk(x, y);
            if (ch != null)
            {
                ch.Update();
            }
        }
        public Chunk GetChunk(int x, int y)
        {
            var pos = GetChunkPosByCoords(x, y);
            return chunks[pos.Item1, pos.Item2];
        }
    }
}
