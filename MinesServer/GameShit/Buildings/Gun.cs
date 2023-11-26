namespace MinesServer.GameShit.Buildings
{
    public class Gun : Pack
    {
        public Gun(int x, int y, int ownerid) : base(x, y, ownerid, PackType.Gun) { }
        public override GUI.Window? GUIWin(Player p)
        {
            return null;
        }
        public override void Build()
        {
            throw new NotImplementedException();
        }
        public int charge { get; set; }
        public int maxcharge { get; set; }
        public int cost { get; set; }
        public int cid { get; set; }
        public override void Update()
        {
            base.Update();
        }
    }
}
