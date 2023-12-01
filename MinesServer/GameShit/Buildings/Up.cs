using MinesServer.Enums;
using MinesServer.GameShit.GUI;
using MinesServer.GameShit.GUI.UP;
using MinesServer.Server;

namespace MinesServer.GameShit.Buildings
{
    public class Up : Pack
    {
        #region fields
        public int hp { get; set; }
        public long moneyinside { get; set; }
        #endregion
        public Up(int x, int y, int ownerid) : base(x, y, ownerid, PackType.Up)
        {
            using var db = new DataBase();
            hp = 100;
            db.ups.Add(this);
            db.SaveChanges();
        }
        public Up() { }
        public override Window? GUIWin(Player p)
        {
            var onskill = (int arg) => { p.skillslist.selectedslot = arg; p.win = GUIWin(p); p.SendWindow(); };
            var oninstall = (int slot, SkillType skilltype) =>
            {
                p.win?.CurrentTab.Replace(new UpPage()
                {
                    Skills = p.skillslist.GetSkills(),
                    OnSkill = onskill,
                    SlotAmount = 25,
                    Title = "титле",
                    SkillIcon = skilltype,
                    Text = "описание и цена установки",
                    Button = new Button("Установить", "confirm", (args) => { p.skillslist.InstallSkill(skilltype.GetCode(), p.skillslist.selectedslot, p); p.win = GUIWin(p); p.SendWindow(); })
                });
                p.SendWindow();
            };
            var skillfromslot = p.skillslist.selectedslot > -1 ? p.skillslist.skills[p.skillslist.selectedslot] : null;
            var uppage = p.skillslist.selectedslot == -1 ? new UpPage()
            {
                Skills = p.skillslist.GetSkills(),
                SkillsToInstall = null,
                SlotAmount = 25,
                OnSkill = onskill,
                Title = "xxx",
                Text = "Выберите скилл или пустой слот",
                Button = new Button("buyslotcost", "buyslot", (args) => { }),
                SkillIcon = SkillType.Unknown
            } : new UpPage()
            {
                SelectedSlot = p.skillslist.selectedslot,
                Skills = p.skillslist.GetSkills(),
                SkillsToInstall = skillfromslot == null ? p.skillslist.SkillToInstall() : null,
                SlotAmount = 25,
                OnInstall = skillfromslot == null ? oninstall : null,
                OnSkill = onskill,
                Title = "penis",
                Text = skillfromslot?.Description(),
                Button = skillfromslot != null && skillfromslot.isUpReady() ? new Button("ап", "upgrade", (args) => { skillfromslot.Up(p); p.win = GUIWin(p); p.SendWindow(); }) : null,
                OnDelete = skillfromslot != null ? (slot) => { p.skillslist.DeletetSkill(p); p.win = GUIWin(p); p.SendWindow(); } : null,
                SkillIcon = skillfromslot?.type
            };
            return new Window()
            {
                Tabs = [new Tab()
                {
                    Action = "хй",
                    Label = "хуху",
                    InitialPage = uppage
                }]
            };
        }
        public override void Build()
        {
            World.SetCell(x - 1, y - 2, 38, true);
            World.SetCell(x + 1, y - 2, 38, true);
            World.SetCell(x, y - 2, 106, true);
            World.SetCell(x - 1, y - 1, 106, true);
            World.SetCell(x, y - 1, 106, true);
            World.SetCell(x + 1, y - 1, 106, true);
            World.SetCell(x + 1, y, 106, true);
            World.SetCell(x, y, 37, true);
            World.SetCell(x - 1, y, 106, true);
            World.SetCell(x + 1, y + 1, 106, true);
            World.SetCell(x - 1, y + 1, 106, true);
            World.SetCell(x, y + 1, 37, true);
            base.Build();
        }
    }
}
