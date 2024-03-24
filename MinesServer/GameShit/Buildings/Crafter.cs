using MinesServer.GameShit.GUI;
using MinesServer.GameShit.Sys_Craft;
using MinesServer.GameShit.SysCraft;
using MinesServer.Server;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesServer.GameShit.Buildings
{
    public class Crafter : Pack, IDamagable
    {
        private Crafter()
        {

        }
        public Crafter(int x, int y, int ownerid) : base(x, y, ownerid, PackType.Craft)
        {
            hp = 1000;
            using var db = new DataBase();
            db.crafts.Add(this);
            db.SaveChanges();
        }
        public CraftEntry? currentcraft { get; set; }
        public DateTime brokentimer { get; set; }
        [NotMapped]
        public override int off
        {
            get
            {
                if (currentcraft != null)
                {
                    var ret = 1 + currentcraft.GetRecipie().result.id;
                    if (currentcraft.progress >= 100)
                    {
                        ret += 50;
                    }
                    return ret;
                }
                return 0;
            }
        }
        [NotMapped]
        public float charge { get; set; }
        public int hp { get; set; }
        #region affectworld
        public override void Build()
        {
            World.SetCell(x, y, 37, true);
            World.SetCell(x, y + 1, 37, true);
            World.SetCell(x + 1, y, 106, true);
            World.SetCell(x + 1, y - 1, 38, true);
            World.SetCell(x - 1, y - 1, 38, true);
            World.SetCell(x, y - 1, 106, true);
            World.SetCell(x - 1, y, 106, true);
            World.SetCell(x - 1, y + 1, 106, true);
            World.SetCell(x + 1, y + 1, 106, true);
            base.Build();
        }
        protected override void ClearBuilding()
        {

        }
        public void Destroy(Player p)
        {

        }
        #endregion
        public override Window? GUIWin(Player p)
        {
            if (p.Id != ownerid)
                return null;
            return new Window()
            {
                Tabs = [new Tab()
                {
                    Action = "хй",
                    Label = "хуху",
                    InitialPage = currentcraft != null ? StaticSystem.FilledPage(p,this)! : StaticSystem.GlobalFirstPage(p)!
                }]
            };
        }
    }
}
