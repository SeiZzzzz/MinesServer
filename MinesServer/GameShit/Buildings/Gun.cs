namespace MinesServer.GameShit.Buildings
{
    public class Gun : Pack,IDamagable
    {
        #region fields
        public int hp { get; set; }
        public int charge { get; set; }
        public int maxcharge { get; set; }
        public int cost { get; set; }
        public override int cid { get; set; }
        public DateTime brokentimer { get; set; }
        #endregion
        public Gun(int x, int y, int ownerid) : base(x, y, ownerid, PackType.Gun)
        {
            hp = 100;
        }
        public void Destroy(Player p)
        {

        }
        public override GUI.Window? GUIWin(Player p)
        {
            return null;
        }
        public override void Build()
        {
            throw new NotImplementedException();
        }
        public override void Update()
        {
            base.Update();
        }
    }
}
