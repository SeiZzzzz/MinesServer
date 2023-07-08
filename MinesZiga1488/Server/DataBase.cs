using Microsoft.EntityFrameworkCore;
using MinesServer.GameShit;

namespace MinesServer.Server
{
    public class DataBase : DbContext
    {
        public DbSet<Player> players { get; set; }
        public DataBase()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;MultipleActiveResultSets=true;Database=M;Trusted_Connection=True;");
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
