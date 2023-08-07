using MinesServer.GameShit;
using MinesServer.Server;
using Newtonsoft.Json;
using System.Drawing.Drawing2D;

namespace MinesServer
{
    public static class Default
    {
        public static Form mf = new Form();
        public static int port = 8090;
        private static Dictionary<string, Action> commands = new Dictionary<string, Action>();
        public static void Main(string[] args)
        {
            CellsSerializer.Load();
            var t = new Thread(ShowUp);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
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
            commands.Add("restart", () => { server.Stop(); Console.WriteLine("kinda restart"); server.Start(); });
            commands.Add("players", () => {
                Console.WriteLine($"online {server.players.Count}");
                for (int i = 0; i < server.players.Count;i++)
                {
                    Console.WriteLine($"id: {server.players.ElementAt(i).Value.player.Id}\n name :[{server.players.ElementAt(i).Value.player.name}]");
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
        public static void ShowUp()
        {
            PictureBox pb = new PictureBox()
            {
                Location = new Point(0, 0)
            };
            pb.MouseWheel += (e, x) => {
                if (x.Delta > 0)
                {
                    size++;
                }
                if (x.Delta < 0)
                {
                    if (size > 1)
                    {
                        size--;
                    }
                }
                pb.Image = ZoomImg(orgimage, (new Size(size, size)));
            };
            pb.SizeMode = PictureBoxSizeMode.AutoSize;
            mf.Width = 500;
            mf.Height = 500;
            mf.Controls.Add(pb);
            var dialog = new OpenFileDialog();
            dialog.Title = "Open Image";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var PictureBox1 = new PictureBox();
                orgimage = new Bitmap(Image.FromFile(dialog.FileName));
                pb.Image = ZoomImg(orgimage, (new Size(size, size)));
            }
            Application.Run(mf);
        }
        public static int size = 1;
        public static Image orgimage;
        private static Image ZoomImg(Image img, Size size)
        {
            Bitmap bm = new Bitmap(img, Convert.ToInt32(img.Width * size.Width), Convert.ToInt32(img.Height * size.Height));
            Graphics g = Graphics.FromImage(img);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            return bm;
        }
        public static MServer server { get; set; }
    }
}