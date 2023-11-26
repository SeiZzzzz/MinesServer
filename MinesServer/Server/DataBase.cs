using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit;
using MinesServer.GameShit.Buildings;

namespace MinesServer.Server
{
    public class DataBase : DbContext
    {
        public DbSet<Player> players { get; set; }
        public DbSet<Health> healths { get; set; }
        public DbSet<Inventory> inventories { get; set; }
        public DbSet<Basket> baskets { get; set; }
        public DbSet<PlayerSkills> skills { get; set; }
        public DbSet<Box> boxes { get; set; }
        public DbSet<Settings> settings { get; set; }
        public DbSet<Resp> resps { get; set; }
        public static bool created = false;
        public DataBase()
        {
            if (!created)
            {
                Database.EnsureCreated();
                created = true;
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;MultipleActiveResultSets=true;Database=M;Trusted_Connection=True;");
        }
        public static Player GetPlayerClassFromBD(int id)
        {
            using var db = new DataBase();
            var p = db.players.SingleOrDefault(p => p.Id == id);
            if (p != default(Player))
            {
                return p;
            }
            return null;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
        public static void Save()
        {
            using var db = new DataBase();
            db.SaveChanges();
            db.Dispose();
        }
        public static void Load()
        {
            using var db = new DataBase();
            try
            {
                foreach(var i in db.boxes)
                {
                    World.W.GetChunk(i.x, i.y).Load();
                    World.SetCell(i.x, i.y, 90);
                    World.W.GetChunk(i.x, i.y).Save();
                }
                foreach (var i in db.resps)
                {
                    World.AddPack(i.x, i.y, i);
                }
            }
            catch (Exception ex)
            {
                Default.WriteError(ex.ToString());
            }
            db.Dispose();
        }
    }
}
