using MinesServer.GameShit;
using MinesServer.Network;
using MinesServer.Network.Auth;
using MinesServer.Network.ConnectionStatus;
using MinesServer.Network.Constraints;
using MinesServer.Network.GUI;
using MinesServer.Network.HubEvents;
using MinesServer.Network.TypicalEvents;
using MinesServer.Network.World;
using NetCoreServer;
using System.Text;

namespace MinesServer.Server
{
    public class Session : TcpSession
    {
        #region fields

        MServer father;
        public Player player;
        public Auth auth;
        public Session(TcpServer server) : base(server) { father = server as MServer; }
        public int online
        {
            get => DataBase.activeplayers.Count;
        }
        public float starttime = 0;
        public string sid { get; set; }
        #endregion


        #region server handlers
        protected override void OnConnected()
        {
            sid = Auth.GenerateSessionId();
            Console.WriteLine($"{this.ToString()} connected");
            SendU(new StatusPacket("черный хуй в твоей жопе"));
            SendU(new AUPacket(sid));
        }
        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            var p = Packet.Decode(buffer);
            switch (p.data)
            {
                case AUPacket au: AU(au); break;
                case TYPacket ty: father.time.AddAction(() => TY(ty)); break;
                default:
                    // Invalid packet
                    break;
            }
        }
        protected override void OnDisconnected()
        {
            if (player == null)
            {
                return;
            }
            Console.WriteLine(player.name + " disconnected");
            using var db = new DataBase();
            db.players.Update(player);
            db.SaveChanges();
            DataBase.activeplayers.Remove(player);
            player = null;
            Dispose();
        }
        #endregion



        #region handlers

