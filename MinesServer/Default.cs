using MinesServer.GameShit;
using MinesServer.Server;
using Newtonsoft.Json;

namespace MinesServer
{
    // <image src="a.jpg"/>
    public static class Default
    {

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
                World.W.map.SaveAllChunks();
            });
            commands.Add("restart", () => { server.Stop(); Console.WriteLine("kinda restart"); server.Start(); });
            commands.Add("players", () =>
            {
                Console.WriteLine($"online {server.players.Count}");
                for (int i = 0; i < server.players.Count; i++)
                {
                    Console.WriteLine($"id: {server.players.ElementAt(i).Value.Id}\n name :[{server.players.ElementAt(i).Value.name}]");
                }
            });
            for (; ; )
            {
                var l = Console.ReadLine();
                if (l != null)
                {
                    if (commands.Keys.Contains(l))
                    {
                        commands[l]();
                    }
                }
            }
        }
        public static Config cfg;
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