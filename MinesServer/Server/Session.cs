﻿using MinesServer.GameShit;
using MinesServer.GameShit.GUI;
using MinesServer.Network;
using MinesServer.Network.Auth;
using MinesServer.Network.BotInfo;
using MinesServer.Network.ConnectionStatus;
using MinesServer.Network.GUI;
using MinesServer.Network.HubEvents;
using MinesServer.Network.TypicalEvents;
using MinesServer.Network.World;
using NetCoreServer;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Interop;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
                case INCLPacket incl: Incl(packet, incl);break;
                case INUSPacket inus: Inus(packet,inus);break;
                case PongPacket pi: Ping(packet, pi); break;
                default:
                    // Invalid event type
                    break;
            }
        }
        private void Ping(TYPacket f,PongPacket p)
        {
            SendU(new PingPacket(0, 0, "f"));
        }
        private void Inus(TYPacket f, INUSPacket inus)
        {
            var x = (int)(this.player.pos.X + (this.player.dir == 3 ? 1 : this.player.dir == 1 ? -1 : 0));
            var y = (int)(this.player.pos.Y + (this.player.dir == 0 ? 1 : this.player.dir == 2 ? -1 : 0));
            player.inventory.Use(x, y);
        }
        private void Incl(TYPacket f, INCLPacket incl)
        {
            if (incl.selection.HasValue)
            {
                if (incl.selection == -1)
                {
                    player.inventory.Choose(-1);
                    SendU(new InventoryClosePacket());
                }
                else
                {
                    player.inventory.Choose(incl.selection.Value);
                    player.SendInventory();
                }
            }
        }
        private void DigHandler(TYPacket parent, XdigPacket packet)
        {
            if (player != null && !player.locked)
            {
                int x = (int)(parent.x + (packet.direction == 3 ? 1 : packet.direction == 1 ? -1 : 0));
                int y = (int)(parent.y + (packet.direction == 0 ? 1 : packet.direction == 2 ? -1 : 0));
                    player.dir = packet.direction;
                    if (World.W.ValidCoord(x, y))
                    {
                        player.Bz(x, y);
                    }
            }
        }
        private void GeoHandler(TYPacket parent, XgeoPacket packet)
        {
            if (player != null && !player.locked)
            {
                int x = (int)(parent.x + (player.dir == 3 ? 1 : player.dir == 1 ? -1 : 0));
                int y = (int)(parent.y + (player.dir == 0 ? 1 : player.dir == 2 ? -1 : 0));
                if (World.W.ValidCoord(x, y))
                {
                    //geo
                }
            }
        }
        private void BuildHandler(TYPacket parent, XbldPacket packet)
        {
            if (player != null && !player.locked)
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
            if (player != null && !player.locked)
            {
                 var dir = packet.direction;
                 player.Move((int)parent.x, (int)parent.y, dir > 9 ? dir - 10 : dir);
            }
        }
        private void WhoisHandler(TYPacket parent, WhoiPacket packet)
        {
            SendU(new NickListPacket(packet.botIds.ToDictionary(x => x, x => MServer.GetPlayer(x).player.name)));
        }
        private void LocalChatHandler(TYPacket parent, LoclPacket packet)
        {
            if (player != null && !player.locked && packet.Length > 0)
            {
                if (packet.message == "console")
                {
                    this.player.ShowConsole();
                }
                else if (packet.message.StartsWith(">") && packet.message.Length > 1)
                {
                    HorbDecoder.ConsoleCommand(packet.message.Substring(1), player);
                    this.player.ShowConsole();
                }
                else if (!string.IsNullOrWhiteSpace(packet.message))
                {
                    this.player.SendLocalMsg(packet.message);
                }
            }
            
        }
        public void GUI(TYPacket p,GUI_Packet ty)
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
                    auth.temp = null;
                    auth.nick = "";
                    auth.passwd = "";
                    auth.createnew = false;
                    new Builder()
                        .SetTitle("ВХОД")
                        .AddTextLine("Ник")
                        .AddIConsole()
                        .AddIConsolePlace("")
                        .AddButton("ОК", "%I%")
                        .AddButton("НОВЫЙ АКК", "newakk")
                        .Send(this);
                    return;
                }
                if (auth != null && auth.createnew)
                {
                    if (auth == null)
                    {
                        return;
                    }
                    if (auth.nick == "")
                    {
                        if (Auth.NickNotAvl(button.ToString()))
                        {
                            new Builder()
                            .SetTitle("НОВЫЙ ИГРОК")
                            .AddTextLine("Ник")
                            .AddTextLine("Ник не доступен")
                            .AddIConsole()
                            .AddIConsolePlace("")
                             .AddButton("ОК", "%I%")
                        .Send(this);
                            return;
                        }
                        auth.nick = button.ToString();
                        auth.SetPasswdForNew(this);
                    }
                    else if (auth.passwd == "")
                    {
                        auth.passwd = button.ToString();
                        auth.EndCreateAndInit(this);
                        auth.createnew = false;
                    }
                    return;
                }
                if (button.ToString().StartsWith("newakk"))
                {
                    auth.CreateNew(this);
                }
                else if (auth.nick == "")
                {
                    auth.TryToFindByNick(button.ToString(), this);
                }
                else if (auth.passwd == "" && auth.temp != null)
                {
                    auth.TryToAuthByPlayer(button.ToString(), this);
                }
                return;
            }
            father.time.AddAction(() =>
            {
                HorbDecoder.Decode(button, player);
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
        public void SendCell(int x,int y,byte cell)
        {
            SendB(new HBPacket([new HBMapPacket(x, y, 1, 1, [cell])]));
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