        private void AU(AUPacket p)
        {
            auth = new Auth();
            auth.TryToAuth(p, sid, this);
        }
        private void TY(TYPacket packet)
        {
            switch (packet.Data)
            {
                case XmovPacket xmov: MoveHandler(packet, xmov); break;
                case LoclPacket locl: LocalChatHandler(packet, locl); break;
                case XbldPacket xbld: BuildHandler(packet, xbld); break;
                case XdigPacket xdig: DigHandler(packet, xdig); break;
                case XgeoPacket xgeo: GeoHandler(packet, xgeo); break;
                case WhoiPacket whoi: WhoisHandler(packet, whoi); break;
                case TADGPacket tadg: AutoDiggHandler(packet, tadg); break;
                case GUI_Packet gui_: GUI(packet, gui_); break;
                case INCLPacket incl: Incl(packet, incl); break;
                case INUSPacket inus: Inus(packet, inus); break;
                case PongPacket pi: Ping(packet, pi); break;
                case DPBXPacket dpbx: Dpbx(packet, dpbx); break;
                case SettPacket sett: Sett(packet, sett); break;
                case ADMNPacket admn: ADMN(packet, admn); break;
                case RESPPacket res: Res(packet, res); break;
                case ClanPacket clan: Clan(packet, clan);break;
                default:
                    // Invalid event type
                    break;
            }
        }
        private void Clan(TYPacket f, ClanPacket p)
        {
            player.OpenClan();
        }
        private void Res(TYPacket f, RESPPacket p)
        {
            player.health.Death();
        }
        private void ADMN(TYPacket f, ADMNPacket p)
        {
            if (player.win != null)
            {
                player.win.AdminButton();
                player.win.ShowTabs = false;
            }
            player.SendWindow();
        }
        private void Sett(TYPacket f, SettPacket p)
        {
            player.settings.SendSettingsGUI(player);
        }
        private void Dpbx(TYPacket f, DPBXPacket p)
        {
            player.win = player.crys.OpenBoxGui();
            player.SendWindow();
        }
        private void Ping(TYPacket f, PongPacket p)
        {
            SendU(new PingPacket(0, 0, "f"));
        }
        private void Inus(TYPacket f, INUSPacket inus)
        {
            player.inventory.Use(player);
        }
        private void Incl(TYPacket f, INCLPacket incl)
        {
            if (incl.selection.HasValue)
            {
                if (incl.selection == -1)
                {
                    player.inventory.Choose(-1, player);
                }
                else
                {
                    player.inventory.Choose(incl.selection.Value, player);
                    player.SendInventory();
                }
            }
        }
        private void DigHandler(TYPacket parent, XdigPacket packet)
        {
            player.AddAciton(() =>
            {
                if (player != null && player.win == null)
                {
                    player.dir = packet.Direction;
                    player.Bz();
                }
            }, 0.2);
        }
        private void GeoHandler(TYPacket parent, XgeoPacket packet)
        {
            player.AddAciton(() =>
            {
                if (player != null && player.win == null)
                {
                    player.Geo();
                }
            }, 0.2);
        }
        private void BuildHandler(TYPacket parent, XbldPacket packet)
        {
            if (player != null && player.win == null)
            {
                player.AddAciton(() =>
                {
                    player.dir = packet.Direction;
                    player.Build(packet.BlockType);
                },0.2);
            }
        }
        private void AutoDiggHandler(TYPacket parent, TADGPacket packet)
        {
            if (player != null)
                SendU(new AutoDiggPacket(player.autoDig = !player.autoDig));
        }
        private void MoveHandler(TYPacket parent, XmovPacket packet)
        {
            if (player != null)
            {
                player.AddAciton(() =>
                {
                    var dir = packet.Direction;
                    player.Move((int)parent.X, (int)parent.Y, dir > 9 ? dir - 10 : dir);
                },0.0001);
            }
        }
        private void WhoisHandler(TYPacket parent, WhoiPacket packet)
        {
            SendU(new NickListPacket(packet.BotIds.ToDictionary(x => x, x => DataBase.GetPlayer(x)?.name)));
        }
        private void LocalChatHandler(TYPacket parent, LoclPacket packet)
        {
            if (player != null && player.win == null && packet.Length > 0)
            {
                if (packet.Message == "console")
                {
                    MConsole.ShowConsole(player);
                }
                else if (packet.Message[0] == '>' && packet.Message.Length > 1)
                {
                    MConsole.ShowConsole(player);
                }
                else if (!string.IsNullOrWhiteSpace(packet.Message))
                {
                    this.player.SendLocalMsg(packet.Message);
                }
            }

        }
        public void GUI(TYPacket p, GUI_Packet ty)
        {
            var button = ty.Button;
            if (button == null)
            {
                return;
            }
            if ((auth != null && !auth.complited))
            {
                if (button.ToString() == "exit")
                {
                    auth.exit();
                    return;
                }
                auth.CallAction(button);
                return;
            }
            father.time.AddAction(() =>
            {
                if (button.ToString() == "exit")
                {
                    CloseWindow();
                    return;
                }
                player.CallWinAction(button);
                player.SendWindow();
            });
        }
        #endregion
        #region senders
        public void SendWorldInfo()
        {
            SendU(new WorldInfoPacket(World.W.name, World.CellsWidth, World.CellsHeight, 123, "COCK", "http://pi.door/", "ok"));
        }
        public void SendPing()
        {
            SendU(new PingPacket(0, 0, "sosi"));
        }
        public void SendWin(string win)
        {
            SendU(new GUIPacket(win));
        }
        public void SendU(ITopLevelPacket data) => Send(new("U", data));

        public void SendB(ITopLevelPacket data) => Send(new("B", data));

        public void SendJ(ITopLevelPacket data) => Send(new("J", data));
        public void Send(Packet p)
        {
            Span<byte> span = stackalloc byte[p.Length];
            p.Encode(span);
            //Console.WriteLine(Encoding.UTF8.GetString(span));
            SendAsync(span);
        }

        public void SendCell(int x, int y, byte cell)
        {
            SendB(new HBPacket([new HBMapPacket(x, y, 1, 1, [cell])]));
        }
        public void CloseWindow()
        {
            player.win = null;
            player.skillslist.selectedslot = -1;
            SendU(new GuPacket());
        }
        #endregion
        public void UpdateMs()
        {
            if (starttime != 0)
            {
                starttime += 0.001f;
            }
        }
    }
}
