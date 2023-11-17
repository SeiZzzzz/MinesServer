using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit;

namespace MinesServer.Server
{
    public class DataBase : DbContext
    {
        public DbSet<Player> players { get; set; }
        public DbSet<Health> healths { get; set; }
        public DbSet<Inventory> inventories { get; set; }
        public DbSet<Basket> baskets { get; set; }
        public DbSet<PlayerSkills> skills { get; set; }
        public DataBase()
        {
            Database.EnsureCreated();
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
        public static void Load()
        {
            using var db = new DataBase();
            try
            {

            }
            catch (Exception ex)
            {
                Default.WriteError(ex.ToString());
            }
        }
    }
}
