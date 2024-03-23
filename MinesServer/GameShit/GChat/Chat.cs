using Azure.Core.GeoJson;
using MinesServer.Network.Chat;
using MinesServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.GChat
{
    public class Chat
    {
        private Chat() { }
        public Chat(string tag,string name)
        {
            this.tag = tag;
            Name = name;this.global = true;
            messages = new();
        }
        public GCMessage[] GetMessages()
        {
            List<GCMessage> l = new();
            for(int i = messages.Count - 1;i > ((messages.Count - 1) > 30 ? (messages.Count - 1) - 30 : 0); i--)
            {
                var line = messages[i];
                l.Add(new GCMessage(line.time, line.player.cid, line.player.Id, line.player.name, line.message, 1));
            }
            return l.ToArray();
        }
        public void AddMessage(Player p,string msg)
        {
            using var db = new DataBase();
            var line = new GLine() { player = p, message = msg};
            var last = messages.Last();
            db.Attach(this);
            messages.Add(line);
            db.SaveChanges();
            if (global)
            {
                foreach (var i in DataBase.activeplayers)
                {
                    i.connection?.SendU(new ChatMessagesPacket("FED", [new GCMessage(line.time, p.cid, p.Id, p.name,msg, 10)]));
                }
            }
        }
        public int id { get; set; }
        public virtual List<GLine> messages { get; set; }
        public string Name { get; init; }
        public string tag { get; init; }
        public bool global { get; set; }
        
        public int? toplayer { get; set; }
        public int? owner { get; set; }
    }
}
