using ApiServer.DB.Mysql.Model;
using Microsoft.EntityFrameworkCore;


namespace ApiServer.DB.Mysql
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<UsersOauth> Users_oauth { get; set; }
    }
}
