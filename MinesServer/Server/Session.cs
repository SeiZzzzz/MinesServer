using MinesServer.GameShit;
using MinesServer.Network;
using MinesServer.Network.Auth;
using MinesServer.Network.ConnectionStatus;
using MinesServer.Network.GUI;
using MinesServer.Network.HubEvents;
using MinesServer.Network.TypicalEvents;
using MinesServer.Network.World;
using NetCoreServer;

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
            get => father.players.Count;
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
            if (father.players.Keys.Contains(player.Id))
            {
                father.players.Remove(player.Id);
                player.OnDisconnect();
            }
            player = null;

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
            switch (packet.data)
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
                case DPBXPacket dpbx: Dpbx(packet, dpbx);break;
                case SettPacket sett: Sett(packet, sett);break;
                case ADMNPacket admn: ADMN(packet, admn);break;
                case RespPacket res: Res(packet, res);break;
                default:
                    // Invalid event type
                    break;
            }
        }
        private void Res(TYPacket f, RespPacket p)
        {
            player.health.Death();
        }
        private void ADMN(TYPacket f, ADMNPacket p)
        {
            player.win.CurrentTab.History.Peek().OnAdmin.Invoke();
        }
        private void Sett(TYPacket f, SettPacket p)
        {
            player.settings.SendSettings(player);
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
            if (player != null && player.win == null)
            {
                 player.dir = packet.direction;
                 player.Bz();
            }
        }
        private void GeoHandler(TYPacket parent, XgeoPacket packet)
        {
            if (player != null && player.win == null)
            {
                    player.Geo();
            }
        }
        private void BuildHandler(TYPacket parent, XbldPacket packet)
        {
            if (player != null && player.win == null)
            {
                int x = (int)(parent.x + (packet.direction == 3 ? 1 : packet.direction == 1 ? -1 : 0));
                int y = (int)(parent.y + (packet.direction == 0 ? 1 : packet.direction == 2 ? -1 : 0));
                if (World.W.ValidCoord(x, y))
                {
                    //bld
                }
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
                var dir = packet.direction;
                player.Move((int)parent.x, (int)parent.y, dir > 9 ? dir - 10 : dir);
            }
        }
        private void WhoisHandler(TYPacket parent, WhoiPacket packet)
        {
            SendU(new NickListPacket(packet.botIds.ToDictionary(x => x, x => MServer.GetPlayer(x)!.player.name)));
        }
        private void LocalChatHandler(TYPacket parent, LoclPacket packet)
        {
            if (player != null && player.win == null && packet.Length > 0)
            {
                if (packet.message == "console")
                {
                    MConsole.ShowConsole(player);
                }
                else if (packet.message[0] == '>' && packet.message.Length > 1)
                {
                    MConsole.ShowConsole(player);
                }
                else if (!string.IsNullOrWhiteSpace(packet.message))
                {
                    this.player.SendLocalMsg(packet.message);
                }
            }

        }
        public void GUI(TYPacket p, GUI_Packet ty)
        {
            var button = ty.button;
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
            });
        }
        #endregion
        #region senders
        public void SendWorldInfo()
        {
            SendU(new WorldInfoPacket(World.W.name, World.W.width, World.W.height, 123, "COCK", "http://pi.door/", "ok"));
        }
        public void SendPing()
        {
            SendU(new PingPacket(0, 0, "sosi"));
        }
        public void SendWin(string win)
        {
            SendU(new GUIPacket(win));
        }
        public void SendU(IDataPartBase data) => Send(new("U", data));

        public void SendB(IDataPartBase data) => Send(new("B", data));

        public void SendJ(IDataPartBase data) => Send(new("J", data));
        public void Send(Packet p)
        {
            Span<byte> span = stackalloc byte[p.Length];
            p.Encode(span);
            SendAsync(span);
        }

        public void SendLocalChat(string msg)
        {
            SendB(new HBChatPacket(player.Id, player.x, player.y, msg));
        }
        public void SendCell(int x, int y, byte cell)
        {
            SendB(new HBPacket([new HBMapPacket(x, y, 1, 1, [cell])]));
        }
        public void CloseWindow()
        {
            player.win = null;
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
