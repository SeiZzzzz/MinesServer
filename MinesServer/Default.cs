﻿using MinesServer.GameShit;
using MinesServer.Server;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MinesServer
{
    /// <summary>
    ///  MinetServer
    ///  GUI and Network implementation by Darkar25
    /// </summary>
    // <image src="a.jpg"/>
    public static class Default
    {
        static public bool HasProperty(this Type type, string name)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Any(p => p.Name == name);
        }
        public static bool ToBool(this string s) => s != "0";
        /*public static Form mf = new Form();*/
        public static int port = 8090;
        private static Dictionary<string, Action> commands = new Dictionary<string, Action>();
        public static void Main(string[] args)
        {
            CellsSerializer.Load();
            //var t = new Thread(ShowUp);
            //t.SetApartmentState(ApartmentState.STA);
            //t.Start();
            var configPath = "config.json";
            if (File.Exists(configPath))
            {
                cfg = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
            }
            else
            {
                cfg = new Config();
                File.WriteAllText(configPath, JsonConvert.SerializeObject(cfg, Formatting.Indented));
            }
            server = new MServer(System.Net.IPAddress.Any, port);
            server.Start();
            Loop();

        }
        private static void Loop()
        {
            commands.Add("save", () =>
            {
                using var db = new DataBase();
                db.SaveChanges();
                World.W.cells.Commit();
                World.W.road.Commit();
                World.W.durability.Commit();
            });
            commands.Add("restart", () => { server.Stop(); Console.WriteLine("kinda restart"); server.Start(); });
            commands.Add("players", () =>
            {
                Console.WriteLine($"online {DataBase.activeplayers.Count}");
                for (int i = 0; i < DataBase.activeplayers.Count; i++)
                {
                    var player = DataBase.activeplayers.ElementAt(i);
                    Console.WriteLine($"id: {player.Id}\n name :[{player.name}] online:{player.online}");
                }
            });
            for (; ; )
            {
                var l = Console.ReadLine();
                if (l != null && commands.Keys.Contains(l))
                    commands[l]();
            }
        }
        public static Config cfg;
        public static Regex def = new Regex("^[а-яА-ЯёЁa-zA-Z 0-9]+$");
        public static void WriteError(string ex)
        {
            var trace = new System.Diagnostics.StackTrace();
            var method = trace.GetFrame(1).GetMethod().Name;
            Console.WriteLine($"{method} caused error {ex}");
        }
        /*
        public static void ShowUp()
        {
            PictureBox pb = new PictureBox()
            {
                Location = new Point(0, 0)
            };
            var dialog = new OpenFileDialog();
            dialog.Title = "Open Image";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var PictureBox1 = new PictureBox();
                orgimage = new Bitmap(Image.FromFile(dialog.FileName));
                pb.Image = orgimage;
            }
            float zoom = 1;
            var i = new Bitmap(orgimage);
            var imageRect = new RectangleF(Point.Empty, i.Size);
            pb.Paint += (e, m) =>
            {

            };
            var step = 0.05f;
            pb.MouseWheel += (e, mouse) =>
            {

            };
            pb.SizeMode = PictureBoxSizeMode.AutoSize;
            mf.Width = 500;
            mf.Height = 500;
            mf.Controls.Add(pb);

            Application.Run(mf);
        }*/
        public static int size = 1;
        /*public static Image orgimage;*/
        public static MServer server { get; set; }
    }
    public static class E
    {

    }
}