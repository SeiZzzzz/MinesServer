using MinesServer.Server;

namespace MinesServer.GameShit.SysMarket
{
    public class Order
    {
        public int id { get; set; }
        public int initiatorid { get; set; }
        public int itemid { get; set; }
        public int num { get; set; }
        public long cost { get; set; }
        public DateTime bettime { get; set; }
        public void Bet(Player p, long money, int order)
        {
            if ((buyerid > 0 ? Math.Ceiling(cost + (cost * 0.001f)) : cost) > money || p.money < cost)
            {
                return;
            }
            using var db = new DataBase();
            Player? buyer = null;
            Console.WriteLine(order);
            if (cost == 0)
            {
                return;
            }
            if (buyerid != 0)
            {
                buyer = db.players.First(i => i.Id == buyerid);
                if (buyer.money + cost > 1000000000)
                {
                    buyer.money = 1000000000;
                }
                else
                {
                    buyer.money += cost;
                }
            }
            var buyno = DataBase.GetPlayer(buyerid);
            if (buyno != null)
            {
                buyno.money += cost;
                buyno.SendMoney();
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
                var buyer = DataBase.GetPlayer(buyerid);
                if (cost == 0)
                {
                    return;
                }
                if (buyer != null && buyer.inventory != null)
                {
                    buyer.inventory[itemid] += num;
                }
                else
                {
                    db.inventories.First(i => i.Id == buyerid)[itemid] += num;
                }
                var initiator = DataBase.GetPlayer(initiatorid);
                if (initiator != null)
                {
                    initiator.money += cost;
                    initiator.SendMoney();
                }
                cost = 0;
                db.SaveChanges();
            }
        }
        public void OwnerCancelBet()
        {

        }
        public int buyerid { get; set; }
    }
}
