namespace MinesServer.GameShit
{
    public class Basket
    {
        public int Id { get; set; }
        private Player player;
        public Basket(Player player) => this.player = player;
        public Basket()
        {

        }
        public long[] cry =
        {
            0,
            0,
            0,
            0,
            0,
            0
        };
        public void AddCrys(int index, long val)
        {
            this.cry[index] += val;
            if (cry[index] < 0)
            {
                cry[index] = long.MaxValue;
            }
            player.connection.Send("@B", GetCry);
        }
        public void Boxcrys(long[] crys)
        {
            for (var i = 0; i < this.cry.Length; i++)
            {
                cry[i] += crys[i];
            }

            player.connection.Send("@B", GetCry);
        }
        public void UpdateBasket()
        {
            player.connection.Send("@B", GetCry);
        }
        public void ClearCrys()
        {
            for (var i = 0; i < this.cry.Length; i++)
            {
                this.cry[i] = 0;
            }

            player.connection.Send("@B", GetCry);
        }
        public bool RemoveCrys(int index, long val)
        {
            if (val < 0)
            {
                return false;
            }

            if ((this.cry[index] - val) >= 0)
            {
                this.cry[index] -= val;
                player.connection.Send("@B", GetCry);
                return true;
            }

            player.connection.Send("@B", GetCry);
            return false;
        }
        public int cap = 0;
        public long AllCry => this.cry.Select((t, i) => cry[i]).Sum();
        public string GetCry => this.cry.Aggregate("", (current, t) => current + (t + ":")) + cap;
    }
}
