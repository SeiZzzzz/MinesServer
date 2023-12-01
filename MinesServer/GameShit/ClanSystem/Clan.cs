using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit.ClanSystem
{
    public class Clan
    {
        [NotMapped]
        public List<int> members
        {
            get
            {
                if (memberlist == null)
                {
                    memberlist = new();
                    foreach (var i in membersbd.Split(","))
                    {
                        memberlist.Add(int.Parse(i));
                    }
                }
                return memberlist;
            }
            set
            {
                membersbd = string.Join(',', memberlist.Select(i => i));
            }
        }
        [NotMapped]
        private List<int> memberlist = null;
        public string membersbd { get; set; }
        public Clan()
        {
        }
        public void Create()
        {

        }
    }
}
