﻿using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit;
using MinesServer.GameShit.Programmator;
using MinesServer.Network;
using MinesServer.Network.Auth;
using MinesServer.Network.ConnectionStatus;
using MinesServer.Network.Constraints;
using MinesServer.Network.GUI;
using MinesServer.Network.HubEvents;
using MinesServer.Network.Programmator;
using MinesServer.Network.TypicalEvents;
using MinesServer.Network.World;
using NetCoreServer;
using System.Text.RegularExpressions;

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
            Packet p = default;
            try
            {
                p = Packet.Decode(buffer);
                switch (p.data)
                {
                    case AUPacket au: AU(au); break;
                    case TYPacket ty: father.time.AddAction(() => TY(ty),player); break;
                    default:
                        // Invalid packet
                        break;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"invalid packet from {player?.Id} {ex}");
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
            player.afkstarttime = DateTime.Now;
            player.connection = null;
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
                case ClanPacket clan: Clan(packet, clan); break;
                case PopePacket pp: Pope(packet, pp); break;
                case PROGPacket prog: PROG(packet, prog); break;
                case PDELPacket pdel: Pdel(packet, pdel);break;
                case pRSTPacket prst: Prst(packet, prst); break;
                case PRENPacket pren: Pren(packet, pren); break;
                case ChatPacket chat: Chat(packet, chat);break;
                case INVNPacket invn: Invn(packet, invn); break;
                case ChinPacket chin: Chin(packet, chin);break;
                default:
                    // Invalid event type
                    break;
            }
        }
        private void Chin(TYPacket f,ChinPacket chin)
        {
           
        }
        private void Invn(TYPacket f,INVNPacket invn)
        {

        }
        private void Chat(TYPacket f,ChatPacket chat)
        {
            if (Default.def.IsMatch(chat.message.Replace("\n", "")))
            {
                player.currentchat?.AddMessage(player, chat.message.Replace("\n", ""));
            }
        }
        private void Pren(TYPacket f,PRENPacket pren)
        {
            StaticGUI.Rename(player, pren.Id);
        }
        private void Prst(TYPacket f, pRSTPacket prst)
        {
            var p = player.programsData.selected;
            if (p != null && !player.programsData.ProgRunning)
            {
                SendU(new OpenProgrammatorPacket(p.id, p.name, p.data));
            }
            if (player.programsData.ProgRunning)
                player.RunProgramm();
            SendU(new ProgrammatorPacket(false));
        }
        private void Pdel(TYPacket f,PDELPacket pdel)
        {
            StaticGUI.DeleteProg(player, pdel.Id);
        }
        private void PROG(TYPacket f, PROGPacket p)
        {
            StaticGUI.StartedProg(player, p.prog);
            SendU(new ProgrammatorPacket(player.programsData.ProgRunning));
        }
        private void Pope(TYPacket f, PopePacket p)
        {
            StaticGUI.OpenGui(player);
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
            }, 200000);
        }
        private void GeoHandler(TYPacket parent, XgeoPacket packet)
        {
            player.AddAciton(() =>
            {
                if (player != null && player.win == null)
                {
                    player.Geo();
                }
            }, 200000);
        }
        private void BuildHandler(TYPacket parent, XbldPacket packet)
        {
            if (player != null && player.win == null)
            {
                player.AddAciton(() =>
                {
                    player.dir = packet.Direction;
                    player.Build(packet.BlockType);
                }, 200000);
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
                    player.Move((int)parent.X, (int)parent.Y, packet.Direction);
                }, player.OnRoad ? (player.pause * 5) * 0.65 : player.pause * 5);
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
            },player);
        }
        #endregion
        #region senders
        public void SendWorldInfo()
        {
            SendU(new WorldInfoPacket(World.W.name, World.CellsWidth, World.CellsHeight, 123, "COCK", "http://pi.door/", "ok"));
        }
        public void SendPing()
        {
            SendU(new PingPacket(1, 1, "sosi"));
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
    }
}
