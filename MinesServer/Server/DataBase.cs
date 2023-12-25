using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit;
using MinesServer.GameShit.Buildings;
using MinesServer.GameShit.ClanSystem;
using MinesServer.GameShit.Marketext;

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
        public DbSet<Market> markets { get; set; }
        public DbSet<Up> ups { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<Clan> clans { get; set; }
        public DbSet<Request> reqs { get; set; }
        public DbSet<Rank> ranks { get; set; }
        public DbSet<Gun> guns { get; set; }
        public static bool created = false;
        public DataBase()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSqlLocalDB;MultipleActiveResultSets=true;Database=M;Trusted_Connection=True;TrustServerCertificate=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Clan>()
                .Navigation(c => c.members)
                .AutoInclude();
            modelBuilder.Entity<Clan>()
               .Navigation(c => c.reqs)
               .AutoInclude();
            modelBuilder.Entity<Clan>()
               .Navigation(c => c.ranks)
               .AutoInclude();
            modelBuilder.Entity<Request>()
                .Navigation(c => c.player)
                .AutoInclude();
        }
        public static void Save()
        {
            using var db = new DataBase();
            db.SaveChanges();
            db.Dispose();
        }
        public static Player? GetPlayer(int id)
        {
            var player = activeplayers.FirstOrDefault(p => p.Id == id);
            if (player != null)
            {
                return player;
            }
            var db = new DataBase();
            return db.players
                .Where(i => i.Id == id)
                .Include(p => p.clanrank)
                .Include(p => p.clan)
                .Include(p => p.inventory)
                .Include(p => p.crys)
                .Include(p => p.skillslist)
                .Include(p => p.settings)
                .Include(p => p.health)
                .Include(p => p.resp)
                .FirstOrDefault();
        }
        public static Player? GetPlayer(string name)
        {
            var player = activeplayers.FirstOrDefault(p => p.name == name);
            if (player != null)
            {
                return player;
            }
            var db = new DataBase();
            return db.players
                .Where(i => i.name == name)
                .Include(p => p.clanrank)
                .Include(p => p.clan)
                .Include(p => p.inventory)
                .Include(p => p.crys)
                .Include(p => p.skillslist)
                .Include(p => p.settings)
                .Include(p => p.health)
                .Include(p => p.resp)
                .FirstOrDefault();
        }
        public static List<Player> activeplayers = new();
        public static void Load()
        {
            using var db = new DataBase();
            try
            {
                foreach (var i in db.boxes)
                {
                    World.SetCell(i.x, i.y, 90);
                }
                foreach (var i in db.resps)
                {
                    World.AddPack(i.x, i.y, i);
                }
                foreach (var i in db.markets)
                {
                    World.AddPack(i.x, i.y, i);
                }
                foreach (var i in db.ups)
                {
                    World.AddPack(i.x, i.y, i);
                }
                foreach (var i in db.guns)
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
