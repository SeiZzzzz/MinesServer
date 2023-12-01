using MinesServer.Server;

namespace MinesServer.GameShit.Marketext
{
    public class Order
    {
        public int id { get; set; }
        public int initiatorid { get; set; }
        public int itemid { get; set; }
        public int num { get; set; }
        public long cost { get; set; }
        public DateTime bettime { get; set; }
        public void Bet(Player p, long money)
        {
            if ((buyerid > 0 ? Math.Ceiling(cost + (cost * 0.01f)) : cost) > money || p.money < cost)
            {
                return;
            }
            using var db = new DataBase();
            Player? buyer = null;
            if (buyerid != 0)
            {
                buyer = db.players.First(i => i.Id == buyerid);
                buyer.money += cost;
            }
            cost = money;
            buyerid = p.Id;
            p.money -= money;
            p.SendMoney();
            bettime = DateTime.Now;
            db.SaveChanges();
        }
        public void CheckReady()
        {
            if (TimeSpan.FromMinutes(5) <= (DateTime.Now - bettime) && buyerid > 0)
            {
                using var db = new DataBase();
                db.orders.Remove(this);
                var buyer = MServer.GetPlayer(buyerid);
                if (buyer != null && buyer.inventory != null)
                {
                    buyer.inventory[itemid] += num;
                }
                else
                {
                    db.inventories.First(i => i.Id == buyerid)[itemid] += num;
                }
                var initiator = MServer.GetPlayer(initiatorid);
                if (initiator != null)
                {
                    initiator.money += cost;
                    initiator.SendMoney();
                }
                db.SaveChanges();
            }
        }
        public void OwnerCancelBet()
        {

        }
        public int buyerid { get; set; }
    }
}
