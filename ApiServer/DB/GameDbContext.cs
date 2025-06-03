using ApiServer.DB.Model;
using Microsoft.EntityFrameworkCore;


namespace ApiServer.DB
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<UsersOauth> Users_oauth{get; set;}
    }
}
