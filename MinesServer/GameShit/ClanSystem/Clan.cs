using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Controls;

namespace MinesServer.GameShit.ClanSystem
{
    public class Clan
    {
        [NotMapped]
        private List<int> memberlist = null;
        public string membersbd { get; set; }
        public Clan()
        {
        }
        public List<int> GetMemberlist()
        {
            if (memberlist == null)
            {
                memberlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(membersbd);
            }
            return memberlist;
        }
        public void Create()
        {
        }
    }
}
